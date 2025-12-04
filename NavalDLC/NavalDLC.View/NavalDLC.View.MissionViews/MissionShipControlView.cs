using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace NavalDLC.View.MissionViews;

public class MissionShipControlView : MissionBattleUIBaseView
{
	public enum CameraModes
	{
		Back,
		Shoulder,
		Front,
		NumPositions
	}

	protected SailInput SailControl;

	protected NavalShipsLogic NavalShipsLogic;

	private Vec3 _lastCameraOffset;

	private float _lastCameraFovMultiplier = 1f;

	private bool _wasOrderMenuOpenLastFrame;

	protected bool IsAimingWithRangedWeapon;

	private float _backCameraDistanceMultiplier = 1f;

	private float _lastForwardKeyPressTime;

	private float _lastBackwardKeyPressTime;

	private int _lastAccelerationAxisInput;

	public CameraModes ActiveCameraMode { get; protected set; }

	public ShipControllerMachine ControllerMachine { get; private set; }

	protected bool IsAimingWithRangedWeaponAndAllowed
	{
		get
		{
			if (IsAimingWithRangedWeapon)
			{
				return IsAimingWithRangedWeaponAllowed;
			}
			return false;
		}
	}

	protected bool IsAimingWithRangedWeaponAllowed
	{
		get
		{
			if (!((MissionBehavior)this).Mission.IsOrderMenuOpen)
			{
				return !_wasOrderMenuOpenLastFrame;
			}
			return false;
		}
	}

	protected bool IsDisplayingADialog
	{
		get
		{
			MissionScreen missionScreen = ((MissionView)this).MissionScreen;
			if (missionScreen == null || !((IMissionScreen)missionScreen).GetDisplayDialog())
			{
				MissionScreen missionScreen2 = ((MissionView)this).MissionScreen;
				if (missionScreen2 == null || !missionScreen2.IsRadialMenuActive)
				{
					return ((MissionBehavior)this).Mission?.IsOrderMenuOpen ?? false;
				}
			}
			return true;
		}
	}

	protected RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

	public override void OnBehaviorInitialize()
	{
		((MissionBehavior)this).OnBehaviorInitialize();
		NavalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
	}

	public override void OnPreMissionTick(float dt)
	{
		((MissionBehavior)this).OnPreMissionTick(dt);
		HandleShipControls(dt);
		HandleShipCamera(dt);
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		StandingPoint standingPoint;
		if (userAgent.IsMainAgent && (standingPoint = (StandingPoint)(object)((usedObject is StandingPoint) ? usedObject : null)) != null && GetUsableMachineFromPoint(standingPoint) is ShipControllerMachine shipControllerMachine && shipControllerMachine.AttachedShip.AnyActiveFormationTroopOnShip)
		{
			ControllerMachine = shipControllerMachine;
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)shipControllerMachine).GameEntity;
			RangedSiegeWeapon firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<RangedSiegeWeapon>(((WeakGameEntity)(ref gameEntity)).Root);
			if (firstScriptInFamilyDescending != null)
			{
				RangedSiegeWeapon = firstScriptInFamilyDescending;
			}
		}
	}

	public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		StandingPoint standingPoint;
		if (userAgent.IsPlayerControlled && (standingPoint = (StandingPoint)(object)((usedObject is StandingPoint) ? usedObject : null)) != null && GetUsableMachineFromPoint(standingPoint) is ShipControllerMachine)
		{
			RangedSiegeWeapon rangedSiegeWeapon = RangedSiegeWeapon;
			if (rangedSiegeWeapon != null)
			{
				rangedSiegeWeapon.SetPlayerForceUse(false);
			}
			ControllerMachine = null;
			RangedSiegeWeapon = null;
			((MissionBehavior)this).Mission.SetListenerAndAttenuationPosBlendFactor(0f);
		}
	}

	private static UsableMachine GetUsableMachineFromPoint(StandingPoint standingPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity val = ((ScriptComponentBehavior)standingPoint).GameEntity;
		while (((WeakGameEntity)(ref val)).IsValid && !((WeakGameEntity)(ref val)).HasScriptOfType<UsableMachine>())
		{
			val = ((WeakGameEntity)(ref val)).Parent;
		}
		if (((WeakGameEntity)(ref val)).IsValid)
		{
			UsableMachine firstScriptOfType = ((WeakGameEntity)(ref val)).GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType != null)
			{
				return firstScriptOfType;
			}
		}
		return null;
	}

	private void TickRowerInput(Vec2 inputVec, out RowerLongitudinalInput longitudinalRowerControl, out RowerLongitudinalInput longitudinalControlDoubleTap, out RowerLateralInput lateralRowerControl)
	{
		int num = 0;
		int num2 = 0;
		if (((Vec2)(ref inputVec)).LengthSquared > 0f)
		{
			((Vec2)(ref inputVec)).Normalize();
			float num3 = MBMath.ToDegrees(((Vec2)(ref inputVec)).RotationInRadians);
			bool flag = false;
			if (num3 < 0f)
			{
				flag = true;
				num3 = 0f - num3;
			}
			if (num3 <= 22.5f)
			{
				num = 1;
			}
			else if (num3 <= 67.5f)
			{
				num = 1;
				num2 = 1;
			}
			else if (num3 <= 112.5f)
			{
				num2 = 1;
			}
			else if (num3 < 157.5f)
			{
				num = -1;
				num2 = 1;
			}
			else
			{
				num = -1;
			}
			if (flag)
			{
				num2 = -num2;
			}
		}
		bool flag2 = num == 1 && _lastAccelerationAxisInput == 1;
		bool flag3 = num == -1 && _lastAccelerationAxisInput == -1;
		_lastAccelerationAxisInput = num;
		bool flag4 = false;
		bool flag5 = false;
		longitudinalRowerControl = RowerLongitudinalInput.None;
		longitudinalControlDoubleTap = RowerLongitudinalInput.None;
		switch (num)
		{
		case 1:
			longitudinalRowerControl = RowerLongitudinalInput.Forward;
			if (flag2 && _lastForwardKeyPressTime + 0.3f > Time.ApplicationTime)
			{
				longitudinalControlDoubleTap = RowerLongitudinalInput.Forward;
				flag4 = true;
			}
			break;
		case -1:
			longitudinalRowerControl = RowerLongitudinalInput.Backward;
			if (flag3 && _lastBackwardKeyPressTime + 0.3f > Time.ApplicationTime)
			{
				longitudinalControlDoubleTap = RowerLongitudinalInput.Backward;
				flag5 = true;
			}
			break;
		}
		lateralRowerControl = RowerLateralInput.None;
		switch (num2)
		{
		case -1:
			lateralRowerControl = RowerLateralInput.Right;
			break;
		case 1:
			lateralRowerControl = RowerLateralInput.Left;
			break;
		}
		if (!flag4 && flag2)
		{
			_lastForwardKeyPressTime = Time.ApplicationTime;
		}
		if (!flag5 && flag3)
		{
			_lastBackwardKeyPressTime = Time.ApplicationTime;
		}
	}

	private float TickRudderInput(Vec2 inputVec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return MathF.Min(MathF.Abs(inputVec.x) * 1.4f, 1f) * (float)MathF.Sign(inputVec.x);
	}

	private void HandleShipControls(float dt)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		_wasOrderMenuOpenLastFrame = ((MissionBehavior)this).Mission.IsOrderMenuOpen;
		MissionShip missionShip = NavalShipsLogic?.PlayerControlledShip;
		if (missionShip == null || !missionShip.IsPlayerControlled)
		{
			return;
		}
		PlayerShipController playerController = missionShip.PlayerController;
		RowerLongitudinalInput longitudinalRowerControl = RowerLongitudinalInput.None;
		RowerLongitudinalInput longitudinalControlDoubleTap = RowerLongitudinalInput.None;
		RowerLateralInput lateralRowerControl = RowerLateralInput.None;
		float rudderLateral = 0f;
		if (!((MissionView)this).MissionScreen.IsCheatGhostMode)
		{
			float gameKeyAxis = ((MissionView)this).Input.GetGameKeyAxis("MovementAxisY");
			float gameKeyAxis2 = ((MissionView)this).Input.GetGameKeyAxis("MovementAxisX");
			Vec2 val = default(Vec2);
			((Vec2)(ref val))._002Ector(gameKeyAxis2, gameKeyAxis);
			if (MathF.Abs(val.x) <= 0.2f)
			{
				val.x = 0f;
			}
			if (MathF.Abs(val.y) <= 0.2f)
			{
				val.y = 0f;
			}
			TickRowerInput(val, out longitudinalRowerControl, out longitudinalControlDoubleTap, out lateralRowerControl);
			rudderLateral = TickRudderInput(val);
		}
		playerController.SetInput(new ShipInputRecord(lateralRowerControl, longitudinalRowerControl, longitudinalControlDoubleTap, rudderLateral, SailControl));
	}

	public void SetSailInput(SailInput sailInput)
	{
		SailControl = sailInput;
	}

	public void SetActiveCameraMode(CameraModes mode)
	{
		ActiveCameraMode = mode;
	}

	private void HandleShipCamera(float dt)
	{
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		if (ControllerMachine != null)
		{
			if (RangedSiegeWeapon != null)
			{
				RangedSiegeWeaponView component = ((UsableMachine)RangedSiegeWeapon).GetComponent<RangedSiegeWeaponView>();
				if (component == null)
				{
					component = (RangedSiegeWeaponView)new BallistaView();
					component.Initialize(RangedSiegeWeapon, ((MissionView)this).MissionScreen);
					((UsableMachine)RangedSiegeWeapon).AddComponent((UsableMissionObjectComponent)(object)component);
				}
				RangedSiegeWeapon.SetPlayerForceUse(IsAimingWithRangedWeaponAndAllowed);
			}
			Agent pilotAgent = ((UsableMachine)ControllerMachine).PilotAgent;
			Vec3 val;
			Vec3 val2;
			float num;
			switch (ActiveCameraMode)
			{
			case CameraModes.Back:
				val = ControllerMachine.BackCameraOffset;
				val2 = ControllerMachine.BackCameraTargetLocalPosition;
				num = ControllerMachine.BackCameraFovMultiplier;
				if (((MissionBehavior)this).Mission.InputManager.IsGameKeyDown(28))
				{
					_backCameraDistanceMultiplier -= 0.5f * dt;
				}
				if (((MissionBehavior)this).Mission.InputManager.IsGameKeyDown(29))
				{
					_backCameraDistanceMultiplier += 0.5f * dt;
				}
				_backCameraDistanceMultiplier = MBMath.ClampFloat(_backCameraDistanceMultiplier, 0.2f, 3f);
				val *= 0.5f;
				((MissionBehavior)this).Mission.SetListenerAndAttenuationPosBlendFactor(0.33f);
				break;
			case CameraModes.Front:
				val = ControllerMachine.FrontCameraOffset;
				val2 = ControllerMachine.FrontCameraTargetLocalPosition;
				num = ControllerMachine.FrontCameraFovMultiplier;
				((MissionBehavior)this).Mission.SetListenerAndAttenuationPosBlendFactor(1f);
				break;
			default:
				val = ControllerMachine.ShoulderCameraOffset;
				val2 = ControllerMachine.ShoulderCameraTargetLocalPosition;
				num = ControllerMachine.ShoulderCameraFovMultiplier;
				((MissionBehavior)this).Mission.SetListenerAndAttenuationPosBlendFactor(0f);
				break;
			}
			bool flag = (!((Vec3)(ref _lastCameraOffset)).NearlyEquals(ref val, 0.001f) || MathF.Abs(_lastCameraFovMultiplier - num) > 0.001f) && !IsAimingWithRangedWeaponAndAllowed;
			_lastCameraOffset = (flag ? MBMath.Lerp(_lastCameraOffset, val, dt * 5f, 0.001f) : val);
			_lastCameraFovMultiplier = (flag ? MBMath.Lerp(_lastCameraFovMultiplier, num, dt * 5f, 0.001f) : num);
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)ControllerMachine).GameEntity;
			WeakGameEntity root = ((WeakGameEntity)(ref gameEntity)).Root;
			float num2;
			Vec3 val3;
			if (pilotAgent != null)
			{
				num2 = MBMath.WrapAngle(((MissionView)this).MissionScreen.CameraBearing - pilotAgent.MovementDirectionAsAngle);
				val3 = pilotAgent.Position;
			}
			else
			{
				num2 = MBMath.WrapAngle(((MissionView)this).MissionScreen.CameraBearing);
				val3 = ((WeakGameEntity)(ref root)).GlobalPosition;
			}
			Vec3 val4;
			if (!((Vec3)(ref val2)).IsNonZero)
			{
				val4 = Vec3.Zero;
			}
			else
			{
				Vec3 val5 = val3;
				gameEntity = ((ScriptComponentBehavior)ControllerMachine).GameEntity;
				MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
				Vec3 val6 = val5 - ((MatrixFrame)(ref globalFrame)).TransformToParent(ref val2);
				Vec3 val7;
				if (ActiveCameraMode != CameraModes.Shoulder)
				{
					if (ActiveCameraMode != CameraModes.Front)
					{
						val7 = Vec3.Zero;
					}
					else
					{
						gameEntity = ((ScriptComponentBehavior)ControllerMachine.AttachedShip).GameEntity;
						globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
						val7 = ((Vec3)(ref globalFrame.rotation.f)).NormalizedCopy() * MathF.Cos(Math.Min(MathF.Abs(num2) * 2.5f, MathF.PI / 2f)) * 8f;
					}
				}
				else
				{
					gameEntity = ((ScriptComponentBehavior)ControllerMachine.AttachedShip).GameEntity;
					globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
					val7 = ((Vec3)(ref globalFrame.rotation.s)).NormalizedCopy() * MathF.Sin(num2) * ControllerMachine.ShoulderCameraDistance;
				}
				val4 = val6 - val7;
			}
			Vec3 val8 = val4;
			Mission.Current.SetCustomCameraFixedDistance((ActiveCameraMode == CameraModes.Front) ? ControllerMachine.FrontCameraDistance : ((ActiveCameraMode == CameraModes.Back) ? (((Vec3)(ref val)).Length * _backCameraDistanceMultiplier) : float.MinValue));
			Mission.Current.SetCustomCameraTargetLocalOffset(MBMath.Lerp(Mission.Current.CustomCameraTargetLocalOffset, -val8, dt * 10f, 0.001f));
			if (ActiveCameraMode == CameraModes.Shoulder)
			{
				if (!flag)
				{
					Mission.Current.SetIgnoredEntityForCamera((GameEntity)null);
				}
			}
			else if (Mission.Current.IgnoredEntityForCamera != root)
			{
				Mission.Current.SetIgnoredEntityForCamera(GameEntity.CreateFromWeakEntity(root));
			}
			Mission.Current.SetCustomCameraIgnoreCollision(ActiveCameraMode == CameraModes.Front);
		}
		else
		{
			_lastCameraOffset = MBMath.Lerp(_lastCameraOffset, Vec3.Zero, dt * 5f, 0.001f);
			_lastCameraFovMultiplier = MBMath.Lerp(_lastCameraFovMultiplier, 1f, dt * 5f, 0.001f);
			Mission.Current.SetCustomCameraFixedDistance(float.MinValue);
			Mission.Current.SetCustomCameraTargetLocalOffset(MBMath.Lerp(Mission.Current.CustomCameraTargetLocalOffset, Vec3.Zero, dt * 5f, 0.001f));
			if (!((Vec3)(ref _lastCameraOffset)).IsNonZero)
			{
				Mission.Current.SetIgnoredEntityForCamera((GameEntity)null);
			}
			Mission.Current.SetCustomCameraIgnoreCollision(false);
		}
		Mission.Current.SetCustomCameraLocalOffset(_lastCameraOffset);
		Mission.Current.SetCustomCameraFovMultiplier(_lastCameraFovMultiplier);
	}

	protected override void OnCreateView()
	{
	}

	protected override void OnDestroyView()
	{
	}

	protected override void OnSuspendView()
	{
	}

	protected override void OnResumeView()
	{
	}
}

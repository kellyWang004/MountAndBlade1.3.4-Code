using System.Collections.Generic;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Objects.UsableMachines;

public class ShipControllerMachine : UsableMachine
{
	public const float CaptureTime = 3f;

	private const string ControllerEntityName = "controller";

	private const string HandTargetEntityName = "hand_position";

	private const string CameraTargetEntityName = "camera_target";

	private const string ShoulderCameraTargetEntityName = "shoulder_camera_target";

	private const string FrontCameraTargetEntityName = "front_camera_target";

	private const string RudderRotationEntityTag = "rudder_rotation_entity";

	public GameEntity _rudderRotationEntity;

	private MatrixFrame _rudderRotationEntityInitialLocalFrame;

	private GameEntity _cameraTargetEntity;

	private GameEntity _shoulderCameraTargetEntity;

	private GameEntity _frontCameraTargetEntity;

	private ActionIndexCache _shipControlActionIndex = ActionIndexCache.act_none;

	private ActionIndexCache _shipCaptureActionIndex = ActionIndexCache.act_none;

	private TextObject _overridenDescriptionForActiveEnemyShipControllerMachine;

	private NavalShipsLogic _navalShipsLogic;

	private NavalAgentsLogic _navalAgentsLogic;

	[EditableScriptComponentVariable(true, "")]
	private Vec3 _cameraOffset = new Vec3(0f, -20f, 5f, -1f);

	[EditableScriptComponentVariable(true, "")]
	private string _shipControlAction = "act_ship_control_rudder_push_to_left_right_idle";

	[EditableScriptComponentVariable(true, "")]
	private Vec3 _shoulderCameraOffset = new Vec3(0f, 0f, 0f, -1f);

	[EditableScriptComponentVariable(true, "")]
	private string _shipCaptureAction = "act_ship_control_rudder_push_to_left_right_idle";

	[EditableScriptComponentVariable(true, "")]
	private Vec3 _frontCameraOffset = new Vec3(0f, -10f, 2f, -1f);

	[EditableScriptComponentVariable(true, "")]
	private float _shoulderCameraDistance = 2f;

	[EditableScriptComponentVariable(true, "")]
	private bool _isLeftHandOnly;

	[EditableScriptComponentVariable(true, "")]
	private float _frontCameraDistance = 10f;

	[EditableScriptComponentVariable(true, "")]
	private bool _isRightHandOnly;

	[EditableScriptComponentVariable(true, "")]
	private float _cameraFovMultiplier = 1f;

	[EditableScriptComponentVariable(true, "")]
	private float _frontCameraFovMultiplier = 1f;

	[EditableScriptComponentVariable(true, "")]
	private float _shoulderCameraFovMultiplier = 1f;

	private float _captureTimer = -1f;

	public MissionShip AttachedShip { get; private set; }

	public GameEntity ControllerEntity { get; private set; }

	public GameEntity HandTargetEntity { get; private set; }

	public float CaptureTimer => _captureTimer;

	public Vec3 BackCameraOffset => _cameraOffset;

	public Vec3 ShoulderCameraOffset => _shoulderCameraOffset;

	public Vec3 FrontCameraOffset => _frontCameraOffset;

	public float ShoulderCameraDistance => _shoulderCameraDistance;

	public float FrontCameraDistance => _frontCameraDistance;

	public float BackCameraFovMultiplier => _cameraFovMultiplier;

	public float ShoulderCameraFovMultiplier => _shoulderCameraFovMultiplier;

	public float FrontCameraFovMultiplier => _frontCameraFovMultiplier;

	public Vec3 BackCameraTargetLocalPosition
	{
		get
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			GameEntity cameraTargetEntity = _cameraTargetEntity;
			if (cameraTargetEntity == null)
			{
				return Vec3.Zero;
			}
			return cameraTargetEntity.GetFrame().origin;
		}
	}

	public Vec3 ShoulderCameraTargetLocalPosition
	{
		get
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			GameEntity shoulderCameraTargetEntity = _shoulderCameraTargetEntity;
			if (shoulderCameraTargetEntity == null)
			{
				return Vec3.Zero;
			}
			return shoulderCameraTargetEntity.GetFrame().origin;
		}
	}

	public Vec3 FrontCameraTargetLocalPosition
	{
		get
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			GameEntity frontCameraTargetEntity = _frontCameraTargetEntity;
			if (frontCameraTargetEntity == null)
			{
				return Vec3.Zero;
			}
			return frontCameraTargetEntity.GetFrame().origin;
		}
	}

	protected override void OnInit()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		((UsableMachine)this).OnInit();
		WeakGameEntity val = ((ScriptComponentBehavior)this).GameEntity;
		AttachedShip = ((WeakGameEntity)(ref val)).GetFirstScriptOfTypeInFamily<MissionShip>();
		val = ((ScriptComponentBehavior)this).GameEntity;
		foreach (WeakGameEntity child in ((WeakGameEntity)(ref val)).GetChildren())
		{
			WeakGameEntity current = child;
			if (((WeakGameEntity)(ref current)).Name == "controller")
			{
				ControllerEntity = GameEntity.CreateFromWeakEntity(current);
				_rudderRotationEntity = ControllerEntity;
				_rudderRotationEntityInitialLocalFrame = _rudderRotationEntity.GetFrame();
				foreach (WeakGameEntity child2 in ((WeakGameEntity)(ref current)).GetChildren())
				{
					WeakGameEntity current2 = child2;
					if (((WeakGameEntity)(ref current2)).Name == "hand_position")
					{
						HandTargetEntity = GameEntity.CreateFromWeakEntity(current2);
					}
				}
			}
			else if (((WeakGameEntity)(ref current)).Name == "hand_position")
			{
				HandTargetEntity = GameEntity.CreateFromWeakEntity(current);
			}
			else if (((WeakGameEntity)(ref current)).Name == "camera_target")
			{
				_cameraTargetEntity = GameEntity.CreateFromWeakEntity(current);
			}
			else if (((WeakGameEntity)(ref current)).Name == "shoulder_camera_target")
			{
				_shoulderCameraTargetEntity = GameEntity.CreateFromWeakEntity(current);
			}
			else if (((WeakGameEntity)(ref current)).Name == "front_camera_target")
			{
				_frontCameraTargetEntity = GameEntity.CreateFromWeakEntity(current);
			}
		}
		if (_rudderRotationEntity == (GameEntity)null)
		{
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			val = ((ScriptComponentBehavior)this).GameEntity;
			val = ((WeakGameEntity)(ref val)).Root;
			((WeakGameEntity)(ref val)).GetChildrenWithTagRecursive(list, "rudder_rotation_entity");
			foreach (WeakGameEntity item in list)
			{
				_rudderRotationEntity = GameEntity.CreateFromWeakEntity(item);
				_rudderRotationEntityInitialLocalFrame = _rudderRotationEntity.GetFrame();
			}
		}
		_shipControlActionIndex = ActionIndexCache.Create(_shipControlAction);
		_shipCaptureActionIndex = ActionIndexCache.Create(_shipCaptureAction);
		((ScriptComponentBehavior)this).SetScriptComponentToTick(((ScriptComponentBehavior)this).GetTickRequirement());
		base.EnemyRangeToStopUsing = 5f;
	}

	public bool CheckControllerMachineFlags(bool editMode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		List<WeakGameEntity> list = new List<WeakGameEntity>();
		WeakGameEntity val = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref val)).GetChildrenRecursive(ref list);
		bool flag = false;
		list.Add(((ScriptComponentBehavior)this).GameEntity);
		foreach (WeakGameEntity item in list)
		{
			WeakGameEntity current = item;
			if (!Extensions.HasAnyFlag<EntityFlags>(((WeakGameEntity)(ref current)).EntityFlags, (EntityFlags)131072) && !Extensions.HasAnyFlag<EntityFlags>(((WeakGameEntity)(ref current)).EntityFlags, (EntityFlags)4096))
			{
				flag = true;
			}
		}
		if (flag)
		{
			val = ((ScriptComponentBehavior)this).GameEntity;
			val = ((WeakGameEntity)(ref val)).Root;
			string name = ((WeakGameEntity)(ref val)).Name;
			val = ((ScriptComponentBehavior)this).GameEntity;
			string text = $"In Root Entity {name}, {((WeakGameEntity)(ref val)).Name}'s every descendant including itself must have Does not Affect Parent's Local Bounding Box flag.";
			if (editMode)
			{
				MBEditor.AddEntityWarning(((ScriptComponentBehavior)this).GameEntity, text);
			}
		}
		return flag;
	}

	public override TickRequirement GetTickRequirement()
	{
		return (TickRequirement)2;
	}

	protected override void OnEditorTick(float dt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		if (!((WeakGameEntity)(ref gameEntity)).IsGhostObject())
		{
			UpdateVisualizer();
		}
	}

	public override void OnDeploymentFinished()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		EnsureStandingPointComponents();
		if (AttachedShip.BattleSide != Mission.Current.PlayerTeam.Side)
		{
			((UsableMachine)this).PilotStandingPoint.SetUsableByAIOnly();
		}
		_navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		_navalAgentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
	}

	private void EnsureStandingPointComponents()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		if (((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).GetComponent<ResetAnimationOnStopUsageComponent>() == null)
		{
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).AddComponent((UsableMissionObjectComponent)new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, false));
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).AddComponent((UsableMissionObjectComponent)new ClearHandInverseKinematicsOnStopUsageComponent());
		}
	}

	public override void OnPilotAssignedDuringSpawn()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		EnsureStandingPointComponents();
		bool flag = MBAnimation.GetAnimationBlendsWithActionIndex(MBActionSet.GetAnimationIndexOfAction(((UsableMachine)this).PilotAgent.ActionSet, ref _shipControlActionIndex)) >= 0f;
		((UsableMachine)this).PilotAgent.SetActionChannel(1, ref _shipControlActionIndex, false, (AnimFlags)71, flag ? 0.5f : 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)((UsableMachine)this).PilotStandingPoint).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		((UsableMachine)this).PilotAgent.TeleportToPosition(globalFrame.origin);
		((UsableMachine)this).PilotAgent.DisableScriptedMovement();
		Agent pilotAgent = ((UsableMachine)this).PilotAgent;
		Vec2 val = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		val = ((Vec2)(ref val)).Normalized();
		pilotAgent.SetMovementDirection(ref val);
	}

	protected override void OnTick(float dt)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		((UsableMachine)this).OnTick(dt);
		float visualRudderRotationPercentage = AttachedShip.VisualRudderRotationPercentage;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		float num = visualRudderRotationPercentage * (float)MathF.Sign(((WeakGameEntity)(ref gameEntity)).GetGlobalScale().x);
		if (_rudderRotationEntity != (GameEntity)null)
		{
			MatrixFrame rudderRotationEntityInitialLocalFrame = _rudderRotationEntityInitialLocalFrame;
			((Mat3)(ref rudderRotationEntityInitialLocalFrame.rotation)).RotateAboutUp(AttachedShip.VisualRudderRotation);
			_rudderRotationEntity.SetLocalFrame(ref rudderRotationEntityInitialLocalFrame, false);
		}
		if (_navalShipsLogic != null && ((UsableMachine)this).PilotAgent == null)
		{
			Agent main = Agent.Main;
			if (((main == null) ? null : main.Formation?.Team) != null && AttachedShip.BattleSide != Agent.Main.Formation.Team.Side)
			{
				((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).IsDisabledForPlayers = !AttachedShip.CanBeTakenOver || !IsAttachedShipVacant() || !MissionShip.AreShipsConnected(_navalShipsLogic.GetShipAssignment(Agent.Main.Formation.Team.TeamSide, Agent.Main.Formation.FormationIndex).MissionShip, AttachedShip);
			}
		}
		if (((UsableMachine)this).PilotAgent == null)
		{
			_captureTimer = -1f;
		}
		if (((UsableMachine)this).PilotAgent == null)
		{
			return;
		}
		if (IsAttachedShipVacant() && ((UsableMachine)this).PilotAgent.Formation != null)
		{
			MissionShip missionShip = _navalShipsLogic.GetShipAssignment(((UsableMachine)this).PilotAgent.Formation.Team.TeamSide, ((UsableMachine)this).PilotAgent.Formation.FormationIndex).MissionShip;
			if (MissionShip.AreShipsConnected(missionShip, AttachedShip))
			{
				if (!((UsableMachine)this).PilotAgent.SetActionChannel(0, ref _shipCaptureActionIndex, false, (AnimFlags)0, 0.5f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
				{
					return;
				}
				if (_captureTimer > 0f)
				{
					_captureTimer -= dt;
					if (_captureTimer <= 0f)
					{
						Agent pilotAgent = ((UsableMachine)this).PilotAgent;
						((UsableMachine)this).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
						OnShipCapturedByAgent(pilotAgent);
						missionShip.InvalidateActiveFormationTroopOnShipCache();
						AttachedShip.InvalidateActiveFormationTroopOnShipCache();
					}
				}
				else
				{
					_captureTimer = 3f;
				}
			}
			else
			{
				_captureTimer = -1f;
				((UsableMachine)this).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
			}
			return;
		}
		num = MBMath.Map(num, -1f, 1f, 0.95f, 0.05f);
		int animationIndexOfAction = MBActionSet.GetAnimationIndexOfAction(((UsableMachine)this).PilotAgent.ActionSet, ref _shipControlActionIndex);
		bool flag = MBAnimation.GetAnimationBlendsWithActionIndex(animationIndexOfAction) >= 0f;
		if (((UsableMachine)this).PilotAgent.SetActionChannel(1, ref _shipControlActionIndex, false, (AnimFlags)71, flag ? num : 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
		{
			if (!(HandTargetEntity != (GameEntity)null))
			{
				return;
			}
			Vec3 origin = HandTargetEntity.GetGlobalFrame().origin;
			float currentActionProgress = ((UsableMachine)this).PilotAgent.GetCurrentActionProgress(1);
			MatrixFrame frame = ((UsableMachine)this).PilotAgent.Frame;
			MatrixFrame val = ((UsableMachine)this).PilotAgent.GetBoneEntitialFrameAtAnimationProgress(((UsableMachine)this).PilotAgent.Monster.MainHandBoneIndex, animationIndexOfAction, currentActionProgress);
			MatrixFrame val2 = ((MatrixFrame)(ref frame)).TransformToParent(ref val);
			val = ((UsableMachine)this).PilotAgent.GetBoneEntitialFrameAtAnimationProgress(((UsableMachine)this).PilotAgent.Monster.OffHandBoneIndex, animationIndexOfAction, currentActionProgress);
			MatrixFrame val3 = ((MatrixFrame)(ref frame)).TransformToParent(ref val);
			if (_isLeftHandOnly)
			{
				val3.origin = origin;
				Agent pilotAgent2 = ((UsableMachine)this).PilotAgent;
				val = MatrixFrame.Identity;
				pilotAgent2.SetHandInverseKinematicsFrame(ref val3, ref val);
				return;
			}
			if (_isRightHandOnly)
			{
				val2.origin = origin;
				Agent pilotAgent3 = ((UsableMachine)this).PilotAgent;
				val = MatrixFrame.Identity;
				pilotAgent3.SetHandInverseKinematicsFrame(ref val, ref val2);
				return;
			}
			Vec3 val4;
			if (!(ControllerEntity != (GameEntity)null))
			{
				gameEntity = ((ScriptComponentBehavior)((UsableMachine)this).PilotStandingPoint).GameEntity;
				val = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
				val4 = ((Vec3)(ref val.rotation.s)).NormalizedCopy();
			}
			else
			{
				val = ControllerEntity.GetGlobalFrame();
				val4 = ((Vec3)(ref val.rotation.s)).NormalizedCopy();
			}
			Vec3 val5 = val4;
			float num2 = Vec3.DotProduct(val5, val2.origin - val3.origin);
			val2.origin = origin + 0.5f * num2 * val5;
			val3.origin = origin - 0.5f * num2 * val5;
			((UsableMachine)this).PilotAgent.SetHandInverseKinematicsFrame(ref val3, ref val2);
		}
		else if (((UsableMachine)this).PilotAgent.IsInBeingStruckAction)
		{
			((UsableMachine)this).PilotAgent.ClearHandInverseKinematics();
		}
		else
		{
			((UsableMachine)this).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
		}
	}

	private void OnShipCapturedByAgent(Agent captorAgent)
	{
		if (_navalShipsLogic != null)
		{
			_navalShipsLogic.SwapShipsBetweenTeams(AttachedShip, captorAgent.Formation);
		}
	}

	public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		TextObject val = new TextObject("{=fEQAPJ2e}{KEY} Use", (Dictionary<string, object>)null);
		val.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
		return val;
	}

	protected override float GetDetachmentWeightAux(BattleSideEnum side)
	{
		return float.MinValue;
	}

	public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		if (AttachedShip.BattleSide == Mission.Current.PlayerTeam.Side)
		{
			return new TextObject("{=6PmvlYcT}Control the Ship", (Dictionary<string, object>)null);
		}
		if (IsAttachedShipVacant())
		{
			if (MissionShip.AreShipsConnected(_navalShipsLogic.GetShipAssignment(Agent.Main.Formation.Team.TeamSide, Agent.Main.Formation.FormationIndex).MissionShip, AttachedShip))
			{
				return new TextObject("{=fOX1aVDv}Capture the ship", (Dictionary<string, object>)null);
			}
			if (_overridenDescriptionForActiveEnemyShipControllerMachine != (TextObject)null)
			{
				return _overridenDescriptionForActiveEnemyShipControllerMachine;
			}
			return new TextObject("{=lS53LgyN}You need to be boarded to capture the ship", (Dictionary<string, object>)null);
		}
		return new TextObject("{=UrBktTYi}Clear the crew to capture the ship", (Dictionary<string, object>)null);
	}

	public override UsableMachineAIBase CreateAIBehaviorObject()
	{
		return (UsableMachineAIBase)(object)new ShipControllerMachineAI(this);
	}

	private void UpdateVisualizer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		WeakGameEntity val = ((WeakGameEntity)(ref gameEntity)).GetFirstChildEntityWithTag("visualizer");
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		StandingPoint firstScriptOfTypeRecursive = ((WeakGameEntity)(ref gameEntity)).GetFirstScriptOfTypeRecursive<StandingPoint>();
		bool flag = false;
		if (_shipControlActionIndex == ActionIndexCache.act_none || ((ActionIndexCache)(ref _shipControlActionIndex)).GetName() != _shipControlAction)
		{
			_shipControlActionIndex = ActionIndexCache.Create(_shipControlAction);
			if (_shipControlActionIndex != ActionIndexCache.act_none)
			{
				flag = MBAnimation.GetAnimationBlendsWithActionIndex(MBActionSet.GetAnimationIndexOfAction(MBActionSet.GetActionSetWithIndex(0), ref _shipControlActionIndex)) >= 0f;
			}
		}
		if (_shipControlActionIndex != ActionIndexCache.act_none && firstScriptOfTypeRecursive != null)
		{
			_ = ((ScriptComponentBehavior)firstScriptOfTypeRecursive).GameEntity;
			if (!((WeakGameEntity)(ref val)).IsValid)
			{
				gameEntity = ((ScriptComponentBehavior)this).GameEntity;
				GameEntity val2 = GameEntity.CreateEmpty(((WeakGameEntity)(ref gameEntity)).Scene, false, true, true);
				val = val2.WeakEntity;
				((WeakGameEntity)(ref val)).SetEntityFlags((EntityFlags)(((WeakGameEntity)(ref val)).EntityFlags | 0x20000));
				((WeakGameEntity)(ref val)).SetName("visualizer");
				((WeakGameEntity)(ref val)).AddTag("visualizer");
				MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(0);
				GameEntityExtensions.CreateAgentSkeleton(val, "human_skeleton", true, actionSetWithIndex, "human", MBObjectManager.Instance.GetObject<Monster>("human"));
				MBSkeletonExtensions.SetAgentActionChannel(((WeakGameEntity)(ref val)).Skeleton, 0, ref _shipControlActionIndex, 0f, 0f, true, flag ? 0.5f : 0f);
				((WeakGameEntity)(ref val)).AddMultiMeshToSkeleton(MetaMesh.GetCopy("roman_cloth_tunic_a", true, false));
				((WeakGameEntity)(ref val)).AddMultiMeshToSkeleton(MetaMesh.GetCopy("casual_02_boots", true, false));
				((WeakGameEntity)(ref val)).AddMultiMeshToSkeleton(MetaMesh.GetCopy("hands_male_a", true, false));
				((WeakGameEntity)(ref val)).AddMultiMeshToSkeleton(MetaMesh.GetCopy("head_male_a", true, false));
				gameEntity = ((ScriptComponentBehavior)this).GameEntity;
				((WeakGameEntity)(ref gameEntity)).AddChild(val2.WeakEntity, false);
			}
		}
		if (((WeakGameEntity)(ref val)).IsValid)
		{
			gameEntity = ((ScriptComponentBehavior)firstScriptOfTypeRecursive).GameEntity;
			MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
			((WeakGameEntity)(ref val)).SetGlobalFrame(ref globalFrame, true);
			if (MBSkeletonExtensions.GetActionAtChannel(((WeakGameEntity)(ref val)).Skeleton, 0) != _shipControlActionIndex)
			{
				MBSkeletonExtensions.SetAgentActionChannel(((WeakGameEntity)(ref val)).Skeleton, 0, ref _shipControlActionIndex, 0f, 0f, true, flag ? 0.5f : 0f);
			}
		}
	}

	public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
	{
		return false;
	}

	public bool IsAttachedShipVacant()
	{
		if (AttachedShip.Formation != null)
		{
			if (!AttachedShip.AnyActiveFormationTroopOnShip)
			{
				return _navalAgentsLogic.GetReservedTroopsCountOfShip(AttachedShip) <= 0;
			}
			return false;
		}
		return true;
	}

	public override void OnMissionEnded()
	{
	}

	public void SetOverridenDescriptionForActiveEnemyShipControllerMachine(TextObject description)
	{
		_overridenDescriptionForActiveEnemyShipControllerMachine = description;
	}
}

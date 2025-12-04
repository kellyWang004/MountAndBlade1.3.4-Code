using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines;

public class ShipOarMachine : UsableMachine, IShipOarScriptComponent
{
	private GameEntity _oarEntity;

	private MatrixFrame _handTargetLocalFrame;

	private MatrixFrame _oarExtractedEntitialFrame;

	private MatrixFrame _oarRetractedEntitialFrame;

	private MissionOar _oar;

	private float _lastIdleTime;

	private ActionIndexCache _rowIdleActionIndex;

	private ActionIndexCache _rowLoopActionIndex;

	private ActionIndexCache _rowDeathActionIndex;

	private ActionIndexCache _rowSitDownActionIndex;

	private ActionIndexCache _rowStandUpActionIndex;

	private bool _isPilotSitting;

	private Agent _lastPilotAgent;

	private (float, StopUsingGameObjectFlags) _pilotRemovalTime;

	private readonly List<GameEntity> _disablingAttachmentRampEntities = new List<GameEntity>();

	private BoundingBox _oarMachineBaseBoundingBox;

	[EditableScriptComponentVariable(true, "")]
	private string _rowIdleAction = "act_usage_row_idle_right";

	[EditableScriptComponentVariable(true, "")]
	private string _rowLoopAction = "act_usage_row_loop_right";

	[EditableScriptComponentVariable(true, "")]
	private string _rowDeathAction = "act_row_death_right";

	[EditableScriptComponentVariable(true, "")]
	private string _rowSitDownAction = "act_row_sit_down_right";

	[EditableScriptComponentVariable(true, "")]
	private string _rowStandUpAction = "act_row_stand_up_right";

	public ResetAnimationOnStopUsageComponent ResetAnimationOnStopUsageComponent { get; private set; }

	public override bool IsFocusable => false;

	protected override void OnInit()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		((UsableMachine)this).OnInit();
		ShipOarDeck.LoadOarScriptEntity(((ScriptComponentBehavior)this).GameEntity, out var oarEntity, ref _oarExtractedEntitialFrame, ref _oarRetractedEntitialFrame, out var handTargetEntity);
		_oarEntity = (((WeakGameEntity)(ref oarEntity)).IsValid ? GameEntity.CreateFromWeakEntity(oarEntity) : null);
		_handTargetLocalFrame = (((WeakGameEntity)(ref handTargetEntity)).IsValid ? ((WeakGameEntity)(ref handTargetEntity)).GetLocalFrame() : MatrixFrame.Identity);
		_rowIdleActionIndex = ActionIndexCache.Create(_rowIdleAction);
		_rowLoopActionIndex = ActionIndexCache.Create(_rowLoopAction);
		_rowDeathActionIndex = ActionIndexCache.Create(_rowDeathAction);
		_rowSitDownActionIndex = ActionIndexCache.Create(_rowSitDownAction);
		_rowStandUpActionIndex = ActionIndexCache.Create(_rowStandUpAction);
		((ScriptComponentBehavior)this).SetScriptComponentToTick(((ScriptComponentBehavior)this).GetTickRequirement());
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref gameEntity)).SetHasCustomBoundingBoxValidationSystem(true);
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		_oarMachineBaseBoundingBox = ((WeakGameEntity)(ref gameEntity)).ComputeBoundingBoxFromLongestHalfDimension(2f);
		ResetAnimationOnStopUsageComponent = new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, true);
		base.EnemyRangeToStopUsing = 5f;
		((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).SetIsDisabledForPlayersSynched(true);
	}

	public void InitializeOar(MissionOar oar)
	{
		_oar = oar;
	}

	public override void OnDeploymentFinished()
	{
		EnsureStandingPointComponents();
	}

	private void EnsureStandingPointComponents()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		if (((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).GetComponent<ResetAnimationOnStopUsageComponent>() == null)
		{
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).AddComponent((UsableMissionObjectComponent)(object)ResetAnimationOnStopUsageComponent);
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).AddComponent((UsableMissionObjectComponent)new ClearHandInverseKinematicsOnStopUsageComponent());
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).AddComponent((UsableMissionObjectComponent)new OverrideStrikeAndDeathActionDuringUsageComponent(ref ActionIndexCache.act_row_strike, ref _rowDeathActionIndex));
		}
	}

	public override TickRequirement GetTickRequirement()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return (TickRequirement)(((UsableMachine)this).GetTickRequirement() | 8);
	}

	public void ArrangeOarBoundingBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity val = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref val)).SetManualLocalBoundingBox(ref _oarMachineBaseBoundingBox);
		val = ((ScriptComponentBehavior)this).GameEntity;
		val = ((WeakGameEntity)(ref val)).Parent;
		((WeakGameEntity)(ref val)).SetBoundingboxDirty();
	}

	protected override void OnBoundingBoxValidate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		BoundingBox val = ((WeakGameEntity)(ref gameEntity)).ComputeBoundingBoxIncludeChildren();
		((BoundingBox)(ref val)).RelaxWithBoundingBox(_oarMachineBaseBoundingBox);
		((BoundingBox)(ref val)).RecomputeRadius();
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref gameEntity)).RelaxLocalBoundingBox(ref val);
	}

	public bool CheckOarMachineFlags(bool editMode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity val = ((ScriptComponentBehavior)this).GameEntity;
		foreach (WeakGameEntity child in ((WeakGameEntity)(ref val)).GetChildren())
		{
			WeakGameEntity current = child;
			if (!Extensions.HasAnyFlag<EntityFlags>(((WeakGameEntity)(ref current)).EntityFlags, (EntityFlags)131072) && !Extensions.HasAnyFlag<EntityFlags>(((WeakGameEntity)(ref current)).EntityFlags, (EntityFlags)4096))
			{
				string[] obj = new string[7] { "Root Entity: ", null, null, null, null, null, null };
				val = ((ScriptComponentBehavior)this).GameEntity;
				val = ((WeakGameEntity)(ref val)).Root;
				obj[1] = ((WeakGameEntity)(ref val)).Name;
				obj[2] = " ";
				val = ((ScriptComponentBehavior)this).GameEntity;
				obj[3] = ((WeakGameEntity)(ref val)).Name;
				obj[4] = "'s child ";
				obj[5] = ((WeakGameEntity)(ref current)).Name;
				obj[6] = " must have Does not Affect Parent's Local Bounding Box flag.";
				string text = string.Concat(obj);
				if (editMode)
				{
					MBEditor.AddEntityWarning(current, text);
				}
				return false;
			}
		}
		return true;
	}

	public void SetSlowDownPhaseForDuration(float slowDownMultiplier, float slowDownDuration)
	{
		_oar.SetSlowDownPhaseForDuration(slowDownMultiplier, slowDownDuration);
	}

	public void RegisterRampEntityDisablingOar(GameEntity rampEntity)
	{
		if (_disablingAttachmentRampEntities.Count == 0)
		{
			if (((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).HasUser)
			{
				((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).UserAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
			}
			else if (((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).HasAIMovingTo)
			{
				((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).MovingAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
			}
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).SetIsDeactivatedSynched(true);
		}
		if (!_disablingAttachmentRampEntities.Contains(rampEntity))
		{
			_disablingAttachmentRampEntities.Add(rampEntity);
		}
	}

	public void DeregisterRampEntityDisablingOar(GameEntity rampEntity)
	{
		if (_disablingAttachmentRampEntities.Remove(rampEntity) && _disablingAttachmentRampEntities.Count == 0)
		{
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).SetIsDeactivatedSynched(false);
		}
	}

	public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
	{
		return false;
	}

	public override void OnPilotAssignedDuringSpawn()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		EnsureStandingPointComponents();
		_lastPilotAgent = ((UsableMachine)this).PilotAgent;
		_isPilotSitting = true;
		((UsableMachine)this).PilotAgent.SetActionChannel(0, ref _rowIdleActionIndex, false, (AnimFlags)0, 0f, 1f, 0f, 0f, 1f, false, -0.2f, 0, true);
		Vec3 animationDisplacementAtProgress = MBAnimation.GetAnimationDisplacementAtProgress(MBActionSet.GetAnimationIndexOfAction(((UsableMachine)this).PilotAgent.ActionSet, ref _rowSitDownActionIndex), 1f);
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)((UsableMachine)this).PilotStandingPoint).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		((Mat3)(ref globalFrame.rotation)).Orthonormalize();
		Vec3 val = ((MatrixFrame)(ref globalFrame)).TransformToParent(ref animationDisplacementAtProgress);
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Vec2 val2 = ((Vec2)(ref asVec)).Normalized();
		((UsableMachine)this).PilotAgent.TeleportToPosition(val);
		((UsableMachine)this).PilotAgent.DisableScriptedMovement();
		((UsableMachine)this).PilotAgent.SetMovementDirection(ref val2);
		Agent pilotAgent = ((UsableMachine)this).PilotAgent;
		asVec = ((Vec3)(ref val)).AsVec2;
		Vec3 val3 = ((Vec2)(ref val2)).ToVec3(0f);
		pilotAgent.SetTargetPositionAndDirection(ref asVec, ref val3);
		_oar.SetOarForceMultiplierFromUserAgent(MissionGameModels.Current.MissionShipParametersModel.CalculateOarForceMultiplier(((UsableMachine)this).PilotAgent, 1f));
		_oar.OnPilotAssignedDuringSpawn();
	}

	public void StartDelayedPilotRemoval(StopUsingGameObjectFlags flags)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_pilotRemovalTime.Item1 <= 0f)
		{
			_pilotRemovalTime = (Mission.Current.CurrentTime + MBRandom.RandomFloat * 2f, flags);
		}
	}

	protected override void OnTickParallel2(float dt)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Invalid comparison between Unknown and I4
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame oarMachineLocalFrame;
		Vec2 val;
		if (_lastPilotAgent != ((UsableMachine)this).PilotAgent)
		{
			StandingPoint pilotStandingPoint = ((UsableMachine)this).PilotStandingPoint;
			oarMachineLocalFrame = MatrixFrame.Identity;
			((UsableMissionObject)pilotStandingPoint).SetCustomLocalFrame(ref oarMachineLocalFrame);
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = true;
			_isPilotSitting = false;
			if (((UsableMachine)this).PilotAgent != null)
			{
				WorldFrame userFrameForAgent = ((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).GetUserFrameForAgent(((UsableMachine)this).PilotAgent);
				Agent pilotAgent = ((UsableMachine)this).PilotAgent;
				val = ((WorldPosition)(ref userFrameForAgent.Origin)).AsVec2;
				pilotAgent.SetTargetPositionAndDirection(ref val, ref userFrameForAgent.Rotation.f);
				((UsableMachine)this).PilotAgent.SetScriptedFlags((AIScriptedFrameFlags)(((UsableMachine)this).PilotAgent.GetScriptedFlags() | 2));
				_oar.SetOarForceMultiplierFromUserAgent(MissionGameModels.Current.MissionShipParametersModel.CalculateOarForceMultiplier(((UsableMachine)this).PilotAgent, 1f));
			}
		}
		_lastPilotAgent = ((UsableMachine)this).PilotAgent;
		bool flag = ((UsableMachine)this).PilotAgent != null;
		bool flag2 = false;
		_oar.SetUsed(flag, flag ? ((UsableMachine)this).PilotAgent.Index : (-1));
		MissionOar oar = _oar;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		oarMachineLocalFrame = ((WeakGameEntity)(ref gameEntity)).GetLocalFrame();
		MatrixFrame val2 = oar.ComputeOarEntityFrame(dt, in oarMachineLocalFrame, _oarEntity.GetLocalFrame(), in _oarExtractedEntitialFrame, in _oarRetractedEntitialFrame, _lastIdleTime, forUnmanned: false);
		_oarEntity.SetLocalFrame(ref val2, false);
		if (flag)
		{
			Vec3 val3;
			if (_pilotRemovalTime.Item1 > 0f && _pilotRemovalTime.Item1 < Mission.Current.CurrentTime)
			{
				((UsableMachine)this).PilotAgent.StopUsingGameObjectMT(true, _pilotRemovalTime.Item2);
				_pilotRemovalTime = (0f, (StopUsingGameObjectFlags)0);
			}
			else if (!_isPilotSitting)
			{
				if (((UsableMachine)this).PilotAgent.GetCurrentAction(0) != _rowStandUpActionIndex)
				{
					if ((int)((UsableMachine)this).PilotAgent.MovementLockedState != 0)
					{
						gameEntity = ((ScriptComponentBehavior)((UsableMachine)this).PilotStandingPoint).GameEntity;
						MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
						Agent pilotAgent2 = ((UsableMachine)this).PilotAgent;
						val = ((Vec3)(ref globalFrame.origin)).AsVec2;
						pilotAgent2.SetTargetPositionAndDirection(ref val, ref globalFrame.rotation.f);
						((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = true;
						val = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
						if (Vec2.DotProduct(((Vec2)(ref val)).Normalized(), ((UsableMachine)this).PilotAgent.GetMovementDirection() * 1.5f) > 0.99f)
						{
							val = ((UsableMachine)this).PilotAgent.GetTargetPosition();
							val3 = ((UsableMachine)this).PilotAgent.Position;
							if (((Vec2)(ref val)).DistanceSquared(((Vec3)(ref val3)).AsVec2) < 0.01f)
							{
								((UsableMachine)this).PilotAgent.ClearTargetFrame();
								((UsableMachine)this).PilotAgent.SetActionChannel(0, ref _rowSitDownActionIndex, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
								((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = false;
							}
						}
					}
					else if (((UsableMachine)this).PilotAgent.GetCurrentAction(0) == _rowSitDownActionIndex)
					{
						gameEntity = ((ScriptComponentBehavior)((UsableMachine)this).PilotStandingPoint).GameEntity;
						MatrixFrame globalFrame2 = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
						Agent pilotAgent3 = ((UsableMachine)this).PilotAgent;
						val = ((Vec3)(ref globalFrame2.origin)).AsVec2;
						pilotAgent3.SetTargetPositionAndDirection(ref val, ref globalFrame2.rotation.f);
						((UsableMachine)this).PilotAgent.ClearTargetFrame();
						((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = false;
						if (((UsableMachine)this).PilotAgent.GetCurrentActionProgress(0) > 0.99f)
						{
							_isPilotSitting = true;
							((UsableMachine)this).PilotAgent.SetActionChannel(0, ref ActionIndexCache.act_usage_row_idle_no_hold, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						}
						else if (((UsableMachine)this).PilotAgent.GetCurrentActionProgress(0) > 0.25f)
						{
							flag2 = true;
						}
					}
					else
					{
						((UsableMachine)this).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)1);
					}
				}
			}
			else
			{
				int animationIndexOfAction = MBActionSet.GetAnimationIndexOfAction(((UsableMachine)this).PilotAgent.ActionSet, ref _rowSitDownActionIndex);
				StandingPoint pilotStandingPoint2 = ((UsableMachine)this).PilotStandingPoint;
				Mat3 identity = Mat3.Identity;
				val3 = MBAnimation.GetAnimationDisplacementAtProgress(animationIndexOfAction, 1f);
				oarMachineLocalFrame = new MatrixFrame(ref identity, ref val3);
				((UsableMissionObject)pilotStandingPoint2).SetCustomLocalFrame(ref oarMachineLocalFrame);
				((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = true;
				if (_oar.IsExtracted)
				{
					float num = ((_oar.VisualPhase + MathF.PI / 2f) / (MathF.PI * 2f) + 1f) % 1f;
					bool flag3 = _oar.IsInRowingMotion();
					ActionIndexCache val4 = (flag3 ? _rowLoopActionIndex : _rowIdleActionIndex);
					ActionIndexCache val5 = ((UsableMachine)this).PilotAgent.GetCurrentAction(0);
					ActionIndexCache val6 = ((UsableMachine)this).PilotAgent.GetCurrentAction(1);
					if (((UsableMachine)this).PilotAgent.SetActionChannel(0, ref val4, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						val5 = val4;
						if (flag3)
						{
							((UsableMachine)this).PilotAgent.SetCurrentActionProgress(0, num);
						}
					}
					bool isInBeingStruckAction = ((UsableMachine)this).PilotAgent.IsInBeingStruckAction;
					if (!isInBeingStruckAction && ((UsableMachine)this).PilotAgent.SetActionChannel(1, ref val4, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						val6 = val4;
						if (flag3)
						{
							((UsableMachine)this).PilotAgent.SetCurrentActionProgress(1, num);
						}
					}
					if (isInBeingStruckAction || (val5 == val4 && val6 == val4))
					{
						MBActionSet actionSet = ((UsableMachine)this).PilotAgent.ActionSet;
						ActionIndexCache val7 = ((val6 != ActionIndexCache.act_none) ? val6 : val5);
						int animationIndexOfAction2 = MBActionSet.GetAnimationIndexOfAction(actionSet, ref val7);
						MatrixFrame frame = ((UsableMachine)this).PilotAgent.Frame;
						oarMachineLocalFrame = ((UsableMachine)this).PilotAgent.GetBoneEntitialFrameAtAnimationProgress(((UsableMachine)this).PilotAgent.Monster.MainHandBoneIndex, animationIndexOfAction2, num);
						MatrixFrame val8 = ((MatrixFrame)(ref frame)).TransformToParent(ref oarMachineLocalFrame);
						oarMachineLocalFrame = ((UsableMachine)this).PilotAgent.GetBoneEntitialFrameAtAnimationProgress(((UsableMachine)this).PilotAgent.Monster.OffHandBoneIndex, animationIndexOfAction2, num);
						MatrixFrame val9 = ((MatrixFrame)(ref frame)).TransformToParent(ref oarMachineLocalFrame);
						oarMachineLocalFrame = _oarEntity.GetGlobalFrame();
						Vec3 val10 = ((Vec3)(ref oarMachineLocalFrame.rotation.f)).NormalizedCopy();
						float num2 = Vec3.DotProduct(val10, val8.origin - val9.origin);
						oarMachineLocalFrame = _oarEntity.GetGlobalFrame();
						MatrixFrame val11 = ((MatrixFrame)(ref oarMachineLocalFrame)).TransformToParent(ref _handTargetLocalFrame);
						val8.origin = val11.origin + 0.5f * num2 * val10;
						val9.origin = val11.origin - 0.5f * num2 * val10;
						((UsableMachine)this).PilotAgent.SetHandInverseKinematicsFrame(ref val9, ref val8);
					}
					else
					{
						((UsableMachine)this).PilotAgent.ClearHandInverseKinematics();
						((UsableMachine)this).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)1);
					}
				}
				else
				{
					((UsableMachine)this).PilotAgent.SetActionChannel(0, ref ActionIndexCache.act_usage_row_idle_no_hold, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					if (!((UsableMachine)this).PilotAgent.IsInBeingStruckAction)
					{
						((UsableMachine)this).PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_row_idle_no_hold, false, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					}
					((UsableMachine)this).PilotAgent.ClearHandInverseKinematics();
				}
			}
		}
		else
		{
			StandingPoint pilotStandingPoint3 = ((UsableMachine)this).PilotStandingPoint;
			oarMachineLocalFrame = MatrixFrame.Identity;
			((UsableMissionObject)pilotStandingPoint3).SetCustomLocalFrame(ref oarMachineLocalFrame);
			((UsableMissionObject)((UsableMachine)this).PilotStandingPoint).LockUserFrames = true;
			_isPilotSitting = false;
			_pilotRemovalTime = (0f, (StopUsingGameObjectFlags)0);
		}
		ResetAnimationOnStopUsageComponent.UpdateSuccessfulResetAction((((UsableMachine)this).PilotAgent != null && (_isPilotSitting || flag2) && (int)((UsableMachine)this).PilotAgent.Mission.Mode != 6) ? _rowStandUpActionIndex : ActionIndexCache.act_none);
		if (!flag || !_oar.IsExtracted)
		{
			_lastIdleTime = Mission.Current.CurrentTime;
		}
	}

	protected override float GetDetachmentWeightAux(BattleSideEnum side)
	{
		return float.MinValue;
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

	public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		return new TextObject("{=4b2SXZG8}Oar", (Dictionary<string, object>)null);
	}

	public override UsableMachineAIBase CreateAIBehaviorObject()
	{
		return (UsableMachineAIBase)(object)new ShipOarMachineAI(this);
	}
}

using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5;

public class Quest5CutLooseObjective : MissionObjective
{
	private class CutLooseObjectiveTarget : MissionObjectiveTarget
	{
		private readonly ShipAttachmentMachine _attachmentMachine;

		private readonly ShipAttachmentPointMachine _attachmentPointMachine;

		public CutLooseObjectiveTarget(ShipAttachmentMachine attachmentMachine)
		{
			_attachmentMachine = attachmentMachine;
		}

		public CutLooseObjectiveTarget(ShipAttachmentPointMachine attachmentPointMachine)
		{
			_attachmentPointMachine = attachmentPointMachine;
		}

		public override bool IsActive()
		{
			return !IsCutLoose();
		}

		public bool IsCutLoose()
		{
			if (_attachmentMachine != null)
			{
				return _attachmentMachine.CurrentAttachment == null;
			}
			if (_attachmentPointMachine != null)
			{
				return _attachmentPointMachine.CurrentAttachment == null;
			}
			return true;
		}

		public override Vec3 GetGlobalPosition()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			WeakGameEntity gameEntity;
			if (_attachmentMachine != null)
			{
				gameEntity = ((ScriptComponentBehavior)_attachmentMachine).GameEntity;
				return ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
			}
			if (_attachmentPointMachine != null)
			{
				gameEntity = ((ScriptComponentBehavior)_attachmentPointMachine).GameEntity;
				return ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
			}
			return Vec3.Zero;
		}

		public override TextObject GetName()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			return new TextObject("{=Cx5qU2jG}Ties", (Dictionary<string, object>)null);
		}
	}

	private readonly List<ShipAttachmentMachine> _attachmentMachines;

	private readonly List<ShipAttachmentPointMachine> _attachmentPointMachines;

	private MissionObjectiveProgressInfo _cachedProgress;

	public override string UniqueId => "naval_storyline_quest_5_cut_loose_objective";

	public override TextObject Name => new TextObject("{=1IpNoNL4}Cut Loose", (Dictionary<string, object>)null);

	public override TextObject Description => new TextObject("{=2cCuu7kv}Cut the prisoner ship loose, so you can sail it to safety.", (Dictionary<string, object>)null);

	public Quest5CutLooseObjective(Mission mission, List<ShipAttachmentMachine> attachmentMachines, List<ShipAttachmentPointMachine> attachmentPointMachines)
		: base(mission)
	{
		_attachmentMachines = attachmentMachines;
		_attachmentPointMachines = attachmentPointMachines;
		foreach (ShipAttachmentMachine attachmentMachine in _attachmentMachines)
		{
			if (attachmentMachine.CurrentAttachment != null)
			{
				CutLooseObjectiveTarget cutLooseObjectiveTarget = new CutLooseObjectiveTarget(attachmentMachine);
				((MissionObjective)this).AddTarget((MissionObjectiveTarget)(object)cutLooseObjectiveTarget);
			}
		}
		foreach (ShipAttachmentPointMachine attachmentPointMachine in _attachmentPointMachines)
		{
			if (attachmentPointMachine.CurrentAttachment != null)
			{
				CutLooseObjectiveTarget cutLooseObjectiveTarget2 = new CutLooseObjectiveTarget(attachmentPointMachine);
				((MissionObjective)this).AddTarget((MissionObjectiveTarget)(object)cutLooseObjectiveTarget2);
			}
		}
	}

	protected override void OnTick(float dt)
	{
		((MissionObjective)this).OnTick(dt);
		foreach (ShipAttachmentMachine attachmentMachine in _attachmentMachines)
		{
			if (attachmentMachine.CurrentAttachment != null)
			{
				continue;
			}
			foreach (StandingPoint item in (List<StandingPoint>)(object)((UsableMachine)attachmentMachine).StandingPoints)
			{
				((UsableMissionObject)item).IsDisabledForPlayers = true;
			}
		}
		foreach (ShipAttachmentPointMachine attachmentPointMachine in _attachmentPointMachines)
		{
			if (attachmentPointMachine.CurrentAttachment != null)
			{
				continue;
			}
			foreach (StandingPoint item2 in (List<StandingPoint>)(object)((UsableMachine)attachmentPointMachine).StandingPoints)
			{
				((UsableMissionObject)item2).IsDisabledForPlayers = true;
			}
		}
		MBReadOnlyList<CutLooseObjectiveTarget> targetsCopy = ((MissionObjective)this).GetTargetsCopy<CutLooseObjectiveTarget>();
		_cachedProgress.RequiredProgressAmount = ((List<CutLooseObjectiveTarget>)(object)targetsCopy).Count;
		_cachedProgress.CurrentProgressAmount = ((IEnumerable<CutLooseObjectiveTarget>)targetsCopy).Count((CutLooseObjectiveTarget t) => t.IsCutLoose());
	}

	public override MissionObjectiveProgressInfo GetCurrentProgress()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return _cachedProgress;
	}

	protected override bool IsActivationRequirementsMet()
	{
		return true;
	}

	protected override bool IsCompletionRequirementsMet()
	{
		return _cachedProgress.CurrentProgressAmount == _cachedProgress.RequiredProgressAmount;
	}
}

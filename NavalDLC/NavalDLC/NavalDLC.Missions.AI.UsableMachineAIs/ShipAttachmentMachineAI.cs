using System.Collections.Generic;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs;

public sealed class ShipAttachmentMachineAI : UsableMachineAIBase
{
	public override bool HasActionCompleted => false;

	protected override MovementOrder NextOrder => MovementOrder.MovementOrderCharge;

	private ShipAttachmentMachine ShipAttachmentMachine => base.UsableMachine as ShipAttachmentMachine;

	public ShipAttachmentMachineAI(ShipAttachmentMachine shipAttachmentMachine)
		: base((UsableMachine)(object)shipAttachmentMachine)
	{
	}

	protected override void OnTick(Agent agentToCompareTo, Formation formationToCompareTo, Team potentialUsersTeam, float dt)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		if (!Mission.Current.MissionEnded && ShipAttachmentMachine.OwnerShip.FireHitPoints <= 0f && ShipAttachmentMachine.CurrentAttachment == null && !((UsableMissionObject)((UsableMachine)ShipAttachmentMachine).PilotStandingPoint).HasAIMovingTo && ((UsableMachine)ShipAttachmentMachine).PilotAgent == null)
		{
			MBReadOnlyList<IFormationUnit> allUnits = formationToCompareTo.Arrangement.GetAllUnits();
			for (int num = ((List<IFormationUnit>)(object)allUnits).Count - 1; num >= 0; num--)
			{
				Agent val = (Agent)((List<IFormationUnit>)(object)allUnits)[num];
				if (val.IsAIControlled && val.HumanAIComponent.GetCurrentlyMovingGameObject() == null && !val.IsUsingGameObject && ShipAttachmentMachine.OwnerShip.GetIsAgentOnShip(val) && !val.HumanAIComponent.HasTimedScriptedFrame)
				{
					((UsableMachine)ShipAttachmentMachine).AddAgentAtSlotIndex(val, 0);
					break;
				}
			}
		}
		else if (((UsableMissionObject)((UsableMachine)ShipAttachmentMachine).PilotStandingPoint).HasAIMovingTo && ((UsableMissionObject)((UsableMachine)ShipAttachmentMachine).PilotStandingPoint).MovingAgent.Formation == null)
		{
			((UsableMachineAIBase)this).OnTick(agentToCompareTo, (Formation)null, potentialUsersTeam, dt);
		}
		else
		{
			((UsableMachineAIBase)this).OnTick(agentToCompareTo, formationToCompareTo, potentialUsersTeam, dt);
		}
	}
}

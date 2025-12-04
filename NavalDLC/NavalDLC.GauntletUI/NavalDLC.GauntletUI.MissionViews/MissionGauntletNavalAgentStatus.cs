using NavalDLC.Missions.MissionLogics;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.GauntletUI.MissionViews;

[OverrideView(typeof(MissionAgentStatusUIHandler))]
internal class MissionGauntletNavalAgentStatus : MissionGauntletAgentStatus
{
	private NavalShipsLogic _navalShipsLogic;

	public override void OnMissionScreenInitialize()
	{
		((MissionGauntletAgentStatus)this).OnMissionScreenInitialize();
		_navalShipsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalShipsLogic>();
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionGauntletAgentStatus)this).OnMissionScreenTick(dt);
		base._dataSource.IsAgentStatusPrioritized = _navalShipsLogic?.PlayerControlledShip == null;
	}
}

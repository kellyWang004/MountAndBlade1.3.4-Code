using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.ComponentInterfaces;

public class NavalCustomBattleInitializationModel : BattleInitializationModel
{
	public override List<FormationClass> GetAllAvailableTroopTypes()
	{
		return ((MBGameModel<BattleInitializationModel>)this).BaseModel.GetAllAvailableTroopTypes();
	}

	protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
	{
		return Mission.Current.GetMissionBehavior<IMissionAgentSpawnLogic>().GetNumberOfPlayerControllableTroops() >= 20;
	}
}

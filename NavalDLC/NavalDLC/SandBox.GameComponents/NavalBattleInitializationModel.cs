using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents;

public class NavalBattleInitializationModel : BattleInitializationModel
{
	public override List<FormationClass> GetAllAvailableTroopTypes()
	{
		return ((MBGameModel<BattleInitializationModel>)this).BaseModel.GetAllAvailableTroopTypes();
	}

	protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (Mission.Current.IsSallyOutBattle)
		{
			return false;
		}
		MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
		if (MapEvent.PlayerMapEvent == null)
		{
			return false;
		}
		PartyBase leaderParty = playerMapEvent.GetLeaderParty(playerMapEvent.PlayerSide);
		if (leaderParty == PartyBase.MainParty || (leaderParty.IsSettlement && leaderParty.Settlement.OwnerClan.Leader == Hero.MainHero) || playerMapEvent.IsPlayerSergeant())
		{
			return Mission.Current.GetMissionBehavior<IMissionAgentSpawnLogic>().GetNumberOfPlayerControllableTroops() >= 20;
		}
		return false;
	}
}

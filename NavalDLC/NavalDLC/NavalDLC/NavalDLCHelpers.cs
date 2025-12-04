using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC;

public static class NavalDLCHelpers
{
	public static ExplainedNumber GetAveragePartySizeLimitFromTemplate(PartyTemplateObject templateObject)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (PartyTemplateStack item in (List<PartyTemplateStack>)(object)templateObject.Stacks)
		{
			num += (item.MaxValue + item.MinValue) / 2;
		}
		return new ExplainedNumber((float)num, false, (TextObject)null);
	}

	public static ExplainedNumber GetMaxPartySizeLimitFromTemplate(PartyTemplateObject templateObject)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (PartyTemplateStack item in (List<PartyTemplateStack>)(object)templateObject.Stacks)
		{
			num += item.MaxValue;
		}
		return new ExplainedNumber((float)num, false, (TextObject)null);
	}

	public static List<Ship> GetSetPieceBattleShips(PartyTemplateObject template, PartyBase party)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		List<Ship> list = ((IEnumerable<Ship>)party.Ships).Where((Ship s) => s.IsUsedByQuest).ToList();
		int num = 0;
		foreach (ShipTemplateStack item in (List<ShipTemplateStack>)(object)template.ShipHulls)
		{
			num += item.MaxValue;
		}
		int num2 = num - list.Count();
		if (num2 > 0)
		{
			foreach (Ship item2 in (from s in (IEnumerable<Ship>)party.Ships
				where !s.IsUsedByQuest
				orderby s.FlagshipScore descending
				select s).ToList())
			{
				if (num2 > 0)
				{
					list.Add(item2);
					num2--;
					continue;
				}
				break;
			}
		}
		return list;
	}

	public static bool IsShipOrdersAvailable()
	{
		if (Mission.Current == null || !Mission.Current.IsNavalBattle)
		{
			return false;
		}
		Team playerTeam = Mission.Current.PlayerTeam;
		if (((playerTeam != null) ? playerTeam.PlayerOrderController : null) == null)
		{
			return false;
		}
		if (Mission.Current.GetMissionBehavior<NavalShipsLogic>() == null)
		{
			return false;
		}
		MBReadOnlyList<Formation> selectedFormations = Mission.Current.PlayerTeam.PlayerOrderController.SelectedFormations;
		if (selectedFormations == null)
		{
			return false;
		}
		for (int i = 0; i < ((List<Formation>)(object)selectedFormations).Count; i++)
		{
			if (IsPlayerCaptainOfFormationShip(((List<Formation>)(object)selectedFormations)[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsPlayerCaptainOfFormationShip(Formation formation)
	{
		return IsAgentCaptainOfFormationShip(Agent.Main, formation);
	}

	public static bool IsAgentCaptainOfFormationShip(Agent agent, Formation formation)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Mission current = Mission.Current;
		NavalShipsLogic navalShipsLogic = ((current != null) ? current.GetMissionBehavior<NavalShipsLogic>() : null);
		if (navalShipsLogic == null || !navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out var ship))
		{
			return false;
		}
		if (agent != null && ship.Captain == agent)
		{
			return true;
		}
		if (agent != null && agent.IsMainAgent && ship.Formation.Team.IsPlayerTeam && ship.Formation.Index == 0)
		{
			return true;
		}
		return false;
	}
}

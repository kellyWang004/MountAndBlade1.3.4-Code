using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Handlers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Storyline;
using NavalDLC.Storyline.MissionControllers;
using SandBox;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace NavalDLC.Missions;

[MissionManager]
public static class NavalMissions
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<MapEventParty, bool> _003C_003E9__1_0;

		public static Func<MapEventParty, bool> _003C_003E9__2_0;

		public static InitializeMissionBehaviorsDelegate _003C_003E9__10_0;

		internal bool _003COpenNavalSetPieceBattleMission_003Eb__1_0(MapEventParty p)
		{
			return p.Party == MobileParty.MainParty.Party;
		}

		internal bool _003COpenBlockedEstuaryMission_003Eb__2_0(MapEventParty p)
		{
			return p.Party == MobileParty.MainParty.Party;
		}

		internal IEnumerable<MissionBehavior> _003COpenNavalStorylineAlleyFightMission_003Eb__10_0(Mission mission)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected O, but got Unknown
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected O, but got Unknown
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Expected O, but got Unknown
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			return new List<MissionBehavior>
			{
				(MissionBehavior)(object)new NavalStorylineAlleyFightMissionController(),
				(MissionBehavior)(object)new NavalStorylineAlleyFightCinematicController(),
				(MissionBehavior)new MissionHintLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionAgentHandler(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(10f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController(),
				(MissionBehavior)new EquipmentControllerLeaveLogic()
			}.ToArray();
		}
	}

	[MissionMethod]
	public static Mission OpenNavalBattleMission(MissionInitializerRecord rec)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		MobileParty mainParty = MobileParty.MainParty;
		MapEvent mapEvent = mainParty.MapEvent;
		bool isPlayerSergeant = mapEvent.IsPlayerSergeant();
		bool isPlayerInArmy = mainParty.Army != null;
		bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(((IEnumerable<MapEventParty>)mapEvent.AttackerSide.Parties).Where((MapEventParty p) => p.Party == mainParty.Party));
		Mission obj = NavalMissionState.OpenNew("NavalBattle", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Expected O, but got Unknown
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Expected O, but got Unknown
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Expected O, but got Unknown
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Expected O, but got Unknown
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Expected O, but got Unknown
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Expected O, but got Unknown
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Expected O, but got Unknown
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Expected O, but got Unknown
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Expected O, but got Unknown
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Expected O, but got Unknown
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Expected O, but got Unknown
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Expected O, but got Unknown
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Expected O, but got Unknown
			IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = mapEvent.PlayerSide;
			BattleSideEnum otherSide = mapEvent.GetOtherSide(playerSide);
			MBReadOnlyList<MapEventParty> parties = mapEvent.GetMapEventSide(playerSide).Parties;
			NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMapEventPartiesOfPlayerTeams(parties, isPlayerSergeant, out var playerMapEventParty, out var playerTeamMapEventParties, out var playerAllyTeamMapEventParties);
			NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetShipDeploymentLimitsOfPlayerTeams(playerTeamMapEventParties, playerAllyTeamMapEventParties, out var playerTeamDeploymentLimit, out var playerAllyTeamDeploymentLimit);
			MBList<IShipOrigin> val = new MBList<IShipOrigin>();
			Ship suitablePlayerShip = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetSuitablePlayerShip(playerMapEventParty, playerTeamMapEventParties);
			((List<IShipOrigin>)(object)val).Add((IShipOrigin)(object)suitablePlayerShip);
			NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties((MBReadOnlyList<MapEventParty>)(object)playerTeamMapEventParties, playerTeamDeploymentLimit, val);
			NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetOrderedCaptainsForPlayerTeamShips((MBReadOnlyList<MapEventParty>)(object)playerTeamMapEventParties, (MBReadOnlyList<IShipOrigin>)(object)val, out var playerTeamCaptainsByPriority);
			MBList<IShipOrigin> val2 = new MBList<IShipOrigin>();
			if (!Extensions.IsEmpty<MapEventParty>((IEnumerable<MapEventParty>)playerAllyTeamMapEventParties))
			{
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties((MBReadOnlyList<MapEventParty>)(object)playerAllyTeamMapEventParties, playerAllyTeamDeploymentLimit, val2);
			}
			MBList<MapEventParty> teamMapEventParties = Extensions.ToMBList<MapEventParty>((List<MapEventParty>)(object)mapEvent.GetMapEventSide(otherSide).Parties);
			NavalShipDeploymentLimit teamShipDeploymentLimit = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetTeamShipDeploymentLimit((MBReadOnlyList<MapEventParty>)(object)teamMapEventParties);
			MBList<IShipOrigin> val3 = new MBList<IShipOrigin>();
			NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties((MBReadOnlyList<MapEventParty>)(object)teamMapEventParties, teamShipDeploymentLimit, val3);
			int[] maximumDeployableTroopCountPerTeam = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountPerTeam(val, val2, val3);
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[31]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new DefaultNavalMissionLogic(val, val2, val3, playerTeamDeploymentLimit, playerAllyTeamDeploymentLimit, teamShipDeploymentLimit),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide, maximumDeployableTroopCountPerTeam),
				(MissionBehavior)new NavalMissionDeploymentPlanningLogic(mission),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new WaveParametersComputerLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new NavalAgentMoraleInteractionLogic(),
				(MissionBehavior)new NavalBattleEndLogic(),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new ShipCollisionOutcomeLogic(mission),
				(MissionBehavior)new ShipRetreatLogic(),
				(MissionBehavior)new NavalBoundaryForceFieldLogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, playerTeamCaptainsByPriority),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController(),
				(MissionBehavior)new NavalDeploymentMissionController(isPlayerAttacker),
				(MissionBehavior)new NavalDeploymentHandler(isPlayerAttacker)
			};
		});
		obj.SetPlayerCanTakeControlOfAnotherAgentWhenDead();
		return obj;
	}

	[MissionMethod]
	public static Mission OpenNavalSetPieceBattleMission(MissionInitializerRecord rec, MBList<IShipOrigin> playerShips, MBList<IShipOrigin> playerAllyShips, MBList<IShipOrigin> enemyShips)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		bool isPlayerInArmy = MobileParty.MainParty.Army != null;
		List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
		bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(((IEnumerable<MapEventParty>)MobileParty.MainParty.MapEvent.AttackerSide.Parties).Where((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
		return NavalMissionState.OpenNew("NavalBattle", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected O, but got Unknown
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Expected O, but got Unknown
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Expected O, but got Unknown
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Expected O, but got Unknown
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Expected O, but got Unknown
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Expected O, but got Unknown
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Expected O, but got Unknown
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Expected O, but got Unknown
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Expected O, but got Unknown
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Expected O, but got Unknown
			IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			NavalShipDeploymentLimit playerTeamShipDeploymentLimit = NavalShipDeploymentLimit.Max();
			NavalShipDeploymentLimit playerAllyTeamShipDeploymentLimit = NavalShipDeploymentLimit.Max();
			NavalShipDeploymentLimit enemyTeamShipDeploymentLimit = NavalShipDeploymentLimit.Max();
			int[] maximumDeployableTroopCountPerTeam = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountPerTeam(playerShips, playerAllyShips, enemyShips);
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[28]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new DefaultNavalMissionLogic(playerShips, playerAllyShips, enemyShips, playerTeamShipDeploymentLimit, playerAllyTeamShipDeploymentLimit, enemyTeamShipDeploymentLimit),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide, maximumDeployableTroopCountPerTeam),
				(MissionBehavior)new NavalMissionDeploymentPlanningLogic(mission),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new WaveParametersComputerLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new NavalBattleEndLogic(),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new ShipCollisionOutcomeLogic(mission),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController(),
				(MissionBehavior)new NavalDeploymentMissionController(isPlayerAttacker),
				(MissionBehavior)new NavalDeploymentHandler(isPlayerAttacker)
			};
		});
	}

	[MissionMethod]
	public static Mission OpenBlockedEstuaryMission(MissionInitializerRecord rec, MobileParty enemyParty, bool startFromCheckPoint)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		bool isPlayerInArmy = MobileParty.MainParty.Army != null;
		List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
		Extensions.IsEmpty<MapEventParty>(((IEnumerable<MapEventParty>)MobileParty.MainParty.MapEvent.AttackerSide.Parties).Where((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
		return NavalMissionState.OpenNew("BlockedEstuary", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected O, but got Unknown
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Expected O, but got Unknown
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Expected O, but got Unknown
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Expected O, but got Unknown
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Expected O, but got Unknown
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Expected O, but got Unknown
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Expected O, but got Unknown
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Expected O, but got Unknown
			IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[25]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BlockedEstuaryMissionController(enemyParty, startFromCheckPoint),
				(MissionBehavior)new BlockedEstuaryBattleEndLogic(),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new ShipCollisionOutcomeLogic(mission),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController()
			};
		});
	}

	[MissionMethod]
	public static Mission OpenNavalStorylineCaptivityMission(MissionInitializerRecord rec, CharacterObject allyCharacter, CharacterObject enemyCharacter, CharacterObject crewCharacter)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		_ = MobileParty.MainParty.Army;
		HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
		return NavalMissionState.OpenNew("NavalCaptivityBattle", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Expected O, but got Unknown
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Expected O, but got Unknown
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Expected O, but got Unknown
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Expected O, but got Unknown
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Expected O, but got Unknown
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Expected O, but got Unknown
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Expected O, but got Unknown
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected O, but got Unknown
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Expected O, but got Unknown
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Expected O, but got Unknown
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Expected O, but got Unknown
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Expected O, but got Unknown
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Expected O, but got Unknown
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Expected O, but got Unknown
			_ = new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
			MBList<IShipOrigin> obj = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val2 = new MBList<IShipOrigin>();
			((List<IShipOrigin>)(object)obj).AddRange((IEnumerable<IShipOrigin>)MobileParty.MainParty.Ships);
			foreach (MapEventParty item in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
			{
				if (item.IsNpcParty)
				{
					((List<IShipOrigin>)(object)val).AddRange((IEnumerable<IShipOrigin>)item.Party.Ships);
				}
			}
			foreach (MapEventParty item2 in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
			{
				((List<IShipOrigin>)(object)val2).AddRange((IEnumerable<IShipOrigin>)item2.Party.Ships);
			}
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[22]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new NavalStorylineCaptivityMissionController(allyCharacter, (BasicCharacterObject)(object)enemyCharacter, crewCharacter),
				(MissionBehavior)new MissionHintLogic(),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new NavalMissionDeploymentPlanningLogic(mission),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new VisualTrackerMissionBehavior(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)new WaveParametersComputerLogic(),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new MissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController()
			};
		});
	}

	[MissionMethod]
	public static Mission OpenNavalStorylinePirateBattleMission(MissionInitializerRecord rec, MobileParty pirateParty, int pirateTroopCount)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		return NavalMissionState.OpenNew("NavalStorylinePirateBattle", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected O, but got Unknown
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Expected O, but got Unknown
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Expected O, but got Unknown
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Expected O, but got Unknown
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Expected O, but got Unknown
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Expected O, but got Unknown
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Expected O, but got Unknown
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Expected O, but got Unknown
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Expected O, but got Unknown
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Expected O, but got Unknown
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Expected O, but got Unknown
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Expected O, but got Unknown
			IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
			MBList<IShipOrigin> obj = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val2 = new MBList<IShipOrigin>();
			((List<IShipOrigin>)(object)obj).AddRange((IEnumerable<IShipOrigin>)MobileParty.MainParty.Ships);
			foreach (MapEventParty item in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
			{
				if (item.IsNpcParty)
				{
					((List<IShipOrigin>)(object)val).AddRange((IEnumerable<IShipOrigin>)item.Party.Ships);
				}
			}
			foreach (MapEventParty item2 in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
			{
				((List<IShipOrigin>)(object)val2).AddRange((IEnumerable<IShipOrigin>)item2.Party.Ships);
			}
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[26]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new NavalMissionDeploymentPlanningLogic(mission),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new PirateBattleMissionController(pirateParty, pirateTroopCount),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)new WaveParametersComputerLogic(),
				(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new NavalAgentMoraleInteractionLogic(),
				(MissionBehavior)new ShipCollisionOutcomeLogic(mission),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController()
			};
		});
	}

	[MissionMethod]
	public static Mission OpenNavalStorylineQuest5SetPieceBattleMission(MissionInitializerRecord rec, MobileParty enemyParty, Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState lastHitCheckpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		return NavalMissionState.OpenNew("NavalStorylineQuest5SetPieceBattleMission", rec, (InitializeMissionBehaviorsDelegate)delegate(Mission mission)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Expected O, but got Unknown
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Expected O, but got Unknown
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Expected O, but got Unknown
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Expected O, but got Unknown
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Expected O, but got Unknown
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Expected O, but got Unknown
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Expected O, but got Unknown
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Expected O, but got Unknown
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Expected O, but got Unknown
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Expected O, but got Unknown
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Expected O, but got Unknown
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Expected O, but got Unknown
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Expected O, but got Unknown
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Expected O, but got Unknown
			_ = new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
			MBList<IShipOrigin> obj = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val = new MBList<IShipOrigin>();
			MBList<IShipOrigin> val2 = new MBList<IShipOrigin>();
			((List<IShipOrigin>)(object)obj).AddRange((IEnumerable<IShipOrigin>)MobileParty.MainParty.Ships);
			foreach (MapEventParty item in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
			{
				if (item.IsNpcParty)
				{
					((List<IShipOrigin>)(object)val).AddRange((IEnumerable<IShipOrigin>)item.Party.Ships);
				}
			}
			foreach (MapEventParty item2 in (List<MapEventParty>)(object)MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
			{
				((List<IShipOrigin>)(object)val2).AddRange((IEnumerable<IShipOrigin>)item2.Party.Ships);
			}
			List<MissionBehavior> result = new List<MissionBehavior>
			{
				(MissionBehavior)(object)new NavalShipsLogic(),
				(MissionBehavior)(object)new NavalFloatsamLogic(),
				(MissionBehavior)(object)new NavalAgentsLogic(),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)(object)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)(object)new Quest5NavalMissionDeploymentPlanningLogic(mission),
				(MissionBehavior)(object)new Quest5SetPieceBattleMissionController(lastHitCheckpoint, enemyParty),
				(MissionBehavior)(object)new NavalBattleAgentLogic(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)(object)new CosmeticShipSpawnMissionLogic(),
				(MissionBehavior)(object)new LightScriptedFiresMissionController(),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)(object)new Quest5BattleObserverMissionLogic(),
				(MissionBehavior)new MissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionConversationLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController(),
				(MissionBehavior)new StealthPatrolPointMissionLogic()
			};
			if (lastHitCheckpoint != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
			{
				_ = lastHitCheckpoint;
				_ = 5;
			}
			return result;
		});
	}

	[MissionMethod]
	public static Mission OpenNavalFinalConversationMission()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		int wallLevel = Settlement.CurrentSettlement.Town.GetWallLevel();
		string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(wallLevel);
		Location location = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port");
		List<Ship> townLordShips = new List<Ship>();
		List<Ship> mainPartyShips = ((IEnumerable<Ship>)MobileParty.MainParty.Ships).ToList();
		foreach (MobileParty item in (List<MobileParty>)(object)Settlement.CurrentSettlement.Parties)
		{
			townLordShips.AddRange((IEnumerable<Ship>)item.Ships);
		}
		return MissionState.OpenNew("NavalFinalConversationMission", SandBoxMissions.CreateSandBoxMissionInitializerRecord(location.GetSceneName(wallLevel), civilianUpgradeLevelTag, true, (DecalAtlasGroup)3), (InitializeMissionBehaviorsDelegate)((Mission mission) => (IEnumerable<MissionBehavior>)(object)new MissionBehavior[23]
		{
			(MissionBehavior)new MissionOptionsComponent(),
			(MissionBehavior)new CampaignMissionComponent(),
			(MissionBehavior)new MissionBasicTeamLogic(),
			(MissionBehavior)new BasicLeaveMissionLogic(),
			(MissionBehavior)new LeaveMissionLogic("settlement_player_unconscious"),
			(MissionBehavior)new SandBoxMissionHandler(),
			(MissionBehavior)new MissionAgentLookHandler(),
			(MissionBehavior)new MissionConversationLogic(),
			(MissionBehavior)new MissionAgentHandler(),
			(MissionBehavior)new MissionLocationLogic(location, (string)null),
			(MissionBehavior)new HeroSkillHandler(),
			(MissionBehavior)new MissionFightHandler(),
			(MissionBehavior)new BattleAgentLogic(),
			(MissionBehavior)new MountAgentLogic(),
			(MissionBehavior)new AgentHumanAILogic(),
			(MissionBehavior)new MissionCrimeHandler(),
			(MissionBehavior)new MissionFacialAnimationHandler(),
			(MissionBehavior)new LocationItemSpawnHandler(),
			(MissionBehavior)new IndoorMissionController(),
			(MissionBehavior)new VisualTrackerMissionBehavior(),
			(MissionBehavior)new EquipmentControllerLeaveLogic(),
			(MissionBehavior)new BattleSurgeonLogic(),
			(MissionBehavior)new CivilianPortShipSpawnMissionLogic(mainPartyShips, townLordShips)
		}), true, true);
	}

	[MissionMethod]
	public static Mission OpenNavalStorylineWoundedBeastBattleMission(MissionInitializerRecord rec)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		bool isPlayerSergeant = true;
		HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
		IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2];
		suppliers[0] = (IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null);
		suppliers[1] = (IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null);
		BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
		return NavalMissionState.OpenNew("NavalStorylineWoundedBeastBattle", rec, (InitializeMissionBehaviorsDelegate)((Mission mission) => (IEnumerable<MissionBehavior>)(object)new MissionBehavior[27]
		{
			(MissionBehavior)new NavalShipsLogic(),
			(MissionBehavior)new NavalFloatsamLogic(),
			(MissionBehavior)new NavalAgentsLogic(),
			(MissionBehavior)new MissionObjectiveLogic(),
			(MissionBehavior)new WoundedBeastMissionController(),
			(MissionBehavior)new BattleAgentLogic(),
			(MissionBehavior)new MissionFightHandler(),
			(MissionBehavior)new WaveParametersComputerLogic(),
			(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide),
			(MissionBehavior)new NavalTrajectoryPlanningLogic(),
			(MissionBehavior)new BattlePowerCalculationLogic(),
			(MissionBehavior)new MissionOptionsComponent(),
			(MissionBehavior)new CampaignMissionComponent(),
			(MissionBehavior)new BattleObserverMissionLogic(),
			(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
			(MissionBehavior)new AgentHumanAILogic(),
			(MissionBehavior)new BattleMissionAgentInteractionLogic(),
			(MissionBehavior)new EquipmentControllerLeaveLogic(),
			(MissionBehavior)new NavalAgentMoraleInteractionLogic(),
			(MissionBehavior)new ShipCollisionOutcomeLogic(mission),
			(MissionBehavior)new AgentVictoryLogic(),
			(MissionBehavior)new NavalBattleEndLogic(),
			(MissionBehavior)new MissionHardBorderPlacer(),
			(MissionBehavior)new MissionBoundaryPlacer(),
			(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
			(MissionBehavior)new HighlightsController(),
			(MissionBehavior)new BattleHighlightsController()
		}));
	}

	[MissionMethod]
	public static Mission OpenHelpingAnAllySetPieceBattleMission(MissionInitializerRecord rec, MobileParty merchantParty, MobileParty seaHoundsParty)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		return NavalMissionState.OpenNew("HelpAnAllySetPieceBattle", rec, (InitializeMissionBehaviorsDelegate)delegate
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected O, but got Unknown
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Expected O, but got Unknown
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected O, but got Unknown
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Expected O, but got Unknown
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Expected O, but got Unknown
			_ = new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[21]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new HelpingAnAllySetPieceBattleMissionController(merchantParty, seaHoundsParty),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController()
			};
		});
	}

	[MissionMethod]
	public static Mission OpenFloatingFortressSetPieceBattleMission(MissionInitializerRecord rec, bool startFromCheckpoint)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
		return NavalMissionState.OpenNew("FloatingFortressSetPieceBattleMission", rec, (InitializeMissionBehaviorsDelegate)delegate
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Expected O, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Expected O, but got Unknown
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected O, but got Unknown
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Expected O, but got Unknown
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Expected O, but got Unknown
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected O, but got Unknown
			IMissionTroopSupplier[] suppliers = (IMissionTroopSupplier[])(object)new IMissionTroopSupplier[2]
			{
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)0, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null),
				(IMissionTroopSupplier)new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, (BattleSideEnum)1, (FlattenedTroopRoster)null, (Func<UniqueTroopDescriptor, MapEventParty, bool>)null)
			};
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			return (IEnumerable<MissionBehavior>)(object)new MissionBehavior[24]
			{
				(MissionBehavior)new NavalShipsLogic(),
				(MissionBehavior)new NavalFloatsamLogic(),
				(MissionBehavior)new NavalAgentsLogic(),
				(MissionBehavior)new NavalTrajectoryPlanningLogic(),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new NavalBattleAgentLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new FloatingFortressSetPieceBattleMissionController(startFromCheckpoint),
				(MissionBehavior)new NavalMissionCombatantsLogic((IEnumerable<IBattleCombatant>)MobileParty.MainParty.MapEvent.InvolvedParties, (IBattleCombatant)(object)PartyBase.MainParty, (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)0), (IBattleCombatant)(object)MobileParty.MainParty.MapEvent.GetLeaderParty((BattleSideEnum)1), (MissionTeamAITypeEnum)4, isPlayerSergeant),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattleMissionAgentInteractionLogic(),
				(MissionBehavior)new EquipmentControllerLeaveLogic(),
				(MissionBehavior)new ShipAgentSpawnLogic(suppliers, playerSide),
				(MissionBehavior)new MissionHintLogic(),
				(MissionBehavior)new MissionObjectiveLogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new NavalBattleEndLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(30f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController()
			};
		});
	}

	[MissionMethod]
	public static Mission OpenNavalStorylineAlleyFightMission(MissionInitializerRecord rec)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		object obj = _003C_003Ec._003C_003E9__10_0;
		if (obj == null)
		{
			InitializeMissionBehaviorsDelegate val = (Mission mission) => new List<MissionBehavior>
			{
				(MissionBehavior)(object)new NavalStorylineAlleyFightMissionController(),
				(MissionBehavior)(object)new NavalStorylineAlleyFightCinematicController(),
				(MissionBehavior)new MissionHintLogic(),
				(MissionBehavior)new MissionOptionsComponent(),
				(MissionBehavior)new AgentHumanAILogic(),
				(MissionBehavior)new BattlePowerCalculationLogic(),
				(MissionBehavior)new CampaignMissionComponent(),
				(MissionBehavior)new BattleObserverMissionLogic(),
				(MissionBehavior)new AgentVictoryLogic(),
				(MissionBehavior)new MissionHardBorderPlacer(),
				(MissionBehavior)new MissionAgentHandler(),
				(MissionBehavior)new MissionFightHandler(),
				(MissionBehavior)new MissionBoundaryPlacer(),
				(MissionBehavior)new MissionBoundaryCrossingHandler(10f),
				(MissionBehavior)new HighlightsController(),
				(MissionBehavior)new BattleHighlightsController(),
				(MissionBehavior)new EquipmentControllerLeaveLogic()
			}.ToArray();
			_003C_003Ec._003C_003E9__10_0 = val;
			obj = (object)val;
		}
		return MissionState.OpenNew("NavalStorylineAlleyFight", rec, (InitializeMissionBehaviorsDelegate)obj, true, true);
	}
}

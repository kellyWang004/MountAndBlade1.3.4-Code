using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.Missions;
using NavalDLC.Storyline;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors;

public class NavalTransitionCampaignBehavior : CampaignBehaviorBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnConsequenceDelegate _003C_003E9__2_1;

		public static Func<string, bool> _003C_003E9__7_0;

		public static Func<string, bool> _003C_003E9__17_0;

		internal void _003CAddGameMenus_003Eb__2_1(MenuCallbackArgs args)
		{
			PlayerEncounter.Current.IsPlayerWaiting = false;
			GameMenu.SwitchToMenu("port_menu");
		}

		internal bool _003Cvisit_port_condition_003Eb__7_0(string x)
		{
			return x == "port";
		}

		internal bool _003Center_port_condition_003Eb__17_0(string x)
		{
			return x == "port";
		}
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnAfterSessionLaunched);
	}

	private void OnAfterSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
	{
		AddGameMenus(campaignGameSystemStarter);
	}

	private void AddGameMenus(CampaignGameStarter campaignGameStarter)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b2: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e3: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0114: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		//IL_0145: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_0176: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01a7: Expected O, but got Unknown
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01d8: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Expected O, but got Unknown
		//IL_0209: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_0247: Expected O, but got Unknown
		//IL_0247: Expected O, but got Unknown
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		campaignGameStarter.AddGameMenuOption("town", "port", "{=JTZ3L8gS}Go to the port", new OnConditionDelegate(visit_port_condition), new OnConsequenceDelegate(visit_port_consequence), false, 3, false, (object)null);
		campaignGameStarter.AddGameMenu("port_menu", "{=AZajdfxc}You are at the port.", new OnInitDelegate(port_game_menu_on_init), (MenuOverlayType)3, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "enter_port", "{=PwV5gaLa}Take a walk around the port", new OnConditionDelegate(enter_port_condition), new OnConsequenceDelegate(enter_port_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "leave_option", "{=VJ4CUSE9}Go to the town center", new OnConditionDelegate(visit_town_condition), new OnConsequenceDelegate(visit_town_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "call_fleet", "{=GsDF9PJb}Call fleet", new OnConditionDelegate(call_fleet_condition), new OnConsequenceDelegate(call_fleet_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "inspect_fleet", "{=KoV93aYE}Inspect shipyard", new OnConditionDelegate(inspect_fleet_condition), new OnConsequenceDelegate(inspect_fleet_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "manage_fleet", "{=rQp1JolW}Manage fleet", new OnConditionDelegate(manage_fleet_condition), new OnConsequenceDelegate(manage_fleet_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "repair_ships", "{=hqGD0o4E}Repair ships ({TOTAL_AMOUNT}{GOLD_ICON})", new OnConditionDelegate(repair_ships_condition), new OnConsequenceDelegate(repair_ships_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "trade", "{=GmcgoiGy}Trade", new OnConditionDelegate(trade_on_condition), new OnConsequenceDelegate(trade_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "port_wait", "{=zEoHYEUS}Wait here for some time", new OnConditionDelegate(wait_here_on_condition), new OnConsequenceDelegate(wait_here_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("port_menu", "sail_option", "{=fbCbFqyj}Set sail", new OnConditionDelegate(set_sail_condition), new OnConsequenceDelegate(set_sail_consequence), true, -1, false, (object)null);
		campaignGameStarter.AddWaitGameMenu("port_wait_menu", "{=VqVYMGIP}You are waiting at the port of {CURRENT_SETTLEMENT}.", new OnInitDelegate(wait_menu_on_init), new OnConditionDelegate(wait_menu_on_condition), (OnConsequenceDelegate)null, (OnTickDelegate)delegate(MenuCallbackArgs args, CampaignTime dt)
		{
			SwitchToMenuIfThereIsAnInterrupt(args.MenuContext.GameMenu.StringId);
		}, (MenuAndOptionType)3, (MenuOverlayType)3, 0f, (MenuFlags)0, (object)null);
		OnConditionDelegate val = wait_menu_back_on_condition;
		object obj = _003C_003Ec._003C_003E9__2_1;
		if (obj == null)
		{
			OnConsequenceDelegate val2 = delegate
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
				GameMenu.SwitchToMenu("port_menu");
			};
			_003C_003Ec._003C_003E9__2_1 = val2;
			obj = (object)val2;
		}
		campaignGameStarter.AddGameMenuOption("port_wait_menu", "wait_leave", "{=UqDNAZqM}Stop waiting", val, (OnConsequenceDelegate)obj, true, -1, false, (object)null);
	}

	[GameMenuInitializationHandler("port_menu")]
	[GameMenuInitializationHandler("port_wait_menu")]
	public static void port_menu_on_init(MenuCallbackArgs args)
	{
		string backgroundMeshName = ((MBObjectBase)Settlement.CurrentSettlement.Culture).StringId + "_port";
		args.MenuContext.SetBackgroundMeshName(backgroundMeshName);
		args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/port");
	}

	private void call_fleet_consequence(MenuCallbackArgs args)
	{
		MobileParty.MainParty.Anchor.CallFleet(Settlement.CurrentSettlement);
		Campaign.Current.GameMenuManager.RefreshMenuOptions(args.MenuContext);
	}

	private bool call_fleet_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		args.optionLeaveType = (LeaveType)44;
		if (!CanMainPartySail())
		{
			args.Tooltip = new TextObject("{=Yu10hbHI}You don't own any ships!", (Dictionary<string, object>)null);
			args.IsEnabled = false;
		}
		else if (MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
		{
			args.Tooltip = new TextObject("{=wXsK2Dj6}Your fleet is already at this settlement.", (Dictionary<string, object>)null);
			args.IsEnabled = false;
		}
		else if (IsFleetMovingToCurrentSettlement())
		{
			CampaignTime val = MobileParty.MainParty.Anchor.ArrivalTime - CampaignTime.Now;
			int num = (int)Math.Ceiling(((CampaignTime)(ref val)).ToHours);
			TextObject val2 = new TextObject("{=aQnwaoXS}Your fleet will be here in {HOURS} {?(HOURS > 1)}hours{?}hour{\\?}!", (Dictionary<string, object>)null);
			val2.SetTextVariable("HOURS", num);
			args.Tooltip = val2;
			args.IsEnabled = false;
		}
		return true;
	}

	private void port_game_menu_on_init(MenuCallbackArgs args)
	{
		if (!MenuHelper.CheckAndOpenNextLocation(args))
		{
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port"));
		}
	}

	private bool visit_port_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)41;
		List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((Func<string, bool>)((string x) => x == "port")).ToList();
		MenuHelper.SetIssueAndQuestDataForLocations(args, list);
		if (MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.CurrentSettlement.HasPort)
		{
			return MobileParty.MainParty.CurrentSettlement.IsTown;
		}
		return false;
	}

	private void visit_port_consequence(MenuCallbackArgs args)
	{
		GameMenu.ActivateGameMenu("port_menu");
	}

	private bool CanMainPartySail()
	{
		return MobileParty.MainParty.HasNavalNavigationCapability;
	}

	private void SetSail()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
		PlayerEncounter.Finish(true);
	}

	private bool IsFleetMovingToCurrentSettlement()
	{
		if (MobileParty.MainParty.Anchor != null && MobileParty.MainParty.Anchor.IsMovingToPoint)
		{
			return MobileParty.MainParty.Anchor.IsTargetingSettlement(Settlement.CurrentSettlement);
		}
		return false;
	}

	private float GetGoldCostToRepairShips()
	{
		int num = 0;
		foreach (Ship item in (List<Ship>)(object)MobileParty.MainParty.Ships)
		{
			if (item.HitPoints < item.MaxHitPoints)
			{
				num += (int)Campaign.Current.Models.ShipCostModel.GetShipRepairCost(item, PartyBase.MainParty);
			}
		}
		return num;
	}

	private bool repair_ships_condition(MenuCallbackArgs args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		if (((List<Ship>)(object)MobileParty.MainParty.Ships).Count == 0)
		{
			return false;
		}
		args.optionLeaveType = (LeaveType)46;
		if (MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
		{
			float goldCostToRepairShips = GetGoldCostToRepairShips();
			if (goldCostToRepairShips > 0f)
			{
				if (goldCostToRepairShips > (float)Hero.MainHero.Gold)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", (Dictionary<string, object>)null);
				}
				MBTextManager.SetTextVariable("TOTAL_AMOUNT", goldCostToRepairShips, 2);
			}
			else if (goldCostToRepairShips == 0f)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=Zgv6NCze}None of your ships are damaged.", (Dictionary<string, object>)null);
				MBTextManager.SetTextVariable("TOTAL_AMOUNT", goldCostToRepairShips, 2);
			}
			else
			{
				Debug.FailedAssert("There is a problem in here with ship repair cost calculation", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\NavalTransitionCampaignBehavior.cs", "repair_ships_condition", 193);
			}
		}
		else
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=EtTUPPeM}None of your ships are docked at this port.", (Dictionary<string, object>)null);
			MBTextManager.SetTextVariable("TOTAL_AMOUNT", 0);
		}
		return true;
	}

	private void repair_ships_consequence(MenuCallbackArgs args)
	{
		foreach (Ship item in (List<Ship>)(object)MobileParty.MainParty.Ships)
		{
			if (item.HitPoints < item.MaxHitPoints)
			{
				RepairShipAction.Apply(item, Settlement.CurrentSettlement);
			}
		}
		GameMenu.SwitchToMenu("port_menu");
	}

	private bool set_sail_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		args.optionLeaveType = (LeaveType)42;
		if (!CanMainPartySail())
		{
			args.Tooltip = new TextObject("{=Yu10hbHI}You don't own any ships!", (Dictionary<string, object>)null);
			args.IsEnabled = false;
		}
		else if (!MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
		{
			args.Tooltip = new TextObject("{=LmTqrE8x}Your fleet is not docked at this settlement.", (Dictionary<string, object>)null);
			args.IsEnabled = false;
		}
		return true;
	}

	private void set_sail_consequence(MenuCallbackArgs args)
	{
		SetSail();
	}

	private bool enter_port_condition(MenuCallbackArgs args)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		bool flag2 = default(bool);
		TextObject val = default(TextObject);
		bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "port", ref flag2, ref val);
		List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((Func<string, bool>)((string x) => x == "port")).ToList();
		MenuHelper.SetIssueAndQuestDataForLocations(args, list);
		args.optionLeaveType = (LeaveType)1;
		return MenuHelper.SetOptionProperties(args, flag, flag2, val);
	}

	private void enter_port_consequence(MenuCallbackArgs args)
	{
		_ = PlayerEncounter.LocationEncounter;
		if (Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
		{
			NavalMissions.OpenNavalFinalConversationMission();
		}
		else
		{
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("port"), (Location)null, (CharacterObject)null, (string)null);
		}
	}

	private bool inspect_fleet_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		args.optionLeaveType = (LeaveType)43;
		MBReadOnlyList<Ship> availableShips = Settlement.CurrentSettlement.Town.AvailableShips;
		bool flag = availableShips != null && ((List<Ship>)(object)availableShips).Count > 0;
		MBReadOnlyList<Ship> ships = MobileParty.MainParty.Ships;
		bool flag2 = ships != null && ((List<Ship>)(object)ships).Count > 0;
		if ((!MobileParty.MainParty.Anchor.IsMovingToPoint && MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement)) || !flag2)
		{
			return false;
		}
		if (!flag)
		{
			args.IsEnabled = false;
			TextObject val = new TextObject("{=kc2wu8UH}{SETTLEMENT} does not have any available ships and your fleet is away.", (Dictionary<string, object>)null);
			val.SetTextVariable("SETTLEMENT", ((object)Settlement.CurrentSettlement.Name).ToString());
			args.Tooltip = val;
		}
		else
		{
			args.Tooltip = new TextObject("{=CuhXWzub}You can only view ships at the port while your fleet is away.", (Dictionary<string, object>)null);
		}
		return true;
	}

	private void inspect_fleet_consequence(MenuCallbackArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		PortStateHelper.OpenAsRestricted(Settlement.CurrentSettlement.Town, new TextObject("{=wkm1Jxap}Your fleet is not in this settlement", (Dictionary<string, object>)null));
	}

	private bool manage_fleet_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		args.optionLeaveType = (LeaveType)43;
		MBReadOnlyList<Ship> availableShips = Settlement.CurrentSettlement.Town.AvailableShips;
		bool flag = availableShips != null && ((List<Ship>)(object)availableShips).Count > 0;
		MBReadOnlyList<Ship> ships = MobileParty.MainParty.Ships;
		bool flag2 = ships != null && ((List<Ship>)(object)ships).Count > 0;
		if ((MobileParty.MainParty.Anchor.IsMovingToPoint || !MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement)) && flag2)
		{
			return false;
		}
		if (!flag && !flag2)
		{
			args.IsEnabled = false;
			args.Tooltip = new TextObject("{=bBT9xyQQ}Both you and the town have no available ships", (Dictionary<string, object>)null);
		}
		return true;
	}

	private void manage_fleet_consequence(MenuCallbackArgs args)
	{
		PortStateHelper.OpenAsTrade(Settlement.CurrentSettlement.Town);
	}

	private void visit_town_consequence(MenuCallbackArgs args)
	{
		GameMenu.ActivateGameMenu("town");
	}

	private bool visit_town_condition(MenuCallbackArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)2;
		return true;
	}

	private bool wait_here_on_condition(MenuCallbackArgs args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		bool flag2 = default(bool);
		TextObject val = default(TextObject);
		bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, (SettlementAction)6, ref flag2, ref val);
		args.optionLeaveType = (LeaveType)15;
		return MenuHelper.SetOptionProperties(args, flag, flag2, val);
	}

	private void wait_here_on_consequence(MenuCallbackArgs args)
	{
		GameMenu.SwitchToMenu("port_wait_menu");
	}

	private void wait_menu_on_init(MenuCallbackArgs args)
	{
		if (!MenuHelper.CheckAndOpenNextLocation(args))
		{
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port"));
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = true;
			}
		}
	}

	private bool wait_menu_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)15;
		MBTextManager.SetTextVariable("CURRENT_SETTLEMENT", Settlement.CurrentSettlement.EncyclopediaLinkWithName, false);
		return true;
	}

	private bool wait_menu_back_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)16;
		return true;
	}

	private void SwitchToMenuIfThereIsAnInterrupt(string currentMenuId)
	{
		string text = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
		if (text == "town_wait_menus")
		{
			text = "port_wait_menu";
		}
		if (text != currentMenuId)
		{
			if (!string.IsNullOrEmpty(text))
			{
				GameMenu.SwitchToMenu(text);
			}
			else
			{
				GameMenu.ExitToLast();
			}
		}
	}

	private bool trade_on_condition(MenuCallbackArgs args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		bool flag2 = default(bool);
		TextObject val = default(TextObject);
		bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, (SettlementAction)5, ref flag2, ref val);
		args.optionLeaveType = (LeaveType)14;
		return MenuHelper.SetOptionProperties(args, flag, flag2, val);
	}

	private void trade_on_consequence(MenuCallbackArgs args)
	{
		_ = PlayerEncounter.LocationEncounter;
		InventoryScreenHelper.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, (SettlementComponent)(object)Settlement.CurrentSettlement.Town, (InventoryCategoryType)(-1), (Action)null);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}
}

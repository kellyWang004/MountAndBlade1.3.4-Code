using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.Storyline.Quests;
using SandBox.GameComponents;
using SandBox.Missions.MissionLogics;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.CampaignBehaviors;

public class NavalStorylineCampaignBehavior : CampaignBehaviorBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnConditionDelegate _003C_003E9__26_0;

		public static OnConditionDelegate _003C_003E9__27_0;

		public static OnConditionDelegate _003C_003E9__28_0;

		internal bool _003CAddGangradirSeaDefaultConversations_003Eb__26_0()
		{
			if (Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && Settlement.CurrentSettlement == null && MobileParty.MainParty.IsCurrentlyAtSea)
			{
				return Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty;
			}
			return false;
		}

		internal bool _003CAddGangradirTownDefaultConversations_003Eb__27_0()
		{
			if (Hero.OneToOneConversationHero == NavalStorylineData.Gangradir)
			{
				return Settlement.CurrentSettlement != null;
			}
			return false;
		}

		internal bool _003CAddGangradirStorylineActivationNotPossibleConversation_003Eb__28_0()
		{
			if (Hero.OneToOneConversationHero == NavalStorylineData.Gangradir)
			{
				return !NavalStorylineData.IsStorylineActivationPossible();
			}
			return false;
		}
	}

	private bool _isNavalStorylineActive;

	private bool _isNavalStorylineCanceled;

	private TroopRoster _troops = TroopRoster.CreateDummyTroopRoster();

	private TroopRoster _prisoners = TroopRoster.CreateDummyTroopRoster();

	private List<Ship> _ships = new List<Ship>();

	private bool _inquiryFired;

	private AnchorPoint _cachedAnchor;

	private NavalStorylineData.NavalStorylineStage _lastCompletedStorylineStage = NavalStorylineData.NavalStorylineStage.None;

	private bool _isFirstReturnToOstican = true;

	private bool _isTutorialSkipped;

	private bool _removeCrimeHandler;

	private int _foodStage = 1;

	private void OnNewGameCreated(CampaignGameStarter starter)
	{
		if (NavalStorylineData.Gangradir.IsDisabled || NavalStorylineData.Gangradir.IsNotSpawned)
		{
			NavalStorylineData.Gangradir.ChangeState((CharacterStates)1);
		}
		if (NavalStorylineData.Gangradir.PartyBelongedTo == null && NavalStorylineData.Gangradir.StayingInSettlement == null)
		{
			EnterSettlementAction.ApplyForCharacterOnly(NavalStorylineData.Gangradir, NavalStorylineData.HomeSettlement);
		}
	}

	public override void RegisterEvents()
	{
		if (!_isNavalStorylineCanceled)
		{
			CampaignEvents.TickEvent.AddNonSerializedListener((object)this, (Action<float>)Tick);
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
			CampaignEvents.OnHeirSelectionOverEvent.AddNonSerializedListener((object)this, (Action<Hero>)OnHeirSelectionOver);
			CampaignEvents.CanHeroDieEvent.AddNonSerializedListener((object)this, (ReferenceAction<Hero, KillCharacterActionDetail, bool>)CanHeroDie);
			CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener((object)this, (ReferenceAction<Hero, bool>)CanHeroBecomePrisoner);
			CampaignEvents.CanHaveCampaignIssuesEvent.AddNonSerializedListener((object)this, (ReferenceAction<Hero, bool>)CanHaveCampaignIssues);
			CampaignEvents.OnMobilePartyNavigationStateChangedEvent.AddNonSerializedListener((object)this, (Action<MobileParty>)OnMobilePartyNavigationStateChanged);
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement>)OnSettlementLeft);
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener((object)this, (Action<QuestBase, QuestCompleteDetails>)OnQuestCompleted);
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnNewGameCreated);
			CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement, Hero>)OnSettlementEntered);
			NavalDLCEvents.OnNavalStorylineTutorialSkippedEvent.AddNonSerializedListener((object)this, (Action)OnNavalStorylineSkipped);
			NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener((object)this, (Action)OnNavalStorylineCanceled);
			CampaignEvents.AfterMissionStarted.AddNonSerializedListener((object)this, (Action<IMission>)AfterMissionStarted);
			CampaignEvents.MissionTickEvent.AddNonSerializedListener((object)this, (Action<float>)MissionTickEvent);
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener((object)this, (Action)OnGameLoadFinished);
		}
	}

	private static void OnGameLoadFinished()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!MBSaveLoad.IsUpdatingGameVersion || !(MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.3.7.103044", 0)))
		{
			return;
		}
		int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
		AgingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AgingCampaignBehavior>();
		Dictionary<Hero, int> dictionary = (Dictionary<Hero, int>)typeof(AgingCampaignBehavior).GetField("_heroesYoungerThanHeroComesOfAge", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(campaignBehavior);
		if (!(StoryModeHeroes.LittleSister.Age >= (float)heroComesOfAge))
		{
			return;
		}
		if (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5))
		{
			if (!dictionary.ContainsKey(StoryModeHeroes.LittleSister))
			{
				if (StoryModeHeroes.LittleSister.IsNotSpawned)
				{
					HeroHelper.SpawnHeroForTheFirstTime(StoryModeHeroes.LittleSister, HeroHelper.GetSettlementForRelativeSpawn(StoryModeHeroes.LittleSister));
				}
				else if (StoryModeHeroes.LittleSister.IsDisabled)
				{
					StoryModeHeroes.LittleSister.ChangeState((CharacterStates)1);
					Settlement val = ((StoryModeHeroes.LittleSister.GovernorOf != null) ? ((SettlementComponent)StoryModeHeroes.LittleSister.GovernorOf).Settlement : HeroHelper.GetSettlementForRelativeSpawn(StoryModeHeroes.LittleSister));
					EnterSettlementAction.ApplyForCharacterOnly(StoryModeHeroes.LittleSister, val);
				}
				if (StoryModeHeroes.LittleSister.Clan == null)
				{
					StoryModeHeroes.LittleSister.Clan = Clan.PlayerClan;
					MakeHeroFugitiveAction.Apply(StoryModeHeroes.LittleSister, false);
				}
			}
			else
			{
				Debug.FailedAssert("Little sister should already be removed from heroesYoungerThanHeroComesOfAge list", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\CampaignBehaviors\\NavalStorylineCampaignBehavior.cs", "OnGameLoadFinished", 133);
			}
		}
		else if (!StoryModeHeroes.LittleSister.IsDisabled && !StoryModeHeroes.LittleSister.IsNotSpawned)
		{
			DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
			if (StoryModeHeroes.LittleSister.GovernorOf != null)
			{
				ChangeGovernorAction.RemoveGovernorOf(StoryModeHeroes.LittleSister);
			}
		}
	}

	private void MissionTickEvent(float dt)
	{
		if (_removeCrimeHandler)
		{
			RemoveCrimeHandler(Mission.Current);
			_removeCrimeHandler = false;
		}
	}

	private void AfterMissionStarted(IMission mission)
	{
		if (_isNavalStorylineActive && LocationComplex.Current != null && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown && Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction))
		{
			_removeCrimeHandler = true;
		}
	}

	private void OnNavalStorylineCanceled()
	{
		_isNavalStorylineCanceled = true;
		((CampaignEventReceiver)CampaignEventDispatcher.Instance).RemoveListeners((object)this);
	}

	private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		if (party == MobileParty.MainParty && NavalStorylineData.Gangradir.StayingInSettlement == settlement && ((MBObjectBase)settlement).StringId.Equals("castle_village_N7_2"))
		{
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(((BasicCharacterObject)NavalStorylineData.Gangradir.CharacterObject).Race, "_settlement");
			(string, Monster) tuple = (ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, ((BasicCharacterObject)NavalStorylineData.Gangradir.CharacterObject).IsFemale, "_lord"), monsterWithSuffix);
			IFaction mapFaction = NavalStorylineData.Gangradir.MapFaction;
			uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515u);
			IFaction mapFaction2 = NavalStorylineData.Gangradir.MapFaction;
			uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515u);
			AgentData val = new AgentData((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)NavalStorylineData.Gangradir.CharacterObject, -1, (Banner)null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).NoHorses(true).ClothingColor1(num)
				.ClothingColor2(num2);
			Location locationWithId = LocationComplex.Current.GetLocationWithId("village_center");
			IAgentBehaviorManager agentBehaviorManager = SandBoxManager.Instance.AgentBehaviorManager;
			locationWithId.AddCharacter(new LocationCharacter(val, new AddBehaviorsDelegate(agentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable", true, (CharacterRelations)1, tuple.Item1, true, false, (ItemObject)null, false, false, true, (AfterAgentCreatedDelegate)null, false));
		}
	}

	private void OnQuestCompleted(QuestBase quest, QuestCompleteDetails details)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (quest is NavalStorylineQuestBase { WillProgressStoryline: not false } navalStorylineQuestBase)
		{
			if ((int)details == 2)
			{
				ChangeNavalStorylineActivity(activity: false);
				return;
			}
			((QuestBase)new ReturnToBaseQuest("naval_storyline_return_to_base", NavalStorylineData.Gangradir)).StartQuest();
			_lastCompletedStorylineStage = navalStorylineQuestBase.Stage;
		}
	}

	private void CanHeroBecomePrisoner(Hero hero, ref bool result)
	{
		if (_isNavalStorylineActive && NavalStorylineData.IsNavalStorylineHero(hero))
		{
			result = false;
		}
	}

	private void OnSettlementLeft(MobileParty party, Settlement settlement)
	{
		if (party == MobileParty.MainParty && _isNavalStorylineActive && !MobileParty.MainParty.IsCurrentlyAtSea)
		{
			ChangeNavalStorylineActivity(activity: false);
		}
	}

	private void OnMobilePartyNavigationStateChanged(MobileParty mobileParty)
	{
		if (_isNavalStorylineActive && mobileParty.IsMainParty && !mobileParty.IsCurrentlyAtSea && PlayerEncounter.EncounterSettlement == null)
		{
			ChangeNavalStorylineActivity(activity: false);
		}
	}

	private void OnHeirSelectionOver(Hero hero)
	{
		if (_isNavalStorylineActive)
		{
			ChangeNavalStorylineActivity(activity: false);
		}
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		AddGameMenus(starter);
		AddDialogues();
	}

	private void AddDialogues()
	{
		AddGangradirSeaDefaultConversations();
		AddGangradirTownDefaultConversations();
		AddGangradirStorylineActivationNotPossibleConversation();
	}

	private void AddGangradirSeaDefaultConversations()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		DialogFlow obj = DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=0zTShzbi}Keep an eye on the horizon, and look for sails.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null);
		object obj2 = _003C_003Ec._003C_003E9__26_0;
		if (obj2 == null)
		{
			OnConditionDelegate val = () => Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && Settlement.CurrentSettlement == null && MobileParty.MainParty.IsCurrentlyAtSea && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty;
			_003C_003Ec._003C_003E9__26_0 = val;
			obj2 = (object)val;
		}
		conversationManager.AddDialogFlow(obj.Condition((OnConditionDelegate)obj2).CloseDialog(), (object)null);
	}

	private void AddGangradirTownDefaultConversations()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		DialogFlow obj = DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=Si6F4bdz}I'm waiting for more news. Soon, I may have more to tell you.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null);
		object obj2 = _003C_003Ec._003C_003E9__27_0;
		if (obj2 == null)
		{
			OnConditionDelegate val = () => Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && Settlement.CurrentSettlement != null;
			_003C_003Ec._003C_003E9__27_0 = val;
			obj2 = (object)val;
		}
		conversationManager.AddDialogFlow(obj.Condition((OnConditionDelegate)obj2).CloseDialog(), (object)null);
	}

	private void AddGangradirStorylineActivationNotPossibleConversation()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		DialogFlow obj = DialogFlow.CreateDialogFlow("start", 30000).NpcLine("{=njVdva7h}This isn't the right time to pursue our war against the Sea Hounds, but believe me, I am not about to abandon it.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null);
		object obj2 = _003C_003Ec._003C_003E9__28_0;
		if (obj2 == null)
		{
			OnConditionDelegate val = () => Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !NavalStorylineData.IsStorylineActivationPossible();
			_003C_003Ec._003C_003E9__28_0 = val;
			obj2 = (object)val;
		}
		conversationManager.AddDialogFlow(obj.Condition((OnConditionDelegate)obj2).PlayerLine("{=KrsZJv1e}I shall return, hopefully under better circumstances.", (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).CloseDialog(), (object)null);
	}

	private void home_settlement_encounter_init(MenuCallbackArgs args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		TextObject val = new TextObject("{=lqy3wHWi}You have returned to Ostican harbor. Gunnar takes his leave to see if any new information about the Sea Hounds has arrived, or any new allies to join you in your fight. He tells you to look for him in the harbor when you are ready to proceed.", (Dictionary<string, object>)null);
		if (_isFirstReturnToOstican)
		{
			val = new TextObject("{=7UmbvMKi}You return to Ostican harbor, and tie your ship up at the pier. Besides the Vlandian traders and fishing vessels lies a small Nordic longship. Gunnar tells you that some of his comrades have responded to his call to hunt the Sea Hounds. He tells you he needs to dictate a letter to some others, and asks you to meet him later in the port.", (Dictionary<string, object>)null);
		}
		MBTextManager.SetTextVariable("MENU_TEXT", val, false);
	}

	private void leave_on_consequence(MenuCallbackArgs args)
	{
		if (_isFirstReturnToOstican)
		{
			_isFirstReturnToOstican = false;
		}
		Settlement val = Settlement.CurrentSettlement ?? PlayerEncounter.EncounterSettlement;
		bool flag = default(bool);
		bool flag2 = default(bool);
		GameMenu.SwitchToMenu(MobileParty.MainParty.HasNavalNavigationCapability ? "naval_town_outside" : Campaign.Current.Models.EncounterGameMenuModel.GetEncounterMenu(PartyBase.MainParty, val.Party, ref flag, ref flag2));
	}

	private bool leave_on_condition(MenuCallbackArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		args.Tooltip = new TextObject("{=wmTjX28f}This will exit story mode and return you to the Sandbox. You can continue the storyline later by talking to Gunnar in the port again.", (Dictionary<string, object>)null);
		args.optionLeaveType = (LeaveType)16;
		return true;
	}

	private void AddGameMenus(CampaignGameStarter campaignGameStarter)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0050: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a0: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d1: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0140: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0171: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01c1: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_01f2: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0223: Expected O, but got Unknown
		campaignGameStarter.AddGameMenu("naval_storyline_encounter_blocking", "{=LptlZGpR}The seas are rough, and it is difficult to bring your ship within hailing distance. Gunnar urges you not to waste time here, as you are in some haste.", new OnInitDelegate(virtual_encounter_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_encounter_blocking", "continue", "{=3sRdGQou}Leave", new OnConditionDelegate(leave_on_condition), new OnConsequenceDelegate(virtual_encounter_end_consequence), true, -1, false, (object)null);
		campaignGameStarter.AddGameMenu("naval_storyline_outside_town", "{MENU_TEXT}", new OnInitDelegate(home_settlement_encounter_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_outside_town", "talk_to_gunnar", "{=fJP8DJcB}Talk to Gunnar in port", new OnConditionDelegate(talk_to_gunnar_on_condition), new OnConsequenceDelegate(talk_to_gunnar_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_outside_town", "continue", "{=qusdJ7nu}Return to your party", new OnConditionDelegate(leave_on_condition), new OnConsequenceDelegate(leave_on_consequence), true, -1, false, (object)null);
		campaignGameStarter.AddGameMenu("naval_storyline_encounter_meeting", "{=!}.", new OnInitDelegate(game_menu_naval_storyline_encounter_meeting_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenu("naval_storyline_encounter", "{=!}{ENCOUNTER_TEXT}", new OnInitDelegate(game_menu_naval_storyline_encounter_on_init), (MenuOverlayType)4, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_encounter", "attack", "{=zxMOqlhs}Attack", new OnConditionDelegate(game_menu_naval_storyline_encounter_attack_on_condition), new OnConsequenceDelegate(game_menu_naval_storyline_encounter_attack_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_encounter", "leave", "{=2YYRyrOO}Leave...", new OnConditionDelegate(game_menu_naval_storyline_encounter_leave_on_condition), new OnConsequenceDelegate(game_menu_naval_storyline_encounter_leave_on_consequence), true, -1, false, (object)null);
		campaignGameStarter.AddGameMenu("naval_storyline_join_encounter", "{=jKWJpIES}{JOIN_ENCOUNTER_TEXT}. You decide to...", new OnInitDelegate(game_menu_join_naval_storyline_encounter_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_help_attackers", "{=h3yEHb4U}Help {ATTACKER}.", new OnConditionDelegate(game_menu_join_naval_storyline_encounter_help_attackers_on_condition), new OnConsequenceDelegate(game_menu_join_naval_storyline_encounter_help_attackers_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_help_defenders", "{=FwIgakj8}Help {DEFENDER}.", new OnConditionDelegate(game_menu_join_naval_storyline_encounter_help_defenders_on_condition), new OnConsequenceDelegate(game_menu_join_naval_storyline_encounter_help_defenders_on_consequence), false, -1, false, (object)null);
		campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_leave", "{=!}{LEAVE_TEXT}", new OnConditionDelegate(game_menu_join_naval_storyline_encounter_leave_no_army_on_condition), new OnConsequenceDelegate(game_menu_join_naval_storyline_encounter_leave_on_condition), false, -1, false, (object)null);
	}

	private void game_menu_naval_storyline_encounter_meeting_on_init(MenuCallbackArgs args)
	{
		if (PlayerEncounter.Current != null && ((PlayerEncounter.Battle != null && PlayerEncounter.Battle.AttackerSide.LeaderParty != PartyBase.MainParty && PlayerEncounter.Battle.DefenderSide.LeaderParty != PartyBase.MainParty) || PlayerEncounter.MeetingDone))
		{
			if (PlayerEncounter.LeaveEncounter)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
			}
			if (PlayerEncounter.BattleChallenge)
			{
				GameMenu.SwitchToMenu("duel_starter_menu");
				return;
			}
			MBTextManager.SetTextVariable("ENCOUNTER_TEXT", GameTexts.FindText("str_you_have_encountered_PARTY", (string)null), false);
			GameMenu.SwitchToMenu("naval_storyline_encounter");
		}
		else
		{
			PlayerEncounter.DoMeeting();
		}
	}

	private void game_menu_naval_storyline_encounter_on_init(MenuCallbackArgs args)
	{
		args.MenuContext.SetPanelSound("event:/ui/panels/battle/slide_in");
		if (PlayerEncounter.Battle == null)
		{
			if (MobileParty.MainParty.MapEvent != null)
			{
				PlayerEncounter.Init();
			}
			else
			{
				PlayerEncounter.StartBattle();
			}
		}
		PlayerEncounter.Update();
		if (PlayerEncounter.Current == null)
		{
			Campaign.Current.SaveHandler.SignalAutoSave();
		}
	}

	private bool game_menu_naval_storyline_encounter_attack_on_condition(MenuCallbackArgs args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MenuCallbackArgs val = new MenuCallbackArgs(args.MapState, TextObject.GetEmpty());
		CampaignBattleResult campaignBattleResult = PlayerEncounter.CampaignBattleResult;
		if (campaignBattleResult != null && !campaignBattleResult.PlayerVictory && Hero.MainHero.IsWounded && !PlayerEncounter.PlayerSurrender)
		{
			PlayerEncounter.PlayerSurrender = true;
			PlayerEncounter.Update();
			return false;
		}
		if (MenuHelper.EncounterOrderAttackCondition(val) && Hero.MainHero.HitPoints < Hero.MainHero.WoundedHealthLimit + 1)
		{
			Hero.MainHero.HitPoints = Hero.MainHero.WoundedHealthLimit + 1;
		}
		MenuHelper.CheckEnemyAttackableHonorably(args);
		return MenuHelper.EncounterAttackCondition(args);
	}

	private void game_menu_naval_storyline_encounter_attack_on_consequence(MenuCallbackArgs args)
	{
		MenuHelper.EncounterAttackConsequence(args);
	}

	private bool game_menu_naval_storyline_encounter_leave_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)16;
		return true;
	}

	private void game_menu_naval_storyline_encounter_leave_on_consequence(MenuCallbackArgs args)
	{
		MenuHelper.EncounterLeaveConsequence();
	}

	private void game_menu_join_naval_storyline_encounter_on_init(MenuCallbackArgs args)
	{
		MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
		PartyBase leaderParty = encounteredBattle.GetLeaderParty((BattleSideEnum)1);
		PartyBase leaderParty2 = encounteredBattle.GetLeaderParty((BattleSideEnum)0);
		if (leaderParty.IsMobile && leaderParty.MobileParty.Army != null)
		{
			MBTextManager.SetTextVariable("ATTACKER", leaderParty.MobileParty.ArmyName, false);
		}
		else
		{
			MBTextManager.SetTextVariable("ATTACKER", leaderParty.Name, false);
		}
		if (leaderParty2.IsMobile && leaderParty2.MobileParty.Army != null)
		{
			MBTextManager.SetTextVariable("DEFENDER", leaderParty2.MobileParty.ArmyName, false);
		}
		else
		{
			MBTextManager.SetTextVariable("DEFENDER", leaderParty2.Name, false);
		}
		MBTextManager.SetTextVariable("JOIN_ENCOUNTER_TEXT", GameTexts.FindText("str_come_across_battle", (string)null), false);
	}

	private void game_menu_join_naval_storyline_encounter_leave_on_condition(MenuCallbackArgs args)
	{
		PlayerEncounter.Finish(true);
	}

	private bool game_menu_join_naval_storyline_encounter_help_attackers_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)23;
		return PlayerEncounter.EncounteredBattle.CanPartyJoinBattle(PartyBase.MainParty, (BattleSideEnum)1);
	}

	private void game_menu_join_naval_storyline_encounter_help_attackers_on_consequence(MenuCallbackArgs args)
	{
		PlayerEncounter.JoinBattle((BattleSideEnum)1);
		GameMenu.SwitchToMenu("naval_storyline_encounter");
	}

	private bool game_menu_join_naval_storyline_encounter_help_defenders_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)23;
		return PlayerEncounter.EncounteredBattle.CanPartyJoinBattle(PartyBase.MainParty, (BattleSideEnum)0);
	}

	private void game_menu_join_naval_storyline_encounter_help_defenders_on_consequence(MenuCallbackArgs args)
	{
		PlayerEncounter.JoinBattle((BattleSideEnum)0);
		GameMenu.ActivateGameMenu("naval_storyline_encounter");
	}

	private bool game_menu_join_naval_storyline_encounter_leave_no_army_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)16;
		MBTextManager.SetTextVariable("LEAVE_TEXT", "{=ebUwP3Q3}Don't get involved.", false);
		return true;
	}

	[GameMenuInitializationHandler("naval_storyline_encounter")]
	private static void game_menu_naval_storyline_encounter_on_init_background(MenuCallbackArgs args)
	{
		args.MenuContext.SetBackgroundMeshName("encounter_naval");
	}

	[GameMenuInitializationHandler("naval_storyline_encounter_meeting")]
	private static void game_menu_naval_storyline_encounter_meeting_on_init_background(MenuCallbackArgs args)
	{
		args.MenuContext.SetBackgroundMeshName("encounter_naval");
	}

	[GameMenuInitializationHandler("naval_storyline_join_encounter")]
	private static void game_menu_naval_storyline_join_encounter_on_init_background(MenuCallbackArgs args)
	{
		string encounterCultureBackgroundMesh = MenuHelper.GetEncounterCultureBackgroundMesh(PlayerEncounter.EncounteredParty.MapFaction.Culture);
		args.MenuContext.SetBackgroundMeshName(encounterCultureBackgroundMesh);
	}

	private void talk_to_gunnar_on_consequence(MenuCallbackArgs args)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		leave_on_consequence(args);
		Mission val = null;
		if (LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null)
		{
			val = (Mission)PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("port"), (Location)null, NavalStorylineData.Gangradir.CharacterObject, (string)null);
		}
		else
		{
			Location locationWithId = NavalStorylineData.HomeSettlement.LocationComplex.GetLocationWithId("port");
			val = (Mission)CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, true, false, false, false, true), new ConversationCharacterData(NavalStorylineData.Gangradir.CharacterObject, PartyBase.MainParty, true, true, false, false, false, true), locationWithId.GetSceneName(NavalStorylineData.HomeSettlement.Town.GetWallLevel()), "", false);
		}
		RemoveCrimeHandler(val);
	}

	[GameMenuInitializationHandler("naval_storyline_encounter_blocking")]
	private static void naval_storyline_encounter_meeting_blocking_on_init_background(MenuCallbackArgs args)
	{
		args.MenuContext.SetBackgroundMeshName(((SettlementComponent)SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, (NavigationType)3, (Func<Settlement, bool>)null)).WaitMeshName);
	}

	private void RemoveCrimeHandler(Mission mission)
	{
		MissionCrimeHandler missionBehavior = mission.GetMissionBehavior<MissionCrimeHandler>();
		if (missionBehavior != null)
		{
			mission.RemoveMissionBehavior((MissionBehavior)(object)missionBehavior);
		}
	}

	[GameMenuInitializationHandler("naval_storyline_outside_town")]
	private static void naval_storyline_outside_town_on_init_background(MenuCallbackArgs args)
	{
		args.MenuContext.SetBackgroundMeshName(NavalStorylineData.HomeSettlement.SettlementComponent.WaitMeshName);
	}

	private bool talk_to_gunnar_on_condition(MenuCallbackArgs args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)1;
		return true;
	}

	private void virtual_encounter_end_consequence(MenuCallbackArgs args)
	{
		PlayerEncounter.Finish(true);
	}

	private void virtual_encounter_init(MenuCallbackArgs args)
	{
	}

	private void CanHaveCampaignIssues(Hero hero, ref bool result)
	{
		if (NavalStorylineData.IsNavalStorylineHero(hero))
		{
			result = false;
		}
	}

	private void OnNavalStorylineSkipped()
	{
		_lastCompletedStorylineStage = NavalStorylineData.NavalStorylineStage.Act2;
		_isTutorialSkipped = true;
	}

	private void Tick(float dt)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		if (!_inquiryFired && !MobileParty.MainParty.IsInRaftState && _isNavalStorylineActive && MobileParty.MainParty.IsCurrentlyAtSea && MobileParty.MainParty.IsTransitionInProgress)
		{
			InformationManager.ShowInquiry(new InquiryData(((object)new TextObject("{=461jcc87}Leaving Story Mode", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=dV92VE8i}When you leave story mode, you will be returned to Ostican. You can speak to Gunnar in port to try again later. Do you wish to continue?", (Dictionary<string, object>)null)).ToString(), true, true, ((object)GameTexts.FindText("str_ok", (string)null)).ToString(), ((object)GameTexts.FindText("str_cancel", (string)null)).ToString(), (Action)OnAcceptDeactivatingNavalStoryline, (Action)OnRejectDeactivatingNavalStoryline, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
		}
	}

	private void OnAcceptDeactivatingNavalStoryline()
	{
		_inquiryFired = true;
	}

	private void OnRejectDeactivatingNavalStoryline()
	{
		MobileParty.MainParty.SetMoveModeHold();
		MobileParty.MainParty.CancelNavigationTransition();
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<bool>("_isActive", ref _isNavalStorylineActive);
		dataStore.SyncData<TroopRoster>("_troops", ref _troops);
		dataStore.SyncData<List<Ship>>("_ships", ref _ships);
		dataStore.SyncData<TroopRoster>("_prisoners", ref _prisoners);
		dataStore.SyncData<bool>("_inquiryFired", ref _inquiryFired);
		dataStore.SyncData<AnchorPoint>("_cachedAnchor", ref _cachedAnchor);
		dataStore.SyncData<NavalStorylineData.NavalStorylineStage>("_storylineStage", ref _lastCompletedStorylineStage);
		dataStore.SyncData<bool>("_isNavalStorylineCanceled", ref _isNavalStorylineCanceled);
		dataStore.SyncData<bool>("_isFirstReturnToOstican", ref _isFirstReturnToOstican);
		dataStore.SyncData<bool>("_isTutorialSkipped", ref _isTutorialSkipped);
		dataStore.SyncData<int>("_foodStage", ref _foodStage);
	}

	public bool IsNavalStorylineActive()
	{
		return _isNavalStorylineActive;
	}

	private void CanHeroDie(Hero hero, KillCharacterActionDetail causeOfDeath, ref bool result)
	{
		if (!_isNavalStorylineCanceled && NavalStorylineData.IsNavalStorylineHero(hero) && !NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5))
		{
			result = false;
		}
	}

	public NavalStorylineData.NavalStorylineStage GetNavalStorylineStage()
	{
		return _lastCompletedStorylineStage;
	}

	public bool GetIsNavalStorylineCanceled()
	{
		return _isNavalStorylineCanceled;
	}

	public bool IsTutorialSkipped()
	{
		return _isTutorialSkipped;
	}

	public void ChangeNavalStorylineActivity(bool activity)
	{
		if (_isNavalStorylineActive != activity)
		{
			_isNavalStorylineActive = activity;
			OnActivityChanged(_isNavalStorylineActive);
		}
	}

	private void OnActivityChanged(bool newState)
	{
		_inquiryFired = false;
		if (newState)
		{
			CacheTroopsAndShips();
		}
		else
		{
			ClearRosters();
			GetTroopsAndShipsFromCache();
			NavalStorylineData.TeleportMainHeroAndGangradirBackToBase();
		}
		MobileParty.MainParty.MemberRoster.UpdateVersion();
		NavalDLCEvents.Instance.OnNavalStorylineActivityChanged(newState);
	}

	public void GiveProvisionsToPlayer()
	{
		int num = (int)(_lastCompletedStorylineStage + 1);
		if (_foodStage < num)
		{
			GiveProvisionsToPlayerInternal();
			_foodStage = (int)(_lastCompletedStorylineStage + 1);
		}
	}

	private void GiveProvisionsToPlayerInternal()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		_ = (float)MobileParty.MainParty.ItemRoster.TotalFood / MathF.Abs(MobileParty.MainParty.FoodChange);
		float num = 3.5f;
		int num2 = (int)(num * MathF.Abs(MobileParty.MainParty.FoodChange));
		if (num2 > 0)
		{
			ItemRosterElement val = default(ItemRosterElement);
			((ItemRosterElement)(ref val))._002Ector(DefaultItems.Grain, num2, (ItemModifier)null);
			MobileParty.MainParty.ItemRoster.Add(val);
		}
		int num3 = (int)((float)MobileParty.MainParty.TotalWage * num);
		num3 = (int)(Math.Round((float)num3 / 100f) * 100.0);
		GiveGoldAction.ApplyBetweenCharacters((Hero)null, Hero.MainHero, num3, false);
		InformationManager.DisplayMessage(new InformationMessage(((object)new TextObject("{=wJefidrb}Gunnar has secured some provisions for the journey.", (Dictionary<string, object>)null)).ToString(), new Color(0f, 1f, 0f, 1f)));
	}

	private void GetTroopsAndShipsFromCache()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		MBList<TroopRosterElement> troopRoster = _troops.GetTroopRoster();
		for (int num = ((List<TroopRosterElement>)(object)troopRoster).Count - 1; num >= 0; num--)
		{
			TroopRosterElement val = ((List<TroopRosterElement>)(object)troopRoster)[num];
			if (((BasicCharacterObject)val.Character).IsHero)
			{
				val.Character.HeroObject.ChangeState((CharacterStates)1);
			}
			MobileParty.MainParty.MemberRoster.AddToCounts(val.Character, ((TroopRosterElement)(ref val)).Number, false, ((TroopRosterElement)(ref val)).WoundedNumber, ((TroopRosterElement)(ref val)).Xp, true, -1);
		}
		MBList<TroopRosterElement> troopRoster2 = _prisoners.GetTroopRoster();
		for (int num2 = ((List<TroopRosterElement>)(object)troopRoster2).Count - 1; num2 >= 0; num2--)
		{
			TroopRosterElement val2 = ((List<TroopRosterElement>)(object)troopRoster2)[num2];
			if (((BasicCharacterObject)val2.Character).IsHero)
			{
				val2.Character.HeroObject.ChangeState((CharacterStates)3);
			}
			MobileParty.MainParty.PrisonRoster.AddToCounts(val2.Character, ((TroopRosterElement)(ref val2)).Number, false, ((TroopRosterElement)(ref val2)).WoundedNumber, 0, true, -1);
		}
		_troops.Clear();
		_prisoners.Clear();
		for (int num3 = _ships.Count - 1; num3 >= 0; num3--)
		{
			Ship val3 = _ships[num3];
			ChangeShipOwnerAction.ApplyByTransferring(PartyBase.MainParty, val3);
		}
		if (_cachedAnchor != null)
		{
			MobileParty.MainParty.SetAnchor(_cachedAnchor);
			_cachedAnchor = null;
		}
		else
		{
			MobileParty.MainParty.Anchor.ResetPosition();
		}
		_ships.Clear();
	}

	private void ClearRosters()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		MBList<TroopRosterElement> troopRoster = MobileParty.MainParty.MemberRoster.GetTroopRoster();
		for (int num = ((List<TroopRosterElement>)(object)troopRoster).Count - 1; num >= 0; num--)
		{
			TroopRosterElement val = ((List<TroopRosterElement>)(object)troopRoster)[num];
			if (val.Character != CharacterObject.PlayerCharacter)
			{
				MobileParty.MainParty.MemberRoster.AddToCounts(val.Character, -((TroopRosterElement)(ref val)).Number, false, -((TroopRosterElement)(ref val)).WoundedNumber, 0, true, -1);
			}
			if (((BasicCharacterObject)val.Character).IsHero)
			{
				foreach (IMissionPlayerFollowerHandler behavior in Campaign.Current.CampaignBehaviorManager.GetBehaviors<IMissionPlayerFollowerHandler>())
				{
					behavior.RemoveFollowingHero(val.Character.HeroObject);
				}
			}
		}
		MBList<TroopRosterElement> troopRoster2 = MobileParty.MainParty.PrisonRoster.GetTroopRoster();
		for (int num2 = ((List<TroopRosterElement>)(object)troopRoster2).Count - 1; num2 >= 0; num2--)
		{
			TroopRosterElement val2 = ((List<TroopRosterElement>)(object)troopRoster2)[num2];
			MobileParty.MainParty.PrisonRoster.AddToCounts(val2.Character, -((TroopRosterElement)(ref val2)).Number, false, -((TroopRosterElement)(ref val2)).WoundedNumber, 0, true, -1);
		}
		for (int num3 = ((List<Ship>)(object)PartyBase.MainParty.Ships).Count - 1; num3 >= 0; num3--)
		{
			DestroyShipAction.Apply(((List<Ship>)(object)PartyBase.MainParty.Ships)[num3]);
		}
	}

	private void CacheTroopsAndShips()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		MBList<TroopRosterElement> troopRoster = MobileParty.MainParty.MemberRoster.GetTroopRoster();
		for (int num = ((List<TroopRosterElement>)(object)troopRoster).Count - 1; num >= 0; num--)
		{
			TroopRosterElement val = ((List<TroopRosterElement>)(object)troopRoster)[num];
			if (val.Character != CharacterObject.PlayerCharacter)
			{
				_troops.Add(val);
				if (((BasicCharacterObject)val.Character).IsHero)
				{
					val.Character.HeroObject.ChangeState((CharacterStates)6);
				}
				MobileParty.MainParty.MemberRoster.AddToCountsAtIndex(num, -((TroopRosterElement)(ref val)).Number, -((TroopRosterElement)(ref val)).WoundedNumber, 0, true);
			}
		}
		MBList<TroopRosterElement> troopRoster2 = MobileParty.MainParty.PrisonRoster.GetTroopRoster();
		for (int num2 = ((List<TroopRosterElement>)(object)troopRoster2).Count - 1; num2 >= 0; num2--)
		{
			TroopRosterElement val2 = ((List<TroopRosterElement>)(object)troopRoster2)[num2];
			_prisoners.Add(val2);
			if (((BasicCharacterObject)val2.Character).IsHero)
			{
				val2.Character.HeroObject.ChangeState((CharacterStates)6);
			}
			MobileParty.MainParty.PrisonRoster.AddToCountsAtIndex(num2, -((TroopRosterElement)(ref val2)).Number, -((TroopRosterElement)(ref val2)).WoundedNumber, 0, true);
		}
		_cachedAnchor = ((!MobileParty.MainParty.Anchor.IsValid) ? ((AnchorPoint)null) : new AnchorPoint(MobileParty.MainParty.Anchor));
		for (int num3 = ((List<Ship>)(object)MobileParty.MainParty.Ships).Count - 1; num3 >= 0; num3--)
		{
			Ship val3 = ((List<Ship>)(object)MobileParty.MainParty.Ships)[num3];
			val3.Owner = null;
			_ships.Add(val3);
		}
		MobileParty.MainParty.Anchor.ResetPosition();
	}
}

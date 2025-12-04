using System;
using System.Collections.Generic;
using NavalDLC.Storyline.MissionControllers;
using NavalDLC.Storyline.Quests;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors;

public class NavalStorylineThirdActFifthQuestBehaviour : CampaignBehaviorBase
{
	private const string QuestConversationMenuId = "naval_storyline_act_3_quest_5_conversation_menu";

	private bool _hasTalkedEarlier;

	private bool _hasSavedSister;

	private Quest5SetPieceBattleMissionController.BossFightOutComeEnum _bossFightOutCome;

	private bool _isQuestAcceptedThroughMission;

	private readonly float _strengthModifier = 1f;

	public override void RegisterEvents()
	{
		if (!NavalStorylineData.IsNavalStorylineCanceled())
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnAfterSessionLaunched);
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener((object)this, (Action<QuestBase, QuestCompleteDetails>)OnQuestCompleted);
			CampaignEvents.GameMenuOpened.AddNonSerializedListener((object)this, (Action<MenuCallbackArgs>)OnGameMenuOpened);
		}
	}

	private void OnGameMenuOpened(MenuCallbackArgs args)
	{
		if (_hasSavedSister && args.MenuContext.GameMenu.StringId == "naval_storyline_outside_town")
		{
			GameMenu.SwitchToMenu("naval_storyline_finalize_menu");
		}
		if (!_hasSavedSister && NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && !Campaign.Current.VisualTrackerManager.CheckTracked((ITrackableBase)(object)NavalStorylineData.Gangradir))
		{
			Campaign.Current.VisualTrackerManager.RegisterObject((ITrackableCampaignObject)(object)NavalStorylineData.Gangradir);
		}
	}

	private void OnQuestCompleted(QuestBase quest, QuestCompleteDetails detail)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)detail == 1 && quest is FreeTheSeaHoundsCaptivesQuest)
		{
			_hasSavedSister = true;
			_bossFightOutCome = ((FreeTheSeaHoundsCaptivesQuest)(object)quest).BossFightOutCome;
		}
	}

	private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
	{
		if (StoryModeManager.Current != null)
		{
			AddDialogs();
			AddGameMenus(campaignGameStarter);
		}
	}

	private void AddDialogs()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		DialogFlow val = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=jWDBinsb}Well... Here we are. Ready to set sail for Angranfjord and settle accounts with our enemies, once and for all. Lahar will sail with us, and Bjolgur, and more of his brothers may join us at our destination. We have Crusas' ship – and Crusas too of course, much as he might not like it – and hopefully the element of surprise. We just need to consider how to turn this best to our advantage.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).Condition((OnConditionDelegate)(() => !_hasTalkedEarlier && Quest5ConversationStartCondition()))
			.Consequence((OnConsequenceDelegate)delegate
			{
				_hasTalkedEarlier = true;
			})
			.BeginPlayerOptions((string)null, false)
			.PlayerOption(new TextObject("{=el44RZG4}Let us set out, then.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Consequence((OnConsequenceDelegate)delegate
			{
				if (Mission.Current == null)
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += ActivateQuest5;
				}
				else
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
				}
			})
			.CloseDialog()
			.PlayerOption(new TextObject("{=a0j86F9C}I need a bit more time.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed))
			.CloseDialog()
			.EndPlayerOptions()
			.CloseDialog();
		DialogFlow val2 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=0Y3S817q}Are you ready to sail to the Angranfjord to carry out our plan? Purig may not be waiting there for much longer.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).Condition((OnConditionDelegate)(() => _hasTalkedEarlier && Quest5ConversationStartCondition()))
			.BeginPlayerOptions((string)null, false)
			.PlayerOption(new TextObject("{=qcYkbX2a}Let us sail.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Consequence((OnConsequenceDelegate)delegate
			{
				if (Mission.Current == null)
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += ActivateQuest5;
				}
				else
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
				}
			})
			.CloseDialog()
			.PlayerOption(new TextObject("{=4LhjHfSY}I am still not ready.", (Dictionary<string, object>)null), (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed))
			.CloseDialog()
			.EndPlayerOptions()
			.CloseDialog();
		TextObject val3 = new TextObject("{=7SzwQ5NK}{PLAYER.NAME}, welcome! I've been entertaining the village with tales of our adventurers. If you're looking for recruits, then I doubt you'll find a more promising batch than the lads of Lagsholfn. You always have a place by my hearth, old friend.", (Dictionary<string, object>)null);
		TextObjectExtensions.SetCharacterProperties(val3, "PLAYER", CharacterObject.PlayerCharacter, false);
		TextObject val4 = new TextObject("{=dV5ai0PF}Well, {PLAYER.NAME}... Alas, you appear to have made some enemies here. I do not know if what they say is true, and at any rate, I will never raise a hand against you. But I do not think it is good for you to stay here just now.", (Dictionary<string, object>)null);
		TextObjectExtensions.SetCharacterProperties(val4, "PLAYER", CharacterObject.PlayerCharacter, false);
		DialogFlow val5 = DialogFlow.CreateDialogFlow("start", 1200).BeginNpcOptions((string)null, false).NpcOption(val3, (OnConditionDelegate)delegate
		{
			if (GunnarNotableConditions())
			{
				Settlement currentSettlement = NavalStorylineData.Gangradir.CurrentSettlement;
				if (currentSettlement == null)
				{
					return false;
				}
				Hero owner = currentSettlement.Owner;
				return ((owner != null) ? new float?(owner.GetRelationWithPlayer()) : ((float?)null)) >= 0f;
			}
			return false;
		}, (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.GotoDialogState("lord_start")
			.NpcOption(val4, (OnConditionDelegate)delegate
			{
				if (GunnarNotableConditions())
				{
					Settlement currentSettlement = NavalStorylineData.Gangradir.CurrentSettlement;
					if (currentSettlement == null)
					{
						return false;
					}
					Hero owner = currentSettlement.Owner;
					return ((owner != null) ? new float?(owner.GetRelationWithPlayer()) : ((float?)null)) < 0f;
				}
				return false;
			}, (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.GotoDialogState("lord_start")
			.EndNpcOptions();
		Campaign.Current.ConversationManager.AddDialogFlow(val, (object)null);
		Campaign.Current.ConversationManager.AddDialogFlow(val2, (object)null);
		Campaign.Current.ConversationManager.AddDialogFlow(val5, (object)null);
	}

	private void OnPlayerAcceptsQuestThroughMission()
	{
		_isQuestAcceptedThroughMission = true;
		OpenQuestMenu();
		Mission.Current.EndMission();
	}

	private void OpenQuestMenu()
	{
		GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_5_conversation_menu");
	}

	private void AddGameMenus(CampaignGameStarter starter)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_006f: Expected O, but got Unknown
		starter.AddGameMenu("naval_storyline_act_3_quest_5_conversation_menu", string.Empty, new OnInitDelegate(naval_storyline_act_3_quest_5_conversation_menu_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		starter.AddGameMenu("naval_storyline_finalize_menu", "{=l1VpTx3x}You have returned to Ostican harbor. Word spreads fast among seafolk, and a trading ship leaving the harbor dips its oars in salute to your victory. As the crews of your ships come ashore, they are clapped on the back by the local fishermen and dock workers and taken to the taverns to drink to the demise of the Sea Hounds.", new OnInitDelegate(naval_storyline_finalize_menu_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
		starter.AddGameMenuOption("naval_storyline_finalize_menu", "naval_storyline_finalize_menu_continue_option", "{=DM6luo3c}Continue", new OnConditionDelegate(naval_storyline_finalize_menu_continue_option_on_condition), new OnConsequenceDelegate(naval_storyline_finalize_menu_continue_option_on_consequence), false, -1, false, (object)null);
	}

	private void naval_storyline_act_3_quest_5_conversation_menu_on_init(MenuCallbackArgs args)
	{
		if (_isQuestAcceptedThroughMission && Mission.Current == null)
		{
			ActivateQuest5();
			_isQuestAcceptedThroughMission = false;
		}
	}

	private void naval_storyline_finalize_menu_on_init(MenuCallbackArgs args)
	{
	}

	private bool naval_storyline_finalize_menu_continue_option_on_condition(MenuCallbackArgs args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		args.optionLeaveType = (LeaveType)41;
		return true;
	}

	private void naval_storyline_finalize_menu_continue_option_on_consequence(MenuCallbackArgs args)
	{
		if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
		{
			((QuestBase)new SpeakToGunnarAndSisterQuest(_bossFightOutCome)).StartQuest();
		}
		GameMenu.SwitchToMenu("naval_town_outside");
	}

	private void ActivateQuest5()
	{
		if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)))
		{
			Campaign.Current.VisualTrackerManager.RemoveTrackedObject((ITrackableBase)(object)NavalStorylineData.Gangradir, false);
			((QuestBase)new FreeTheSeaHoundsCaptivesQuest("naval_storyline_act3_quest5_1", _strengthModifier)).StartQuest();
		}
	}

	private bool Quest5ConversationStartCondition()
	{
		if (NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement)
		{
			return Hero.OneToOneConversationHero == NavalStorylineData.Gangradir;
		}
		return false;
	}

	private bool GunnarNotableConditions()
	{
		if (Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !NavalStorylineData.IsNavalStoryLineActive() && !NavalStorylineData.IsNavalStorylineCanceled())
		{
			return NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5);
		}
		return false;
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<bool>("_hasTalkedEarlier", ref _hasTalkedEarlier);
		dataStore.SyncData<bool>("_hasSavedSister", ref _hasSavedSister);
		dataStore.SyncData<Quest5SetPieceBattleMissionController.BossFightOutComeEnum>("_bossFightOutCome", ref _bossFightOutCome);
	}

	public Quest5SetPieceBattleMissionController.BossFightOutComeEnum GetBossFightOutcome()
	{
		return _bossFightOutCome;
	}
}

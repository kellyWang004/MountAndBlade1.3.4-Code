using System;
using System.Runtime.CompilerServices;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors;

public class NavalStorylineThirdActThirdQuestBehavior : CampaignBehaviorBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnConditionDelegate _003C_003E9__10_1;

		internal bool _003CAddGangradirInitialDialogFlow_003Eb__10_1()
		{
			MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
			return true;
		}
	}

	private const string _questConversationMenuId = "naval_storyline_act_3_quest_3_conversation_menu";

	private bool _isQuestAcceptedThroughMission;

	private bool _isIntroGiven;

	public override void RegisterEvents()
	{
		if (!NavalStorylineData.IsNavalStorylineCanceled())
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
			NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener((object)this, (Action)OnNavalStorylineCanceled);
		}
	}

	private void OnNavalStorylineCanceled()
	{
		((CampaignEventReceiver)CampaignEventDispatcher.Instance).RemoveListeners((object)this);
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<bool>("_isIntroGiven", ref _isIntroGiven);
	}

	private void OnSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
	{
		AddDialogs();
		AddGameMenus(campaignGameSystemStarter);
	}

	private void AddGameMenus(CampaignGameStarter starter)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		starter.AddGameMenu("naval_storyline_act_3_quest_3_conversation_menu", string.Empty, new OnInitDelegate(naval_storyline_act_3_quest_3_conversation_menu_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
	}

	private void naval_storyline_act_3_quest_3_conversation_menu_on_init(MenuCallbackArgs args)
	{
		if (_isQuestAcceptedThroughMission && Mission.Current == null)
		{
			StartQuest();
			_isQuestAcceptedThroughMission = false;
		}
	}

	private void AddDialogs()
	{
		AddGangradirInitialDialogFlow();
	}

	private void AddGangradirInitialDialogFlow()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		DialogFlow obj = DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=0xymiaMQ}{PLAYER.NAME}... So… I have been making inquiries into what Fahda told us, about these Vlandian pirates in Purig's employ and their plan to steal the Sturgian silver. Several large warships have been sighted patrolling near {SETTLEMENT_LINK}. I suspect that these are the Vlandians.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).Condition((OnConditionDelegate)delegate
		{
			MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
			return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest2) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToTheSailorsQuest)) && !_isIntroGiven;
		})
			.NpcLine("{=Jjm2hpCl}{SETTLEMENT_LINK} is linked to the Byalic Sea by a wide estuary. It would be easy for the pirates to sit there, like spiders in a web, and wait until the Sturgians despair of losing all their commerce and try to run the blockade. Then the Vlandians will snap up the ships and their treasure.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null);
		object obj2 = _003C_003Ec._003C_003E9__10_1;
		if (obj2 == null)
		{
			OnConditionDelegate val = delegate
			{
				MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
				return true;
			};
			_003C_003Ec._003C_003E9__10_1 = val;
			obj2 = (object)val;
		}
		conversationManager.AddDialogFlow(obj.Condition((OnConditionDelegate)obj2).NpcLine("{=jFhkURpP}I'm sure Purig could wreak a great deal of wickedness with this silver in his hands, and I would very much like to foil this plan of his.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).Consequence((OnConsequenceDelegate)delegate
		{
			_isIntroGiven = true;
		})
			.BeginPlayerOptions((string)null, false)
			.PlayerOption("{=el44RZG4}Let us set out, then.", (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Consequence((OnConsequenceDelegate)delegate
			{
				if (Mission.Current == null)
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += StartQuest;
				}
				else
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
				}
			})
			.CloseDialog()
			.PlayerOption("{=a0j86F9C}I need a bit more time.", (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed))
			.CloseDialog()
			.EndPlayerOptions()
			.CloseDialog(), (object)null);
		Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=LnqHcu5S}Are we ready to sail for {SETTLEMENT_LINK}? The tide and winds are right.", (OnMultipleConversationConsequenceDelegate)null, (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null).Condition((OnConditionDelegate)delegate
		{
			MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
			return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest2) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToTheSailorsQuest)) && _isIntroGiven;
		})
			.BeginPlayerOptions((string)null, false)
			.PlayerOption("{=EjnrlsjX}Get the men to their ships. We sail at once.", (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Consequence((OnConsequenceDelegate)delegate
			{
				if (Mission.Current == null)
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += StartQuest;
				}
				else
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
				}
			})
			.CloseDialog()
			.PlayerOption("{=Ebk8s9s1}I am not yet ready.", (OnMultipleConversationConsequenceDelegate)null, (string)null, (string)null)
			.Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed))
			.CloseDialog()
			.EndPlayerOptions()
			.CloseDialog(), (object)null);
	}

	private void OnPlayerAcceptsQuestThroughMission()
	{
		_isQuestAcceptedThroughMission = true;
		OpenQuestMenu();
		Mission.Current.EndMission();
	}

	private void OpenQuestMenu()
	{
		GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_3_conversation_menu");
	}

	private void StartQuest()
	{
		((QuestBase)new SpeakToTheSailorsQuest("speak_to_the_sailors_quest", NavalStorylineData.Act3Quest3TargetSettlement)).StartQuest();
	}
}

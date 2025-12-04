using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors;

public class NavalStorylineThirdActFourthQuestBehavior : CampaignBehaviorBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_0;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_1;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_2;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_3;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_4;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_5;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_6;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_7;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_8;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_9;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_10;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_12;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_13;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_14;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_15;

		public static OnMultipleConversationConsequenceDelegate _003C_003E9__8_17;

		public static Func<Agent, bool> _003C_003E9__10_0;

		public static Func<Agent, bool> _003C_003E9__12_0;

		internal bool _003CAddDialogs_003Eb__8_0(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_1(IAgent agent)
		{
			return (object)agent.Character == CharacterObject.PlayerCharacter;
		}

		internal bool _003CAddDialogs_003Eb__8_2(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_3(IAgent agent)
		{
			return (object)agent.Character == CharacterObject.PlayerCharacter;
		}

		internal bool _003CAddDialogs_003Eb__8_4(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_5(IAgent agent)
		{
			return (object)agent.Character == CharacterObject.PlayerCharacter;
		}

		internal bool _003CAddDialogs_003Eb__8_6(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_7(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_8(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_9(IAgent agent)
		{
			return (object)agent.Character == CharacterObject.PlayerCharacter;
		}

		internal bool _003CAddDialogs_003Eb__8_10(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_12(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_13(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_14(IAgent agent)
		{
			return (object)agent.Character == CharacterObject.PlayerCharacter;
		}

		internal bool _003CAddDialogs_003Eb__8_15(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CAddDialogs_003Eb__8_17(IAgent agent)
		{
			return (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
		}

		internal bool _003CGangradirActivateQuestFourDialog1OnCondition_003Eb__10_0(Agent x)
		{
			return (object)x.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		internal bool _003CSpawnBjolgur_003Eb__12_0(Agent x)
		{
			return (object)x.Character == NavalStorylineData.Gangradir.CharacterObject;
		}
	}

	private const string QuestConversationMenuId = "naval_storyline_act_3_quest_4_conversation_menu";

	private bool _isQuestAcceptedThroughMission;

	private bool _initialConversationIsDone;

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
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		AddDialogs();
		AddGameMenus(starter);
	}

	private void AddGameMenus(CampaignGameStarter starter)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		starter.AddGameMenu("naval_storyline_act_3_quest_4_conversation_menu", string.Empty, new OnInitDelegate(naval_storyline_act_3_quest_4_conversation_menu_on_init), (MenuOverlayType)0, (MenuFlags)0, (object)null);
	}

	private void AddDialogs()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0153: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Expected O, but got Unknown
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Expected O, but got Unknown
		//IL_0331: Expected O, but got Unknown
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Expected O, but got Unknown
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Expected O, but got Unknown
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Expected O, but got Unknown
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Expected O, but got Unknown
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		ConversationManager conversationManager = Campaign.Current.ConversationManager;
		DialogFlow obj = DialogFlow.CreateDialogFlow("start", 1200);
		TextObject obj2 = new TextObject("{=sob0plMW}Good news, {PLAYERNAME}... Bjolgur’s order has given him permission to sail with us.", (Dictionary<string, object>)null).SetTextVariable("PLAYERNAME", Hero.MainHero.Name);
		object obj3 = _003C_003Ec._003C_003E9__8_0;
		if (obj3 == null)
		{
			OnMultipleConversationConsequenceDelegate val = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_0 = val;
			obj3 = (object)val;
		}
		object obj4 = _003C_003Ec._003C_003E9__8_1;
		if (obj4 == null)
		{
			OnMultipleConversationConsequenceDelegate val2 = (IAgent agent) => (object)agent.Character == CharacterObject.PlayerCharacter;
			_003C_003Ec._003C_003E9__8_1 = val2;
			obj4 = (object)val2;
		}
		DialogFlow obj5 = obj.NpcLine(obj2, (OnMultipleConversationConsequenceDelegate)obj3, (OnMultipleConversationConsequenceDelegate)obj4, (string)null, (string)null).Condition(new OnConditionDelegate(GangradirActivateQuestFourDialog1OnCondition)).Consequence(new OnConsequenceDelegate(GangradirActivateQuestFourDialog1OnConsequence));
		TextObject obj6 = new TextObject("{=eiX98VE9}Greetings, {PLAYERNAME}... I’ve got my longship, Corpse-Maker, and more of my brothers may yet join us on the journey. We also brought a captured vessel, agile and light, which mounts a ballista. We call it the Golden Wasp. We’ve bought up most of the ale in Ostican for our voyage, as I think we’ll be heading for the sweltering seas of the south.", (Dictionary<string, object>)null).SetTextVariable("PLAYERNAME", Hero.MainHero.Name);
		object obj7 = _003C_003Ec._003C_003E9__8_2;
		if (obj7 == null)
		{
			OnMultipleConversationConsequenceDelegate val3 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
			_003C_003Ec._003C_003E9__8_2 = val3;
			obj7 = (object)val3;
		}
		object obj8 = _003C_003Ec._003C_003E9__8_3;
		if (obj8 == null)
		{
			OnMultipleConversationConsequenceDelegate val4 = (IAgent agent) => (object)agent.Character == CharacterObject.PlayerCharacter;
			_003C_003Ec._003C_003E9__8_3 = val4;
			obj8 = (object)val4;
		}
		DialogFlow obj9 = obj5.NpcLine(obj6, (OnMultipleConversationConsequenceDelegate)obj7, (OnMultipleConversationConsequenceDelegate)obj8, (string)null, (string)null);
		TextObject val5 = new TextObject("{=egYc68CI}I’ve been making some inquiries. Crusas is well-known and respected in the Empire and in Vlandia. He mines sulfur from islands in the Gulf of Charas. No doubt he uses some of Purig’s slaves, but I guess the grand lords and ladies don’t know that, or choose not to know.", (Dictionary<string, object>)null);
		object obj10 = _003C_003Ec._003C_003E9__8_4;
		if (obj10 == null)
		{
			OnMultipleConversationConsequenceDelegate val6 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_4 = val6;
			obj10 = (object)val6;
		}
		object obj11 = _003C_003Ec._003C_003E9__8_5;
		if (obj11 == null)
		{
			OnMultipleConversationConsequenceDelegate val7 = (IAgent agent) => (object)agent.Character == CharacterObject.PlayerCharacter;
			_003C_003Ec._003C_003E9__8_5 = val7;
			obj11 = (object)val7;
		}
		DialogFlow obj12 = obj9.NpcLine(val5, (OnMultipleConversationConsequenceDelegate)obj10, (OnMultipleConversationConsequenceDelegate)obj11, (string)null, (string)null).BeginPlayerOptions((string)null, false);
		object obj13 = _003C_003Ec._003C_003E9__8_6;
		if (obj13 == null)
		{
			OnMultipleConversationConsequenceDelegate val8 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_6 = val8;
			obj13 = (object)val8;
		}
		DialogFlow obj14 = obj12.PlayerOption("{=npbsJToM}I hope, then, that he should not be difficult to find.", (OnMultipleConversationConsequenceDelegate)obj13, (string)null, (string)null).GotoDialogState("q4_next_line");
		object obj15 = _003C_003Ec._003C_003E9__8_7;
		if (obj15 == null)
		{
			OnMultipleConversationConsequenceDelegate val9 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_7 = val9;
			obj15 = (object)val9;
		}
		DialogFlow obj16 = obj14.PlayerOption("{=Cywj1xTj}Well respected or not, I’m ready to track him down.", (OnMultipleConversationConsequenceDelegate)obj15, (string)null, (string)null).GotoDialogState("q4_next_line").EndPlayerOptions();
		TextObject obj17 = new TextObject("{=sghtD7ov}Not hard to find at all.. On the way here I hailed some fishermen who chase tuna in the Gulf of Charas, and they say he is known to frequent a string of islands known as the Skatrias. They are said to be barren and foul-smelling. I can’t think why a merchant would want to anchor there, were they not the site of these sulfur mines where the captives are sent.{NEW_LINE}{NEW_LINE}So… I say we set out for these islands and hunt for Crusas.", (Dictionary<string, object>)null).SetTextVariable("NEW_LINE", "\n");
		object obj18 = _003C_003Ec._003C_003E9__8_8;
		if (obj18 == null)
		{
			OnMultipleConversationConsequenceDelegate val10 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
			_003C_003Ec._003C_003E9__8_8 = val10;
			obj18 = (object)val10;
		}
		object obj19 = _003C_003Ec._003C_003E9__8_9;
		if (obj19 == null)
		{
			OnMultipleConversationConsequenceDelegate val11 = (IAgent agent) => (object)agent.Character == CharacterObject.PlayerCharacter;
			_003C_003Ec._003C_003E9__8_9 = val11;
			obj19 = (object)val11;
		}
		DialogFlow obj20 = obj16.NpcLine(obj17, (OnMultipleConversationConsequenceDelegate)obj18, (OnMultipleConversationConsequenceDelegate)obj19, "q4_next_line", "q4_next_line_player_choices").BeginPlayerOptions("q4_next_line_player_choices", false);
		object obj21 = _003C_003Ec._003C_003E9__8_10;
		if (obj21 == null)
		{
			OnMultipleConversationConsequenceDelegate val12 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
			_003C_003Ec._003C_003E9__8_10 = val12;
			obj21 = (object)val12;
		}
		DialogFlow obj22 = obj20.PlayerOption("{=el44RZG4}Let us set out, then.", (OnMultipleConversationConsequenceDelegate)obj21, (string)null, (string)null).Consequence((OnConsequenceDelegate)delegate
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
		}).CloseDialog();
		object obj23 = _003C_003Ec._003C_003E9__8_12;
		if (obj23 == null)
		{
			OnMultipleConversationConsequenceDelegate val13 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
			_003C_003Ec._003C_003E9__8_12 = val13;
			obj23 = (object)val13;
		}
		conversationManager.AddDialogFlow(obj22.PlayerOption("{=a0j86F9C}I need a bit more time.", (OnMultipleConversationConsequenceDelegate)obj23, (string)null, (string)null).Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed)).CloseDialog()
			.EndPlayerOptions(), (object)null);
		ConversationManager conversationManager2 = Campaign.Current.ConversationManager;
		DialogFlow obj24 = DialogFlow.CreateDialogFlow("start", 1200);
		TextObject val14 = new TextObject("{=C8aEfvMM}Are we ready to set sail for the Skatrias? I imagine that Crusas will be docked there for some time, but we don’t want to miss this opportunity.", (Dictionary<string, object>)null);
		object obj25 = _003C_003Ec._003C_003E9__8_13;
		if (obj25 == null)
		{
			OnMultipleConversationConsequenceDelegate val15 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_13 = val15;
			obj25 = (object)val15;
		}
		object obj26 = _003C_003Ec._003C_003E9__8_14;
		if (obj26 == null)
		{
			OnMultipleConversationConsequenceDelegate val16 = (IAgent agent) => (object)agent.Character == CharacterObject.PlayerCharacter;
			_003C_003Ec._003C_003E9__8_14 = val16;
			obj26 = (object)val16;
		}
		DialogFlow obj27 = obj24.NpcLine(val14, (OnMultipleConversationConsequenceDelegate)obj25, (OnMultipleConversationConsequenceDelegate)obj26, (string)null, (string)null).Condition(new OnConditionDelegate(GangradirActivateQuestFourDialog2OnCondition)).BeginPlayerOptions((string)null, false);
		object obj28 = _003C_003Ec._003C_003E9__8_15;
		if (obj28 == null)
		{
			OnMultipleConversationConsequenceDelegate val17 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_15 = val17;
			obj28 = (object)val17;
		}
		DialogFlow obj29 = obj27.PlayerOption("{=el44RZG4}Let us set out, then.", (OnMultipleConversationConsequenceDelegate)obj28, (string)null, (string)null).Consequence((OnConsequenceDelegate)delegate
		{
			if (Mission.Current == null)
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += ActivateQuest4;
			}
			else
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += OnPlayerAcceptsQuestThroughMission;
			}
		}).CloseDialog();
		object obj30 = _003C_003Ec._003C_003E9__8_17;
		if (obj30 == null)
		{
			OnMultipleConversationConsequenceDelegate val18 = (IAgent agent) => (object)agent.Character == NavalStorylineData.Gangradir.CharacterObject;
			_003C_003Ec._003C_003E9__8_17 = val18;
			obj30 = (object)val18;
		}
		conversationManager2.AddDialogFlow(obj29.PlayerOption("{=a0j86F9C}I need a bit more time.", (OnMultipleConversationConsequenceDelegate)obj30, (string)null, (string)null).Condition(new OnConditionDelegate(NavalStorylineData.IsMainPartyAllowed)).CloseDialog()
			.EndPlayerOptions()
			.CloseDialog(), (object)null);
	}

	private void naval_storyline_act_3_quest_4_conversation_menu_on_init(MenuCallbackArgs args)
	{
		if (_isQuestAcceptedThroughMission && Mission.Current == null)
		{
			ActivateQuest4();
			_isQuestAcceptedThroughMission = false;
		}
	}

	private bool GangradirActivateQuestFourDialog1OnCondition()
	{
		int num;
		if (!_initialConversationIsDone && Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !NavalStorylineData.IsNavalStoryLineActive() && NavalStorylineData.IsStorylineActivationPossible())
		{
			num = (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors) ? 1 : 0);
			if (num != 0)
			{
				SpawnBjolgur();
				Agent item = ((IEnumerable<Agent>)Mission.Current.Agents).First((Agent x) => (object)x.Character == NavalStorylineData.Bjolgur.CharacterObject);
				ConversationManager conversationManager = Campaign.Current.ConversationManager;
				MBList<IAgent> obj = new MBList<IAgent>();
				((List<IAgent>)(object)obj).Add((IAgent)(object)item);
				conversationManager.AddConversationAgents((IEnumerable<IAgent>)obj, false);
			}
		}
		else
		{
			num = 0;
		}
		return (byte)num != 0;
	}

	private void GangradirActivateQuestFourDialog1OnConsequence()
	{
		_initialConversationIsDone = true;
	}

	private static void SpawnBjolgur()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		Agent val = ((IEnumerable<Agent>)Mission.Current.Agents).First((Agent x) => (object)x.Character == NavalStorylineData.Gangradir.CharacterObject);
		AgentBuildData val2 = new AgentBuildData((BasicCharacterObject)(object)NavalStorylineData.Bjolgur.CharacterObject);
		val2.TroopOrigin((IAgentOriginBase)new SimpleAgentOrigin(val2.AgentCharacter, -1, (Banner)null, default(UniqueTroopDescriptor)));
		Vec3 val3 = val.Position - Agent.Main.Position;
		((Vec3)(ref val3)).RotateAboutZ(0.34906584f);
		val3 += Agent.Main.Position;
		int num = 250;
		while (true)
		{
			Mission current = Mission.Current;
			UIntPtr? obj;
			if (current == null)
			{
				obj = null;
			}
			else
			{
				Scene scene = current.Scene;
				obj = ((scene != null) ? new UIntPtr?(scene.GetNavigationMeshForPosition(ref val3)) : ((UIntPtr?)null));
			}
			UIntPtr? uIntPtr = obj;
			UIntPtr zero = UIntPtr.Zero;
			if (!uIntPtr.HasValue || (uIntPtr.HasValue && !(uIntPtr.GetValueOrDefault() == zero)) || num == 0)
			{
				break;
			}
			if (MBRandom.RandomFloat > 0.5f)
			{
				((Vec3)(ref val3)).RotateAboutZ(MathF.PI / 180f * (float)MBRandom.RandomInt(20, 45));
			}
			else
			{
				((Vec3)(ref val3)).RotateAboutZ(MathF.PI / 180f * (float)MBRandom.RandomInt(-45, -20));
			}
			num--;
		}
		if (num == 0)
		{
			Debug.FailedAssert("Couldn't find a valid position for Bjolgur around Gunnar", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\CampaignBehaviors\\NavalStorylineThirdActFourthQuestBehavior.cs", "SpawnBjolgur", 166);
			val3 = Mission.Current.GetRandomPositionAroundPoint(val.Position, 1f, 3f, true);
		}
		val2.InitialPosition(ref val3);
		Vec3 lookDirection = Agent.Main.LookDirection;
		Vec2 val4 = ((Vec3)(ref lookDirection)).AsVec2;
		val4 = -((Vec2)(ref val4)).Normalized();
		val2.InitialDirection(ref val4);
		val2.NoHorses(true);
		val2.CivilianEquipment(true);
		Mission.Current.SpawnAgent(val2, false);
	}

	private bool GangradirActivateQuestFourDialog2OnCondition()
	{
		if (_initialConversationIsDone && Hero.OneToOneConversationHero == NavalStorylineData.Gangradir && !NavalStorylineData.IsNavalStoryLineActive() && NavalStorylineData.IsStorylineActivationPossible())
		{
			return NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors);
		}
		return false;
	}

	private void OnPlayerAcceptsQuestThroughMission()
	{
		_isQuestAcceptedThroughMission = true;
		OpenQuestMenu();
		Mission.Current.EndMission();
	}

	private void OpenQuestMenu()
	{
		GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_4_conversation_menu");
	}

	private void ActivateQuest4()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		CampaignVec2 corsairSpawnPosition = default(CampaignVec2);
		((CampaignVec2)(ref corsairSpawnPosition))._002Ector(new Vec2(285f, 300f), false);
		((QuestBase)new GoToSkatriaIslandsQuest("naval_storyline_act_3_quest_4", NavalStorylineData.Gangradir, corsairSpawnPosition)).StartQuest();
	}
}

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests;

public class SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest : NavalStorylineQuestBase
{
	public const float DistanceToSettlementToSpawnMerchantParty = 10f;

	private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string>
	{
		{ "sail", "sails_lvl2" },
		{ "side", "side_northern_shields_lvl2" }
	};

	[SaveableField(1)]
	private Settlement _targetSettlement;

	[SaveableField(2)]
	private bool _willProgressStoryline;

	public override bool WillProgressStoryline => _willProgressStoryline;

	public override TextObject Title
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			TextObject val = new TextObject("{=ugNRbWrI}Meet the Vlandian Merchants", (Dictionary<string, object>)null);
			val.SetTextVariable("SETTLEMENT_NAME", _targetSettlement.Name);
			return val;
		}
	}

	public override NavalStorylineData.NavalStorylineStage Stage => NavalStorylineData.NavalStorylineStage.Act3Quest1;

	protected override string MainPartyTemplateStringId => "storyline_act3_quest_1_main_party_template";

	private TextObject _descriptionLogText
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			TextObject val = new TextObject("{=eIGO7zhf}Locate the Vlandian merchant ship in the waters off {SETTLEMENT_LINK}.", (Dictionary<string, object>)null);
			val.SetTextVariable("SETTLEMENT_LINK", _targetSettlement.EncyclopediaLinkWithName);
			return val;
		}
	}

	public SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest(string questId, Hero questGiver, Settlement targetSettlement)
		: base(questId, questGiver, CampaignTime.Never, 0)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		_willProgressStoryline = true;
		_targetSettlement = targetSettlement;
		((QuestBase)this).AddTrackedObject((ITrackableCampaignObject)(object)_targetSettlement);
		((QuestBase)this).AddLog(_descriptionLogText, false);
	}

	protected override void OnStartQuestInternal()
	{
		Ship val = ((IEnumerable<Ship>)MobileParty.MainParty.Ships).FirstOrDefault();
		if (val == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> playerShipUpgradePiece in PlayerShipUpgradePieces)
		{
			if (val.HasSlot(playerShipUpgradePiece.Key))
			{
				val.SetPieceAtSlot(playerShipUpgradePiece.Key, MBObjectManager.Instance.GetObject<ShipUpgradePiece>(playerShipUpgradePiece.Value));
			}
		}
		val.ChangeFigurehead(DefaultFigureheads.Dragon);
	}

	protected override void SetDialogs()
	{
	}

	protected override void HourlyTick()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		CampaignVec2 position = MobileParty.MainParty.Position;
		if (((CampaignVec2)(ref position)).Distance(_targetSettlement.PortPosition) <= MathF.Min(10f, MobileParty.MainParty.SeeingRange))
		{
			_willProgressStoryline = false;
			((QuestBase)new SetSailAndEscortTheFortuneSeekersQuest("naval_storyline_act3_quest1_2", NavalStorylineData.Gangradir, _targetSettlement)).StartQuest();
			((QuestBase)this).CompleteQuestWithSuccess();
		}
	}

	protected override void RegisterEventsInternal()
	{
	}
}

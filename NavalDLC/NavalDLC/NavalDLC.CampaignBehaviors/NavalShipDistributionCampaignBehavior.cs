using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors;

public class NavalShipDistributionCampaignBehavior : CampaignBehaviorBase
{
	public override void SyncData(IDataStore dataStore)
	{
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement>)OnPartyDisbanded);
		CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener((object)this, (Action<MobileParty, PartyBase>)OnMobilePartyDestroyed);
	}

	private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
	{
		if (party.ActualClan != null && !party.IsCurrentlyAtSea)
		{
			DistributeShips(party);
			RecoverGoldFromRemainingShipsBeforePartyDestroy(party);
		}
	}

	private static void RecoverGoldFromRemainingShipsBeforePartyDestroy(MobileParty party)
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		if (LinQuick.AnyQ<Ship>((List<Ship>)(object)party.Ships) && party.ActualClan != null && !party.ActualClan.IsBanditFaction && party.ActualClan.Leader != null && party.ActualClan.Leader.IsActive)
		{
			int num = (int)LinQuick.SumQ<Ship>((List<Ship>)(object)party.Ships, (Func<Ship, float>)((Ship x) => Campaign.Current.Models.ShipCostModel.GetShipTradeValue(x, party.Party, (PartyBase)null)));
			if (party.ActualClan == Clan.PlayerClan)
			{
				float shipSellingPenalty = Campaign.Current.Models.ShipCostModel.GetShipSellingPenalty();
				num = (int)((float)num * shipSellingPenalty);
				MBTextManager.SetTextVariable("GOLD_AMOUNT", num);
				MBTextManager.SetTextVariable("LEADER_NAME", party.Owner.Name, false);
				MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
				MBInformationManager.AddQuickInformation(new TextObject("{=YaSnA9j0}{LEADER_NAME}'s party has disbanded. You recovered {GOLD_AMOUNT}{GOLD_ICON} from its ships.", (Dictionary<string, object>)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
			}
			GiveGoldAction.ApplyBetweenCharacters((Hero)null, party.ActualClan.Leader, num, false);
		}
	}

	private void OnPartyDisbanded(MobileParty disbandParty, Settlement relatedSettlement)
	{
		if (disbandParty.ActualClan != null && !disbandParty.ActualClan.IsBanditFaction)
		{
			DistributeShips(disbandParty);
		}
	}

	private void DistributeShips(MobileParty party)
	{
		for (int num = ((List<Ship>)(object)party.Ships).Count - 1; num >= 0; num--)
		{
			Ship shipToSend = ((List<Ship>)(object)party.Ships)[num];
			if (LinQuick.AnyQ<WarPartyComponent>((List<WarPartyComponent>)(object)party.ActualClan.WarPartyComponents, (Func<WarPartyComponent, bool>)((WarPartyComponent x) => ((PartyComponent)x).MobileParty != party && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(shipToSend, ((PartyComponent)x).MobileParty))))
			{
				MobileParty clanPartyToGetShipOfDisbandingParty = GetClanPartyToGetShipOfDisbandingParty(shipToSend, party.ActualClan);
				if (clanPartyToGetShipOfDisbandingParty != null && clanPartyToGetShipOfDisbandingParty != party)
				{
					ChangeShipOwnerAction.ApplyByTransferring(clanPartyToGetShipOfDisbandingParty.Party, shipToSend);
				}
			}
		}
	}

	private static MobileParty GetClanPartyToGetShipOfDisbandingParty(Ship ship, Clan clan)
	{
		MobileParty val = null;
		float num = float.MinValue;
		MBList<Ship> val2 = new MBList<Ship>();
		foreach (WarPartyComponent item in (List<WarPartyComponent>)(object)clan.WarPartyComponents)
		{
			if (((PartyComponent)item).Party != ship.Owner && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(ship, ((PartyComponent)item).MobileParty) && (val == null || ((List<Ship>)(object)val.Ships).Count >= ((List<Ship>)(object)((PartyComponent)item).Party.Ships).Count))
			{
				((List<Ship>)(object)val2).Clear();
				((List<Ship>)(object)val2).AddRange((IEnumerable<Ship>)((PartyComponent)item).Party.Ships);
				float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(((PartyComponent)item).MobileParty, (MBReadOnlyList<Ship>)(object)val2);
				((List<Ship>)(object)val2).Add(ship);
				float num2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(((PartyComponent)item).MobileParty, (MBReadOnlyList<Ship>)(object)val2) - scoreForPartyShipComposition;
				if (num2 > num)
				{
					val = ((PartyComponent)item).MobileParty;
					num = num2;
				}
			}
		}
		return val;
	}
}

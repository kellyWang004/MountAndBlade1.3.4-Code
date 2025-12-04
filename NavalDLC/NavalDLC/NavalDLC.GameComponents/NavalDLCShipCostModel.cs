using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents;

public class NavalDLCShipCostModel : ShipCostModel
{
	private const float BuyPenalty = 1.5f;

	private const float RepairPenalty = 0.25f;

	private const float SellPenalty = 0.3f;

	private const float UpgradePiecePenalty = 0.3f;

	private const float AIClansShipValueDiscountRatio = 0.01f;

	public override float GetShipTradeValue(Ship ship, PartyBase seller, PartyBase buyer)
	{
		bool applyAiDiscount = buyer != null && buyer.IsMobile && buyer.MobileParty.ActualClan != Clan.PlayerClan && seller.IsSettlement;
		float num = GetShipBaseValue(ship, applyAiDiscount, seller) * 1.5f;
		if (buyer != null)
		{
			Clan val = null;
			Kingdom val2 = null;
			if (buyer.IsMobile)
			{
				val = buyer.MobileParty.ActualClan;
				val2 = ((val != null) ? val.Kingdom : null);
			}
			else if (buyer.IsSettlement)
			{
				val = buyer.Settlement.OwnerClan;
				val2 = ((val != null) ? val.Kingdom : null);
			}
			if (val2 != null)
			{
				if (val2.HasPolicy(NavalPolicies.RoyalNavyPrerogative) && val2.RulingClan == val)
				{
					num *= 0.9f;
				}
				if (ship.Owner.IsSettlement && ship.Owner.Settlement.OwnerClan.Kingdom != null && ship.Owner.Settlement.OwnerClan.Kingdom == val2 && val2.HasPolicy(NavalPolicies.ArsenalDepositoryAct))
				{
					num *= 0.85f;
				}
			}
			if (seller.IsMobile && buyer.IsSettlement)
			{
				num = num * 0.3f - Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, ship.Owner);
			}
		}
		return num;
	}

	public override float GetShipRepairCost(Ship ship, PartyBase owner)
	{
		float num = (ship.MaxHitPoints - ship.HitPoints) / ship.MaxHitPoints;
		object obj;
		if (owner == null)
		{
			obj = null;
		}
		else
		{
			MobileParty mobileParty = owner.MobileParty;
			obj = ((mobileParty != null) ? mobileParty.ActualClan : null);
		}
		bool applyAiDiscount = obj != Clan.PlayerClan;
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector(GetShipBaseValue(ship, applyAiDiscount, owner) * num * 0.25f, false, (TextObject)null);
		if (owner != null && owner.MobileParty != null)
		{
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.MerchantPrince, owner.MobileParty, true, ref val, false);
		}
		return ((ExplainedNumber)(ref val)).ResultNumber;
	}

	public override int GetShipUpgradeCost(Ship ship, ShipUpgradePiece piece, PartyBase owner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected I4, but got Unknown
		int num = 0;
		ShipType type = ship.ShipHull.Type;
		switch ((int)type)
		{
		case 0:
			num = piece.LightValue;
			break;
		case 1:
			num = piece.MediumValue;
			break;
		case 2:
			num = piece.HeavyValue;
			break;
		}
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector((float)num, false, (TextObject)null);
		if (owner != null && owner.MobileParty != null)
		{
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.MasterShipwright, owner.MobileParty, true, ref val, false);
		}
		float num2 = ((ExplainedNumber)(ref val)).ResultNumber;
		if (owner != null && owner.MobileParty != null)
		{
			MobileParty mobileParty = owner.MobileParty;
			Clan val2 = ((mobileParty != null) ? mobileParty.ActualClan : null);
			Kingdom val3 = ((val2 != null) ? val2.Kingdom : null);
			if (val3 != null && val3.RulingClan == val2 && val3.HasPolicy(NavalPolicies.RoyalNavyPrerogative))
			{
				num2 *= 0.9f;
			}
			if (val2 != Clan.PlayerClan)
			{
				num2 *= 0.01f;
			}
		}
		return MathF.Round(num2);
	}

	private static float GetShipBaseValue(Ship ship, bool applyAiDiscount, PartyBase owner)
	{
		float num = ship.ShipHull.Value;
		if (applyAiDiscount)
		{
			num *= 0.01f;
		}
		int num2 = 0;
		foreach (KeyValuePair<string, ShipSlot> availableSlot in ship.ShipHull.AvailableSlots)
		{
			ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(availableSlot.Key);
			if (pieceAtSlot != null)
			{
				num2 += Campaign.Current.Models.ShipCostModel.GetShipUpgradeCost(ship, pieceAtSlot, owner);
			}
		}
		return num + (float)num2 * 0.3f;
	}

	public override float GetShipSellingPenalty()
	{
		return 0.3f;
	}
}

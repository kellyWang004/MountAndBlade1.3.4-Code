using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents;

public class NavalDLCMobilePartyFoodConsumptionModel : MobilePartyFoodConsumptionModel
{
	public override int NumberOfMenOnMapToEatOneFood => ((MBGameModel<MobilePartyFoodConsumptionModel>)this).BaseModel.NumberOfMenOnMapToEatOneFood;

	public override ExplainedNumber CalculateDailyBaseFoodConsumptionf(MobileParty party, bool includeDescription = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<MobilePartyFoodConsumptionModel>)this).BaseModel.CalculateDailyBaseFoodConsumptionf(party, includeDescription);
	}

	public override ExplainedNumber CalculateDailyFoodConsumptionf(MobileParty party, ExplainedNumber baseConsumption)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber result = ((MBGameModel<MobilePartyFoodConsumptionModel>)this).BaseModel.CalculateDailyFoodConsumptionf(party, baseConsumption);
		if (party.IsCurrentlyAtSea)
		{
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.SmoothOperator, party, false, ref result, false);
		}
		return result;
	}

	public override bool DoesPartyConsumeFood(MobileParty mobileParty)
	{
		return ((MBGameModel<MobilePartyFoodConsumptionModel>)this).BaseModel.DoesPartyConsumeFood(mobileParty);
	}
}

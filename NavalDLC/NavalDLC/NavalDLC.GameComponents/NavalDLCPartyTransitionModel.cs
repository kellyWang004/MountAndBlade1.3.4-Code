using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents;

public class NavalDLCPartyTransitionModel : PartyTransitionModel
{
	public override CampaignTime GetTransitionTimeForEmbarking(MobileParty mobileParty)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<PartyTransitionModel>)this).BaseModel.GetTransitionTimeForEmbarking(mobileParty);
	}

	public override CampaignTime GetTransitionTimeDisembarking(MobileParty mobileParty)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		CampaignTime result = ((MBGameModel<PartyTransitionModel>)this).BaseModel.GetTransitionTimeDisembarking(mobileParty);
		if (mobileParty.HasPerk(NavalPerks.Shipmaster.Unflinching, false))
		{
			float num = NavalPerks.Shipmaster.Unflinching.PrimaryBonus * 100f;
			float num2 = (0f - num * 100f) / (100f + num);
			result = CampaignTime.Hours((float)((CampaignTime)(ref result)).ToHours * num2);
		}
		return result;
	}

	public override CampaignTime GetFleetTravelTimeToPoint(MobileParty owner, CampaignVec2 target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		CampaignTime result = ((MBGameModel<PartyTransitionModel>)this).BaseModel.GetFleetTravelTimeToPoint(owner, target);
		if (owner.HasPerk(NavalPerks.Shipmaster.ShoreMaster, false))
		{
			result = CampaignTime.Hours((float)((CampaignTime)(ref result)).ToHours * NavalPerks.Shipmaster.ShoreMaster.PrimaryBonus * -1f);
		}
		return result;
	}
}

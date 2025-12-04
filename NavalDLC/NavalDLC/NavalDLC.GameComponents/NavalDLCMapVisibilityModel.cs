using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents;

public class NavalDLCMapVisibilityModel : MapVisibilityModel
{
	public override float MaximumSeeingRange()
	{
		return ((MBGameModel<MapVisibilityModel>)this).BaseModel.MaximumSeeingRange();
	}

	public override float GetPartySpottingRangeBase(MobileParty party)
	{
		float num = ((MBGameModel<MapVisibilityModel>)this).BaseModel.GetPartySpottingRangeBase(party);
		if (party.IsCurrentlyAtSea)
		{
			if (party.IsInRaftState)
			{
				num *= 0.5f;
			}
			if (Campaign.Current.IsNight && party.HasPerk(NavalPerks.Shipmaster.NightRaider, false))
			{
				num += 3f;
			}
		}
		return num;
	}

	public override ExplainedNumber GetPartySpottingRange(MobileParty party, bool includeDescriptions = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber partySpottingRange = ((MBGameModel<MapVisibilityModel>)this).BaseModel.GetPartySpottingRange(party, includeDescriptions);
		if (party.IsCurrentlyAtSea)
		{
			PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.RavenEye, party, true, ref partySpottingRange, false);
		}
		return partySpottingRange;
	}

	public override float GetPartyRelativeInspectionRange(IMapPoint party)
	{
		return ((MBGameModel<MapVisibilityModel>)this).BaseModel.GetPartyRelativeInspectionRange(party);
	}

	public override float GetPartySpottingDifficulty(MobileParty spotterParty, MobileParty party)
	{
		return ((MBGameModel<MapVisibilityModel>)this).BaseModel.GetPartySpottingDifficulty(spotterParty, party);
	}

	public override float GetHideoutSpottingDistance()
	{
		return ((MBGameModel<MapVisibilityModel>)this).BaseModel.GetHideoutSpottingDistance();
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace TaleWorlds.CampaignSystem.FastMode;

public class FastModeOptionsProvider : ICampaignOptionProvider
{
	public IEnumerable<ICampaignOptionData> GetGameplayCampaignOptions()
	{
		yield return (ICampaignOptionData)new BooleanCampaignOptionData("IsFastModeEnabled", 880, (CampaignOptionEnableState)2, (Func<float>)(() => 1f), (Action<float>)delegate
		{
		}, (Func<CampaignOptionDisableStatus>)null, false, (Func<float, CampaignOptionsDifficultyPresets>)null, (Func<CampaignOptionsDifficultyPresets, float>)null);
	}

	public IEnumerable<ICampaignOptionData> GetCharacterCreationCampaignOptions()
	{
		yield return (ICampaignOptionData)new BooleanCampaignOptionData("IsFastModeEnabled", 880, (CampaignOptionEnableState)2, (Func<float>)(() => 1f), (Action<float>)delegate
		{
		}, (Func<CampaignOptionDisableStatus>)null, false, (Func<float, CampaignOptionsDifficultyPresets>)null, (Func<CampaignOptionsDifficultyPresets, float>)null);
	}
}

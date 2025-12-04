using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.GameComponents;

public class NavalDLCVoiceOverModel : VoiceOverModel
{
	private const string NordClass = "nord";

	private const string PirateClass = "sea_raiders";

	public override string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject)
	{
		return ((MBGameModel<VoiceOverModel>)this).BaseModel.GetSoundPathForCharacter(character, voiceObject);
	}

	public override string GetAccentClass(CultureObject culture, bool isHighClass)
	{
		if (((MBObjectBase)culture).StringId == "nord")
		{
			return "nord";
		}
		if (((MBObjectBase)culture).StringId == "sea_raiders")
		{
			return "sea_raiders";
		}
		return ((MBGameModel<VoiceOverModel>)this).BaseModel.GetAccentClass(culture, isHighClass);
	}
}

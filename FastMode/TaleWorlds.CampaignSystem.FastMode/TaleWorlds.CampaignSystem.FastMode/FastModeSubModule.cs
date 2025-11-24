using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TaleWorlds.CampaignSystem.FastMode;

public class FastModeSubModule : MBSubModuleBase
{
	protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		GameType gameType = game.GameType;
		Campaign val = (Campaign)(object)((gameType is Campaign) ? gameType : null);
		if (val == null || (int)val.CampaignGameLoadingType != 1)
		{
			return;
		}
		val.Options.AccelerationMode = (GameAccelerationMode)1;
		CampaignGameStarter val2;
		if ((val2 = (CampaignGameStarter)(object)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null)) != null)
		{
			DefaultCharacterDevelopmentModel model = val2.GetModel<DefaultCharacterDevelopmentModel>();
			if (model != null)
			{
				model.InitializeXpRequiredForSkillLevel();
			}
		}
	}
}

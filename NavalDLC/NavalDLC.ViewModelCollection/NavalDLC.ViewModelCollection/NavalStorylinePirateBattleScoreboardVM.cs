using TaleWorlds.CampaignSystem;

namespace NavalDLC.ViewModelCollection;

public class NavalStorylinePirateBattleScoreboardVM : NavalScoreboardVM
{
	public NavalStorylinePirateBattleScoreboardVM(BattleSimulation simulation)
		: base(simulation)
	{
	}

	protected override bool IsPowerComparerRelevant()
	{
		return false;
	}
}

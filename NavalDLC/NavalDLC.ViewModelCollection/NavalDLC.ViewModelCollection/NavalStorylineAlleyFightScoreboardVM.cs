using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;

namespace NavalDLC.ViewModelCollection;

public class NavalStorylineAlleyFightScoreboardVM : SPScoreboardVM
{
	public NavalStorylineAlleyFightScoreboardVM(BattleSimulation simulation)
		: base(simulation)
	{
	}

	protected override bool IsPowerComparerRelevant()
	{
		return false;
	}
}

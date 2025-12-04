using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5;

public class Quest5DefeatPurigsShipObjective : MissionObjective
{
	private readonly List<Agent> _purigShipAgents;

	public override string UniqueId => "quest_5_defeat_purigs_ship_objective";

	public override TextObject Name => new TextObject("{=CedcuMUS}Defeat Purig's crew", (Dictionary<string, object>)null);

	public override TextObject Description => new TextObject("{=YDPv1Nsm}Board Purig's ship and defeat his crew.", (Dictionary<string, object>)null);

	public Quest5DefeatPurigsShipObjective(Mission mission, List<Agent> purigShipAgents)
		: base(mission)
	{
		_purigShipAgents = purigShipAgents;
	}

	protected override bool IsActivationRequirementsMet()
	{
		return true;
	}

	protected override bool IsCompletionRequirementsMet()
	{
		if (!Extensions.IsEmpty<Agent>((IEnumerable<Agent>)_purigShipAgents))
		{
			return !_purigShipAgents.Any((Agent a) => a.IsActive());
		}
		return true;
	}
}

using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace NavalDLC;

public interface INavalMapSceneWrapper
{
	List<(CampaignVec2, float)> GetSpawnPoints(string tag);

	CampaignVec2 GetDropOffLocation(Village village);

	Dictionary<Village, CampaignVec2> GetAllDropOffLocations();

	Vec2 GetWindAtPosition(Vec2 position);

	void Tick(float dt);
}

using System.Collections.Generic;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View;

public class NavalMapSceneWrapper : INavalMapSceneWrapper
{
	private MapScene _mapScene;

	private Dictionary<string, List<(CampaignVec2, float)>> _spawnPoints = new Dictionary<string, List<(CampaignVec2, float)>>();

	private Dictionary<Village, CampaignVec2> _dropOffPositions = new Dictionary<Village, CampaignVec2>();

	private float MaxDistanceBetweenDropOffLocationAndFishingZone => Campaign.Current.EstimatedAverageVillagerPartyNavalSpeed * (float)CampaignTime.HoursInDay;

	private float MaxDistanceBetweenDropOffLocationAndVillage => Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType((NavigationType)2) * 1.5f;

	public NavalMapSceneWrapper()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		_mapScene = (MapScene)Campaign.Current.MapSceneWrapper;
		InitializeSpawnPoints();
		InitializeDropOffLocations();
		InitializeMapWaterWake();
	}

	public void Tick(float dt)
	{
	}

	private void InitializeSpawnPoints()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		List<GameEntity> list = new List<GameEntity>();
		_mapScene.Scene.GetAllEntitiesWithScriptComponent<PirateSpawnPoint>(ref list);
		CampaignVec2 item = default(CampaignVec2);
		for (int i = 0; i < list.Count; i++)
		{
			PirateSpawnPoint firstScriptOfType = list[i].GetFirstScriptOfType<PirateSpawnPoint>();
			string clanStringId = firstScriptOfType.ClanStringId;
			if (!_spawnPoints.TryGetValue(clanStringId, out var _))
			{
				_spawnPoints[clanStringId] = new List<(CampaignVec2, float)>();
			}
			((CampaignVec2)(ref item))._002Ector(firstScriptOfType.GetPosition(), false);
			_spawnPoints[clanStringId].Add((item, firstScriptOfType.Radius));
		}
	}

	public List<(CampaignVec2, float)> GetSpawnPoints(string stringId)
	{
		if (_spawnPoints.TryGetValue(stringId, out var value))
		{
			return value;
		}
		return new List<(CampaignVec2, float)>();
	}

	private List<(CampaignVec2, float)> GetSpawnPoints()
	{
		List<(CampaignVec2, float)> list = new List<(CampaignVec2, float)>();
		foreach (KeyValuePair<string, List<(CampaignVec2, float)>> spawnPoint in _spawnPoints)
		{
			list.AddRange(spawnPoint.Value);
		}
		return list;
	}

	private void InitializeDropOffLocations()
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		MBList<int> obj = Extensions.ToMBList<int>(Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType((NavigationType)3));
		((List<int>)(object)obj).Add(11);
		((List<int>)(object)obj).Add(22);
		((List<int>)(object)obj).Add(25);
		int[] array = ((List<int>)(object)obj).ToArray();
		float embarkDisembarkThresholdDistance = Campaign.Current.Models.PartyNavigationModel.GetEmbarkDisembarkThresholdDistance();
		int num = (int)(Campaign.MapDiagonal * 0.5f);
		GetSpawnPoints();
		float embarkDisembarkThresholdDistance2 = Campaign.Current.Models.PartyNavigationModel.GetEmbarkDisembarkThresholdDistance();
		CampaignVec2 val3 = default(CampaignVec2);
		CampaignVec2 value = default(CampaignVec2);
		foreach (Settlement item in (List<Settlement>)(object)Settlement.All)
		{
			if (!item.IsVillage)
			{
				continue;
			}
			Village village = item.Village;
			if (village.VillageType != DefaultVillageTypes.Fisherman)
			{
				continue;
			}
			Debug.Print($"Initializing fishing points for: {((SettlementComponent)village).Name}", 0, (DebugColor)12, 17592186044416uL);
			CampaignVec2 position = ((SettlementComponent)village).Settlement.Position;
			MapScene mapScene = _mapScene;
			CampaignVec2 position2 = ((SettlementComponent)village).Settlement.Position;
			CampaignVec2 nearestFaceCenterForPositionWithPath = mapScene.GetNearestFaceCenterForPositionWithPath(((CampaignVec2)(ref position2)).Face, false, Campaign.MapDiagonal / 2f, array);
			NavigationPath val = new NavigationPath();
			if (_mapScene.GetPathBetweenAIFaces(((CampaignVec2)(ref position)).Face, ((CampaignVec2)(ref nearestFaceCenterForPositionWithPath)).Face, ((CampaignVec2)(ref position)).ToVec2(), ((CampaignVec2)(ref nearestFaceCenterForPositionWithPath)).ToVec2(), embarkDisembarkThresholdDistance, val, array, 2f, 0, num))
			{
				float num2 = 0f;
				bool flag = false;
				for (int i = 0; i < val.Size; i++)
				{
					if (flag)
					{
						break;
					}
					Vec2 val2 = val.PathPoints[i];
					if (!((Vec2)(ref val2)).IsValid || !((Vec2)(ref val2)).IsNonZero())
					{
						break;
					}
					((CampaignVec2)(ref val3))._002Ector(val2, false);
					if (((CampaignVec2)(ref val3)).IsValid())
					{
						TerrainType terrainTypeAtPosition = _mapScene.GetTerrainTypeAtPosition(ref val3);
						if (Campaign.Current.Models.PartyNavigationModel.IsTerrainTypeValidForNavigationType(terrainTypeAtPosition, (NavigationType)2))
						{
							Vec2 val4 = ((CampaignVec2)(ref position)).ToVec2();
							if (i > 0)
							{
								val4 = val.PathPoints[i - 1];
							}
							float num3 = embarkDisembarkThresholdDistance2;
							Vec2 val5 = val2 - val4;
							if (num2 + ((Vec2)(ref val5)).Length > MaxDistanceBetweenDropOffLocationAndVillage)
							{
								val5 *= (MaxDistanceBetweenDropOffLocationAndVillage - num2) / ((Vec2)(ref val5)).Length;
							}
							int num4 = MathF.Ceiling(((Vec2)(ref val5)).Length / num3);
							num3 = ((Vec2)(ref val5)).Length / (float)num4;
							((Vec2)(ref val5)).Normalize();
							for (int j = 1; j <= num4; j++)
							{
								if (flag)
								{
									break;
								}
								Vec2 val6 = val4 + val5 * (float)j * num3;
								((CampaignVec2)(ref value))._002Ector(val6, false);
								if (((CampaignVec2)(ref value)).IsValid())
								{
									flag = true;
									_dropOffPositions[village] = value;
								}
							}
						}
					}
					if (!flag)
					{
						if (i > 0)
						{
							num2 += ((Vec2)(ref val2)).Distance(val.PathPoints[i - 1]);
						}
						else
						{
							Vec2 val7 = ((CampaignVec2)(ref position)).ToVec2();
							num2 = ((Vec2)(ref val7)).Distance(val.PathPoints[0]);
						}
						if (num2 >= MaxDistanceBetweenDropOffLocationAndVillage)
						{
							Debug.Print($"Closest distance between {((SettlementComponent)village).Name} and its drop off is too far", 0, (DebugColor)12, 17592186044416uL);
							flag = true;
							break;
						}
					}
				}
			}
			else
			{
				Debug.FailedAssert("Path cannot be found to closest valid point for fishing village drop of point", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\NavalMapSceneWrapper.cs", "InitializeDropOffLocations", 195);
			}
		}
	}

	public CampaignVec2 GetDropOffLocation(Village village)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (_dropOffPositions.TryGetValue(village, out var value))
		{
			return value;
		}
		return CampaignVec2.Invalid;
	}

	public Dictionary<Village, CampaignVec2> GetAllDropOffLocations()
	{
		return _dropOffPositions;
	}

	public Vec2 GetWindAtPosition(Vec2 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _mapScene.GetWindAtPosition(position);
	}

	private void InitializeMapWaterWake()
	{
		_mapScene.SetupWaterWake(128f, 8f);
	}
}

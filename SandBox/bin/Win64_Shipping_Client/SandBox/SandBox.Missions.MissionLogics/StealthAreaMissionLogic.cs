using System.Collections.Generic;
using System.Linq;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics;

public class StealthAreaMissionLogic : MissionLogic
{
	public delegate List<IAgentOriginBase> GetReinforcementAllyTroopsDelegate(StealthAreaData triggeredStealthAreaData, StealthAreaMarker stealthAreaMarker);

	public class StealthAreaData
	{
		internal bool IsStealthAreaTriggered;

		internal bool IsReinforcementCalled;

		internal readonly StealthAreaUsePoint StealthAreaUsePoint;

		internal readonly Dictionary<StealthAreaMarker, List<Agent>> StealthAreaMarkers;

		internal StealthAreaData(StealthAreaUsePoint stealthAreaUsePoint)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			StealthAreaUsePoint = stealthAreaUsePoint;
			StealthAreaMarkers = new Dictionary<StealthAreaMarker, List<Agent>>();
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)stealthAreaUsePoint).GameEntity;
			foreach (WeakGameEntity child in ((WeakGameEntity)(ref gameEntity)).GetChildren())
			{
				WeakGameEntity current = child;
				if (((WeakGameEntity)(ref current)).HasScriptOfType<StealthAreaMarker>())
				{
					StealthAreaMarkers.Add(((WeakGameEntity)(ref current)).GetFirstScriptOfType<StealthAreaMarker>(), new List<Agent>());
				}
			}
		}

		internal void AddAgentToStealthAreaMarker(StealthAreaMarker stealthAreaMarker, Agent agent)
		{
			StealthAreaMarkers[stealthAreaMarker].Add(agent);
		}

		internal void RemoveAgentFromStealthAreaMarker(StealthAreaMarker stealthAreaMarker, Agent agent)
		{
			StealthAreaMarkers[stealthAreaMarker].Remove(agent);
			if (StealthAreaMarkers.All((KeyValuePair<StealthAreaMarker, List<Agent>> x) => Extensions.IsEmpty<Agent>((IEnumerable<Agent>)x.Value)))
			{
				StealthAreaUsePoint.EnableStealthAreaUsePoint();
				IsStealthAreaTriggered = true;
			}
		}
	}

	private readonly MBList<StealthAreaData> _stealthAreaData = new MBList<StealthAreaData>();

	private readonly Dictionary<string, Dictionary<string, int>> _agentSpawnTypes = new Dictionary<string, Dictionary<string, int>>();

	private readonly MBList<Agent> _allyTroops = new MBList<Agent>();

	public GetReinforcementAllyTroopsDelegate GetReinforcementAllyTroops;

	public MBReadOnlyList<Agent> AllyTroops => (MBReadOnlyList<Agent>)(object)_allyTroops;

	public bool AllReinforcementsCalled { get; private set; }

	public StealthAreaMissionLogic()
	{
		SetAgentSpawnTypes();
	}

	public bool IsSentry(Agent agent)
	{
		foreach (StealthAreaData item in (List<StealthAreaData>)(object)_stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in item.StealthAreaMarkers)
			{
				if (stealthAreaMarker.Value.Contains(agent))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnBehaviorInitialize()
	{
		((MissionBehavior)this).OnBehaviorInitialize();
		foreach (StealthAreaUsePoint item in MBExtensions.FindAllWithType<StealthAreaUsePoint>((IEnumerable<MissionObject>)((MissionBehavior)this).Mission.MissionObjects))
		{
			((List<StealthAreaData>)(object)_stealthAreaData).Add(new StealthAreaData(item));
		}
	}

	public void AddAgentSpawnType(string spawnGroupId, Dictionary<string, int> spawnDictionary)
	{
		_agentSpawnTypes[spawnGroupId] = spawnDictionary;
	}

	private void SetAgentSpawnTypes()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("deserter", 1);
		dictionary.Add("forest_bandits_bandit", 2);
		_agentSpawnTypes.Add("reinforcement_ally_group_1", dictionary);
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		dictionary2.Add("aserai_footman", 3);
		dictionary2.Add("aserai_skirmisher", 2);
		_agentSpawnTypes.Add("reinforcement_ally_group_cambush", dictionary2);
	}

	private List<IAgentOriginBase> GetReinforcementAllyGroupTroops(StealthAreaData triggeredStealthAreaData, StealthAreaMarker stealthAreaMarker)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		if (GetReinforcementAllyTroops == null)
		{
			string reinforcementAllyGroupId = stealthAreaMarker.ReinforcementAllyGroupId;
			List<IAgentOriginBase> list = new List<IAgentOriginBase>();
			if (_agentSpawnTypes.TryGetValue(reinforcementAllyGroupId, out var value))
			{
				foreach (KeyValuePair<string, int> item in value)
				{
					CharacterObject val = MBObjectManager.Instance.GetObject<CharacterObject>(item.Key);
					int value2 = item.Value;
					for (int i = 0; i < value2; i++)
					{
						list.Add((IAgentOriginBase)new PartyAgentOrigin(PartyBase.MainParty, val, -1, default(UniqueTroopDescriptor), false, false));
					}
				}
			}
			return list;
		}
		return GetReinforcementAllyTroops(triggeredStealthAreaData, stealthAreaMarker);
	}

	public override void OnAgentBuild(Agent agent, Banner banner)
	{
		((MissionBehavior)this).OnAgentBuild(agent, banner);
		CheckStealthAreaMarkerForAgent(agent);
	}

	public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
	{
		((MissionBehavior)this).OnAgentTeamChanged(prevTeam, newTeam, agent);
		CheckStealthAreaMarkerForAgent(agent);
	}

	private void CheckStealthAreaMarkerForAgent(Agent agent)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!agent.IsHuman || agent.Team != Mission.Current.PlayerEnemyTeam)
		{
			return;
		}
		foreach (StealthAreaData item in (List<StealthAreaData>)(object)_stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in item.StealthAreaMarkers)
			{
				if (((AreaMarker)stealthAreaMarker.Key).IsPositionInRange(agent.Position))
				{
					item.AddAgentToStealthAreaMarker(stealthAreaMarker.Key, agent);
					break;
				}
			}
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		if (affectorAgent == null || !affectorAgent.IsMainAgent)
		{
			return;
		}
		foreach (StealthAreaData item in (List<StealthAreaData>)(object)_stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in item.StealthAreaMarkers)
			{
				if (stealthAreaMarker.Value.Contains(affectedAgent))
				{
					item.RemoveAgentFromStealthAreaMarker(stealthAreaMarker.Key, affectedAgent);
				}
			}
		}
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (usedObject is StealthAreaUsePoint)
		{
			StealthAreaData stealthAreaData = null;
			foreach (StealthAreaData item in (List<StealthAreaData>)(object)_stealthAreaData)
			{
				if ((object)item.StealthAreaUsePoint == usedObject)
				{
					stealthAreaData = item;
					break;
				}
			}
			if (stealthAreaData != null)
			{
				stealthAreaData.IsReinforcementCalled = true;
				foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in stealthAreaData.StealthAreaMarkers)
				{
					List<IAgentOriginBase> reinforcementAllyGroupTroops = GetReinforcementAllyGroupTroops(stealthAreaData, stealthAreaMarker.Key);
					if (!Extensions.IsEmpty<IAgentOriginBase>((IEnumerable<IAgentOriginBase>)reinforcementAllyGroupTroops))
					{
						foreach (IAgentOriginBase item2 in reinforcementAllyGroupTroops)
						{
							SpawnAllyAgent(item2, stealthAreaMarker.Key.ReinforcementAllyGroupSpawnPoint, stealthAreaMarker.Key.WaitPoint.GlobalPosition);
						}
					}
					else
					{
						Debug.FailedAssert("There is not any troops to spawn as stealth area reinforcement.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\StealthAreaMissionLogic.cs", "OnObjectUsed", 269);
					}
				}
			}
		}
		AllReinforcementsCalled = ((IEnumerable<StealthAreaData>)_stealthAreaData).All((StealthAreaData x) => x.IsReinforcementCalled);
	}

	private void SpawnAllyAgent(IAgentOriginBase character, GameEntity spawnPoint, Vec3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = spawnPoint.GetGlobalFrame();
		Mission current = Mission.Current;
		Vec3? val = globalFrame.origin;
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Agent val2 = current.SpawnTroop(character, true, false, false, false, 0, 0, true, true, true, val, (Vec2?)((Vec2)(ref asVec)).Normalized(), (string)null, (ItemObject)null, (FormationClass)10, false);
		Vec3 randomPositionAroundPoint = Mission.Current.GetRandomPositionAroundPoint(position, 0f, 2f, true);
		WorldPosition val3 = default(WorldPosition);
		((WorldPosition)(ref val3))._002Ector(spawnPoint.Scene, randomPositionAroundPoint);
		val2.SetScriptedPosition(ref val3, true, (AIScriptedFrameFlags)514);
		((List<Agent>)(object)_allyTroops).Add(val2);
	}

	public bool CheckIfAllStealthAreasAreTriggered()
	{
		return ((IEnumerable<StealthAreaData>)_stealthAreaData).All((StealthAreaData x) => x.IsStealthAreaTriggered);
	}

	public bool CheckIfAllStealthAreasReinforcementsAreCalled()
	{
		return AllReinforcementsCalled;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Missions.MissionLogics.Hideout;

public class HideoutMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
{
	private class MissionSide
	{
		private readonly BattleSideEnum _side;

		private readonly IMissionTroopSupplier _troopSupplier;

		public readonly bool IsPlayerSide;

		private int _numberOfSpawnedTroops;

		public bool TroopSpawningActive { get; private set; }

		public int NumberOfActiveTroops => _numberOfSpawnedTroops - _troopSupplier.NumRemovedTroops;

		public int NumberOfTroopsNotSupplied => _troopSupplier.NumTroopsNotSupplied;

		public MissionSide(BattleSideEnum side, IMissionTroopSupplier troopSupplier, bool isPlayerSide)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_side = side;
			IsPlayerSide = isPlayerSide;
			_troopSupplier = troopSupplier;
		}

		public void SpawnTroops(List<CommonAreaMarker> areaMarkers, List<PatrolArea> patrolAreas, Dictionary<Agent, UsedObject> defenderAgentObjects, int spawnCount)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Invalid comparison between I4 and Unknown
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			bool flag = false;
			List<StandingPoint> list = new List<StandingPoint>();
			foreach (CommonAreaMarker areaMarker in areaMarkers)
			{
				foreach (UsableMachine item in ((AreaMarker)areaMarker).GetUsableMachinesInRange((string)null))
				{
					list.AddRange((IEnumerable<StandingPoint>)item.StandingPoints);
				}
			}
			List<IAgentOriginBase> list2 = _troopSupplier.SupplyTroops(spawnCount).ToList();
			for (int i = 0; i < list2.Count; i++)
			{
				if (1 == (int)_side)
				{
					Mission.Current.SpawnTroop(list2[i], true, true, false, false, 0, 0, true, true, true, (Vec3?)null, (Vec2?)null, (string)null, (ItemObject)null, (FormationClass)10, false);
					_numberOfSpawnedTroops++;
				}
				else
				{
					if (areaMarkers.Count <= num)
					{
						continue;
					}
					StandingPoint val = null;
					int num2 = list2.Count - i;
					if (num2 < list.Count / 2 && num2 < 4)
					{
						flag = true;
					}
					if (!flag)
					{
						Extensions.Shuffle<StandingPoint>((IList<StandingPoint>)list);
						val = ((IEnumerable<StandingPoint>)list).FirstOrDefault((Func<StandingPoint, bool>)((StandingPoint point) => !((UsableMissionObject)point).IsDeactivated && !((MissionObject)point).IsDisabled && !((UsableMissionObject)point).HasUser));
					}
					else
					{
						IEnumerable<PatrolArea> enumerable = patrolAreas.Where((PatrolArea area) => ((IEnumerable<StandingPoint>)((UsableMachine)area).StandingPoints).All((StandingPoint point) => !((UsableMissionObject)point).HasUser && !((UsableMissionObject)point).HasAIMovingTo));
						if (!Extensions.IsEmpty<PatrolArea>(enumerable))
						{
							val = ((List<StandingPoint>)(object)((UsableMachine)enumerable.First()).StandingPoints)[0];
						}
					}
					if (val != null && !((MissionObject)val).IsDisabled)
					{
						WeakGameEntity val2 = ((ScriptComponentBehavior)val).GameEntity;
						MatrixFrame globalFrame = ((WeakGameEntity)(ref val2)).GetGlobalFrame();
						((Mat3)(ref globalFrame.rotation)).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
						Mission current2 = Mission.Current;
						IAgentOriginBase obj = list2[i];
						Vec3? val3 = globalFrame.origin;
						Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
						Agent agent = current2.SpawnTroop(obj, false, false, false, false, 0, 0, false, false, false, val3, (Vec2?)((Vec2)(ref asVec)).Normalized(), "_hideout_bandit", (ItemObject)null, (FormationClass)10, false);
						InitializeBanditAgent(agent, val, flag, defenderAgentObjects);
						_numberOfSpawnedTroops++;
						int groupId = ((AnimationPoint)(object)val).GroupId;
						if (flag)
						{
							continue;
						}
						val2 = ((ScriptComponentBehavior)val).GameEntity;
						val2 = ((WeakGameEntity)(ref val2)).Parent;
						foreach (StandingPoint item2 in (List<StandingPoint>)(object)((WeakGameEntity)(ref val2)).GetFirstScriptOfType<UsableMachine>().StandingPoints)
						{
							int groupId2 = ((AnimationPoint)(object)item2).GroupId;
							if (groupId == groupId2 && item2 != val)
							{
								((MissionObject)item2).SetDisabledAndMakeInvisible(false, false);
							}
						}
					}
					else
					{
						num++;
					}
				}
			}
			foreach (Formation item3 in (List<Formation>)(object)Mission.Current.AttackerTeam.FormationsIncludingEmpty)
			{
				if (item3.CountOfUnits > 0)
				{
					item3.SetMovementOrder(MovementOrder.MovementOrderMove(item3.CachedMedianPosition));
				}
				item3.SetFiringOrder(FiringOrder.FiringOrderHoldYourFire);
				if (Mission.Current.AttackerTeam == Mission.Current.PlayerTeam)
				{
					item3.PlayerOwner = Mission.Current.MainAgent;
				}
			}
		}

		public void SpawnRemainingTroopsForBossFight(List<MatrixFrame> spawnFrames, int spawnCount)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			List<IAgentOriginBase> list = _troopSupplier.SupplyTroops(spawnCount).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				MatrixFrame val = spawnFrames.FirstOrDefault();
				((Mat3)(ref val.rotation)).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Mission current = Mission.Current;
				IAgentOriginBase obj = list[i];
				Vec3? val2 = val.origin;
				Vec2 asVec = ((Vec3)(ref val.rotation.f)).AsVec2;
				Agent val3 = current.SpawnTroop(obj, false, false, false, false, 0, 0, false, false, false, val2, (Vec2?)((Vec2)(ref asVec)).Normalized(), "_hideout_bandit", (ItemObject)null, (FormationClass)10, false);
				AgentFlag agentFlags = val3.GetAgentFlags();
				if (Extensions.HasAnyFlag<AgentFlag>(agentFlags, (AgentFlag)1048576))
				{
					val3.SetAgentFlags((AgentFlag)(agentFlags & -1048577));
				}
				_numberOfSpawnedTroops++;
			}
			foreach (Formation item in (List<Formation>)(object)Mission.Current.AttackerTeam.FormationsIncludingEmpty)
			{
				if (item.CountOfUnits > 0)
				{
					item.SetMovementOrder(MovementOrder.MovementOrderMove(item.CachedMedianPosition));
				}
				item.SetFiringOrder(FiringOrder.FiringOrderHoldYourFire);
				if (Mission.Current.AttackerTeam == Mission.Current.PlayerTeam)
				{
					item.PlayerOwner = Mission.Current.MainAgent;
				}
			}
		}

		private void InitializeBanditAgent(Agent agent, StandingPoint spawnPoint, bool isPatrolling, Dictionary<Agent, UsedObject> defenderAgentObjects)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			WeakGameEntity val;
			object obj;
			if (!isPatrolling)
			{
				val = ((ScriptComponentBehavior)spawnPoint).GameEntity;
				val = ((WeakGameEntity)(ref val)).Parent;
				obj = ((WeakGameEntity)(ref val)).GetScriptComponents<UsableMachine>().FirstOrDefault();
			}
			else
			{
				val = ((ScriptComponentBehavior)spawnPoint).GameEntity;
				val = ((WeakGameEntity)(ref val)).Parent;
				obj = ((WeakGameEntity)(ref val)).GetScriptComponents<PatrolArea>().FirstOrDefault();
			}
			UsableMachine val2 = (UsableMachine)obj;
			if (isPatrolling)
			{
				((IDetachment)val2).AddAgent(agent, -1, (AIScriptedFrameFlags)0);
				agent.WieldInitialWeapons((WeaponWieldActionType)2, (InitialWeaponEquipPreference)0);
			}
			else
			{
				agent.UseGameObject((UsableMissionObject)(object)spawnPoint, -1);
			}
			defenderAgentObjects.Add(agent, new UsedObject(val2, isPatrolling));
			AgentFlag agentFlags = agent.GetAgentFlags();
			agent.SetAgentFlags((AgentFlag)((agentFlags | 0x10000) & -1048577));
			agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator().AddBehaviorGroup<AlarmedBehaviorGroup>()
				.AddBehavior<CautiousBehavior>();
			SimulateTick(agent);
		}

		private void SimulateTick(Agent agent)
		{
			int num = MBRandom.RandomInt(1, 20);
			for (int i = 0; i < num; i++)
			{
				if (agent.IsUsingGameObject)
				{
					agent.CurrentlyUsedGameObject.SimulateTick(0.1f);
				}
			}
		}

		public void SetSpawnTroops(bool spawnTroops)
		{
			TroopSpawningActive = spawnTroops;
		}

		public IEnumerable<IAgentOriginBase> GetAllTroops()
		{
			return _troopSupplier.GetAllTroops();
		}
	}

	private class UsedObject
	{
		public readonly UsableMachine Machine;

		public readonly UsableMachineAIBase MachineAI;

		public bool IsMachineAITicked;

		public UsedObject(UsableMachine machine, bool isMachineAITicked)
		{
			Machine = machine;
			MachineAI = machine.CreateAIBehaviorObject();
			IsMachineAITicked = isMachineAITicked;
		}
	}

	private enum HideoutMissionState
	{
		NotDecided,
		WithoutBossFight,
		InitialFightBeforeBossFight,
		CutSceneBeforeBossFight,
		ConversationBetweenLeaders,
		BossFightWithDuel,
		BossFightWithAll
	}

	private const int FirstPhaseEndInSeconds = 4;

	private readonly List<CommonAreaMarker> _areaMarkers;

	private readonly List<PatrolArea> _patrolAreas;

	private readonly Dictionary<Agent, UsedObject> _defenderAgentObjects;

	private readonly MissionSide[] _missionSides;

	private List<Agent> _duelPhaseAllyAgents;

	private List<Agent> _duelPhaseBanditAgents;

	private BattleAgentLogic _battleAgentLogic;

	private BattleEndLogic _battleEndLogic;

	private AgentVictoryLogic _agentVictoryLogic;

	private HideoutMissionState _hideoutMissionState;

	private Agent _bossAgent;

	private Team _enemyTeam;

	private Timer _firstPhaseEndTimer;

	private bool _troopsInitialized;

	private bool _isMissionInitialized;

	private bool _battleResolved;

	private int _firstPhaseEnemyTroopCount;

	private int _firstPhasePlayerSideTroopCount;

	private MissionMode _oldMissionMode;

	private HideoutCinematicController _cinematicController;

	public HideoutMissionController(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, int firstPhaseEnemyTroopCount, int firstPhasePlayerSideTroopCount)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between I4 and Unknown
		_areaMarkers = new List<CommonAreaMarker>();
		_patrolAreas = new List<PatrolArea>();
		_defenderAgentObjects = new Dictionary<Agent, UsedObject>();
		_firstPhaseEnemyTroopCount = firstPhaseEnemyTroopCount;
		_firstPhasePlayerSideTroopCount = firstPhasePlayerSideTroopCount;
		_missionSides = new MissionSide[2];
		for (int i = 0; i < 2; i++)
		{
			IMissionTroopSupplier troopSupplier = suppliers[i];
			bool isPlayerSide = i == (int)playerSide;
			_missionSides[i] = new MissionSide((BattleSideEnum)i, troopSupplier, isPlayerSide);
		}
	}

	public override void OnCreated()
	{
		((MissionBehavior)this).OnCreated();
		((MissionBehavior)this).Mission.DoesMissionRequireCivilianEquipment = false;
	}

	public override void OnBehaviorInitialize()
	{
		((MissionBehavior)this).OnBehaviorInitialize();
		_battleAgentLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<BattleAgentLogic>();
		_battleEndLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<BattleEndLogic>();
		_battleEndLogic.ChangeCanCheckForEndCondition(false);
		_agentVictoryLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<AgentVictoryLogic>();
		_cinematicController = ((MissionBehavior)this).Mission.GetMissionBehavior<HideoutCinematicController>();
		((MissionBehavior)this).Mission.IsMainAgentObjectInteractionEnabled = false;
		_cinematicController = ((MissionBehavior)this).Mission.GetMissionBehavior<HideoutCinematicController>();
	}

	public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (usedObject != null && usedObject is AnimationPoint && userAgent.IsActive() && userAgent.IsAIControlled && (int)userAgent.CurrentWatchState == 0)
		{
			WeakGameEntity val = ((ScriptComponentBehavior)usedObject).GameEntity;
			val = ((WeakGameEntity)(ref val)).Parent;
			PatrolArea firstScriptOfType = ((WeakGameEntity)(ref val)).GetFirstScriptOfType<PatrolArea>();
			if (firstScriptOfType != null)
			{
				((IDetachment)firstScriptOfType).AddAgent(userAgent, -1, (AIScriptedFrameFlags)0);
			}
		}
	}

	public override void OnAgentAlarmedStateChanged(Agent agent, AIStateFlag flag)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (_hideoutMissionState >= HideoutMissionState.ConversationBetweenLeaders || agent.Team != ((MissionBehavior)this).Mission.DefenderTeam)
		{
			return;
		}
		bool num = (flag & 3) == 3;
		if (num || (flag & 3) == 1)
		{
			if (agent.IsUsingGameObject)
			{
				agent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)1);
			}
			else
			{
				agent.DisableScriptedMovement();
				if (agent.IsAIControlled && AgentComponentExtensions.AIMoveToGameObjectIsEnabled(agent))
				{
					AgentComponentExtensions.AIMoveToGameObjectDisable(agent);
					Formation formation = agent.Formation;
					if (formation != null)
					{
						formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(agent);
					}
				}
			}
			_defenderAgentObjects[agent].IsMachineAITicked = false;
		}
		else if ((flag & 3) == 0)
		{
			_defenderAgentObjects[agent].IsMachineAITicked = true;
			agent.TryToSheathWeaponInHand((HandIndex)0, (WeaponWieldActionType)0);
			((IDetachment)_defenderAgentObjects[agent].Machine).AddAgent(agent, -1, (AIScriptedFrameFlags)0);
		}
		if (num)
		{
			agent.SetWantsToYell();
		}
	}

	public override void OnMissionTick(float dt)
	{
		if (!_isMissionInitialized)
		{
			InitializeMission();
			_isMissionInitialized = true;
			return;
		}
		if (!_troopsInitialized)
		{
			_troopsInitialized = true;
			foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.Agents)
			{
				((MissionBehavior)_battleAgentLogic).OnAgentBuild(item, (Banner)null);
			}
		}
		UsedObjectTick(dt);
		if (!_battleResolved)
		{
			CheckBattleResolved();
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		if (_hideoutMissionState == HideoutMissionState.BossFightWithDuel)
		{
			foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.Agents)
			{
				if (item != affectedAgent && item != affectorAgent && item.IsActive() && item.GetLookAgent() == affectedAgent)
				{
					item.SetLookAgent((Agent)null);
				}
			}
			return;
		}
		if (_hideoutMissionState == HideoutMissionState.InitialFightBeforeBossFight && affectedAgent.IsMainAgent)
		{
			((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
			affectedAgent.Formation = null;
			((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SetOrder((OrderType)9);
		}
	}

	private void InitializeMission()
	{
		((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(isDisabled: true);
		((MissionBehavior)this).Mission.SetMissionMode((MissionMode)4, true);
		_areaMarkers.AddRange(from area in MBExtensions.FindAllWithType<CommonAreaMarker>((IEnumerable<MissionObject>)((MissionBehavior)this).Mission.ActiveMissionObjects)
			orderby ((AreaMarker)area).AreaIndex
			select area);
		_patrolAreas.AddRange(from area in MBExtensions.FindAllWithType<PatrolArea>((IEnumerable<MissionObject>)((MissionBehavior)this).Mission.ActiveMissionObjects)
			orderby area.AreaIndex
			select area);
		DecideMissionState();
		((MissionBehavior)this).Mission.DeploymentPlan.MakeDefaultDeploymentPlans();
		for (int num = 0; num < 2; num++)
		{
			int spawnCount;
			if (_missionSides[num].IsPlayerSide)
			{
				spawnCount = _firstPhasePlayerSideTroopCount;
			}
			else
			{
				if (_missionSides[num].NumberOfTroopsNotSupplied <= _firstPhaseEnemyTroopCount)
				{
					Debug.FailedAssert("_missionSides[i].NumberOfTroopsNotSupplied <= _firstPhaseEnemyTroopCount", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\SandBox\\Missions\\MissionLogics\\Hideout\\HideoutMissionController.cs", "InitializeMission", 482);
					_firstPhaseEnemyTroopCount = (int)((float)_missionSides[num].NumberOfTroopsNotSupplied * 0.7f);
				}
				spawnCount = ((_hideoutMissionState == HideoutMissionState.InitialFightBeforeBossFight) ? _firstPhaseEnemyTroopCount : _missionSides[num].NumberOfTroopsNotSupplied);
			}
			_missionSides[num].SpawnTroops(_areaMarkers, _patrolAreas, _defenderAgentObjects, spawnCount);
		}
	}

	private void UsedObjectTick(float dt)
	{
		foreach (KeyValuePair<Agent, UsedObject> defenderAgentObject in _defenderAgentObjects)
		{
			if (defenderAgentObject.Value.IsMachineAITicked)
			{
				defenderAgentObject.Value.MachineAI.Tick(defenderAgentObject.Key, (Formation)null, (Team)null, dt);
			}
		}
	}

	protected override void OnEndMission()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (_hideoutMissionState == HideoutMissionState.BossFightWithDuel)
		{
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				num = _duelPhaseAllyAgents?.Count ?? 0;
			}
			else if (_bossAgent == null || !_bossAgent.IsActive())
			{
				PlayerEncounter.EnemySurrender = true;
			}
		}
		if (MobileParty.MainParty.MemberRoster.TotalHealthyCount <= num && (int)MapEvent.PlayerMapEvent.BattleState == 0)
		{
			MapEvent.PlayerMapEvent.SetOverrideWinner((BattleSideEnum)0);
		}
	}

	private void CheckBattleResolved()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (_hideoutMissionState == HideoutMissionState.CutSceneBeforeBossFight || _hideoutMissionState == HideoutMissionState.ConversationBetweenLeaders)
		{
			return;
		}
		if (IsSideDepleted((BattleSideEnum)1))
		{
			if (_hideoutMissionState == HideoutMissionState.BossFightWithDuel)
			{
				OnDuelOver((BattleSideEnum)0);
			}
			_battleEndLogic.ChangeCanCheckForEndCondition(true);
			_battleResolved = true;
		}
		else
		{
			if (!IsSideDepleted((BattleSideEnum)0))
			{
				return;
			}
			if (_hideoutMissionState == HideoutMissionState.InitialFightBeforeBossFight)
			{
				if (_firstPhaseEndTimer == null)
				{
					_firstPhaseEndTimer = new Timer(((MissionBehavior)this).Mission.CurrentTime, 4f, true);
					_oldMissionMode = Mission.Current.Mode;
					Mission.Current.SetMissionMode((MissionMode)9, false);
				}
				else if (_firstPhaseEndTimer.Check(((MissionBehavior)this).Mission.CurrentTime))
				{
					_cinematicController.StartCinematic(OnInitialFadeOutOver, OnCutSceneOver);
				}
			}
			else
			{
				if (_hideoutMissionState == HideoutMissionState.BossFightWithDuel)
				{
					OnDuelOver((BattleSideEnum)1);
				}
				_battleEndLogic.ChangeCanCheckForEndCondition(true);
				MapEvent.PlayerMapEvent.SetOverrideWinner((BattleSideEnum)1);
				_battleResolved = true;
			}
		}
	}

	public void StartSpawner(BattleSideEnum side)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_missionSides[side].SetSpawnTroops(spawnTroops: true);
	}

	public void StopSpawner(BattleSideEnum side)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_missionSides[side].SetSpawnTroops(spawnTroops: false);
	}

	public bool IsSideSpawnEnabled(BattleSideEnum side)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _missionSides[side].TroopSpawningActive;
	}

	public float GetReinforcementInterval()
	{
		return 0f;
	}

	public unsafe bool IsSideDepleted(BattleSideEnum side)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _missionSides[side].NumberOfActiveTroops == 0;
		if (!flag)
		{
			if ((Agent.Main == null || !Agent.Main.IsActive()) && (int)side == 1)
			{
				if (_hideoutMissionState == HideoutMissionState.BossFightWithDuel || _hideoutMissionState == HideoutMissionState.InitialFightBeforeBossFight)
				{
					flag = true;
				}
				else if (_hideoutMissionState == HideoutMissionState.WithoutBossFight || _hideoutMissionState == HideoutMissionState.BossFightWithAll)
				{
					bool num = ((IEnumerable<Formation>)((MissionBehavior)this).Mission.Teams.Attacker.FormationsIncludingEmpty).Any(delegate(Formation f)
					{
						//IL_000f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0014: Unknown result type (might be due to invalid IL or missing references)
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						//IL_001d: Invalid comparison between Unknown and I4
						if (f.CountOfUnits > 0)
						{
							MovementOrder readonlyMovementOrderReference = Unsafe.Read<MovementOrder>((void*)f.GetReadonlyMovementOrderReference());
							return (int)((MovementOrder)(ref readonlyMovementOrderReference)).OrderType == 4;
						}
						return false;
					});
					bool flag2 = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Teams.Defender.ActiveAgents).Any((Agent t) => (int)t.CurrentWatchState == 2);
					flag = !num && !flag2;
				}
			}
			else if ((int)side == 0 && _hideoutMissionState == HideoutMissionState.BossFightWithDuel && (_bossAgent == null || !_bossAgent.IsActive()))
			{
				flag = true;
			}
		}
		else if ((int)side == 0 && _hideoutMissionState == HideoutMissionState.InitialFightBeforeBossFight && (Agent.Main == null || !Agent.Main.IsActive()))
		{
			flag = false;
		}
		return flag;
	}

	private void DecideMissionState()
	{
		MissionSide missionSide = _missionSides[0];
		_hideoutMissionState = (missionSide.IsPlayerSide ? HideoutMissionState.WithoutBossFight : HideoutMissionState.InitialFightBeforeBossFight);
	}

	private void SetWatchStateOfAIAgents(WatchState state)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.Agents)
		{
			if (item.IsAIControlled)
			{
				item.SetWatchState(state);
			}
		}
	}

	private void SpawnBossAndBodyguards()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		MissionSide missionSide = _missionSides[0];
		MatrixFrame banditsInitialFrame = _cinematicController.GetBanditsInitialFrame();
		missionSide.SpawnRemainingTroopsForBossFight(new List<MatrixFrame> { banditsInitialFrame }, missionSide.NumberOfTroopsNotSupplied);
		_bossAgent = SelectBossAgent();
		_bossAgent.WieldInitialWeapons((WeaponWieldActionType)2, (InitialWeaponEquipPreference)1);
		foreach (Agent item in (List<Agent>)(object)_enemyTeam.ActiveAgents)
		{
			if (item != _bossAgent)
			{
				item.WieldInitialWeapons((WeaponWieldActionType)3, (InitialWeaponEquipPreference)0);
			}
		}
	}

	private Agent SelectBossAgent()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Agent val = null;
		Agent val2 = null;
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.Agents)
		{
			if (item.Team != _enemyTeam || !item.IsHuman)
			{
				continue;
			}
			if (item.IsHero)
			{
				val = item;
				val2 = item;
				break;
			}
			if (item.Character.Culture.IsBandit)
			{
				BasicCultureObject culture = item.Character.Culture;
				BasicCultureObject obj = ((culture is CultureObject) ? culture : null);
				if (((obj != null) ? ((CultureObject)obj).BanditBoss : null) != null && (object)((CultureObject)item.Character.Culture).BanditBoss == item.Character)
				{
					val = item;
				}
			}
			if (val2 == null || item.Character.Level > val2.Character.Level)
			{
				val2 = item;
			}
		}
		return val ?? val2;
	}

	private void OnInitialFadeOutOver(ref Agent playerAgent, ref List<Agent> playerCompanions, ref Agent bossAgent, ref List<Agent> bossCompanions, ref float placementPerturbation, ref float placementAngle)
	{
		_hideoutMissionState = HideoutMissionState.CutSceneBeforeBossFight;
		_enemyTeam = ((MissionBehavior)this).Mission.PlayerEnemyTeam;
		SpawnBossAndBodyguards();
		((MissionBehavior)this).Mission.PlayerTeam.SetIsEnemyOf(_enemyTeam, false);
		SetWatchStateOfAIAgents((WatchState)0);
		if (Agent.Main.IsUsingGameObject)
		{
			Agent.Main.StopUsingGameObject(false, (StopUsingGameObjectFlags)1);
		}
		playerAgent = Agent.Main;
		playerCompanions = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Agents).Where((Agent x) => x.IsActive() && x.Team == ((MissionBehavior)this).Mission.PlayerTeam && x.IsHuman && x.IsAIControlled).ToList();
		bossAgent = _bossAgent;
		bossCompanions = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Agents).Where((Agent x) => x.IsActive() && x.Team == _enemyTeam && x.IsHuman && x.IsAIControlled && x != _bossAgent).ToList();
	}

	private void OnCutSceneOver()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Mission.Current.SetMissionMode(_oldMissionMode, false);
		_hideoutMissionState = HideoutMissionState.ConversationBetweenLeaders;
		MissionConversationLogic missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>();
		missionBehavior.DisableStartConversation(isDisabled: false);
		missionBehavior.StartConversation(_bossAgent, setActionsInstantly: false);
	}

	private void OnDuelOver(BattleSideEnum winnerSide)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		AgentVictoryLogic missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<AgentVictoryLogic>();
		if (missionBehavior != null)
		{
			missionBehavior.SetCheerActionGroup((CheerActionGroupEnum)3);
		}
		if (missionBehavior != null)
		{
			missionBehavior.SetCheerReactionTimerSettings(0.25f, 3f);
		}
		if ((int)winnerSide == 1 && _duelPhaseAllyAgents != null)
		{
			foreach (Agent duelPhaseAllyAgent in _duelPhaseAllyAgents)
			{
				if ((int)duelPhaseAllyAgent.State == 1)
				{
					duelPhaseAllyAgent.SetTeam(((MissionBehavior)this).Mission.PlayerTeam, true);
					duelPhaseAllyAgent.SetWatchState((WatchState)2);
				}
			}
			return;
		}
		if ((int)winnerSide != 0 || _duelPhaseBanditAgents == null)
		{
			return;
		}
		foreach (Agent duelPhaseBanditAgent in _duelPhaseBanditAgents)
		{
			if ((int)duelPhaseBanditAgent.State == 1)
			{
				duelPhaseBanditAgent.SetTeam(_enemyTeam, true);
				duelPhaseBanditAgent.SetWatchState((WatchState)2);
			}
		}
	}

	public static void StartBossFightDuelMode()
	{
		Mission current = Mission.Current;
		((current != null) ? current.GetMissionBehavior<HideoutMissionController>() : null)?.StartBossFightDuelModeInternal();
	}

	private void StartBossFightDuelModeInternal()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(isDisabled: true);
		((MissionBehavior)this).Mission.PlayerTeam.SetIsEnemyOf(_enemyTeam, true);
		_duelPhaseAllyAgents = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Agents).Where((Agent x) => x.IsActive() && x.Team == ((MissionBehavior)this).Mission.PlayerTeam && x.IsHuman && x.IsAIControlled && x != Agent.Main).ToList();
		_duelPhaseBanditAgents = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Agents).Where((Agent x) => x.IsActive() && x.Team == _enemyTeam && x.IsHuman && x.IsAIControlled && x != _bossAgent).ToList();
		foreach (Agent duelPhaseAllyAgent in _duelPhaseAllyAgents)
		{
			duelPhaseAllyAgent.SetTeam(Team.Invalid, true);
			WorldPosition worldPosition = duelPhaseAllyAgent.GetWorldPosition();
			duelPhaseAllyAgent.SetScriptedPosition(ref worldPosition, false, (AIScriptedFrameFlags)0);
			duelPhaseAllyAgent.SetLookAgent(Agent.Main);
		}
		foreach (Agent duelPhaseBanditAgent in _duelPhaseBanditAgents)
		{
			duelPhaseBanditAgent.SetTeam(Team.Invalid, true);
			WorldPosition worldPosition2 = duelPhaseBanditAgent.GetWorldPosition();
			duelPhaseBanditAgent.SetScriptedPosition(ref worldPosition2, false, (AIScriptedFrameFlags)0);
			duelPhaseBanditAgent.SetLookAgent(_bossAgent);
		}
		_bossAgent.SetWatchState((WatchState)2);
		_hideoutMissionState = HideoutMissionState.BossFightWithDuel;
	}

	public static void StartBossFightBattleMode()
	{
		Mission current = Mission.Current;
		((current != null) ? current.GetMissionBehavior<HideoutMissionController>() : null)?.StartBossFightBattleModeInternal();
	}

	private void StartBossFightBattleModeInternal()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(isDisabled: true);
		((MissionBehavior)this).Mission.PlayerTeam.SetIsEnemyOf(_enemyTeam, true);
		SetWatchStateOfAIAgents((WatchState)2);
		_hideoutMissionState = HideoutMissionState.BossFightWithAll;
		foreach (Formation item in (List<Formation>)(object)((MissionBehavior)this).Mission.PlayerTeam.FormationsIncludingEmpty)
		{
			if (item.CountOfUnits > 0)
			{
				item.SetMovementOrder(MovementOrder.MovementOrderCharge);
				item.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			}
		}
	}

	public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Expected I4, but got Unknown
		int num = (int)side;
		return _missionSides[num].GetAllTroops();
	}

	public int GetNumberOfPlayerControllableTroops()
	{
		throw new NotImplementedException();
	}

	public bool GetSpawnHorses(BattleSideEnum side)
	{
		return false;
	}
}

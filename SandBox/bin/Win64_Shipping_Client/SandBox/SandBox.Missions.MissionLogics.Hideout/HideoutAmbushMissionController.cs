using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Conversation.MissionLogics;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace SandBox.Missions.MissionLogics.Hideout;

public class HideoutAmbushMissionController : MissionLogic
{
	public class TroopData
	{
		public CharacterObject Troop;

		public int Number;

		public int Level;

		public TroopData(CharacterObject troop, int number)
		{
			Troop = troop;
			Number = number;
		}
	}

	private enum HideoutMissionState
	{
		NotDecided,
		StealthState,
		CallTroopsCutSceneState,
		BattleBeforeBossFight,
		CutSceneBeforeBossFight,
		ConversationBetweenLeaders,
		BossFightWithDuel,
		BossFightWithAll
	}

	private const int FirstPhaseEndInSeconds = 4;

	private int _initialHideoutPopulation;

	private bool _troopsInitialized;

	private bool _isMissionInitialized;

	private bool _battleResolved;

	private readonly BattleSideEnum _playerSide;

	private HideoutMissionState _currentHideoutMissionState;

	private List<Agent> _duelPhaseAllyAgents;

	private List<Agent> _duelPhaseBanditAgents;

	private List<IAgentOriginBase> _allEnemyTroops;

	private List<IAgentOriginBase> _priorAllyTroops;

	private List<StealthAreaMissionLogic.StealthAreaData> _stealthAreaData;

	private Timer _waitTimerToChangeStealthModeIntoBattle;

	private Timer _firstPhaseEndTimer;

	private int _sentryCount;

	private int _remainingSentryCount;

	private BattleAgentLogic _battleAgentLogic;

	private BattleEndLogic _battleEndLogic;

	private HideoutAmbushBossFightCinematicController _hideoutAmbushBossFightCinematicController;

	private StealthAreaMissionLogic _stealthAreaMissionLogic;

	private Agent _bossAgent;

	private Team _enemyTeam;

	private CharacterObject _overriddenHideoutBossCharacterObject;

	public bool IsReadyForCallTroopsCinematic => _currentHideoutMissionState == HideoutMissionState.CallTroopsCutSceneState;

	public HideoutAmbushMissionController(BattleSideEnum playerSide, FlattenedTroopRoster priorAllyTroops)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_playerSide = playerSide;
		_allEnemyTroops = new List<IAgentOriginBase>();
		_priorAllyTroops = new List<IAgentOriginBase>();
		_stealthAreaData = new List<StealthAreaMissionLogic.StealthAreaData>();
		_waitTimerToChangeStealthModeIntoBattle = null;
		_currentHideoutMissionState = HideoutMissionState.NotDecided;
		_overriddenHideoutBossCharacterObject = null;
		InitializeTroops(priorAllyTroops);
		_initialHideoutPopulation = _allEnemyTroops.Count;
		Campaign.Current.CampaignBehaviorManager.GetBehavior<IHideoutCampaignBehavior>();
	}

	private void InitializeTroops(FlattenedTroopRoster priorAllyTroops)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		foreach (FlattenedTroopRosterElement priorAllyTroop in priorAllyTroops)
		{
			FlattenedTroopRosterElement current = priorAllyTroop;
			if (((FlattenedTroopRosterElement)(ref current)).Troop != CharacterObject.PlayerCharacter)
			{
				_priorAllyTroops.Add((IAgentOriginBase)new PartyAgentOrigin(PartyBase.MainParty, ((FlattenedTroopRosterElement)(ref current)).Troop, -1, ((FlattenedTroopRosterElement)(ref current)).Descriptor, false, false));
			}
		}
		foreach (PartyBase involvedParty in MapEvent.PlayerMapEvent.InvolvedParties)
		{
			if (involvedParty.Side == _playerSide)
			{
				continue;
			}
			foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)involvedParty.MemberRoster.GetTroopRoster())
			{
				TroopRosterElement current3 = item;
				int num = ((TroopRosterElement)(ref current3)).Number - ((TroopRosterElement)(ref current3)).WoundedNumber;
				for (int i = 0; i < num; i++)
				{
					_allEnemyTroops.Add((IAgentOriginBase)new PartyAgentOrigin(involvedParty, current3.Character, -1, default(UniqueTroopDescriptor), false, false));
				}
			}
		}
		Extensions.Shuffle<IAgentOriginBase>((IList<IAgentOriginBase>)_allEnemyTroops);
	}

	public override void OnCreated()
	{
		((MissionBehavior)this).OnCreated();
		((MissionBehavior)this).Mission.DoesMissionRequireCivilianEquipment = false;
		CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener((object)this, (Action<Dictionary<string, int>>)LocationCharactersAreReadyToSpawn);
	}

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		SandBoxHelpers.MissionHelper.SpawnPlayer(civilianEquipment: false, noHorses: true);
		Mission.Current.GetMissionBehavior<MissionAgentHandler>().SpawnLocationCharacters();
		Agent.Main.SetClothingColor1(4281281067u);
		Agent.Main.SetClothingColor2(4281281067u);
		Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(Hero.MainHero.StealthEquipment);
		foreach (StealthAreaMissionLogic.StealthAreaData stealthAreaDatum in _stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in stealthAreaDatum.StealthAreaMarkers)
			{
				_sentryCount += stealthAreaMarker.Value.Count;
				_remainingSentryCount += stealthAreaMarker.Value.Count;
			}
		}
		Mission.Current.GetMissionBehavior<StealthFailCounterMissionLogic>().FailCounterSeconds = 15f;
	}

	public override void OnBehaviorInitialize()
	{
		((MissionBehavior)this).OnBehaviorInitialize();
		_battleAgentLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<BattleAgentLogic>();
		_battleEndLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<BattleEndLogic>();
		_battleEndLogic.ChangeCanCheckForEndCondition(false);
		_stealthAreaMissionLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<StealthAreaMissionLogic>();
		_stealthAreaMissionLogic.GetReinforcementAllyTroops = GetReinforcementAllyTroops;
		_hideoutAmbushBossFightCinematicController = ((MissionBehavior)this).Mission.GetMissionBehavior<HideoutAmbushBossFightCinematicController>();
		foreach (StealthAreaUsePoint item in MBExtensions.FindAllWithType<StealthAreaUsePoint>((IEnumerable<MissionObject>)((MissionBehavior)this).Mission.ActiveMissionObjects))
		{
			_stealthAreaData.Add(new StealthAreaMissionLogic.StealthAreaData(item));
		}
		Game.Current.EventManager.RegisterEvent<OnStealthMissionCounterFailedEvent>((Action<OnStealthMissionCounterFailedEvent>)OnStealthMissionCounterFailed);
	}

	private List<IAgentOriginBase> GetReinforcementAllyTroops(StealthAreaMissionLogic.StealthAreaData triggeredStealthAreaData, StealthAreaMarker stealthAreaMarker)
	{
		int count = triggeredStealthAreaData.StealthAreaMarkers.Count;
		StealthAreaMarker[] array = triggeredStealthAreaData.StealthAreaMarkers.Keys.ToArray();
		List<IAgentOriginBase> list = new List<IAgentOriginBase>();
		for (int i = 0; i < _priorAllyTroops.Count; i++)
		{
			if (array[i % count] == stealthAreaMarker)
			{
				list.Add(_priorAllyTroops[i]);
			}
		}
		return list;
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		if (!(usedObject is StealthAreaUsePoint))
		{
			return;
		}
		StealthAreaMissionLogic.StealthAreaData stealthAreaData = null;
		foreach (StealthAreaMissionLogic.StealthAreaData stealthAreaDatum in _stealthAreaData)
		{
			if ((object)stealthAreaDatum.StealthAreaUsePoint == usedObject)
			{
				stealthAreaData = stealthAreaDatum;
				break;
			}
		}
		if (stealthAreaData != null)
		{
			_currentHideoutMissionState = HideoutMissionState.CallTroopsCutSceneState;
			_waitTimerToChangeStealthModeIntoBattle = new Timer(((MissionBehavior)this).Mission.CurrentTime, 10f, true);
		}
	}

	public void SetOverriddenHideoutBossCharacterObject(CharacterObject characterObject)
	{
		_overriddenHideoutBossCharacterObject = characterObject;
	}

	private IAgentOriginBase GetOneEnemyTroopToSpawnInFirstPhase()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		IAgentOriginBase val = null;
		foreach (IAgentOriginBase allEnemyTroop in _allEnemyTroops)
		{
			CharacterObject val2 = (CharacterObject)allEnemyTroop.Troop;
			if (!((BasicCharacterObject)val2).IsHero && (object)val2.Culture.BanditBoss != allEnemyTroop.Troop && val2 != _overriddenHideoutBossCharacterObject)
			{
				val = allEnemyTroop;
				break;
			}
		}
		CharacterObject val3 = (CharacterObject)val.Troop;
		PartyBase party = ((PartyAgentOrigin)val).Party;
		party.AddMember(val3, 1, 0);
		return (IAgentOriginBase)new PartyAgentOrigin(party, val3, -1, default(UniqueTroopDescriptor), false, false);
	}

	public void SpawnRemainingTroopsForBossFight(List<MatrixFrame> spawnFrames)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)MathF.Clamp((float)(_initialHideoutPopulation / 2), 4f, 20f);
		_allEnemyTroops = _allEnemyTroops.OrderByDescending((IAgentOriginBase x) => x.Troop.IsHero).ToList();
		Vec2 asVec;
		if (_overriddenHideoutBossCharacterObject != null)
		{
			IAgentOriginBase val = _allEnemyTroops.Find((IAgentOriginBase t) => (object)t.Troop == _overriddenHideoutBossCharacterObject);
			MatrixFrame val2 = spawnFrames.FirstOrDefault();
			((Mat3)(ref val2.rotation)).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current = Mission.Current;
			Vec3? val3 = val2.origin;
			asVec = ((Vec3)(ref val2.rotation.f)).AsVec2;
			Agent val4 = current.SpawnTroop(val, false, false, false, false, 0, 0, false, false, false, val3, (Vec2?)((Vec2)(ref asVec)).Normalized(), "_hideout_bandit", (ItemObject)null, (FormationClass)10, false);
			AgentFlag agentFlags = val4.GetAgentFlags();
			if (Extensions.HasAnyFlag<AgentFlag>(agentFlags, (AgentFlag)1048576))
			{
				val4.SetAgentFlags((AgentFlag)(agentFlags & -1048577));
			}
			_allEnemyTroops.Remove(val);
		}
		foreach (IAgentOriginBase allEnemyTroop in _allEnemyTroops)
		{
			MatrixFrame val5 = spawnFrames.FirstOrDefault();
			((Mat3)(ref val5.rotation)).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			Mission current3 = Mission.Current;
			Vec3? val6 = val5.origin;
			asVec = ((Vec3)(ref val5.rotation.f)).AsVec2;
			Agent val7 = current3.SpawnTroop(allEnemyTroop, false, false, false, false, 0, 0, false, false, false, val6, (Vec2?)((Vec2)(ref asVec)).Normalized(), "_hideout_bandit", (ItemObject)null, (FormationClass)10, false);
			AgentFlag agentFlags2 = val7.GetAgentFlags();
			if (Extensions.HasAnyFlag<AgentFlag>(agentFlags2, (AgentFlag)1048576))
			{
				val7.SetAgentFlags((AgentFlag)(agentFlags2 & -1048577));
			}
			num--;
			if (num <= 0)
			{
				break;
			}
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

	private void SpawnAllyAgent(IAgentOriginBase troopOrigin, GameEntity spawnPoint, Vec3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = spawnPoint.GetGlobalFrame();
		Mission current = Mission.Current;
		Vec3? val = globalFrame.origin;
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Agent obj = current.SpawnTroop(troopOrigin, true, false, false, false, 0, 0, true, true, true, val, (Vec2?)((Vec2)(ref asVec)).Normalized(), (string)null, (ItemObject)null, (FormationClass)10, false);
		WorldPosition val2 = default(WorldPosition);
		((WorldPosition)(ref val2))._002Ector(spawnPoint.Scene, position);
		obj.SetScriptedPosition(ref val2, true, (AIScriptedFrameFlags)514);
	}

	private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("hideout_center");
		int value = 0;
		if (unusedUsablePointCount.TryGetValue("stealth_agent_forced", out value))
		{
			locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CreateForcedSentry), Settlement.CurrentSettlement.Culture, (CharacterRelations)2, value);
		}
		if (unusedUsablePointCount.TryGetValue("stealth_agent", out value))
		{
			int num = _initialHideoutPopulation / 8;
			if (num >= 1)
			{
				locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(CreateSentry), Settlement.CurrentSettlement.Culture, (CharacterRelations)2, Math.Min(num, value));
			}
		}
	}

	private LocationCharacter CreateForcedSentry(CultureObject culture, CharacterRelations relation)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		IAgentOriginBase oneEnemyTroopToSpawnInFirstPhase = GetOneEnemyTroopToSpawnInFirstPhase();
		CharacterObject val = (CharacterObject)oneEnemyTroopToSpawnInFirstPhase.Troop;
		int num = default(int);
		int num2 = default(int);
		Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(val, ref num, ref num2, "");
		AgentData obj = new AgentData(oneEnemyTroopToSpawnInFirstPhase).Monster(FaceGen.GetMonsterWithSuffix(((BasicCharacterObject)val).Race, "_settlement_slow")).Age(MBRandom.RandomInt(num, num2));
		IAgentBehaviorManager agentBehaviorManager = SandBoxManager.Instance.AgentBehaviorManager;
		return new LocationCharacter(obj, new AddBehaviorsDelegate(agentBehaviorManager.AddStealthAgentBehaviors), "stealth_agent_forced", true, relation, (string)null, false, false, (ItemObject)null, false, false, true, (AfterAgentCreatedDelegate)null, true);
	}

	private LocationCharacter CreateSentry(CultureObject culture, CharacterRelations relation)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		IAgentOriginBase oneEnemyTroopToSpawnInFirstPhase = GetOneEnemyTroopToSpawnInFirstPhase();
		CharacterObject val = (CharacterObject)oneEnemyTroopToSpawnInFirstPhase.Troop;
		int num = default(int);
		int num2 = default(int);
		Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(val, ref num, ref num2, "");
		AgentData obj = new AgentData(oneEnemyTroopToSpawnInFirstPhase).Monster(FaceGen.GetMonsterWithSuffix(((BasicCharacterObject)val).Race, "_settlement_slow")).Age(MBRandom.RandomInt(num, num2));
		IAgentBehaviorManager agentBehaviorManager = SandBoxManager.Instance.AgentBehaviorManager;
		return new LocationCharacter(obj, new AddBehaviorsDelegate(agentBehaviorManager.AddStealthAgentBehaviors), "stealth_agent", true, relation, (string)null, false, false, (ItemObject)null, false, false, true, (AfterAgentCreatedDelegate)null, false);
	}

	public override void OnMissionTick(float dt)
	{
		((MissionBehavior)this).OnMissionTick(dt);
		if (_waitTimerToChangeStealthModeIntoBattle != null && _waitTimerToChangeStealthModeIntoBattle.Check(((MissionBehavior)this).Mission.CurrentTime))
		{
			ChangeHideoutMissionModeToBattle();
			_waitTimerToChangeStealthModeIntoBattle = null;
		}
		if (!_isMissionInitialized)
		{
			Agent main = Agent.Main;
			if (main != null && main.IsActive())
			{
				InitializeMission();
				_isMissionInitialized = true;
				return;
			}
		}
		if (!_isMissionInitialized)
		{
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
		if (!_battleResolved)
		{
			CheckBattleResolved();
		}
		else if (!((MissionBehavior)this).Mission.ForceNoFriendlyFire)
		{
			((MissionBehavior)this).Mission.ForceNoFriendlyFire = true;
		}
	}

	private void InitializeMission()
	{
		((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(isDisabled: true);
		((MissionBehavior)this).Mission.SetMissionMode((MissionMode)4, true);
		_currentHideoutMissionState = HideoutMissionState.StealthState;
		((MissionBehavior)this).Mission.DeploymentPlan.MakeDefaultDeploymentPlans();
	}

	private void ChangeHideoutMissionModeToBattle()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		_currentHideoutMissionState = HideoutMissionState.BattleBeforeBossFight;
		Mission.Current.SetMissionMode((MissionMode)2, false);
		foreach (Agent item in (List<Agent>)(object)Mission.Current.PlayerTeam.ActiveAgents)
		{
			if (!item.IsMainAgent)
			{
				item.ClearTargetFrame();
				item.DisableScriptedMovement();
			}
		}
		((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
		((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SetOrder((OrderType)4);
		((MissionBehavior)this).Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
		((MissionBehavior)this).Mission.PlayerEnemyTeam.MasterOrderController.SetOrder((OrderType)4);
		foreach (Agent item2 in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			item2.SetAlarmState((AIStateFlag)3);
		}
		Vec3 position = Agent.Main.Position;
		SoundManager.StartOneShotEvent("event:/ui/mission/horns/attack", ref position);
	}

	public override void OnAgentBuild(Agent agent, Banner banner)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (_currentHideoutMissionState >= HideoutMissionState.CutSceneBeforeBossFight || !agent.IsHuman || agent.Team != Mission.Current.PlayerEnemyTeam)
		{
			return;
		}
		foreach (StealthAreaMissionLogic.StealthAreaData stealthAreaDatum in _stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in stealthAreaDatum.StealthAreaMarkers)
			{
				if (((AreaMarker)stealthAreaMarker.Key).IsPositionInRange(agent.Position))
				{
					stealthAreaDatum.AddAgentToStealthAreaMarker(stealthAreaMarker.Key, agent);
					break;
				}
			}
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		if (affectorAgent != null && affectorAgent.IsMainAgent)
		{
			_remainingSentryCount = 0;
			foreach (StealthAreaMissionLogic.StealthAreaData stealthAreaDatum in _stealthAreaData)
			{
				foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in stealthAreaDatum.StealthAreaMarkers)
				{
					if (stealthAreaMarker.Value.Contains(affectedAgent) || Extensions.IsEmpty<Agent>((IEnumerable<Agent>)stealthAreaMarker.Value))
					{
						stealthAreaDatum.RemoveAgentFromStealthAreaMarker(stealthAreaMarker.Key, affectedAgent);
					}
					_remainingSentryCount += stealthAreaMarker.Value.Count;
				}
			}
		}
		if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel)
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
		if ((_currentHideoutMissionState == HideoutMissionState.StealthState || _currentHideoutMissionState == HideoutMissionState.BattleBeforeBossFight) && affectedAgent.IsMainAgent)
		{
			((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
			affectedAgent.Formation = null;
			((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SetOrder((OrderType)9);
		}
	}

	public void OnStealthMissionCounterFailed(OnStealthMissionCounterFailedEvent obj)
	{
		Campaign.Current.GameMenuManager.SetNextMenu("hideout_after_found_by_sentries");
	}

	private void CheckBattleResolved()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		if (_currentHideoutMissionState == HideoutMissionState.NotDecided || _currentHideoutMissionState == HideoutMissionState.CutSceneBeforeBossFight || _currentHideoutMissionState == HideoutMissionState.ConversationBetweenLeaders)
		{
			return;
		}
		if (IsSideDepleted(((MissionBehavior)this).Mission.PlayerTeam.Side))
		{
			if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel)
			{
				OnDuelOver(((MissionBehavior)this).Mission.PlayerEnemyTeam.Side);
			}
			Campaign.Current.SkillLevelingManager.OnHideoutMissionEnd(false);
			_battleEndLogic.ChangeCanCheckForEndCondition(true);
			_battleResolved = true;
		}
		else
		{
			if (!IsSideDepleted(((MissionBehavior)this).Mission.PlayerEnemyTeam.Side))
			{
				return;
			}
			if (_currentHideoutMissionState == HideoutMissionState.BattleBeforeBossFight || _currentHideoutMissionState == HideoutMissionState.StealthState)
			{
				Agent main = Agent.Main;
				if (main != null && main.IsActive())
				{
					if (_firstPhaseEndTimer == null)
					{
						_firstPhaseEndTimer = new Timer(((MissionBehavior)this).Mission.CurrentTime, 4f, true);
						Mission.Current.SetMissionMode((MissionMode)9, false);
					}
					else if (_firstPhaseEndTimer.Check(((MissionBehavior)this).Mission.CurrentTime))
					{
						_hideoutAmbushBossFightCinematicController.StartCinematic(OnInitialFadeOutOver, OnCutSceneOver);
					}
				}
			}
			else
			{
				if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel)
				{
					OnDuelOver(((MissionBehavior)this).Mission.PlayerTeam.Side);
				}
				Campaign.Current.SkillLevelingManager.OnHideoutMissionEnd(true);
				_battleEndLogic.ChangeCanCheckForEndCondition(true);
				MapEvent.PlayerMapEvent.SetOverrideWinner(((MissionBehavior)this).Mission.PlayerTeam.Side);
				_battleResolved = true;
			}
		}
	}

	public bool IsSideDepleted(BattleSideEnum side)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ((List<Agent>)(object)(((int)side == 1) ? Mission.Current.Teams.Attacker : Mission.Current.Teams.Defender).ActiveAgents).Count == 0;
		if (!flag)
		{
			if (_playerSide == side)
			{
				if (Agent.Main == null || !Agent.Main.IsActive())
				{
					if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel || _currentHideoutMissionState == HideoutMissionState.BattleBeforeBossFight)
					{
						flag = true;
					}
					else if (_currentHideoutMissionState == HideoutMissionState.BossFightWithAll)
					{
						flag = Extensions.IsEmpty<Agent>((IEnumerable<Agent>)((MissionBehavior)this).Mission.PlayerTeam.ActiveAgents) && !Extensions.IsEmpty<Agent>((IEnumerable<Agent>)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents);
					}
				}
			}
			else if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel && (_bossAgent == null || !_bossAgent.IsActive()))
			{
				flag = true;
			}
		}
		return flag;
	}

	public void OnAgentsShouldBeEnabled()
	{
		foreach (Agent item in (List<Agent>)(object)Mission.Current.Agents)
		{
			if (item.IsActive() && item.IsAIControlled)
			{
				item.SetIsAIPaused(false);
			}
		}
	}

	protected override void OnEndMission()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		((CampaignEventReceiver)CampaignEventDispatcher.Instance).RemoveListeners((object)this);
		int num = 0;
		if (_currentHideoutMissionState == HideoutMissionState.BossFightWithDuel)
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
		if (!PlayerEncounter.EnemySurrender && num <= 0 && MobileParty.MainParty.MemberRoster.TotalHealthyCount <= 0 && (int)MapEvent.PlayerMapEvent.BattleState == 0)
		{
			MapEvent.PlayerMapEvent.SetOverrideWinner(((MissionBehavior)this).Mission.PlayerEnemyTeam.Side);
		}
		Game.Current.EventManager.UnregisterEvent<OnStealthMissionCounterFailedEvent>((Action<OnStealthMissionCounterFailedEvent>)OnStealthMissionCounterFailed);
	}

	private void SpawnBossAndBodyguards()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame identity = MatrixFrame.Identity;
		identity.origin = Agent.Main.Position + Agent.Main.LookDirection * -3f;
		SpawnRemainingTroopsForBossFight(new List<MatrixFrame> { identity });
		_bossAgent = SelectBossAgent();
		_bossAgent.WieldInitialWeapons((WeaponWieldActionType)2, (InitialWeaponEquipPreference)0);
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			if (item != _bossAgent)
			{
				item.WieldInitialWeapons((WeaponWieldActionType)3, (InitialWeaponEquipPreference)0);
			}
		}
	}

	private Agent SelectBossAgent()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Agent val = null;
		Agent val2 = null;
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.Agents)
		{
			if (!item.IsHuman || item.Team.IsPlayerAlly)
			{
				continue;
			}
			if (_overriddenHideoutBossCharacterObject == null)
			{
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
			}
			else if ((object)item.Character == _overriddenHideoutBossCharacterObject)
			{
				val = item;
				val2 = item;
				break;
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
		_currentHideoutMissionState = HideoutMissionState.CutSceneBeforeBossFight;
		_enemyTeam = ((MissionBehavior)this).Mission.PlayerEnemyTeam;
		SpawnBossAndBodyguards();
		((MissionBehavior)this).Mission.PlayerTeam.SetIsEnemyOf(_enemyTeam, false);
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
		Mission.Current.SetMissionMode((MissionMode)2, false);
		_currentHideoutMissionState = HideoutMissionState.ConversationBetweenLeaders;
		MissionConversationLogic missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>();
		missionBehavior.DisableStartConversation(isDisabled: false);
		missionBehavior.StartConversation(_bossAgent, setActionsInstantly: false);
	}

	private void OnDuelOver(BattleSideEnum winnerSide)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Invalid comparison between Unknown and I4
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		if (winnerSide == ((MissionBehavior)this).Mission.PlayerTeam.Side && _duelPhaseAllyAgents != null)
		{
			foreach (Agent duelPhaseAllyAgent in _duelPhaseAllyAgents)
			{
				if ((int)duelPhaseAllyAgent.State == 1)
				{
					duelPhaseAllyAgent.SetTeam(((MissionBehavior)this).Mission.PlayerTeam, true);
				}
			}
			return;
		}
		if (winnerSide != ((MissionBehavior)this).Mission.PlayerEnemyTeam.Side || _duelPhaseBanditAgents == null)
		{
			return;
		}
		foreach (Agent duelPhaseBanditAgent in _duelPhaseBanditAgents)
		{
			if ((int)duelPhaseBanditAgent.State == 1)
			{
				duelPhaseBanditAgent.SetTeam(_enemyTeam, true);
				duelPhaseBanditAgent.DisableScriptedMovement();
				duelPhaseBanditAgent.ClearTargetFrame();
			}
		}
		foreach (Agent duelPhaseAllyAgent2 in _duelPhaseAllyAgents)
		{
			if ((int)duelPhaseAllyAgent2.State == 1)
			{
				duelPhaseAllyAgent2.SetTeam(((MissionBehavior)this).Mission.PlayerTeam, true);
				duelPhaseAllyAgent2.DisableScriptedMovement();
				duelPhaseAllyAgent2.ClearTargetFrame();
			}
		}
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			item.SetAlarmState((AIStateFlag)3);
		}
	}

	public static void StartBossFightDuelMode()
	{
		Mission current = Mission.Current;
		((current != null) ? current.GetMissionBehavior<HideoutAmbushMissionController>() : null)?.StartBossFightDuelModeInternal();
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
		_bossAgent.SetAlarmState((AIStateFlag)3);
		_currentHideoutMissionState = HideoutMissionState.BossFightWithDuel;
	}

	public static void StartBossFightBattleMode()
	{
		Mission current = Mission.Current;
		((current != null) ? current.GetMissionBehavior<HideoutAmbushMissionController>() : null)?.StartBossFightBattleModeInternal();
	}

	private void StartBossFightBattleModeInternal()
	{
		((MissionBehavior)this).Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(isDisabled: true);
		((MissionBehavior)this).Mission.PlayerTeam.SetIsEnemyOf(_enemyTeam, true);
		_currentHideoutMissionState = HideoutMissionState.BossFightWithAll;
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			item.SetAlarmState((AIStateFlag)3);
		}
		((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
		((MissionBehavior)this).Mission.PlayerTeam.PlayerOrderController.SetOrder((OrderType)4);
		((MissionBehavior)this).Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
		((MissionBehavior)this).Mission.PlayerEnemyTeam.MasterOrderController.SetOrder((OrderType)4);
	}

	private void KillAllSentries()
	{
		List<Agent> list = new List<Agent>();
		foreach (StealthAreaMissionLogic.StealthAreaData stealthAreaDatum in _stealthAreaData)
		{
			foreach (KeyValuePair<StealthAreaMarker, List<Agent>> stealthAreaMarker in stealthAreaDatum.StealthAreaMarkers)
			{
				list.AddRange(stealthAreaMarker.Value);
			}
		}
		foreach (Agent item in list)
		{
			((MissionBehavior)this).Mission.KillAgentCheat(item);
		}
	}

	[CommandLineArgumentFunction("kill_all_sentries", "mission")]
	public static string KillAllSentries(List<string> strings)
	{
		string empty = string.Empty;
		if (!CampaignCheats.CheckCheatUsage(ref empty))
		{
			return empty;
		}
		Mission current = Mission.Current;
		HideoutAmbushMissionController hideoutAmbushMissionController = ((current != null) ? current.GetMissionBehavior<HideoutAmbushMissionController>() : null);
		if (hideoutAmbushMissionController != null)
		{
			hideoutAmbushMissionController.KillAllSentries();
			return "Done";
		}
		return "This cheat only works in hideout ambush mission!";
	}
}

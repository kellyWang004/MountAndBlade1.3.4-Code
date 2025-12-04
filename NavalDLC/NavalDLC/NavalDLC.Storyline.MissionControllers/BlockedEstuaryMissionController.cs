using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Storyline.Objectives.Quest3;
using NavalDLC.Storyline.Objects;
using NavalDLC.Storyline.Quests;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.Missions.Objectives;
using TaleWorlds.MountAndBlade.Objects.Usables;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers;

public class BlockedEstuaryMissionController : MissionLogic
{
	public enum BattlePhase
	{
		Phase1,
		Phase2,
		Phase3
	}

	private class BurningProjectile
	{
		private const string ProjectileFireParticleId = "fire_obstacle";

		private float _minLifeTime;

		private float _timer;

		private float _spawnTime;

		private Vec3 _position;

		private Func<bool> _endCondition;

		public bool Initialized { get; private set; }

		public GameEntity GameEntity { get; private set; }

		public BurningProjectile(Vec3 position, float minLifeTime = 10f, float spawnAfterTime = 1f, Func<bool> enderFunction = null)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_position = position;
			_spawnTime = spawnAfterTime;
			_endCondition = enderFunction;
			_minLifeTime = minLifeTime;
		}

		public void Tick(float dt, out bool shouldBeRemoved)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			shouldBeRemoved = false;
			if (Initialized)
			{
				shouldBeRemoved = _timer >= _minLifeTime || (_endCondition != null && _endCondition());
			}
			else if (_timer >= _spawnTime)
			{
				SpawnEntity(_position);
				_timer = 0f;
			}
			_timer += dt;
		}

		private void SpawnEntity(Vec3 position)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			GameEntity = GameEntity.Instantiate(Mission.Current.Scene, "fire_obstacle", true, true, "");
			MatrixFrame globalFrame = GameEntity.GetGlobalFrame();
			globalFrame.origin = position;
			GameEntity.SetFrame(ref globalFrame, true);
			Initialized = true;
		}

		public void Clear()
		{
			Mission.Current.Scene.RemoveEntity(GameEntity, 0);
			GameEntity = null;
			Initialized = false;
		}
	}

	private class EnemySpawnPoint
	{
		private const float GroupRadius = 20f;

		private GameEntity _entity;

		private AgentNavigator _navigator;

		public bool IsAlerted { get; private set; }

		public Vec3 Position => _entity.GlobalPosition;

		public Agent Agent { get; private set; }

		public EnemySpawnPoint(string spawnId, CharacterObject character, bool isNight)
		{
			IsAlerted = false;
			_entity = Mission.Current.Scene.FindEntityWithTag(spawnId);
			SpawnAgent(character, isNight);
		}

		public EnemySpawnPoint(GameEntity spawnEntity, CharacterObject character, bool isNight)
		{
			IsAlerted = false;
			_entity = spawnEntity;
			SpawnAgent(character, isNight);
		}

		public void CalmDown()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Agent.SetAlarmState((AIStateFlag)1);
			Vec3 position = Agent.Position;
			if (((Vec3)(ref position)).Distance(Position) >= 20f)
			{
				Vec3 randomPositionAroundPoint = Mission.Current.GetRandomPositionAroundPoint(Position, 1f, 3f, false);
				Agent.SetTargetPosition(((Vec3)(ref randomPositionAroundPoint)).AsVec2);
			}
			IsAlerted = false;
		}

		public bool CanSeeAgent(Agent agent)
		{
			if (Agent != null && Agent.IsActive() && _navigator.CanSeeAgent(agent))
			{
				return true;
			}
			return false;
		}

		public void Alert()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Agent.SetTeam(Mission.Current.PlayerEnemyTeam, true);
			Agent.SetAgentFlags((AgentFlag)(Agent.GetAgentFlags() | 0x10000));
			Agent.SetAlarmState((AIStateFlag)3);
			Agent.ClearTargetFrame();
			IsAlerted = true;
		}

		public void Clear()
		{
			if (Agent != null && Agent.IsActive())
			{
				Agent.FadeOut(true, true);
			}
			IsAlerted = false;
			Agent = null;
			_navigator = null;
		}

		private void SpawnAgent(CharacterObject character, bool isNight)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Vec3 globalPosition = _entity.GlobalPosition;
			Vec3 randomPositionAroundPoint = Mission.Current.GetRandomPositionAroundPoint(globalPosition, 1f, 3f, false);
			Vec3 val = randomPositionAroundPoint - globalPosition;
			Vec2 asVec = ((Vec3)(ref val)).AsVec2;
			Vec2 direction = ((Vec2)(ref asVec)).Normalized();
			Agent = SpawnAgentAux(randomPositionAroundPoint, direction, character, isNight);
			_navigator = Agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
		}

		private Agent SpawnAgentAux(Vec3 position, Vec2 direction, CharacterObject character, bool isNight, string patrolTag = null)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Invalid comparison between Unknown and I4
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			Equipment val = ((BasicCharacterObject)character).Equipment.Clone(false);
			if (isNight)
			{
				val[(EquipmentIndex)4] = new EquipmentElement(MBObjectManager.Instance.GetObject<ItemObject>("torch"), (ItemModifier)null, (ItemObject)null, false);
			}
			AgentBuildData val2 = new AgentBuildData((BasicCharacterObject)(object)character).TroopOrigin((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)character, -1, (Banner)null, default(UniqueTroopDescriptor))).Team(Team.Invalid).InitialPosition(ref position)
				.InitialDirection(ref direction)
				.Equipment(val)
				.NoHorses(true)
				.NoWeapons(false)
				.Banner(NavalStorylineData.CorsairBanner);
			Agent val3 = Mission.Current.SpawnAgent(val2, false);
			EquipmentIndex val4 = default(EquipmentIndex);
			EquipmentIndex val5 = default(EquipmentIndex);
			bool flag = default(bool);
			val3.SpawnEquipment.GetInitialWeaponIndicesToEquip(ref val4, ref val5, ref flag, (InitialWeaponEquipPreference)0);
			if ((int)val5 != -1)
			{
				val3.TryToWieldWeaponInSlot(val5, (WeaponWieldActionType)2, true);
			}
			CampaignAgentComponent component = val3.GetComponent<CampaignAgentComponent>();
			component.CreateAgentNavigator();
			SandBoxManager.Instance.AgentBehaviorManager.AddFixedGuardBehaviors((IAgent)(object)val3);
			if (!string.IsNullOrEmpty(patrolTag))
			{
				component.AgentNavigator.SpecialTargetTag = patrolTag;
			}
			return val3;
		}

		public bool IsDepleted()
		{
			if (Agent != null)
			{
				return !Agent.IsActive();
			}
			return true;
		}

		public void Tick(float dt, BlockedEstuaryMissionController controller)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (IsAlerted)
			{
				return;
			}
			Vec3 position;
			if (Agent.Main != null && Agent.Main.IsActive())
			{
				position = Position;
				if (((Vec3)(ref position)).DistanceSquared(Agent.Main.Position) < 5625f || CanSeeAgent(Agent.Main))
				{
					Alert();
					return;
				}
			}
			if (controller.IsGangradirActive())
			{
				position = Position;
				if (((Vec3)(ref position)).DistanceSquared(controller._gangradir.Position) < 3600f || CanSeeAgent(controller._gangradir))
				{
					Alert();
				}
			}
		}
	}

	private class EnemyShipTrigger
	{
		private VolumeBox _trigger;

		private MissionShip _ship;

		private IShipOrigin _shipOrigin;

		private GameEntity _spawnEntity;

		private GameEntity _destination;

		private bool _isTriggered;

		public EnemyShipTrigger(GameEntity spawnPoint, VolumeBox volumeBox, IShipOrigin shipOrigin, string destinationId = null)
		{
			_trigger = volumeBox;
			_shipOrigin = shipOrigin;
			if (!string.IsNullOrEmpty(destinationId))
			{
				_destination = Mission.Current.Scene.FindEntityWithTag(destinationId);
			}
			_spawnEntity = spawnPoint;
			SpawnShip();
		}

		public void Tick(MissionShip target, float dt)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			Vec3 globalPosition;
			WeakGameEntity gameEntity;
			if (!_isTriggered && _destination != (GameEntity)null)
			{
				globalPosition = _destination.GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)_ship).GameEntity;
				if (((Vec3)(ref globalPosition)).DistanceSquared(((WeakGameEntity)(ref gameEntity)).GlobalPosition) < 100f && !_ship.Physics.IsAnchored)
				{
					AnchorShip();
				}
			}
			if (_isTriggered)
			{
				return;
			}
			VolumeBox trigger = _trigger;
			gameEntity = ((ScriptComponentBehavior)target).GameEntity;
			if (!trigger.IsPointIn(((WeakGameEntity)(ref gameEntity)).GlobalPosition))
			{
				gameEntity = ((ScriptComponentBehavior)target).GameEntity;
				globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)_ship).GameEntity;
				if (!(((Vec3)(ref globalPosition)).DistanceSquared(((WeakGameEntity)(ref gameEntity)).GlobalPosition) < 10000f))
				{
					return;
				}
			}
			TriggerShip();
		}

		private void SpawnShip()
		{
			BlockedEstuaryMissionController missionBehavior = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
			_ship = missionBehavior.SpawnEnemyChaserShip(_spawnEntity, _shipOrigin);
			AnchorShip();
			missionBehavior.ToggleShipBallistas(_ship, enabled: false);
		}

		private void AnchorShip()
		{
			_ship.SetAnchor(isAnchored: true, anchorInPlace: true);
			_ship.ShipOrder.SetShipStopOrder();
			_ship.SetShipOrderActive(isOrderActive: false);
			_ship.Formation.SetControlledByAI(false, false);
		}

		public void SendToDestination()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (_destination != (GameEntity)null)
			{
				ShipOrder shipOrder = _ship.ShipOrder;
				Vec3 globalPosition = _destination.GlobalPosition;
				Vec2 asVec = ((Vec3)(ref globalPosition)).AsVec2;
				globalPosition = _destination.GlobalPosition;
				Vec2 asVec2 = ((Vec3)(ref globalPosition)).AsVec2;
				WeakGameEntity gameEntity = ((ScriptComponentBehavior)_ship).GameEntity;
				MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
				Vec2 targetDirection = asVec2 - ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
				targetDirection = ((Vec2)(ref targetDirection)).Normalized();
				shipOrder.SetShipMovementOrder(asVec, in targetDirection);
				_ship.Formation.SetControlledByAI(false, false);
			}
		}

		public void TriggerShip()
		{
			BlockedEstuaryMissionController missionBehavior = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
			missionBehavior.TriggerEnemyShip(_ship, missionBehavior._playerShip);
			_isTriggered = true;
		}
	}

	private const string EscapeZoneId = "escape_zone";

	private const string JumpingZoneId = "jumping_zone";

	private const string Fire2ZoneId = "fire_2_zone";

	private const string InitialTriggerZoneId = "burning_zone";

	private const string FireSystemId = "fire_particles";

	private const string Fire3ZoneId = "fire_3_zone";

	private const string CheckPointZoneId = "dismount_zone";

	private const string RampHolderId = "ramp_holder";

	private const string EnemyShipSpawnIdBase = "sp_enemy_ship_";

	private const string EnemyShipTriggerSpawnIdBase = "sp_enemy_trigger_";

	private const string EnemyShipDestinationIdBase = "sp_enemy_ship_destination_";

	private const string TargetShipSpawnId = "sp_enemy_ship_1";

	private const string PlayerBurningShipSpawnId = "sp_player_burning_ship";

	private const string PlayerBurningShipCheckpointSpawnId = "sp_player_burning_ship_checkpoint";

	private const string PlayerShipSpawnId = "sp_player_ship";

	private const string PlayerWaterSpawnPointAfterFadeToBlackId = "sp_player_mount";

	private const string PlayerCheckPointSpawnPointId = "sp_player_checkpoint";

	private const string GangradirBurningShipSpawnId = "sp_gangradir_burning_ship";

	private const string HorseSpawnPointId = "sp_horse";

	private const string HorseItemId = "sturgia_horse_tournament";

	private const string EnemyAgentPatrolPointBaseId = "sp_guard_patrol";

	private const string EnemyAgentSpawnPointBaseId = "enemy_group_parent";

	private const float WindStrength = 4f;

	private static MBList<string> _enemyAgentCharacterIds;

	private MissionObjectiveLogic _missionObjectiveLogic;

	public Action OnCheckPointReachedEvent;

	public Action OnLastExitZoneReachedEvent;

	public Action OnPhaseEnd;

	private NavalShipsLogic _navalShipsLogic;

	private NavalAgentsLogic _navalAgentsLogic;

	private ShipAgentSpawnLogic _shipAgentSpawnLogic;

	private AgentNavalComponent _mainAgentNavalComponent;

	private VolumeBox _escapeZone;

	private VolumeBox _jumpingZone;

	private VolumeBox _fire2Zone;

	private VolumeBox _initialTriggerZone;

	private VolumeBox _fire3Zone;

	private VolumeBox _checkPointZone;

	private MBList<EnemyShipTrigger> _triggers = new MBList<EnemyShipTrigger>();

	private Dictionary<BurnShipObject, (BurningSystem, float)> _playerShipBurningSystems;

	private BurningSystem _enemyShipBurningSystem;

	private List<BurningProjectile> _projectileParticles = new List<BurningProjectile>();

	private float _shipDamageCheckTimer;

	private float _shipBurnProgress;

	private List<Agent> _burntShipAgents;

	private MBList<BurnShipObject> _burningMachines;

	private bool _initializeGangradirBurningShip;

	private bool _showedLastWarning;

	private BattlePhase _currentPhase;

	private IShipOrigin _playerBurningShipOrigin;

	private IShipOrigin _enemyBurningShipOrigin;

	private IShipOrigin _playerShipOrigin;

	private MBList<IShipOrigin> _enemyShipOrigins = new MBList<IShipOrigin>();

	private MissionShip _playerShip;

	private bool _initialized;

	private bool _enemiesPanicked;

	private bool _shipsCollided;

	private MissionTimer _missionEndTimer;

	private MissionTimer _missionPhaseEndTimer;

	private MissionTimer _collisionTimer;

	private bool _talkedToGangradir;

	private Agent _playerHorse;

	private Agent _horse;

	private Agent _gangradir;

	private bool _gangradirShouldEscape;

	private WorldPosition _escapePosition;

	private readonly MobileParty _enemyParty;

	private readonly bool _startFromCheckPoint;

	private bool _checkPointReached;

	private MBList<EnemySpawnPoint> _enemyAgentSpawnPoints = new MBList<EnemySpawnPoint>();

	public bool CanEndBattleNatively => CurrentPhase == BattlePhase.Phase3;

	private float BurningSpreadRateMultiplier => 20f;

	private float DefaultSpreadRate => 0.5f;

	public BattlePhase CurrentPhase
	{
		get
		{
			return _currentPhase;
		}
		private set
		{
			if (value != _currentPhase)
			{
				_currentPhase = value;
				OnPhaseEnd?.Invoke();
			}
		}
	}

	public MissionShip BurningShip { get; private set; }

	public bool IsShipBurning { get; private set; }

	public bool ShipsCollided => _shipsCollided;

	private bool IsEnding => _missionEndTimer != null;

	public bool CollisionImminent { get; private set; }

	public bool LastExitZoneReached { get; private set; }

	private MissionShip TargetShip { get; set; }

	public BlockedEstuaryMissionController(MobileParty enemyParty, bool startFromCheckPoint)
	{
		_enemyParty = enemyParty;
		_startFromCheckPoint = startFromCheckPoint;
		_checkPointReached = _startFromCheckPoint;
		CollectShips();
	}

	private void CollectShips()
	{
		new MBList<IShipOrigin>();
		Ship playerShipOrigin = ((IEnumerable<Ship>)MobileParty.MainParty.Ships).FirstOrDefault((Func<Ship, bool>)((Ship x) => ((MBObjectBase)x.ShipHull).StringId == "ship_trade_cog_q3")) ?? ((IEnumerable<Ship>)MobileParty.MainParty.Ships).First();
		Ship enemyBurningShip = ((IEnumerable<Ship>)_enemyParty.Ships).FirstOrDefault((Func<Ship, bool>)((Ship x) => ((MBObjectBase)x.ShipHull).StringId == "burning_cog_ship"));
		_enemyShipOrigins = Extensions.ToMBList<IShipOrigin>(((IEnumerable<Ship>)_enemyParty.Ships).Where((Ship x) => x != enemyBurningShip).Cast<IShipOrigin>());
		_playerBurningShipOrigin = (IShipOrigin)(object)((IEnumerable<Ship>)MobileParty.MainParty.Ships).FirstOrDefault((Func<Ship, bool>)((Ship x) => ((MBObjectBase)x.ShipHull).StringId == "burning_fishing_ship"));
		_enemyBurningShipOrigin = (IShipOrigin)(object)enemyBurningShip;
		_playerShipOrigin = (IShipOrigin)(object)playerShipOrigin;
	}

	public override void OnMissionTick(float dt)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (!_initialized)
		{
			Initialize();
		}
		if (_missionEndTimer != null && _missionEndTimer.Check(false))
		{
			OnFinalize();
		}
		if ((Agent.Main == null || !Agent.Main.IsActive()) && !IsEnding)
		{
			OnFail(new TextObject("{=ay5y18aq}You pass out from the pain of your wounds.", (Dictionary<string, object>)null));
		}
		switch (CurrentPhase)
		{
		case BattlePhase.Phase1:
			TickMissionPhase1(dt);
			break;
		case BattlePhase.Phase2:
			TickMissionPhase2(dt);
			break;
		case BattlePhase.Phase3:
			TickMissionPhase3(dt);
			break;
		}
		TickParticlesAndBurningSystems(dt);
		TickGangradir(dt);
	}

	private void TickMissionPhase1(float dt)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		MatrixFrame globalFrame = BurningShip.GlobalFrame;
		WeakGameEntity gameEntity;
		Vec3 val;
		if (_collisionTimer != null && _collisionTimer.Check(false) && !IsEnding)
		{
			OnFail(new TextObject("{=CAyVaV0Y}Your fireship missed its target! The enemy flagship is unscathed.", (Dictionary<string, object>)null));
		}
		else if (IsShipBurning && !IsEnding)
		{
			if (_missionPhaseEndTimer != null && _missionPhaseEndTimer.Check(false))
			{
				ProceedToPhase2();
				_missionPhaseEndTimer = null;
			}
			else if (Agent.Main.IsInWater())
			{
				if (_shipsCollided && _missionPhaseEndTimer == null)
				{
					DestroyCollidingShips();
					_missionPhaseEndTimer = new MissionTimer(6f);
				}
			}
			else if (_jumpingZone.IsPointIn(globalFrame.origin))
			{
				OnFail(new TextObject("{=Uj6t6FES}You missed the oppurtunity to jump off the ship.", (Dictionary<string, object>)null));
			}
			if (((MissionObject)BurningShip).IsDisabled && _collisionTimer == null && !_shipsCollided)
			{
				OnFail(new TextObject("{=S0L5Zi8a}Your ship is engulfed by flames.", (Dictionary<string, object>)null));
			}
			if (_jumpingZone.IsPointIn(globalFrame.origin) && _collisionTimer == null)
			{
				_collisionTimer = new MissionTimer(15f);
				CollisionImminent = true;
			}
			if (CollisionImminent && !_enemiesPanicked)
			{
				gameEntity = ((ScriptComponentBehavior)BurningShip).GameEntity;
				Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				val = BurningShip.Physics.LinearVelocity;
				Vec2 velocity2D = ((Vec3)(ref val)).AsVec2 * 3f;
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				Vec3 globalPosition2 = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				Vec3 boxMin = globalPosition2 + ((WeakGameEntity)(ref gameEntity)).GetBoundingBoxMin();
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				Vec3 globalPosition3 = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				if (WillHitBoundingBox(globalPosition, velocity2D, boxMin, globalPosition3 + ((WeakGameEntity)(ref gameEntity)).GetBoundingBoxMax()))
				{
					MakeEnemiesPanic(TargetShip);
				}
			}
		}
		if ((_fire3Zone.IsPointIn(globalFrame.origin) || _shipBurnProgress >= 0.6f) && !_gangradirShouldEscape)
		{
			ShowGangradirEscapeNotification();
			_gangradirShouldEscape = true;
			if (_gangradir != null)
			{
				SetEscapePosition();
			}
		}
		if (!LastExitZoneReached && !_showedLastWarning && !((MissionObject)BurningShip).IsDisabled && !BurningShip.IsSinking)
		{
			gameEntity = ((ScriptComponentBehavior)BurningShip).GameEntity;
			val = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
			gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
			if (((Vec3)(ref val)).Distance(((WeakGameEntity)(ref gameEntity)).GlobalPosition) < 120f && !IsEnding && !Agent.Main.IsInWater())
			{
				ShowNotification(new TextObject("{=yYkI9ezi}Jump now! You want your breeks to catch fire?", (Dictionary<string, object>)null), isAnnouncedByGunnar: true);
				_showedLastWarning = true;
			}
		}
		if (_jumpingZone.IsPointIn(globalFrame.origin) && !LastExitZoneReached)
		{
			LastExitZoneReached = true;
			OnLastExitZoneReachedEvent?.Invoke();
			if (!IsShipBurning)
			{
				ActivateAllBurningSystems(DefaultSpreadRate);
			}
		}
		if (!CollisionImminent)
		{
			TickShipHealth(dt);
		}
		if (_initialTriggerZone.IsPointIn(globalFrame.origin) && !_initializeGangradirBurningShip)
		{
			_initializeGangradirBurningShip = true;
		}
	}

	private void TickShipHealth(float dt)
	{
		if (_shipsCollided || !IsShipBurning || !(BurningShip.HitPoints > 0f) || LastExitZoneReached)
		{
			return;
		}
		float num = 0f;
		foreach (KeyValuePair<BurnShipObject, (BurningSystem, float)> playerShipBurningSystem in _playerShipBurningSystems)
		{
			if (playerShipBurningSystem.Value.Item1 != null)
			{
				num += playerShipBurningSystem.Value.Item1.GetFlameProgress();
			}
		}
		num /= (float)_playerShipBurningSystems.Count;
		_shipDamageCheckTimer += dt;
		while (_shipDamageCheckTimer > 0.1f)
		{
			_shipDamageCheckTimer -= 0.1f;
			float rawDamage = (num - _shipBurnProgress) * BurningShip.MaxHealth;
			_shipBurnProgress = num;
			BurningShip.DealDamage(rawDamage, null, out var _, out var _, out var _, out var _);
			float fireDamage = (num - (1f - BurningShip.FireHitPoints / BurningShip.MaxFireHealth)) * BurningShip.MaxFireHealth;
			BurningShip.DealFireDamage(fireDamage);
		}
	}

	private void EnableRamp(MissionShip targetShip)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)targetShip).GameEntity;
		WeakGameEntity firstChildEntityWithTagRecursive = ((WeakGameEntity)(ref gameEntity)).GetFirstChildEntityWithTagRecursive("ramp_holder");
		((WeakGameEntity)(ref firstChildEntityWithTagRecursive)).SetVisibilityExcludeParents(true);
	}

	private void MakeEnemiesPanic(MissionShip targetShip)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		EnableRamp(targetShip);
		_burntShipAgents = ((IEnumerable<Agent>)_navalAgentsLogic.GetActiveAgentsOfShip(targetShip)).ToList();
		_navalAgentsLogic.RemoveAllReservedTroopsFromShip(targetShip);
		targetShip.Formation.SetControlledByAI(true, false);
		targetShip.ShipOrder.FormationLeaveShip();
		Vec2 val2 = default(Vec2);
		for (int num = _burntShipAgents.Count - 1; num >= 0; num--)
		{
			Agent val = _burntShipAgents[num];
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)targetShip).GameEntity;
			Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
			((Vec2)(ref val2))._002Ector(MBRandom.RandomFloatRanged(60f, 110f), MBRandom.RandomFloatRanged(70f, 120f));
			Vec2 val3 = ((Vec3)(ref globalPosition)).AsVec2 + ((MBRandom.RandomFloat < 0.5f) ? val2 : (-val2));
			Vec3 val4 = ((Vec2)(ref val3)).ToVec3(0f) - val.Position;
			val4 = ((Vec3)(ref val4)).NormalizedCopy();
			val.SetTargetPositionAndDirection(ref val3, ref val4);
		}
		_enemiesPanicked = true;
	}

	private void TickParticlesAndBurningSystems(float dt)
	{
		if (IsShipBurning)
		{
			foreach (KeyValuePair<BurnShipObject, (BurningSystem, float)> playerShipBurningSystem in _playerShipBurningSystems)
			{
				if (playerShipBurningSystem.Value.Item1 != null)
				{
					playerShipBurningSystem.Value.Item1.Tick(dt);
					playerShipBurningSystem.Value.Item1.CheckWater();
				}
			}
		}
		if (!((MissionObject)BurningShip).IsDisabled && (!LastExitZoneReached || _shipsCollided))
		{
			bool flag = true;
			foreach (KeyValuePair<BurnShipObject, (BurningSystem, float)> playerShipBurningSystem2 in _playerShipBurningSystems)
			{
				if (playerShipBurningSystem2.Value.Item1 != null && !playerShipBurningSystem2.Value.Item1.FlamesReachedEnd())
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				DisableShip(BurningShip);
			}
		}
		if (_shipsCollided)
		{
			_enemyShipBurningSystem.Tick(dt);
			_enemyShipBurningSystem.CheckWater();
		}
		if (_enemyShipBurningSystem.FlamesReachedEnd() && !((MissionObject)TargetShip).IsDisabled)
		{
			DisableShip(TargetShip);
		}
		bool flag2 = false;
		for (int num = _projectileParticles.Count - 1; num >= 0; num--)
		{
			BurningProjectile burningProjectile = _projectileParticles[num];
			burningProjectile.Tick(dt, out var shouldBeRemoved);
			if (shouldBeRemoved)
			{
				burningProjectile.Clear();
				_projectileParticles.RemoveAt(num);
			}
			else if (!flag2 && CurrentPhase == BattlePhase.Phase1 && DoesShipCollideWithProjectile(BurningShip, burningProjectile))
			{
				flag2 = true;
			}
		}
		if (CurrentPhase != BattlePhase.Phase1)
		{
			return;
		}
		if (IsShipBurning)
		{
			if (LastExitZoneReached)
			{
				return;
			}
			{
				foreach (KeyValuePair<BurnShipObject, (BurningSystem, float)> playerShipBurningSystem3 in _playerShipBurningSystems)
				{
					if (playerShipBurningSystem3.Value.Item1 != null)
					{
						float spreadRate = (flag2 ? (playerShipBurningSystem3.Value.Item2 * BurningSpreadRateMultiplier) : playerShipBurningSystem3.Value.Item2);
						playerShipBurningSystem3.Value.Item1.SetSpreadRate(spreadRate);
					}
				}
				return;
			}
		}
		if (flag2)
		{
			ActivateAllBurningSystems(DefaultSpreadRate);
		}
	}

	private void BurnSails(MissionShip ship)
	{
		foreach (MissionSail item in (List<MissionSail>)(object)ship.Sails)
		{
			if (!item.IsBurning())
			{
				item.StartFire();
			}
		}
	}

	private void ToggleShipBallistas(MissionShip ship, bool enabled)
	{
		if (ship.ShipSiegeWeapon == null)
		{
			return;
		}
		foreach (StandingPoint item in (List<StandingPoint>)(object)((UsableMachine)ship.ShipSiegeWeapon).StandingPoints)
		{
			((UsableMissionObject)item).IsDeactivated = !enabled;
		}
	}

	private void DisableShip(MissionShip ship, bool burnSails = true)
	{
		if (((MissionObject)ship).IsDisabled)
		{
			return;
		}
		foreach (ShipAttachmentMachine item in (List<ShipAttachmentMachine>)(object)ship.AttachmentMachines)
		{
			((MissionObject)item).SetDisabled(false);
		}
		((MissionObject)ship.ShipControllerMachine).SetDisabled(false);
		foreach (ClimbingMachine item2 in (List<ClimbingMachine>)(object)ship.ClimbingMachines)
		{
			((MissionObject)item2).SetDisabled(false);
		}
		foreach (ShipOarMachine item3 in (List<ShipOarMachine>)(object)ship.LeftSideShipOarMachines)
		{
			((MissionObject)item3).SetDisabled(false);
		}
		foreach (ShipOarMachine item4 in (List<ShipOarMachine>)(object)ship.RightSideShipOarMachines)
		{
			((MissionObject)item4).SetDisabled(false);
		}
		ToggleShipBallistas(ship, enabled: false);
		if (((UsableMachine)ship.ShipControllerMachine).PilotAgent != null)
		{
			((UsableMachine)ship.ShipControllerMachine).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
		}
		((MissionObject)ship.ShipControllerMachine).SetDisabled(false);
		((MissionObject)ship).SetDisabled(false);
		DisableTargetShipObject(ship);
		ship.SetAnchor(isAnchored: true, anchorInPlace: true);
		if (burnSails)
		{
			BurnSails(ship);
		}
	}

	private void SetWindStrengthAndDirection(Vec2 direction, float strength)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Scene scene = Mission.Current.Scene;
		Vec2 val = strength * direction;
		scene.SetGlobalWindVelocity(ref val);
	}

	private void ProceedToPhase2()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		IsShipBurning = true;
		_shipsCollided = true;
		SpawnPlayerTradeShip();
		FadeoutEnemyAgents();
		SpawnEnemyAgentsOnRoad();
		DisableShip(BurningShip);
		DisableShip(TargetShip);
		ActivateEnemyShips();
		CollisionImminent = false;
		_playerHorse = SpawnPlayerHorse();
		Vec3 randomPositionAroundPoint = ((MissionBehavior)this).Mission.GetRandomPositionAroundPoint(_playerHorse.Position, 2f, 4f, false);
		Vec3 val = randomPositionAroundPoint - Agent.Main.Position;
		_horse = SpawnHorse(randomPositionAroundPoint, ((Vec3)(ref val)).AsVec2);
		TeleportMainAgent("sp_player_mount");
		PrepareGangradirForSecondPhase();
		if (IsGangradirActive())
		{
			ShowNotification(new TextObject("{=NB2HCGUq}Head for shore! There are a pair of horses waiting for us. We must ride quickly back to the Sturgians before the Sea Hounds can reorganize the blockade.", (Dictionary<string, object>)null), isAnnouncedByGunnar: true);
		}
		else
		{
			ShowNotification(new TextObject("{=mlMbHCaG}Head for shore! There are a pair of horses waiting for you. Ride quickly back to the Sturgians before the Sea Hounds can reorganize the blockade.", (Dictionary<string, object>)null), isAnnouncedByGunnar: false);
		}
		GameEntity val2 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_wind");
		if (val2 != (GameEntity)null)
		{
			MatrixFrame globalFrame = val2.GetGlobalFrame();
			Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
			SetWindStrengthAndDirection(((Vec2)(ref asVec)).Normalized(), val2.GetGlobalScale().z);
		}
		CurrentPhase = BattlePhase.Phase2;
		_missionObjectiveLogic.StartObjective((MissionObjective)(object)new SwimToShoreObjective(((MissionBehavior)this).Mission, _gangradir));
	}

	public List<Agent> GetAgentsOfInterest()
	{
		List<Agent> list = new List<Agent>();
		if (CurrentPhase == BattlePhase.Phase2)
		{
			if (_horse != null && _horse.IsActive())
			{
				list.Add(_horse);
			}
			if (_playerHorse != null && _playerHorse.IsActive())
			{
				list.Add(_playerHorse);
			}
		}
		if (IsGangradirActive())
		{
			list.Add(_gangradir);
		}
		return list;
	}

	private void PrepareGangradirForSecondPhase()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Vec3 randomPositionAroundPoint = ((MissionBehavior)this).Mission.GetRandomPositionAroundPoint(Agent.Main.Position, 1f, 3f, false);
		Vec3 val;
		if (_gangradir == null || !_gangradir.IsActive())
		{
			SpawnGangradir(randomPositionAroundPoint, noHorses: true);
		}
		else
		{
			val = Agent.Main.Position;
			if (((Vec3)(ref val)).Distance(_gangradir.Position) > 5f)
			{
				_gangradir.TeleportToPosition(randomPositionAroundPoint);
			}
		}
		_gangradir.SetTeam(Team.Invalid, true);
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			item.ResetEnemyCaches();
		}
		_gangradir.GetComponent<CampaignAgentComponent>().AgentNavigator.ClearTarget();
		_gangradir.SetAgentFlags((AgentFlag)(_gangradir.GetAgentFlags() | 0x2000));
		_gangradir.SetRidingOrder((RidingOrderEnum)1);
		_gangradir.SetAlarmState((AIStateFlag)3);
		WorldPosition val2 = default(WorldPosition);
		((WorldPosition)(ref val2))._002Ector(((MissionBehavior)this).Mission.Scene, _horse.Position);
		Agent gangradir = _gangradir;
		val = _horse.Position - _gangradir.Position;
		Vec2 asVec = ((Vec3)(ref val)).AsVec2;
		gangradir.SetScriptedPositionAndDirection(ref val2, ((Vec2)(ref asVec)).RotationInRadians, true, (AIScriptedFrameFlags)8);
	}

	private void SpawnEnemyAgentsOnRoad()
	{
		bool isNight = Campaign.Current.IsNight;
		GameEntity val = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("enemy_group_parent");
		if (val != (GameEntity)null)
		{
			for (int i = 0; i < val.ChildCount; i++)
			{
				GameEntity child = val.GetChild(i);
				((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints).Add(new EnemySpawnPoint(child, MBObjectManager.Instance.GetObject<CharacterObject>(Extensions.GetRandomElement<string>(_enemyAgentCharacterIds)), isNight));
			}
		}
	}

	private void FadeoutEnemyAgents()
	{
		if (_burntShipAgents != null)
		{
			foreach (Agent burntShipAgent in _burntShipAgents)
			{
				if (burntShipAgent.IsActive())
				{
					burntShipAgent.FadeOut(true, true);
				}
			}
		}
		_burntShipAgents = null;
	}

	private void TeleportMainAgent(string spawnPointId)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag(spawnPointId).GetGlobalFrame();
		Agent.Main.TeleportToPosition(globalFrame.origin);
		Agent.Main.LookDirection = ((Vec3)(ref globalFrame.rotation.f)).NormalizedCopy();
	}

	private static void ShowNotification(TextObject text, bool isAnnouncedByGunnar)
	{
		if (!isAnnouncedByGunnar)
		{
			MBInformationManager.AddQuickInformation(text, 0, (BasicCharacterObject)null, (Equipment)null, "");
		}
		else
		{
			CampaignInformationManager.AddDialogLine(text, NavalStorylineData.Gangradir.CharacterObject, (Equipment)null, 0, (NotificationPriority)2);
		}
	}

	private void DestroyCollidingShips()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		TargetShip.SetShipOrderActive(isOrderActive: false);
		TargetShip.Formation.SetControlledByAI(false, false);
		TargetShip.SetAnchor(isAnchored: false);
		BurningShip.SetShipOrderActive(isOrderActive: false);
		BurningShip.ShipOrder.SetFormation(null);
		TargetShip.ShipOrder.SetFormation(null);
		for (int num = _burntShipAgents.Count - 1; num >= 0; num--)
		{
			Agent val = _burntShipAgents[num];
			if (!val.IsInWater() && _navalAgentsLogic.IsAgentOnAnyShip(val, out var onShip, (TeamSideEnum)2) && onShip == TargetShip)
			{
				Blow val2 = new Blow
				{
					InflictedDamage = 1000,
					DamagedPercentage = 1f
				};
				val.Die(val2, (KillInfo)(-1));
			}
		}
		BurnSails(TargetShip);
		BurnSails(BurningShip);
	}

	public void OnBurningMachineUsed(BurnShipObject burnShipObject)
	{
		ActivateBurningSystem(burnShipObject, DefaultSpreadRate);
	}

	private void MakeGangradirEscapeShip()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_gangradir.IsUsingGameObject)
		{
			_gangradir.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
		}
		EnableRamp(BurningShip);
		Vec2 asVec = ((WorldPosition)(ref _escapePosition)).AsVec2;
		Vec3 escapePosition = GetEscapePosition(BurningShip);
		if (((Vec3)(ref escapePosition)).Distance(((Vec2)(ref asVec)).ToVec3(escapePosition.z)) > 10f)
		{
			SetEscapePosition(escapePosition);
		}
	}

	private void ShowGangradirEscapeNotification()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		ShowNotification(new TextObject("{=yXOnEQJ6}Our ship is ablaze! Get ready to jump!", (Dictionary<string, object>)null), IsGangradirActive());
	}

	private void SetEscapePosition()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SetEscapePosition(GetEscapePosition(BurningShip));
	}

	private void SetEscapePosition(Vec3 position)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		_escapePosition = new WorldPosition(((MissionBehavior)this).Mission.Scene, position);
		Agent gangradir = _gangradir;
		Vec2 asVec = ((Vec3)(ref position)).AsVec2;
		Vec3 val = position - _gangradir.Position;
		val = ((Vec3)(ref val)).NormalizedCopy();
		gangradir.SetTargetPositionAndDirection(ref asVec, ref val);
	}

	private Vec3 GetEscapePosition(MissionShip ship)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		Vec3 val = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame().rotation.f * 10f;
		gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		Vec3 val2 = val - ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame().rotation.s * 15f;
		gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		return val2 + ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
	}

	public void ActivateAllBurningSystems(float spreadRate)
	{
		for (int i = 0; i < ((List<BurnShipObject>)(object)_burningMachines).Count; i++)
		{
			ActivateBurningSystem(((List<BurnShipObject>)(object)_burningMachines)[i], spreadRate);
		}
	}

	public void ActivateBurningSystem(BurnShipObject burnShipObject, float spreadRate)
	{
		if (burnShipObject != null)
		{
			(BurningSystem, float) tuple = _playerShipBurningSystems[burnShipObject];
			_playerShipBurningSystems[burnShipObject] = (tuple.Item1, spreadRate);
		}
		IsShipBurning = true;
	}

	private void TickMissionPhase2(float dt)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		if (IsEnding)
		{
			return;
		}
		if (!_startFromCheckPoint)
		{
			if (_checkPointZone.IsPointIn(Agent.Main.Position) && !_checkPointReached && !((IEnumerable<EnemySpawnPoint>)_enemyAgentSpawnPoints).Any(delegate(EnemySpawnPoint x)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				Vec3 position = x.Position;
				return ((Vec3)(ref position)).Distance(Agent.Main.Position) < 50f;
			}))
			{
				OnCheckPointReached();
			}
			if (_checkPointReached)
			{
				if (Agent.Main.HasMount)
				{
					TickHorse(Agent.Main);
				}
			}
			else
			{
				if (Agent.Main.HasMount)
				{
					float stat = Agent.Main.MountAgent.AgentDrivenProperties.GetStat((DrivenProperty)92);
					float num = ((IsGangradirActive() && _gangradir.HasMount) ? _gangradir.MountAgent.AgentDrivenProperties.GetStat((DrivenProperty)92) : stat);
					if (!MBMath.ApproximatelyEqualsTo(stat, num, 1E-05f) && stat < num)
					{
						Agent.Main.MountAgent.AgentDrivenProperties.SetStat((DrivenProperty)92, num);
						Agent.Main.MountAgent.UpdateCustomDrivenProperties();
					}
				}
				bool flag = false;
				float stat2 = Agent.Main.AgentDrivenProperties.GetStat((DrivenProperty)66);
				float num2 = MathF.Max(stat2, 1f);
				if (!MBMath.ApproximatelyEqualsTo(stat2, num2, 1E-05f))
				{
					flag = true;
					Agent.Main.AgentDrivenProperties.SetStat((DrivenProperty)66, num2);
				}
				float stat3 = Agent.Main.AgentDrivenProperties.GetStat((DrivenProperty)67);
				float num3 = MathF.Max(stat3, 1f);
				if (!MBMath.ApproximatelyEqualsTo(stat3, num3, 1E-05f))
				{
					flag = true;
					Agent.Main.AgentDrivenProperties.SetStat((DrivenProperty)67, num3);
				}
				float stat4 = Agent.Main.AgentDrivenProperties.GetStat((DrivenProperty)96);
				float num4 = MathF.Max(stat4, 1f);
				if (!MBMath.ApproximatelyEqualsTo(stat4, num4, 1E-05f))
				{
					flag = true;
					Agent.Main.AgentDrivenProperties.SetStat((DrivenProperty)96, num4);
				}
				if (flag)
				{
					Agent.Main.UpdateCustomDrivenProperties();
				}
			}
		}
		if (!_checkPointReached)
		{
			CheckEnemyGroups(dt);
		}
		else if (_playerShip == _mainAgentNavalComponent.SteppedShip && _missionPhaseEndTimer == null)
		{
			_missionPhaseEndTimer = new MissionTimer(1f);
		}
		else if (_gangradir == null && !Agent.Main.HasMount)
		{
			SpawnGangradirOnShip(_playerShip);
		}
		if (_missionPhaseEndTimer != null && _missionPhaseEndTimer.Check(false) && _talkedToGangradir)
		{
			ProceedToPhase3();
			_missionPhaseEndTimer = null;
		}
	}

	private void TickGangradir(float dt)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnding)
		{
			return;
		}
		bool flag = IsGangradirActive();
		if (CurrentPhase == BattlePhase.Phase1)
		{
			if (flag && !_gangradir.IsUsingGameObject && _initializeGangradirBurningShip && !LastExitZoneReached && !_gangradirShouldEscape)
			{
				BurnShipObject burnShipObject = ((IEnumerable<BurnShipObject>)_burningMachines).FirstOrDefault((BurnShipObject x) => !((UsableMachine)x).IsDeactivated && !x.HasUser);
				if (burnShipObject != null && !((UsableMissionObject)((UsableMachine)burnShipObject).PilotStandingPoint).HasAIMovingTo)
				{
					_gangradir.GetComponent<CampaignAgentComponent>().AgentNavigator.SetTarget((UsableMachine)(object)burnShipObject, false, (AIScriptedFrameFlags)0);
				}
			}
			else if (flag && _gangradirShouldEscape && !_gangradir.IsInWater())
			{
				MakeGangradirEscapeShip();
			}
		}
		else
		{
			if (CurrentPhase != BattlePhase.Phase2 || _checkPointReached || _talkedToGangradir)
			{
				return;
			}
			if (_missionPhaseEndTimer != null && _missionPhaseEndTimer.Check(false))
			{
				if (flag && _gangradir.HasMount)
				{
					Vec3 position = Agent.Main.Position;
					if (((Vec3)(ref position)).Distance(_gangradir.Position) <= 30f)
					{
						StartConversation();
						_missionPhaseEndTimer = null;
						_talkedToGangradir = true;
					}
					else
					{
						ProceedToRideWithoutTalkingToGangradir();
					}
				}
			}
			else if (!flag && Agent.Main.HasMount)
			{
				ProceedToRideWithoutTalkingToGangradir();
			}
		}
	}

	private void ProceedToRideWithoutTalkingToGangradir()
	{
		if (IsGangradirActive())
		{
			OnTalkedToGangradirPhase2();
		}
		_missionPhaseEndTimer = null;
		_talkedToGangradir = true;
	}

	private void StartConversation()
	{
		Campaign.Current.ConversationManager.SetupAndStartMissionConversation((IAgent)(object)_gangradir, (IAgent)(object)Agent.Main, false);
		((MissionBehavior)this).Mission.SetMissionMode((MissionMode)1, true);
	}

	private void SpawnGangradirOnShip(MissionShip ship)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		_navalAgentsLogic.AddReservedTroopToShip((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)NavalStorylineData.Gangradir.CharacterObject, -1, (Banner)null, default(UniqueTroopDescriptor)), ship);
		_navalAgentsLogic.SpawnNextBatch((TeamSideEnum)0);
		_gangradir = ((IEnumerable<Agent>)((MissionBehavior)this).Mission.Agents).First((Agent x) => (object)x.Character == NavalStorylineData.Gangradir.CharacterObject);
		_gangradir.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
	}

	private void CheckEnemyGroups(float dt)
	{
		foreach (EnemySpawnPoint item in (List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints)
		{
			item.Tick(dt, this);
		}
	}

	private void ProceedToPhase3()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		if (!_checkPointReached)
		{
			OnCheckPointReached();
		}
		CurrentPhase = BattlePhase.Phase3;
		_playerShip.SetAnchor(isAnchored: false);
		_playerShip.Formation.SetControlledByAI(true, false);
		_playerShip.ShipOrder.FormationJoinShip(_playerShip.Formation);
		_playerShip.SetShipOrderActive(isOrderActive: true);
		if (Agent.Main != null)
		{
			if (_navalAgentsLogic.IsAgentOnAnyShip(Agent.Main, out var _, (TeamSideEnum)(-1)))
			{
				_navalAgentsLogic.TransferAgentToShip(Agent.Main, _playerShip);
			}
			else
			{
				_navalAgentsLogic.AddAgentToShip(Agent.Main, _playerShip);
			}
			if (!_startFromCheckPoint)
			{
				Agent.Main.UseGameObject((UsableMissionObject)(object)((UsableMachine)_playerShip.ShipControllerMachine).PilotStandingPoint, -1);
				((UsableMachine)_playerShip.ShipControllerMachine).OnPilotAssignedDuringSpawn();
			}
		}
		MissionObjectiveLogic missionObjectiveLogic = _missionObjectiveLogic;
		Mission mission = ((MissionBehavior)this).Mission;
		MissionShip playerShip = _playerShip;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)_escapeZone).GameEntity;
		missionObjectiveLogic.StartObjective((MissionObjective)(object)new ReachEscapeZoneObjective(mission, playerShip, ((WeakGameEntity)(ref gameEntity)).GlobalPosition + new Vec3(0f, 0f, 5f, -1f)));
		ShowNotification(new TextObject("{=riDim3Wf}You've made it! Now, we shall break through the scattered enemy blockade and reach the escape zone!", (Dictionary<string, object>)null), IsGangradirActive());
	}

	private void ActivateEnemyShips()
	{
		foreach (EnemyShipTrigger item in (List<EnemyShipTrigger>)(object)_triggers)
		{
			item.SendToDestination();
		}
	}

	private void InitializeShipTriggers()
	{
		for (int i = 0; i < 10; i++)
		{
			int num = i + 2;
			GameEntity val = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_enemy_ship_" + num);
			GameEntity obj = Mission.Current.Scene.FindEntityWithTag("sp_enemy_trigger_" + num);
			VolumeBox val2 = ((obj != null) ? obj.GetFirstScriptOfType<VolumeBox>() : null);
			if (!(val == (GameEntity)null))
			{
				if (val2 == null)
				{
					Debug.FailedAssert("There is no volume box for spawn point: sp_enemy_trigger_" + num, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "InitializeShipTriggers", 1301);
					break;
				}
				if (num - 1 > ((List<IShipOrigin>)(object)_enemyShipOrigins).Count)
				{
					Debug.FailedAssert("There are not enough ships in party", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "InitializeShipTriggers", 1307);
					break;
				}
				if (val != (GameEntity)null)
				{
					((List<EnemyShipTrigger>)(object)_triggers).Add(new EnemyShipTrigger(val, val2, ((List<IShipOrigin>)(object)_enemyShipOrigins)[num - 2], "sp_enemy_ship_destination_" + num));
					continue;
				}
				break;
			}
			break;
		}
	}

	private void ClearEnemyGroups()
	{
		for (int num = ((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints).Count - 1; num >= 0; num--)
		{
			((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints)[num].Clear();
		}
		_enemyAgentSpawnPoints = null;
	}

	public override void OnAgentMount(Agent agent)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		if (CurrentPhase == BattlePhase.Phase2 && !_checkPointReached && !_startFromCheckPoint && !_talkedToGangradir && !IsEnding && IsGangradirActive() && _gangradir.HasMount && Agent.Main.HasMount)
		{
			_missionPhaseEndTimer = new MissionTimer(1f);
		}
		if (_gangradir == agent)
		{
			_gangradir.SetAlarmState((AIStateFlag)0);
			Agent gangradir = _gangradir;
			Vec3 position = _gangradir.Position;
			gangradir.SetTargetPosition(((Vec3)(ref position)).AsVec2);
		}
	}

	public void OnTalkedToGangradirPhase2()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		_gangradir.MountAgent.ClearTargetFrame();
		_gangradir.ClearTargetFrame();
		_horse.SetMortalityState((MortalityState)0);
		_playerHorse.SetMortalityState((MortalityState)0);
		((MissionBehavior)this).Mission.SetMissionMode((MissionMode)2, false);
		Mission mission = ((MissionBehavior)this).Mission;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)_checkPointZone).GameEntity;
		Vec3 randomPositionAroundPoint = mission.GetRandomPositionAroundPoint(((WeakGameEntity)(ref gameEntity)).GlobalPosition, 1f, 3f, false);
		WorldPosition val = default(WorldPosition);
		((WorldPosition)(ref val))._002Ector(((MissionBehavior)this).Mission.Scene, randomPositionAroundPoint);
		_gangradir.SetScriptedPosition(ref val, true, (AIScriptedFrameFlags)9);
		_missionObjectiveLogic.StartObjective((MissionObjective)(object)new ReachShipObjective(((MissionBehavior)this).Mission, _gangradir, _playerShip));
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		if (affectedAgent.IsHuman && _enemyAgentSpawnPoints != null)
		{
			for (int num = ((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints).Count - 1; num >= 0; num--)
			{
				if (((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints)[num].IsDepleted())
				{
					((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints)[num].Clear();
					((List<EnemySpawnPoint>)(object)_enemyAgentSpawnPoints).RemoveAt(num);
				}
			}
		}
		if (affectedAgent == _gangradir)
		{
			_gangradir = null;
		}
	}

	public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((AttackCollisionData)(ref collisionData)).MissileGoneUnderWater && _navalShipsLogic.IsMissileFromShipSiegeEngine(((AttackCollisionData)(ref collisionData)).AffectorWeaponSlotOrMissileIndex))
		{
			_projectileParticles.Add(new BurningProjectile(((AttackCollisionData)(ref collisionData)).CollisionGlobalPosition, 300f, MBRandom.RandomFloatRanged(0.2f, 1.5f), () => CurrentPhase != BattlePhase.Phase1));
		}
	}

	public override void OnAgentDismount(Agent agent)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).OnAgentDismount(agent);
		if (_checkPointReached && agent.IsMainAgent)
		{
			Agent.Main.SetAgentFlags((AgentFlag)(Agent.Main.GetAgentFlags() & -8193));
			if (IsGangradirActive())
			{
				_gangradir.FadeOut(true, true);
			}
		}
	}

	private void TickHorse(Agent rider)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Vec2 currentVelocity = rider.GetCurrentVelocity();
		float num = ((MathF.Abs(currentVelocity.x) <= 0.2f) ? 0f : currentVelocity.x);
		float num2 = ((MathF.Abs(currentVelocity.y) <= 0.2f) ? 0f : currentVelocity.y);
		Vec2 movementInputVector = default(Vec2);
		((Vec2)(ref movementInputVector))._002Ector(0f - num, 0f - num2);
		rider.MovementInputVector = movementInputVector;
		rider.EventControlFlags = (EventControlFlag)(rider.EventControlFlags | 1);
	}

	private void OnCheckPointReached()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!_startFromCheckPoint)
		{
			GetQuest().OnCheckPointReached();
			ClearEnemyGroups();
		}
		InformationManager.DisplayMessage(new InformationMessage(((object)new TextObject("{=BWSp3Uyj}Checkpoint reached.", (Dictionary<string, object>)null)).ToString(), new Color(0f, 1f, 0f, 1f)));
		ShowNotification(new TextObject("{=McvglMqm}Time to get back aboard. Get back on the ship.", (Dictionary<string, object>)null), IsGangradirActive());
		OnCheckPointReachedEvent?.Invoke();
		_checkPointReached = true;
		if (IsGangradirActive())
		{
			_gangradir.SetTeam(((MissionBehavior)this).Mission.PlayerTeam, true);
			DailyBehaviorGroup behaviorGroup = _gangradir.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			if (behaviorGroup != null)
			{
				((AgentBehaviorGroup)behaviorGroup).RemoveBehavior<FollowAgentBehavior>();
			}
		}
		GameEntity val = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_wind_checkpoint");
		if (val != (GameEntity)null)
		{
			MatrixFrame globalFrame = val.GetGlobalFrame();
			Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
			SetWindStrengthAndDirection(((Vec2)(ref asVec)).Normalized(), val.GetGlobalScale().y);
		}
	}

	private void TickMissionPhase3(float dt)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (_escapeZone.IsPointIn(_playerShip.GlobalFrame.origin) && !_playerShip.GetIsConnected())
		{
			if (!IsEnding)
			{
				OnPlayerShipReachedDestination();
			}
		}
		else if (GetTroopCountOfShip(_playerShip) == 0 && !IsEnding)
		{
			OnShipCaptured(_playerShip);
		}
		foreach (EnemyShipTrigger item in (List<EnemyShipTrigger>)(object)_triggers)
		{
			item.Tick(_playerShip, dt);
		}
	}

	private void OnShipCaptured(MissionShip ship)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		ship.SetAnchor(isAnchored: true, anchorInPlace: true);
		ship.ShipOrder.SetShipStopOrder();
		ship.SetShipOrderActive(isOrderActive: false);
		OnFail(new TextObject("{=EydY9CXU}The enemy has captured your ship!", (Dictionary<string, object>)null));
	}

	private int GetTroopCountOfShip(MissionShip ship)
	{
		return _navalAgentsLogic.GetActiveAgentCountOfShip(ship) - _navalAgentsLogic.GetActiveHeroCountOfShip(ship);
	}

	private void OnPlayerShipReachedDestination()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		OnSuccess();
		ShowNotification(new TextObject("{=7arwZMka}Success! You have run the Sea Hound blockade and reached the sea.", (Dictionary<string, object>)null), isAnnouncedByGunnar: false);
	}

	public override void OnBehaviorInitialize()
	{
		_navalShipsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalShipsLogic>();
		_navalAgentsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalAgentsLogic>();
		_shipAgentSpawnLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<ShipAgentSpawnLogic>();
		_navalShipsLogic.SetDeploymentMode(value: true);
		_navalShipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)0, NavalShipDeploymentLimit.Max());
		_navalShipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)1, NavalShipDeploymentLimit.Max());
		_navalShipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)2, NavalShipDeploymentLimit.Max());
		_navalShipsLogic.SetDeploymentMode(value: false);
		_navalShipsLogic.ShipCollisionEvent += OnShipCollision;
		_navalShipsLogic.ShipSunkEvent += OnShipSunk;
		GameEntity obj = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("escape_zone");
		_escapeZone = ((obj != null) ? obj.GetFirstScriptOfType<VolumeBox>() : null);
		GameEntity obj2 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("jumping_zone");
		_jumpingZone = ((obj2 != null) ? obj2.GetFirstScriptOfType<VolumeBox>() : null);
		GameEntity obj3 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("fire_2_zone");
		_fire2Zone = ((obj3 != null) ? obj3.GetFirstScriptOfType<VolumeBox>() : null);
		GameEntity obj4 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("burning_zone");
		_initialTriggerZone = ((obj4 != null) ? obj4.GetFirstScriptOfType<VolumeBox>() : null);
		GameEntity obj5 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("fire_3_zone");
		_fire3Zone = ((obj5 != null) ? obj5.GetFirstScriptOfType<VolumeBox>() : null);
		GameEntity obj6 = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("dismount_zone");
		_checkPointZone = ((obj6 != null) ? obj6.GetFirstScriptOfType<VolumeBox>() : null);
		if (!SailWindProfile.IsSailWindProfileInitialized)
		{
			SailWindProfile.InitializeProfile();
		}
	}

	private void OnShipSunk(MissionShip ship)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		if (CurrentPhase == BattlePhase.Phase1)
		{
			if (ship == BurningShip)
			{
				OnFail(new TextObject("{=Ctrq2rg7}Your ship has sunk!", (Dictionary<string, object>)null));
			}
		}
		else if (ship == _playerShip)
		{
			OnFail(new TextObject("{=Ctrq2rg7}Your ship has sunk!", (Dictionary<string, object>)null));
		}
	}

	private void CacheParticleEntities()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		_playerShipBurningSystems = CreateBurningSystemForPlayerShip(BurningShip);
		_enemyShipBurningSystem = CreateBurningSystem(((ScriptComponentBehavior)TargetShip).GameEntity);
	}

	private Dictionary<BurnShipObject, (BurningSystem, float)> CreateBurningSystemForPlayerShip(MissionShip burningShip)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<BurnShipObject, (BurningSystem, float)> dictionary = new Dictionary<BurnShipObject, (BurningSystem, float)>();
		for (int i = 0; i < ((List<BurnShipObject>)(object)_burningMachines).Count; i++)
		{
			BurnShipObject burnShipObject = ((List<BurnShipObject>)(object)_burningMachines)[i];
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)burnShipObject).GameEntity;
			dictionary[burnShipObject] = (CreateBurningSystem(gameEntity), 0f);
		}
		return dictionary;
	}

	private BurningSystem CreateBurningSystem(WeakGameEntity parent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GameEntity val = GameEntity.CreateFromWeakEntity(((WeakGameEntity)(ref parent)).GetFirstChildEntityWithTagRecursive("fire_particles"));
		if (val == (GameEntity)null)
		{
			return null;
		}
		val.SetVisibilityExcludeParents(true);
		List<GameEntity> list = val.GetChildren().ToList();
		BurningSystem burningSystem = new BurningSystem(val, DefaultSpreadRate);
		foreach (GameEntity item in list)
		{
			CreateBurningNode(burningSystem, item);
		}
		burningSystem.SetExternalFlameMultiplier(2f);
		return burningSystem;
	}

	private void CreateBurningNode(BurningSystem system, GameEntity newNode)
	{
		BurningNode firstScriptOfType = newNode.GetFirstScriptOfType<BurningNode>();
		if (firstScriptOfType != null)
		{
			system.AddNewNode(firstScriptOfType);
			if (MBRandom.RandomFloat > 0.9f)
			{
				firstScriptOfType.EnableSparks();
			}
		}
	}

	private void OnShipCollision(MissionShip ship1, WeakGameEntity targetEntity, Vec3 averageContactPoint, Vec3 totalImpulseOnShip, bool isFirstImpact)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		if (CurrentPhase == BattlePhase.Phase1 && !_shipsCollided && !IsEnding && ((IsShipBurning && targetEntity == ((ScriptComponentBehavior)TargetShip).GameEntity && ship1 == BurningShip) || (ship1 == TargetShip && targetEntity == ((ScriptComponentBehavior)BurningShip).GameEntity)))
		{
			ShowNotification(new TextObject("{=TTiKbNaI}The enemy flagship was hit and is engulfed in flames! The blockaders are in disarray.", (Dictionary<string, object>)null), IsGangradirActive());
			_shipsCollided = true;
			_collisionTimer = null;
		}
	}

	public override void OnMissionStateFinalized()
	{
		Clear();
		SailWindProfile.FinalizeProfile();
	}

	private void Clear()
	{
		OnCheckPointReachedEvent = null;
		OnLastExitZoneReachedEvent = null;
		OnPhaseEnd = null;
	}

	public override void AfterStart()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).AfterStart();
		((MissionBehavior)this).Mission.Scene.SetWaterStrength(1f);
		_missionObjectiveLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionObjectiveLogic>();
		SpawnEnemyTargetShip();
		SpawnPlayerBurningShip();
		CacheParticleEntities();
		if (_startFromCheckPoint)
		{
			SpawnPlayerTradeShip();
			SpawnPlayerOnShip(_playerShip);
			CurrentPhase = BattlePhase.Phase3;
			_playerShip.SetAnchor(isAnchored: false);
			_playerShip.Formation.SetControlledByAI(true, false);
			_playerShip.ShipOrder.FormationJoinShip(_playerShip.Formation);
			_playerShip.SetShipOrderActive(isOrderActive: true);
			DisableShip(BurningShip);
			DisableShip(TargetShip);
			_shipsCollided = true;
			IsShipBurning = true;
			MissionObjectiveLogic missionObjectiveLogic = _missionObjectiveLogic;
			Mission mission = ((MissionBehavior)this).Mission;
			MissionShip playerShip = _playerShip;
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)_escapeZone).GameEntity;
			missionObjectiveLogic.StartObjective((MissionObjective)(object)new ReachEscapeZoneObjective(mission, playerShip, ((WeakGameEntity)(ref gameEntity)).GlobalPosition + new Vec3(0f, 0f, 5f, -1f)));
		}
		else
		{
			SpawnPlayerOnShip(BurningShip);
			SpawnGangradir("sp_gangradir_burning_ship", noHorses: true);
			_shipAgentSpawnLogic.AllocateAndDeployInitialTroops((BattleSideEnum)1);
			_missionObjectiveLogic.StartObjective((MissionObjective)(object)new BurnShipObjective(((MissionBehavior)this).Mission, TargetShip));
		}
		InitializeShipTriggers();
	}

	private void SpawnPlayerOnShip(MissionShip ship)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		_navalAgentsLogic.AddReservedTroopToShip((IAgentOriginBase)new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), ship);
		_navalAgentsLogic.SpawnNextBatch((TeamSideEnum)0);
		_mainAgentNavalComponent = Agent.Main.GetComponent<AgentNavalComponent>();
		_navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, ship);
		Mission.Current.PlayerTeam.PlayerOrderController.Owner = Agent.Main;
	}

	private bool IsGangradirActive()
	{
		if (_gangradir != null)
		{
			return _gangradir.IsActive();
		}
		return false;
	}

	private void Initialize()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		_initialized = true;
		if (!_startFromCheckPoint)
		{
			InitializeEnemyShip(TargetShip);
		}
		MatrixFrame globalFrame = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_player_ship").GetGlobalFrame();
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Vec2 direction = ((Vec2)(ref asVec)).Normalized();
		GameEntity val = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_wind");
		if (val != (GameEntity)null)
		{
			globalFrame = val.GetGlobalFrame();
			asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
			SetWindStrengthAndDirection(((Vec2)(ref asVec)).Normalized(), val.GetGlobalScale().y);
		}
		else
		{
			SetWindStrengthAndDirection(direction, 4f);
		}
		((MissionBehavior)this).Mission.OnDeploymentFinished();
		((MissionBehavior)this).Mission.OnAfterDeploymentFinished();
		if (!_startFromCheckPoint)
		{
			ShowNotification(new TextObject("{=6ZiKOdbI}Once we get within range, their ballista will pelt us with fiery missiles. Avoid them – even if they just hit the water, the flames will keep burning and can spread to our hull.", (Dictionary<string, object>)null), isAnnouncedByGunnar: true);
			ShowNotification(new TextObject("{=b1KaR0Hk}When we get close, I will set fire to our ship and then we swim to shore.", (Dictionary<string, object>)null), isAnnouncedByGunnar: true);
		}
	}

	private void OnFail(TextObject notification)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult((BattleState)2, false);
		_missionEndTimer = new MissionTimer(2f);
		ShowNotification(notification, isAnnouncedByGunnar: false);
	}

	private void OnSuccess(TextObject notification = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult((BattleState)1, false);
		_missionEndTimer = new MissionTimer(2f);
		if (!TextObject.IsNullOrEmpty(notification))
		{
			ShowNotification(notification, isAnnouncedByGunnar: false);
		}
	}

	private void OnFinalize()
	{
		_navalShipsLogic.ShipCollisionEvent -= OnShipCollision;
		_navalShipsLogic.ShipSunkEvent -= OnShipSunk;
		((MissionBehavior)this).Mission.EndMission();
	}

	private void SpawnPlayerBurningShip()
	{
		Formation formation = ((MissionBehavior)this).Mission.PlayerTeam.GetFormation((FormationClass)1);
		string text = (_startFromCheckPoint ? "sp_player_burning_ship_checkpoint" : "sp_player_burning_ship");
		GameEntity spawnEntity = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag(text);
		BurningShip = CreateShip(_playerBurningShipOrigin, ((MissionBehavior)this).Mission.PlayerTeam, formation, spawnEntity);
		formation.SetControlledByAI(false, false);
		BurningShip.SetShipOrderActive(isOrderActive: false);
		InitializeBurningMachines();
	}

	private void InitializeBurningMachines()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_burningMachines = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<BurnShipObject>(((ScriptComponentBehavior)BurningShip).GameEntity);
	}

	private void SpawnPlayerTradeShip()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Formation formation = ((MissionBehavior)this).Mission.PlayerTeam.GetFormation((FormationClass)0);
		GameEntity spawnEntity = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_player_ship");
		_playerShip = CreateShip(_playerShipOrigin, ((MissionBehavior)this).Mission.PlayerTeam, formation, spawnEntity);
		if (!_startFromCheckPoint)
		{
			((MissionObject)_playerShip).OnDeploymentFinished();
		}
		_playerShip.SetAnchor(isAnchored: true, anchorInPlace: true);
		SpawnPlayerTeamAgents();
		_playerShip.ShipOrder.SetShipStopOrder();
		SetTargetPoint(_playerShip, new Vec3(0f, -20f, 0f, -1f));
	}

	private void SetTargetPoint(MissionShip playerShip, Vec3 localOffset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ShipTargetMissionObject firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<ShipTargetMissionObject>(((ScriptComponentBehavior)playerShip).GameEntity);
		if (firstScriptInFamilyDescending != null)
		{
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)firstScriptInFamilyDescending).GameEntity;
			WeakGameEntity gameEntity2 = ((ScriptComponentBehavior)firstScriptInFamilyDescending).GameEntity;
			((WeakGameEntity)(ref gameEntity)).SetLocalPosition(localOffset + ((WeakGameEntity)(ref gameEntity2)).GetLocalFrame().origin);
		}
	}

	private void DisableTargetShipObject(MissionShip ship)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ShipTargetMissionObject firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<ShipTargetMissionObject>(((ScriptComponentBehavior)ship).GameEntity);
		if (firstScriptInFamilyDescending != null)
		{
			((MissionObject)firstScriptInFamilyDescending).SetDisabled(false);
		}
	}

	private void SpawnPlayerTeamAgents()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		int num = _playerShip.ShipOrigin.MainDeckCrewCapacity - 2;
		_navalAgentsLogic.SetDesiredTroopCountOfShip(_playerShip, _playerShip.ShipOrigin.MainDeckCrewCapacity);
		int num2 = 0;
		foreach (FlattenedTroopRosterElement item in PartyBase.MainParty.MemberRoster.ToFlattenedRoster())
		{
			FlattenedTroopRosterElement current = item;
			if (!((BasicCharacterObject)((FlattenedTroopRosterElement)(ref current)).Troop).IsHero)
			{
				_navalAgentsLogic.AddReservedTroopToShip((IAgentOriginBase)new PartyAgentOrigin(PartyBase.MainParty, ((FlattenedTroopRosterElement)(ref current)).Troop, -1, default(UniqueTroopDescriptor), false, false), _playerShip);
				num2++;
			}
			if (num2 >= num)
			{
				break;
			}
		}
		_navalAgentsLogic.SpawnNextBatch((TeamSideEnum)0);
	}

	private void SpawnGangradir(string spawnId, bool noHorses)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		GameEntity val = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag(spawnId);
		if (val != (GameEntity)null)
		{
			SpawnGangradir(val.GlobalPosition, noHorses);
		}
		else
		{
			Debug.FailedAssert("Cant find entity.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "SpawnGangradir", 1861);
		}
	}

	private void SpawnGangradir(Vec3 position, bool noHorses)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Vec3 val = position;
		Vec3 val2 = Agent.Main.Position - val;
		Vec2 asVec = ((Vec3)(ref val2)).AsVec2;
		Vec2 val3 = ((Vec2)(ref asVec)).Normalized();
		Equipment val4 = NavalStorylineData.Gangradir.BattleEquipment.Clone(false);
		if (!noHorses)
		{
			ItemObject val5 = MBObjectManager.Instance.GetObject<ItemObject>("sturgia_horse_tournament");
			val4[(EquipmentIndex)10] = new EquipmentElement(val5, (ItemModifier)null, (ItemObject)null, false);
		}
		MissionEquipment val6 = new MissionEquipment(val4, (Banner)null);
		AgentBuildData val7 = new AgentBuildData((BasicCharacterObject)(object)NavalStorylineData.Gangradir.CharacterObject).TroopOrigin((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)NavalStorylineData.Gangradir.CharacterObject, -1, (Banner)null, default(UniqueTroopDescriptor))).Team(((MissionBehavior)this).Mission.PlayerTeam).InitialPosition(ref val)
			.InitialDirection(ref val3)
			.NoHorses(noHorses)
			.NoWeapons(true)
			.Equipment(val4)
			.MissionEquipment(val6);
		_gangradir = Mission.Current.SpawnAgent(val7, false);
		_gangradir.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
		_gangradir.GetComponent<CampaignAgentComponent>().AgentNavigator.AddBehaviorGroup<DailyBehaviorGroup>();
		_gangradir.GetComponent<AgentNavalComponent>()?.SetCanDrown(canDrown: false);
	}

	private void TriggerEnemyShip(MissionShip ship, MissionShip target = null)
	{
		ship.Formation.SetControlledByAI(true, false);
		ship.SetAnchor(isAnchored: false);
		ship.SetShipOrderActive(isOrderActive: true);
		ship.ShipOrder.SetShipEngageOrder(target);
		ship.ShipOrder.SetBoardingTargetShip(target);
		ToggleShipBallistas(ship, enabled: true);
		ship.ShipOrder.FormationJoinShip(ship.Formation);
	}

	private void InitializeEnemyShip(MissionShip ship)
	{
		ship.ShipOrder.FormationJoinShip(ship.Formation);
		ship.ShipOrder.SetShipStopOrder();
		ship.SetAnchor(isAnchored: true, anchorInPlace: true);
		ship.Formation.SetControlledByAI(false, false);
		ship.SetShipOrderActive(isOrderActive: true);
	}

	private void SpawnEnemyTargetShip()
	{
		Formation formation = ((MissionBehavior)this).Mission.PlayerEnemyTeam.GetFormation((FormationClass)0);
		GameEntity spawnEntity = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_enemy_ship_1");
		TargetShip = CreateShip(_enemyBurningShipOrigin, ((MissionBehavior)this).Mission.PlayerEnemyTeam, formation, spawnEntity);
		TargetShip.SetCanBeTakenOver(value: false);
	}

	private MissionShip SpawnEnemyChaserShip(GameEntity spawnPoint, IShipOrigin shipOrigin)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		Formation formation = ((IEnumerable<Formation>)((MissionBehavior)this).Mission.PlayerEnemyTeam.FormationsIncludingEmpty).First((Formation x) => x.CountOfUnits == 0 && x != TargetShip.Formation);
		MissionShip missionShip = CreateShip(shipOrigin, ((MissionBehavior)this).Mission.PlayerEnemyTeam, formation, spawnPoint);
		missionShip.SetCanBeTakenOver(value: false);
		int num = MBRandom.RandomInt(12, 14);
		int num2 = MBRandom.RandomInt(8, 10);
		CharacterObject val = MBObjectManager.Instance.GetObject<CharacterObject>("vlandian_swordsman");
		CharacterObject val2 = MBObjectManager.Instance.GetObject<CharacterObject>("vlandian_marine_t5");
		_navalAgentsLogic.SetDesiredTroopCountOfShip(missionShip, missionShip.ShipOrigin.MainDeckCrewCapacity);
		for (int num3 = 0; num3 < num; num3++)
		{
			_navalAgentsLogic.AddReservedTroopToShip((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)val, -1, (Banner)null, default(UniqueTroopDescriptor)), missionShip);
		}
		for (int num4 = 0; num4 < num2; num4++)
		{
			_navalAgentsLogic.AddReservedTroopToShip((IAgentOriginBase)new SimpleAgentOrigin((BasicCharacterObject)(object)val2, -1, (Banner)null, default(UniqueTroopDescriptor)), missionShip);
		}
		_navalAgentsLogic.SpawnNextBatch((TeamSideEnum)2);
		return missionShip;
	}

	private SpeakToTheSailorsQuest GetQuest()
	{
		foreach (QuestBase item in (List<QuestBase>)(object)Campaign.Current.QuestManager.Quests)
		{
			if (item is SpeakToTheSailorsQuest result)
			{
				return result;
			}
		}
		return null;
	}

	private MissionShip CreateShip(IShipOrigin ship, Team team, Formation formation, GameEntity spawnEntity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame shipFrame = spawnEntity.GetGlobalFrame();
		Scene scene = Mission.Current.Scene;
		Vec3 globalPosition = spawnEntity.GlobalPosition;
		float waterLevelAtPosition = scene.GetWaterLevelAtPosition(((Vec3)(ref globalPosition)).AsVec2, true, false);
		shipFrame.origin = new Vec3(spawnEntity.GlobalPosition.x, spawnEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
		return _navalShipsLogic.SpawnShip(ship, in shipFrame, team, formation, spawnAnchored: false, (FormationClass)8);
	}

	private Agent SpawnPlayerHorse()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = ((MissionBehavior)this).Mission.Scene.FindEntityWithTag("sp_horse").GetGlobalFrame();
		return SpawnHorse(globalFrame.origin, ((Vec3)(ref globalFrame.rotation.f)).AsVec2);
	}

	private Agent SpawnHorse(Vec3 position, Vec2 direction)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ItemObject val = MBObjectManager.Instance.GetObject<ItemObject>("sturgia_horse_tournament");
		ItemRosterElement val2 = default(ItemRosterElement);
		((ItemRosterElement)(ref val2))._002Ector(val, 1, (ItemModifier)null);
		ItemObject val3 = MBObjectManager.Instance.GetObject<ItemObject>("light_harness");
		ItemRosterElement val4 = default(ItemRosterElement);
		((ItemRosterElement)(ref val4))._002Ector(val3, 0, (ItemModifier)null);
		Mission current = Mission.Current;
		ItemRosterElement val5 = val2;
		ItemRosterElement val6 = val4;
		Vec2 val7 = ((Vec2)(ref direction)).Normalized();
		Agent obj = current.SpawnMonster(val5, val6, ref position, ref val7, -1);
		obj.SetTargetPosition(((Vec3)(ref position)).AsVec2);
		obj.SetMortalityState((MortalityState)1);
		return obj;
	}

	public static bool WillHitBoundingBox(Vec3 origin, Vec2 velocity2D, Vec3 boxMin, Vec3 boxMax)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (velocity2D == Vec2.Zero)
		{
			return false;
		}
		Vec3 val = ((Vec2)(ref velocity2D)).ToVec3(0f);
		Vec3 val2 = default(Vec3);
		((Vec3)(ref val2))._002Ector((((Vec3)(ref val)).X == 0f) ? float.PositiveInfinity : (1f / ((Vec3)(ref val)).X), (((Vec3)(ref val)).Y == 0f) ? float.PositiveInfinity : (1f / ((Vec3)(ref val)).Y), (((Vec3)(ref val)).Z == 0f) ? float.PositiveInfinity : (1f / ((Vec3)(ref val)).Z), -1f);
		float val3 = (((Vec3)(ref boxMin)).X - ((Vec3)(ref origin)).X) * ((Vec3)(ref val2)).X;
		float val4 = (((Vec3)(ref boxMax)).X - ((Vec3)(ref origin)).X) * ((Vec3)(ref val2)).X;
		float val5 = (((Vec3)(ref boxMin)).Y - ((Vec3)(ref origin)).Y) * ((Vec3)(ref val2)).Y;
		float val6 = (((Vec3)(ref boxMax)).Y - ((Vec3)(ref origin)).Y) * ((Vec3)(ref val2)).Y;
		float val7 = (((Vec3)(ref boxMin)).Z - ((Vec3)(ref origin)).Z) * ((Vec3)(ref val2)).Z;
		float val8 = (((Vec3)(ref boxMax)).Z - ((Vec3)(ref origin)).Z) * ((Vec3)(ref val2)).Z;
		float num = Math.Max(Math.Max(Math.Min(val3, val4), Math.Min(val5, val6)), Math.Min(val7, val8));
		float num2 = Math.Min(Math.Min(Math.Max(val3, val4), Math.Max(val5, val6)), Math.Max(val7, val8));
		if (num2 < 0f)
		{
			return false;
		}
		if (num > num2)
		{
			return false;
		}
		return Math.Max(0f, num) <= Math.Min(1f, num2);
	}

	private Vec2[] GetShipPhysicsBox(MissionShip ship)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		float num = (ship.Physics.PhysicsBoundingBoxWithChildren.max.x - ship.Physics.PhysicsBoundingBoxWithChildren.min.x) / 2f - 6f;
		float num2 = (ship.Physics.PhysicsBoundingBoxWithChildren.max.y - ship.Physics.PhysicsBoundingBoxWithChildren.min.y) / 2f - 2f;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		Vec2 asVec2 = ((Vec3)(ref globalFrame.rotation.s)).AsVec2;
		gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		Vec2 asVec3 = ((Vec3)(ref globalPosition)).AsVec2;
		Vec2 val = asVec2 * num;
		Vec2 val2 = asVec * num2;
		Vec2 val3 = asVec3 - val - val2;
		Vec2 val4 = asVec3 + val - val2;
		Vec2 val5 = asVec3 + val + val2;
		Vec2 val6 = asVec3 - val + val2;
		return (Vec2[])(object)new Vec2[4] { val3, val4, val5, val6 };
	}

	private bool DoesShipCollideWithProjectile(MissionShip ship, BurningProjectile projectile)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (projectile.Initialized)
		{
			Vec3 globalPosition = projectile.GameEntity.GlobalPosition;
			return DoesShipCollideWithSphere(ship, ((Vec3)(ref globalPosition)).AsVec2, 1f);
		}
		return false;
	}

	private bool DoesShipCollideWithSphere(MissionShip ship, Vec2 origin, float radius)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return PlaneIntersectsCircle(GetShipPhysicsBox(ship), origin, radius);
	}

	private bool PlaneIntersectsCircle(Vec2[] corners, Vec2 circleOrigin, float radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (IsPointInPolygon(circleOrigin, corners))
		{
			return true;
		}
		float num = radius * radius;
		for (int i = 0; i < corners.Length; i++)
		{
			Vec2 val = corners[i];
			Vec2 val2 = corners[(i + 1) % corners.Length];
			float num2 = (((Vec2)(ref val2)).X - ((Vec2)(ref val)).X) * (((Vec2)(ref val2)).X - ((Vec2)(ref val)).X) + (((Vec2)(ref val2)).Y - ((Vec2)(ref val)).Y) * (((Vec2)(ref val2)).Y - ((Vec2)(ref val)).Y);
			float num3 = Math.Max(0f, Math.Min(1f, ((((Vec2)(ref circleOrigin)).X - ((Vec2)(ref val)).X) * (((Vec2)(ref val2)).X - ((Vec2)(ref val)).X) + (((Vec2)(ref circleOrigin)).Y - ((Vec2)(ref val)).Y) * (((Vec2)(ref val2)).Y - ((Vec2)(ref val)).Y)) / num2));
			float num4 = ((Vec2)(ref val)).X + num3 * (((Vec2)(ref val2)).X - ((Vec2)(ref val)).X);
			float num5 = ((Vec2)(ref val)).Y + num3 * (((Vec2)(ref val2)).Y - ((Vec2)(ref val)).Y);
			if ((((Vec2)(ref circleOrigin)).X - num4) * (((Vec2)(ref circleOrigin)).X - num4) + (((Vec2)(ref circleOrigin)).Y - num5) * (((Vec2)(ref circleOrigin)).Y - num5) <= num)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsPointInPolygon(Vec2 point, Vec2[] polygonCorners)
	{
		bool flag = false;
		int num = polygonCorners.Length;
		int num2 = 0;
		int num3 = num - 1;
		while (num2 < num)
		{
			if (((Vec2)(ref polygonCorners[num2])).Y > ((Vec2)(ref point)).Y != ((Vec2)(ref polygonCorners[num3])).Y > ((Vec2)(ref point)).Y && ((Vec2)(ref point)).X < (((Vec2)(ref polygonCorners[num3])).X - ((Vec2)(ref polygonCorners[num2])).X) * (((Vec2)(ref point)).Y - ((Vec2)(ref polygonCorners[num2])).Y) / (((Vec2)(ref polygonCorners[num3])).Y - ((Vec2)(ref polygonCorners[num2])).Y) + ((Vec2)(ref polygonCorners[num2])).X)
			{
				flag = !flag;
			}
			num3 = num2++;
		}
		return flag;
	}

	static BlockedEstuaryMissionController()
	{
		MBList<string> obj = new MBList<string>();
		((List<string>)(object)obj).Add("vlandian_spearman");
		((List<string>)(object)obj).Add("vlandian_billman");
		((List<string>)(object)obj).Add("vlandian_marine_t4");
		_enemyAgentCharacterIds = obj;
	}
}

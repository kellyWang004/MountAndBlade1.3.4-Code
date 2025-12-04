using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Objects;

public class ShipFloatsamManager : ScriptComponentBehavior
{
	private enum DebrisType
	{
		Generic,
		Scrape
	}

	private enum DecalType
	{
		Collision,
		Scrape
	}

	private struct ImpulseRecord
	{
		internal Vec3 AveragePosition;

		internal Vec3 AverageNormal;

		internal float TotalImpulse;

		internal Vec3 Speed;

		internal DebrisType DebrisType;

		internal float InitialSpeedMultiplier;

		internal Vec3 ShipLocalPosition;

		internal Vec3 ShipLocalNormal;

		internal DecalType DecalType;
	}

	private struct ShieldBreakRecord
	{
		internal Vec3 LinearVelocity;

		internal Texture BannerTexture;

		internal MatrixFrame ShipLocalSpawnFrame;

		internal string PrefabName;
	}

	private class ScrapeRecord
	{
		internal ParticleSystem Particle;

		internal float AccumulatedDistance;

		internal Vec3 PreviousPosition;
	}

	private static readonly string[] GenericPrefabNames = new string[3] { "floatable_debris_broken_barrel", "floatable_debris_door", "floatable_debris_barrel_a" };

	private static readonly string[] ScrapeDebrisPrefabNames = new string[7] { "floatable_debris_plank_b", "floatable_debris_plank_e", "floatable_debris_plank_f", "floatable_debris_plank_g", "floatable_debris_plank_h", "floatable_debris_plank_j", "floatable_debris_plank_k" };

	private static readonly string[] CollisionDecalPrefabNames = new string[3] { "decal_ship_damaged_a", "decal_ship_damaged_b", "decal_ship_damaged_c" };

	private static readonly string[] ScrapeDecalPrefabNames = new string[3] { "decal_ship_damage_02", "decal_ship_damage_03", "decal_ship_damage_04" };

	private const string RudderPrefabName = "floatable_debris_rudder";

	private const string ShieldPrefabName = "floatable_debris_";

	private const string OarPrefabName = "floatable_debris_oar_a";

	private const string MastPrefabName = "floatable_debris_mast";

	private const string BodyMeshTag = "body_mesh";

	private const string BannerTag = "banner_with_faction_color";

	private const int MaxNumberOfPendingImpulseRecords = 10;

	private const float DebrisBreakImpulseThreshold = 150000f;

	private const int MaxDecalCount = 30;

	private Dictionary<WeakGameEntity, ScrapeRecord> _scrapeRecords = new Dictionary<WeakGameEntity, ScrapeRecord>();

	private GameEntity _identityFrameParticleParent;

	private int _scrapeParticleIndex = -1;

	private int _collisionHitParticleIndex = -1;

	private int _midCollisionHitParticleIndex = -1;

	private int _bigCollisionHitParticleIndex = -1;

	private readonly MBFastRandom _randomGenerator = new MBFastRandom();

	private ImpulseRecord[] _impulseRecordsToProcess = new ImpulseRecord[10];

	private ShieldBreakRecord[] _shieldBreakRecords = new ShieldBreakRecord[10];

	private uint _shipColor;

	private int _numberOfPendingImpulseRecords;

	private int _numberOfPendingShieldBreakRecords;

	private uint _shipDecalColor;

	private bool _navalPhysicsSunk;

	private List<GameEntity> _collisionDecals;

	private string _shieldName;

	private BoundingBox _shipBodyBB;

	private NavalFloatsamLogic _floatsamMissionLogic;

	private GameEntity _bodyEntity;

	private MissionShip _ownMissionShipCached;

	private bool _floatsamSystemEnabled;

	internal ShipFloatsamManager()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Color white = Colors.White;
		_shipColor = ((Color)(ref white)).ToUnsignedInteger();
		white = Colors.White;
		_shipDecalColor = ((Color)(ref white)).ToUnsignedInteger();
		_collisionDecals = new List<GameEntity>();
		_shieldName = "";
		((ScriptComponentBehavior)this)._002Ector();
	}

	protected override void OnInit()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		_identityFrameParticleParent = GameEntity.CreateEmpty(((WeakGameEntity)(ref gameEntity)).Scene, false, false, false);
		GameEntity identityFrameParticleParent = _identityFrameParticleParent;
		identityFrameParticleParent.EntityFlags = (EntityFlags)(identityFrameParticleParent.EntityFlags | 0x20000);
		_scrapeParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_ship_scrape_emit_on_move");
		_collisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_ship_collision");
		_midCollisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_hit_mid");
		_bigCollisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_hit_large");
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		WeakGameEntity firstChildEntityWithTagRecursive = ((WeakGameEntity)(ref gameEntity)).GetFirstChildEntityWithTagRecursive("body_mesh");
		if (firstChildEntityWithTagRecursive != (GameEntity)null)
		{
			_shipBodyBB = ((WeakGameEntity)(ref firstChildEntityWithTagRecursive)).GetLocalBoundingBox();
			_bodyEntity = GameEntity.CreateFromWeakEntity(firstChildEntityWithTagRecursive);
		}
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		ColorAssigner firstScriptOfType = ((WeakGameEntity)(ref gameEntity)).GetFirstScriptOfType<ColorAssigner>();
		if (firstScriptOfType != null)
		{
			Color val = firstScriptOfType.ShipColor;
			_shipColor = ((Color)(ref val)).ToUnsignedInteger();
			val = firstScriptOfType.RamDebrisColor;
			_shipDecalColor = ((Color)(ref val)).ToUnsignedInteger();
		}
		_floatsamMissionLogic = Mission.Current.GetMissionBehavior<NavalFloatsamLogic>();
		List<WeakGameEntity> list = new List<WeakGameEntity>();
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref gameEntity)).GetChildrenRecursive(ref list);
		foreach (WeakGameEntity item in list)
		{
			WeakGameEntity current = item;
			ShipShieldComponent firstScriptOfType2 = ((WeakGameEntity)(ref current)).GetFirstScriptOfType<ShipShieldComponent>();
			if (firstScriptOfType2 != null)
			{
				((DestructableComponent)firstScriptOfType2).OnDestroyed += new OnHitTakenAndDestroyedDelegate(OnShieldDestroyed);
				_shieldName = ((WeakGameEntity)(ref current)).Name;
			}
		}
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		_ownMissionShipCached = ((WeakGameEntity)(ref gameEntity)).GetFirstScriptOfType<MissionShip>();
		if (_ownMissionShipCached != null)
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.ShipHitEvent += OnShipHit;
			}
		}
	}

	protected override void OnTick(float dt)
	{
		if (_floatsamSystemEnabled)
		{
			CheckSinking();
			ProcessImpulseEffects();
			ProcessShieldBreakRecords();
		}
	}

	public override TickRequirement GetTickRequirement()
	{
		return (TickRequirement)2;
	}

	protected override void OnPhysicsCollision(ref PhysicsContact contact, WeakGameEntity entity0, WeakGameEntity entity1, bool isFirstShape)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Invalid comparison between Unknown and I4
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		if (!((WeakGameEntity)(ref entity1)).HasScriptComponent(MissionShip.MissionShipScriptNameHash) || !_floatsamSystemEnabled)
		{
			return;
		}
		MatrixFrame bodyWorldTransform = ((WeakGameEntity)(ref entity0)).GetBodyWorldTransform();
		bool flag = true;
		Vec3 val = Vec3.Zero;
		Vec3 val2 = Vec3.Zero;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < contact.NumberOfContactPairs; i++)
		{
			PhysicsContactPair val3 = ((PhysicsContact)(ref contact))[i];
			for (int j = 0; j < val3.NumberOfContacts; j++)
			{
				PhysicsContactInfo val4 = ((PhysicsContactPair)(ref val3))[j];
				val += val4.Position;
				num += ((Vec3)(ref val4.Impulse)).Length;
				val2 += val4.Normal;
				_ = Colors.White;
				if ((int)val3.ContactEventType == 0)
				{
					flag = false;
				}
				else if ((int)val3.ContactEventType == 1)
				{
					flag = false;
				}
				num2 += 1f;
			}
		}
		if (num2 > 0f)
		{
			val /= num2;
			val2 /= num2;
			((Vec3)(ref val2)).Normalize();
			if (isFirstShape)
			{
				val2 *= -1f;
			}
		}
		WeakGameEntity gameEntity;
		if (_scrapeRecords.TryGetValue(entity1, out var value))
		{
			if (flag || num2 == 0f)
			{
				gameEntity = ((ScriptComponentBehavior)this).GameEntity;
				((WeakGameEntity)(ref gameEntity)).RemoveComponent((GameEntityComponent)(object)value.Particle);
				_scrapeRecords.Remove(entity1);
				return;
			}
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.u = Vec3.Up;
			identity.rotation.s = val2;
			identity.rotation.f = -((Vec3)(ref identity.rotation.s)).CrossProductWithUp();
			identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
			identity.origin = val;
			value.AccumulatedDistance += ((Vec3)(ref value.PreviousPosition)).Distance(val);
			value.PreviousPosition = val;
			value.Particle.SetLocalFrame(ref identity);
			if (!(value.AccumulatedDistance > 2.5f))
			{
				return;
			}
			value.AccumulatedDistance = 0f;
			if (_numberOfPendingImpulseRecords < 10)
			{
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AveragePosition = val;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AverageNormal = val2;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].TotalImpulse = 150000f;
				Vec3 speed = Vec3.Zero;
				if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity0))
				{
					speed = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(((ScriptComponentBehavior)this).GameEntity, val);
				}
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].Speed = speed;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DebrisType = DebrisType.Scrape;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DecalType = DecalType.Scrape;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].InitialSpeedMultiplier = 0.25f;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalPosition = ((MatrixFrame)(ref bodyWorldTransform)).TransformToLocal(ref val);
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalNormal = ((Mat3)(ref bodyWorldTransform.rotation)).TransformToLocal(ref val2);
				_numberOfPendingImpulseRecords++;
			}
		}
		else if (num2 > 0f)
		{
			ScrapeRecord scrapeRecord = new ScrapeRecord();
			MatrixFrame identity2 = MatrixFrame.Identity;
			identity2.rotation.u = Vec3.Up;
			identity2.rotation.s = val2;
			identity2.rotation.f = -((Vec3)(ref identity2.rotation.s)).CrossProductWithUp();
			identity2.rotation.s = Vec3.CrossProduct(identity2.rotation.f, identity2.rotation.u);
			identity2.origin = val;
			scrapeRecord.Particle = ParticleSystem.CreateParticleSystemAttachedToEntity(_scrapeParticleIndex, _identityFrameParticleParent, ref identity2);
			scrapeRecord.PreviousPosition = val;
			_scrapeRecords.Add(entity1, scrapeRecord);
			if (num > 15000f)
			{
				gameEntity = ((ScriptComponentBehavior)this).GameEntity;
				((WeakGameEntity)(ref gameEntity)).Scene.CreateBurstParticle(_collisionHitParticleIndex, identity2);
			}
			Vec3 val5 = Vec3.Zero;
			if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity0))
			{
				val5 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(((ScriptComponentBehavior)this).GameEntity, val);
			}
			Vec3 val6 = Vec3.Zero;
			if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity1))
			{
				val6 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(((ScriptComponentBehavior)this).GameEntity, val);
			}
			if (_numberOfPendingImpulseRecords < 10)
			{
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AveragePosition = val;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AverageNormal = val2;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].TotalImpulse = num;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].Speed = val5 - val6;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DebrisType = DebrisType.Scrape;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DecalType = DecalType.Collision;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].InitialSpeedMultiplier = 1f;
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalPosition = ((MatrixFrame)(ref bodyWorldTransform)).TransformToLocal(ref val);
				_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalNormal = ((Mat3)(ref bodyWorldTransform.rotation)).TransformToLocal(ref val2);
				_numberOfPendingImpulseRecords++;
			}
		}
	}

	private void ProcessImpulseEffects()
	{
		while (_numberOfPendingImpulseRecords > 0)
		{
			int num = _numberOfPendingImpulseRecords - 1;
			ProcessImpactEffect(_impulseRecordsToProcess[num]);
			_numberOfPendingImpulseRecords--;
		}
	}

	private void ProcessShieldBreakRecords()
	{
		while (_numberOfPendingShieldBreakRecords > 0)
		{
			int num = _numberOfPendingShieldBreakRecords - 1;
			SpawnBrokenShield(_shieldBreakRecords[num]);
			_numberOfPendingShieldBreakRecords--;
		}
	}

	private void SpawnBrokenShield(ShieldBreakRecord record)
	{
	}

	private void ProcessImpactEffect(ImpulseRecord record)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		if (_collisionDecals.Count >= 30)
		{
			return;
		}
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		Vec3 origin = record.ShipLocalPosition;
		Vec3 u = record.ShipLocalNormal;
		Vec3 val;
		if (_bodyEntity != (GameEntity)null)
		{
			float num = 2.5f;
			val = ((Mat3)(ref globalFrame.rotation)).TransformToParent(ref record.ShipLocalNormal);
			Vec3 val2 = -((Vec3)(ref val)).NormalizedCopy();
			Vec3 val3 = ((MatrixFrame)(ref globalFrame)).TransformToParent(ref record.ShipLocalPosition) - val2 * num;
			Vec3 zero = Vec3.Zero;
			float num2 = 0f;
			if (_bodyEntity.RayHitEntityWithNormal(val3, val2, num, ref zero, ref num2))
			{
				val = val3 + val2 * num2;
				origin = ((MatrixFrame)(ref globalFrame)).TransformToLocalNonOrthogonal(ref val);
				val = ((Mat3)(ref globalFrame.rotation)).TransformToLocal(ref zero);
				u = ((Vec3)(ref val)).NormalizedCopy();
			}
		}
		MatrixFrame identity = MatrixFrame.Identity;
		identity.origin = origin;
		identity.rotation.u = u;
		identity.rotation.f = Vec3.Up;
		identity.rotation.s = Vec3.CrossProduct(identity.rotation.u, identity.rotation.s);
		((Vec3)(ref identity.rotation.f)).Normalize();
		identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
		if (record.DecalType == DecalType.Scrape)
		{
			float num3 = _randomGenerator.NextFloatRanged(1.75f, 2.75f);
			float num4 = _randomGenerator.NextFloatRanged(1.25f, 1.75f);
			ref Mat3 rotation = ref identity.rotation;
			val = new Vec3(num3, num4, 0.2f, -1f);
			((Mat3)(ref rotation)).ApplyScaleLocal(ref val);
		}
		else if (record.DecalType == DecalType.Collision)
		{
			float num5 = _randomGenerator.NextFloatRanged(1.55f, 2.55f);
			ref Mat3 rotation2 = ref identity.rotation;
			val = new Vec3(num5, 1f, 0.2f, -1f);
			((Mat3)(ref rotation2)).ApplyScaleLocal(ref val);
		}
		string text = "";
		if (record.DecalType == DecalType.Collision)
		{
			text = GetRandomCollisionDecalPrefab();
		}
		else if (record.DecalType == DecalType.Scrape)
		{
			text = GetRandomScrapeDecalPrefab();
		}
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		GameEntity val4 = GameEntity.Instantiate(((WeakGameEntity)(ref gameEntity)).Scene, text, MatrixFrame.Identity, true, "");
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref gameEntity)).AddChild(val4.WeakEntity, false);
		val4.SetFrame(ref identity, true);
		val4.SetFactorColor(_shipDecalColor);
		_collisionDecals.Add(val4);
	}

	private string GetRandomDebrisPrefab(DebrisType type)
	{
		switch (type)
		{
		case DebrisType.Generic:
		{
			int num2 = _randomGenerator.Next(GenericPrefabNames.Length);
			return GenericPrefabNames[num2];
		}
		case DebrisType.Scrape:
		{
			int num = _randomGenerator.Next(ScrapeDebrisPrefabNames.Length);
			return ScrapeDebrisPrefabNames[num];
		}
		default:
			return "";
		}
	}

	private string GetRandomCollisionDecalPrefab()
	{
		int num = _randomGenerator.Next(CollisionDecalPrefabNames.Length);
		return CollisionDecalPrefabNames[num];
	}

	private string GetRandomScrapeDecalPrefab()
	{
		int num = _randomGenerator.Next(ScrapeDecalPrefabNames.Length);
		return ScrapeDecalPrefabNames[num];
	}

	private void SetRandomAngularVelocityToEntity(GameEntity entity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.8f;
		GameEntityPhysicsExtensions.SetAngularVelocity(entity, new Vec3(_randomGenerator.NextFloatRanged(0f - num, num), _randomGenerator.NextFloatRanged(0f - num, num), _randomGenerator.NextFloatRanged(0f - num, num), -1f));
	}

	private void CheckSinking()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		if (_navalPhysicsSunk)
		{
			return;
		}
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		float num = (_shipBodyBB.max.z - _shipBodyBB.min.z) * 0.75f;
		float num2 = globalPosition.z + num;
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		if (!(num2 < ((WeakGameEntity)(ref gameEntity)).GetWaterLevelAtPosition(((Vec3)(ref globalPosition)).AsVec2, true, false)))
		{
			return;
		}
		_navalPhysicsSunk = true;
		Vec3 min = _shipBodyBB.min;
		Vec3 max = _shipBodyBB.max;
		max.z = min.z;
		Vec3 val = max - min;
		float num3 = 1.5f;
		float num4 = MathF.Max(Vec2.DotProduct(((Vec3)(ref val)).AsVec2, ((Vec3)(ref val)).AsVec2) / 1000f, 1f);
		int num5 = (int)((float)_randomGenerator.Next(7, 10) * num4);
		for (int i = 0; i < num5; i++)
		{
			gameEntity = ((ScriptComponentBehavior)this).GameEntity;
			GameEntity val2 = GameEntity.Instantiate(((WeakGameEntity)(ref gameEntity)).Scene, "floatable_debris_oar_a", true, true, "");
			if (val2 != (GameEntity)null)
			{
				Vec3 val3 = min + new Vec3(val.x * _randomGenerator.NextFloat(), val.y * _randomGenerator.NextFloat(), 0f, -1f);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = globalPosition + val3;
				gameEntity = ((ScriptComponentBehavior)this).GameEntity;
				float waterLevelAtPosition = ((WeakGameEntity)(ref gameEntity)).GetWaterLevelAtPosition(((Vec3)(ref identity.origin)).AsVec2, true, false);
				identity.origin.z = waterLevelAtPosition - num3 * _randomGenerator.NextFloatRanged(1f, 4.5f);
				val2.SetFrame(ref identity, true);
				val2.SetFactorColor(_shipColor);
				SetRandomAngularVelocityToEntity(val2);
			}
		}
		Vec3 val4 = min + new Vec3(val.x * _randomGenerator.NextFloat(), val.y * _randomGenerator.NextFloat(), 0f, -1f);
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		GameEntity val5 = GameEntity.Instantiate(((WeakGameEntity)(ref gameEntity)).Scene, "floatable_debris_rudder", true, true, "");
		MatrixFrame identity2 = MatrixFrame.Identity;
		identity2.origin = globalPosition + val4;
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		float waterLevelAtPosition2 = ((WeakGameEntity)(ref gameEntity)).GetWaterLevelAtPosition(((Vec3)(ref identity2.origin)).AsVec2, true, false);
		identity2.origin.z = waterLevelAtPosition2 - num3 * _randomGenerator.NextFloatRanged(1f, 4.5f);
		val5.SetFrame(ref identity2, true);
		val5.SetFactorColor(_shipColor);
		SetRandomAngularVelocityToEntity(val5);
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		GameEntity val6 = GameEntity.Instantiate(((WeakGameEntity)(ref gameEntity)).Scene, "floatable_debris_mast", true, true, "");
		if (val6 != (GameEntity)null)
		{
			Vec3 val7 = min + new Vec3(val.x * _randomGenerator.NextFloat(), val.y * _randomGenerator.NextFloat(), 0f, -1f);
			MatrixFrame identity3 = MatrixFrame.Identity;
			identity3.origin = globalPosition + val7;
			gameEntity = ((ScriptComponentBehavior)this).GameEntity;
			float waterLevelAtPosition3 = ((WeakGameEntity)(ref gameEntity)).GetWaterLevelAtPosition(((Vec3)(ref identity3.origin)).AsVec2, true, false);
			identity3.origin.z = waterLevelAtPosition3 - num3 * _randomGenerator.NextFloatRanged(3.5f, 5.5f);
			val6.SetFrame(ref identity3, true);
			val6.SetFactorColor(_shipColor);
			SetRandomAngularVelocityToEntity(val6);
		}
		int num6 = (int)((float)_randomGenerator.Next(10, 16) * num4);
		for (int j = 0; j < num6; j++)
		{
			Vec3 val8 = min + new Vec3(val.x * _randomGenerator.NextFloat(), val.y * _randomGenerator.NextFloat(), 0f, -1f);
			gameEntity = ((ScriptComponentBehavior)this).GameEntity;
			GameEntity val9 = GameEntity.Instantiate(((WeakGameEntity)(ref gameEntity)).Scene, GetRandomDebrisPrefab(DebrisType.Generic), true, true, "");
			MatrixFrame identity4 = MatrixFrame.Identity;
			identity4.origin = globalPosition + val8;
			gameEntity = ((ScriptComponentBehavior)this).GameEntity;
			float waterLevelAtPosition4 = ((WeakGameEntity)(ref gameEntity)).GetWaterLevelAtPosition(((Vec3)(ref identity4.origin)).AsVec2, true, false);
			identity4.origin.z = waterLevelAtPosition4 - num3 * _randomGenerator.NextFloatRanged(1f, 4.5f);
			val9.SetFrame(ref identity4, true);
			val9.SetFactorColor(_shipColor);
			SetRandomAngularVelocityToEntity(val9);
		}
	}

	private void OnShieldDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!_floatsamSystemEnabled || _numberOfPendingShieldBreakRecords >= 10)
		{
			return;
		}
		Texture bannerTexture = null;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)target).GameEntity;
		GameEntityComponent componentAtIndex = ((WeakGameEntity)(ref gameEntity)).GetComponentAtIndex(0, (ComponentType)0);
		MetaMesh val = (MetaMesh)(object)((componentAtIndex is MetaMesh) ? componentAtIndex : null);
		if ((NativeObject)(object)val != (NativeObject)null && val.MeshCount > 0)
		{
			bannerTexture = val.GetMeshAtIndex(0).GetMaterial().GetTexture((MBTextureType)1);
		}
		string text = "floatable_debris_";
		text += _shieldName;
		if (_randomGenerator.NextFloat() > 0.15f)
		{
			switch (_randomGenerator.Next(0, 3))
			{
			case 0:
				text += "_broken_a";
				break;
			case 1:
				text += "_broken_b";
				break;
			case 2:
				text += "_broken_c";
				break;
			}
		}
		gameEntity = ((ScriptComponentBehavior)target).GameEntity;
		Vec3 linearVelocity = GameEntityPhysicsExtensions.GetLinearVelocity(((WeakGameEntity)(ref gameEntity)).Root);
		linearVelocity += Vec3.Up * 1.5f;
		ref ShieldBreakRecord reference = ref _shieldBreakRecords[_numberOfPendingShieldBreakRecords];
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		gameEntity = ((ScriptComponentBehavior)target).GameEntity;
		MatrixFrame globalFrame2 = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		reference.ShipLocalSpawnFrame = ((MatrixFrame)(ref globalFrame)).TransformToLocal(ref globalFrame2);
		_shieldBreakRecords[_numberOfPendingShieldBreakRecords].BannerTexture = bannerTexture;
		_shieldBreakRecords[_numberOfPendingShieldBreakRecords].LinearVelocity = linearVelocity;
		_shieldBreakRecords[_numberOfPendingShieldBreakRecords].PrefabName = text;
		_numberOfPendingShieldBreakRecords++;
	}

	private void OnShipHit(MissionShip ship, Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, MissionWeapon weapon, int missileIndex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if (!_floatsamSystemEnabled || ship != _ownMissionShipCached || ((MissionWeapon)(ref weapon)).CurrentUsageItem == null)
		{
			return;
		}
		WeaponClass weaponClass = ((MissionWeapon)(ref weapon)).CurrentUsageItem.WeaponClass;
		if (((int)weaponClass != 20 && (int)weaponClass != 19 && (int)weaponClass != 26 && (int)weaponClass != 27) || _numberOfPendingImpulseRecords >= 10)
		{
			return;
		}
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		MatrixFrame globalFrame = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
		Vec3 val = -impactDirection;
		Vec3 val2 = val;
		if (_bodyEntity != (GameEntity)null)
		{
			Vec3 zero = Vec3.Zero;
			float num = 0f;
			if (_bodyEntity.RayHitEntityWithNormal(impactPosition - impactDirection, ((Vec3)(ref impactDirection)).NormalizedCopy(), 2f, ref zero, ref num))
			{
				val = zero;
				val2 = zero;
				((Vec3)(ref val)).Normalize();
			}
		}
		bool num2 = ((MBObjectBase)((MissionWeapon)(ref weapon)).Item).StringId.Contains("grape");
		int num3 = -1;
		num3 = ((!num2) ? _midCollisionHitParticleIndex : _collisionHitParticleIndex);
		MatrixFrame identity = MatrixFrame.Identity;
		identity.rotation.u = Vec3.Up;
		identity.rotation.s = val;
		identity.rotation.f = -((Vec3)(ref globalFrame.rotation.s)).CrossProductWithUp();
		identity.rotation.s = Vec3.CrossProduct(globalFrame.rotation.f, globalFrame.rotation.u);
		identity.origin = impactPosition;
		gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		((WeakGameEntity)(ref gameEntity)).Scene.CreateBurstParticle(num3, identity);
		Vec3 speed = Vec3.Zero;
		if (GameEntityPhysicsExtensions.HasDynamicRigidBody(((ScriptComponentBehavior)this).GameEntity))
		{
			speed = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(((ScriptComponentBehavior)this).GameEntity, impactPosition);
		}
		float num4 = (float)damage / 150f;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AveragePosition = impactPosition;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].AverageNormal = val;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].TotalImpulse = 150000f * num4;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].Speed = speed;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DebrisType = DebrisType.Scrape;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].DecalType = DecalType.Collision;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].InitialSpeedMultiplier = 1f;
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalPosition = ((MatrixFrame)(ref globalFrame)).TransformToLocal(ref impactPosition);
		_impulseRecordsToProcess[_numberOfPendingImpulseRecords].ShipLocalNormal = ((Mat3)(ref globalFrame.rotation)).TransformToLocal(ref val2);
		_numberOfPendingImpulseRecords++;
	}

	public void EnableFloatsamSystem()
	{
		_floatsamSystemEnabled = true;
	}
}

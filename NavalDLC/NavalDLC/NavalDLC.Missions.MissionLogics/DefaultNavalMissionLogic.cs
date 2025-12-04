using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics;

public class DefaultNavalMissionLogic : MissionLogic, IAgentStateDecider, IMissionBehavior
{
	private NavalShipsLogic _shipsLogic;

	private NavalMissionDeploymentPlanningLogic _deploymentPlan;

	private readonly MBList<IShipOrigin> _playerTeamShips;

	private readonly MBList<IShipOrigin> _playerAllyTeamShips;

	private readonly MBList<IShipOrigin> _enemyTeamShips;

	private readonly NavalShipDeploymentLimit _playerTeamShipDeploymentLimit;

	private readonly NavalShipDeploymentLimit _playerAllyTeamShipDeploymentLimit;

	private readonly NavalShipDeploymentLimit _enemyTeamShipDeploymentLimit;

	public MBReadOnlyList<IShipOrigin> PlayerShips => (MBReadOnlyList<IShipOrigin>)(object)_playerTeamShips;

	public MBReadOnlyList<IShipOrigin> PlayerAllyShips => (MBReadOnlyList<IShipOrigin>)(object)_playerAllyTeamShips;

	public MBReadOnlyList<IShipOrigin> PlayerEnemyShips => (MBReadOnlyList<IShipOrigin>)(object)_enemyTeamShips;

	protected override void OnEndMission()
	{
		((MissionBehavior)this).OnEndMission();
		Mission.Current.OnMissileRemovedEvent -= ((MissionBehavior)this).OnMissileRemoved;
	}

	public override void OnMissionStateFinalized()
	{
		SailWindProfile.FinalizeProfile();
	}

	public override void OnDeploymentFinished()
	{
		foreach (MissionShip item in (List<MissionShip>)(object)_shipsLogic.AllShips)
		{
			item.SetAnchor(isAnchored: false);
			if (!item.IsPlayerShip)
			{
				item.SetController(ShipControllerType.AI);
			}
		}
		_shipsLogic.SetDeploymentMode(value: false);
	}

	public override void OnMissionTick(float dt)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).OnMissionTick(dt);
		foreach (Missile item in (List<Missile>)(object)((MissionBehavior)this).Mission.MissilesList)
		{
			MissionWeapon missileWeapon = item.Weapon;
			WeaponComponentData currentUsageItem = ((MissionWeapon)(ref missileWeapon)).CurrentUsageItem;
			if (currentUsageItem == null || !Extensions.HasAnyFlag<WeaponFlags>(currentUsageItem.WeaponFlags, (WeaponFlags)32768))
			{
				continue;
			}
			Vec3 missileOldPosition = ((MBMissile)item).GetOldPosition();
			Vec3 missilePosition = ((MBMissile)item).GetPosition();
			GameEntity val = null;
			foreach (MissionShip item2 in (List<MissionShip>)(object)_shipsLogic.AllShips)
			{
				Agent shooterAgent = item.ShooterAgent;
				GameEntity alreadyHitEntityToIgnore = item.AlreadyHitEntityToIgnore;
				int index = ((MBMissile)item).Index;
				missileWeapon = item.Weapon;
				val = item2.CheckHitSails(shooterAgent, alreadyHitEntityToIgnore, index, in missileOldPosition, in missilePosition, in missileWeapon);
			}
			if (val != (GameEntity)null)
			{
				item.PassThroughEntity(val);
			}
		}
	}

	internal void DeployBattleSide(BattleSideEnum battleSide)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		MakeDeploymentPlansForSide(battleSide);
		foreach (Team item in Mission.GetTeamsOfSide(battleSide))
		{
			foreach (Formation item2 in (List<Formation>)(object)item.FormationsIncludingEmpty)
			{
				FormationClass formationIndex = item2.FormationIndex;
				IFormationDeploymentPlan formationPlan = ((MissionDeploymentPlanningLogic)_deploymentPlan).GetFormationPlan(item, formationIndex, false);
				if (formationPlan.HasFrame())
				{
					MatrixFrame spawnFrame = formationPlan.GetFrame();
					_shipsLogic.SpawnShip(item2, in spawnFrame).SetController(ShipControllerType.None);
				}
			}
		}
	}

	public DefaultNavalMissionLogic(MBList<IShipOrigin> playerShips, MBList<IShipOrigin> playerAllyShips, MBList<IShipOrigin> enemyShips, NavalShipDeploymentLimit playerTeamShipDeploymentLimit, NavalShipDeploymentLimit playerAllyTeamShipDeploymentLimit, NavalShipDeploymentLimit enemyTeamShipDeploymentLimit)
	{
		_playerTeamShips = playerShips;
		_playerAllyTeamShips = playerAllyShips;
		_enemyTeamShips = enemyShips;
		_playerTeamShipDeploymentLimit = playerTeamShipDeploymentLimit;
		_playerAllyTeamShipDeploymentLimit = playerAllyTeamShipDeploymentLimit;
		_enemyTeamShipDeploymentLimit = enemyTeamShipDeploymentLimit;
	}

	public override void AfterStart()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		((MissionBehavior)this).AfterStart();
		_deploymentPlan = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalMissionDeploymentPlanningLogic>();
		UpdateSceneWindDirection();
		if ((int)((MissionBehavior)this).Mission.TerrainType != 11)
		{
			UpdateSceneWaterStrength();
		}
		InitializeShipAssignments();
	}

	public override void OnBehaviorInitialize()
	{
		if (!SailWindProfile.IsSailWindProfileInitialized)
		{
			SailWindProfile.InitializeProfile();
		}
		_shipsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalShipsLogic>();
		_shipsLogic.SetDeploymentMode(value: true);
		_shipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)0, _playerTeamShipDeploymentLimit);
		_shipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)1, _playerAllyTeamShipDeploymentLimit);
		_shipsLogic.SetTeamShipDeploymentLimit((TeamSideEnum)2, _enemyTeamShipDeploymentLimit);
		MissionGameModels.Current.BattleInitializationModel.InitializeModel();
	}

	private void InitializeShipAssignments()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		NavalAgentsLogic missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalAgentsLogic>();
		_shipsLogic.ClearShipAssignments();
		if (((List<IShipOrigin>)(object)_playerTeamShips).Count > 0)
		{
			int num = MathF.Min(_playerTeamShipDeploymentLimit.NetDeploymentLimit, ((List<IShipOrigin>)(object)_playerTeamShips).Count);
			num = MathF.Min(missionBehavior.GetTeamTroopOrigins((TeamSideEnum)0).Count(), num);
			foreach (var item in AssignShipsToFormations((MBReadOnlyList<IShipOrigin>)(object)_playerTeamShips, num))
			{
				_shipsLogic.SetShipAssignment((TeamSideEnum)0, item.formationIndex, item.ship);
			}
		}
		if (_playerAllyTeamShips != null && ((List<IShipOrigin>)(object)_playerAllyTeamShips).Count > 0)
		{
			int num2 = MathF.Min(_playerAllyTeamShipDeploymentLimit.NetDeploymentLimit, ((List<IShipOrigin>)(object)_playerAllyTeamShips).Count);
			num2 = MathF.Min(missionBehavior.GetTeamTroopOrigins((TeamSideEnum)1).Count(), num2);
			foreach (var item2 in AssignShipsToFormations((MBReadOnlyList<IShipOrigin>)(object)_playerAllyTeamShips, num2))
			{
				_shipsLogic.SetShipAssignment((TeamSideEnum)1, item2.formationIndex, item2.ship);
			}
		}
		if (((List<IShipOrigin>)(object)_enemyTeamShips).Count <= 0)
		{
			return;
		}
		int num3 = MathF.Min(_enemyTeamShipDeploymentLimit.NetDeploymentLimit, ((List<IShipOrigin>)(object)_enemyTeamShips).Count);
		num3 = MathF.Min(missionBehavior.GetTeamTroopOrigins((TeamSideEnum)2).Count(), num3);
		foreach (var item3 in AssignShipsToFormations((MBReadOnlyList<IShipOrigin>)(object)_enemyTeamShips, num3))
		{
			_shipsLogic.SetShipAssignment((TeamSideEnum)2, item3.formationIndex, item3.ship);
		}
	}

	private List<(FormationClass formationIndex, IShipOrigin ship)> AssignShipsToFormations(MBReadOnlyList<IShipOrigin> ships, int shipCount)
	{
		List<(FormationClass, IShipOrigin)> list = new List<(FormationClass, IShipOrigin)>();
		int num = 8;
		int num2 = 0;
		foreach (IShipOrigin item in (List<IShipOrigin>)(object)ships)
		{
			if (num2 < num && num2 < shipCount)
			{
				list.Add(((FormationClass)num2, item));
				num2++;
				continue;
			}
			break;
		}
		return list;
	}

	private void MakeDeploymentPlansForSide(BattleSideEnum battleSide)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<Team> enumerable = ((IEnumerable<Team>)((MissionBehavior)this).Mission.Teams).Where((Team t) => t.Side == battleSide && _shipsLogic.GetCountOfSetShipAssignments(t.TeamSide) > 0);
		if (Extensions.IsEmpty<Team>(enumerable))
		{
			return;
		}
		foreach (Team item in enumerable)
		{
			AddTeamShipsToDeploymentPlan(item);
		}
		int battleSizeFromShipAssignments = _shipsLogic.GetBattleSizeFromShipAssignments();
		Path initialSpawnPath = ((MissionBehavior)this).Mission.GetInitialSpawnPath();
		float battleSizeOffset = Mission.GetBattleSizeOffset(battleSizeFromShipAssignments, initialSpawnPath);
		IOrderedEnumerable<Team> orderedEnumerable = enumerable.OrderByDescending((Team t) => t == ((MissionBehavior)this).Mission.PlayerTeam || t == ((MissionBehavior)this).Mission.PlayerEnemyTeam);
		SpawnPathData initialSpawnPathData = ((MissionBehavior)this).Mission.GetInitialSpawnPathData(battleSide);
		float offsetOverflow = ((SpawnPathData)(ref initialSpawnPathData)).GetOffsetOverflow(battleSizeOffset);
		float offsetOverflow2 = ((SpawnPathData)(ref initialSpawnPathData)).GetOffsetOverflow(0f - battleSizeOffset);
		float num = battleSizeOffset - offsetOverflow2;
		float num2 = 0f - (battleSizeOffset + offsetOverflow);
		float num3 = ((SpawnPathData)(ref initialSpawnPathData)).ClampPathOffset(num);
		float num4 = ((SpawnPathData)(ref initialSpawnPathData)).ClampPathOffset(num2);
		foreach (Team item2 in orderedEnumerable)
		{
			if (item2 == ((MissionBehavior)this).Mission.PlayerTeam || item2 == ((MissionBehavior)this).Mission.PlayerEnemyTeam)
			{
				((MissionDeploymentPlanningLogic)_deploymentPlan).MakeDeploymentPlan(item2, num3, num4);
				continue;
			}
			float num5 = 0f;
			_ = item2.TeamSide;
			for (int num6 = 0; num6 < 11; num6++)
			{
				ShipAssignment shipAssignment = _shipsLogic.GetShipAssignment(item2.TeamSide, (FormationClass)num6);
				if (shipAssignment.IsSet)
				{
					num5 = Math.Max(shipAssignment.MissionShipObject.DeploymentArea.y, num5);
				}
			}
			float pathOffsetFromDistance = Mission.GetPathOffsetFromDistance(1.5f * num5 + 20f, initialSpawnPath);
			num3 += pathOffsetFromDistance;
			((MissionDeploymentPlanningLogic)_deploymentPlan).MakeDeploymentPlan(item2, num3, num4);
		}
	}

	private void UpdateSceneWindDirection()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vec2 globalWindVelocity = Mission.Current.Scene.GetGlobalWindVelocity();
		if (((Vec2)(ref globalWindVelocity)).IsNonZero())
		{
			float northRotation = Mission.Current.Scene.GetNorthRotation();
			((Vec2)(ref globalWindVelocity)).RotateCCW(northRotation);
			Mission.Current.Scene.SetGlobalWindVelocity(ref globalWindVelocity);
		}
	}

	private void UpdateSceneWaterStrength()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vec2 globalWindVelocity = Mission.Current.Scene.GetGlobalWindVelocity();
		float length = ((Vec2)(ref globalWindVelocity)).Length;
		float num = 30f;
		float num2 = 10f;
		Mission.Current.Scene.SetWaterStrength(length * num2 / num);
	}

	private void AddTeamShipsToDeploymentPlan(Team team)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 11; i++)
		{
			ShipAssignment shipAssignment = _shipsLogic.GetShipAssignment(team.TeamSide, (FormationClass)i);
			if (shipAssignment.IsSet)
			{
				_deploymentPlan.AddShip(team, shipAssignment.FormationIndex, shipAssignment.ShipOrigin);
			}
		}
	}

	public AgentState GetAgentState(Agent affectedAgent, float deathProbability, out bool usedSurgery)
	{
		if (affectedAgent.IsInWater())
		{
			usedSurgery = true;
			if (affectedAgent.Character != null && affectedAgent.Character.IsHero)
			{
				return (AgentState)3;
			}
			return (AgentState)4;
		}
		usedSurgery = false;
		return (AgentState)0;
	}
}

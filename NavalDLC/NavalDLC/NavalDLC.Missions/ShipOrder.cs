using System;
using System.Collections.Generic;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions;

public class ShipOrder
{
	public enum ShipMovementOrderEnum
	{
		Stop = 0,
		Move = 1,
		Retreat = 2,
		StaticOrderCount = 3,
		Follow = 3,
		Engage = 4,
		Skirmish = 5
	}

	private enum ShipIndependenceState
	{
		Independent,
		Connected,
		EnemyOnShip
	}

	private enum ShipDetachmentPriority
	{
		PlacementDetachment = 1,
		Oar,
		ControllerMachine,
		SiegeWeapon,
		ConnectionMachine
	}

	private const float BoardingDistance = 12f;

	private const float SkirmishDistance = 60f;

	private const float TimerDuration = 1f;

	private const float TargetCorrectionCheckDistance = 2f;

	private readonly QueryData<bool> _isEnemyOnShip;

	private readonly QueryData<MissionShip> _closestEnemyShip;

	private Vec2 _orderGlobalPosition = Vec2.Invalid;

	private Vec2 _orderGlobalDirection = Vec2.Forward;

	private readonly MissionShip _ownerShip;

	private float _offsetDirection;

	private bool _autoSelectTargetShip;

	private Vec2 _offsetPosition = Vec2.Zero;

	private Formation _ownerFormation;

	private NavalShipsLogic _navalShipsLogic;

	private bool _cutLooseOrderActive;

	private MissionShip _boardingTargetShip;

	private ShipIndependenceState _shipIndependenceState;

	private RandomTimer _detachmentTickTimer;

	private bool _oarLevelOverridden;

	private int _originalOarsmenLevel = 2;

	private bool _isChargeOrderOverridden;

	private MBList<IFormationUnit> _availableUnitList;

	private Vec2 _lastCheckedOrderPosition = Vec2.Invalid;

	private MissionTimer _orderTimer;

	private MissionTimer _placementDetachmentTimer;

	public MissionShip TargetShip { get; private set; }

	public bool HasAIController => _ownerShip.IsAIControlled;

	public bool HasStaticOrder => MovementOrderEnum < ShipMovementOrderEnum.StaticOrderCount;

	public bool IsAutoSelectingTargetShip => _autoSelectTargetShip;

	public int OarsmenLevel { get; private set; } = 2;

	public bool TickDetachmentsNeeded { get; private set; }

	public bool BoardAtWill { get; private set; }

	public bool IsBoardingAvailable { get; set; } = true;

	public ShipMovementOrderEnum MovementOrderEnum { get; private set; }

	public MissionShip ClosestEnemyShip => _closestEnemyShip.Value;

	public bool IsEnemyOnShip => _isEnemyOnShip.Value;

	public ShipOrder(MissionShip missionShip, Formation ownerFormation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		_ownerShip = missionShip;
		FormationJoinShip(ownerFormation);
		_navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		_navalShipsLogic.ShipControllerChanged += OnShipControllerChanged;
		_navalShipsLogic.ShipRemovedEvent += OnShipRemoved;
		_isEnemyOnShip = new QueryData<bool>((Func<bool>)delegate
		{
			if (_ownerShip.Team == null)
			{
				return false;
			}
			foreach (Team item in (List<Team>)(object)Mission.Current.Teams)
			{
				if (item.IsEnemyOf(_ownerShip.Team))
				{
					foreach (Agent item2 in (List<Agent>)(object)item.ActiveAgents)
					{
						if (_ownerShip.GetIsAgentOnShip(item2))
						{
							return true;
						}
					}
				}
			}
			return false;
		}, 3f);
		_closestEnemyShip = new QueryData<MissionShip>((Func<MissionShip>)delegate
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			MissionShip result = null;
			Vec3 origin = _ownerShip.GlobalFrame.origin;
			foreach (Team item3 in (List<Team>)(object)Mission.Current.Teams)
			{
				if (MBExtensions.IsOpponentOf(_ownerFormation.Team.Side, item3.Side))
				{
					foreach (Formation item4 in (List<Formation>)(object)item3.FormationsIncludingEmpty)
					{
						if (item4.CountOfUnits > 0)
						{
							_navalShipsLogic.GetShip(item3.TeamSide, item4.FormationIndex, out var ship);
							MatrixFrame globalFrame = ship.GlobalFrame;
							float num2 = ((Vec3)(ref globalFrame.origin)).DistanceSquared(origin);
							if (num2 < num)
							{
								num = num2;
								result = ship;
							}
						}
					}
				}
			}
			return result;
		}, 5f);
		MovementOrderEnum = ShipMovementOrderEnum.Stop;
		_shipIndependenceState = ShipIndependenceState.Independent;
		_detachmentTickTimer = new RandomTimer(Mission.Current.CurrentTime, 0.9f, 1.1f);
		TickDetachmentsNeeded = true;
		_availableUnitList = new MBList<IFormationUnit>();
		_orderTimer = new MissionTimer(1f);
		_orderTimer.Set(MBRandom.RandomFloat * 1f);
		_placementDetachmentTimer = new MissionTimer(5f);
		_placementDetachmentTimer.Set(MBRandom.RandomFloat * 5f);
	}

	public void MakeEnemyOnShipExpire()
	{
		_isEnemyOnShip.Expire();
	}

	public void SetFormation(Formation formation)
	{
		_ownerFormation = formation;
	}

	public void OnShipSwappedBetweenTeams(MissionShip ship1, MissionShip ship2)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		_closestEnemyShip.Expire();
		_isEnemyOnShip.Expire();
		MissionShip targetShip = TargetShip;
		TeamSideEnum? obj;
		if (targetShip == null)
		{
			obj = null;
		}
		else
		{
			Formation formation = targetShip.Formation;
			if (formation == null)
			{
				obj = null;
			}
			else
			{
				Team team = formation.Team;
				obj = ((team != null) ? new TeamSideEnum?(team.TeamSide) : ((TeamSideEnum?)null));
			}
		}
		TeamSideEnum? val = obj;
		Formation formation2 = _ownerShip.Formation;
		TeamSideEnum? obj2;
		if (formation2 == null)
		{
			obj2 = null;
		}
		else
		{
			Team team2 = formation2.Team;
			obj2 = ((team2 != null) ? new TeamSideEnum?(team2.TeamSide) : ((TeamSideEnum?)null));
		}
		if (val != obj2)
		{
			TargetShip = null;
			if (!HasStaticOrder)
			{
				_orderTimer.Reset();
				UpdateDynamicMovementOrder();
			}
		}
		if (_ownerShip == ship1 || _ownerShip == ship2)
		{
			RangedSiegeWeapon shipSiegeWeapon = _ownerShip.ShipSiegeWeapon;
			if (shipSiegeWeapon != null)
			{
				((SiegeWeapon)shipSiegeWeapon).OnShipSwappedBetweenTeams((_ownerShip == ship1) ? ship1.Formation.Team.Side : ship2.Formation.Team.Side);
			}
		}
	}

	public void FormationJoinShip(Formation formation)
	{
		if (formation == null || formation == _ownerFormation)
		{
			return;
		}
		_ownerFormation = formation;
		_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ClimbingMachineDetachment);
		foreach (ShipOarMachine item in (List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)
		{
			ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)item, true);
		}
		foreach (ShipOarMachine item2 in (List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)
		{
			ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)item2, true);
		}
		if (_ownerShip.ShipSiegeWeapon != null)
		{
			ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)_ownerShip.ShipSiegeWeapon, true);
		}
		ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)_ownerShip.ShipControllerMachine, true);
		_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
		foreach (ShipAttachmentMachine item3 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
		{
			ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)item3, true);
			((UsableMachine)item3).SetIsDisabledForAI(true);
		}
		foreach (ShipAttachmentPointMachine item4 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
		{
			ModuleExtensions.StartUsingMachine(_ownerFormation, (UsableMachine)(object)item4, true);
			((UsableMachine)item4).SetIsDisabledForAI(true);
		}
	}

	public void StopUsingMachines(bool includeAttachmentMachines)
	{
		if (_ownerFormation == null)
		{
			return;
		}
		if (_ownerShip.FireHitPoints > 0f)
		{
			_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ClimbingMachineDetachment);
			foreach (ShipOarMachine item in (List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)
			{
				if (!((MissionObject)item).IsDisabled)
				{
					ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)item, true);
				}
			}
			foreach (ShipOarMachine item2 in (List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)
			{
				if (!((MissionObject)item2).IsDisabled)
				{
					ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)item2, true);
				}
			}
			if (_ownerShip.ShipSiegeWeapon != null && !((MissionObject)_ownerShip.ShipSiegeWeapon).IsDisabled && !Extensions.IsEmpty<Formation>((IEnumerable<Formation>)((UsableMachine)_ownerShip.ShipSiegeWeapon).UserFormations))
			{
				ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)_ownerShip.ShipSiegeWeapon, true);
			}
			if (!((MissionObject)_ownerShip.ShipControllerMachine).IsDisabled)
			{
				ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)_ownerShip.ShipControllerMachine, true);
			}
			if (includeAttachmentMachines)
			{
				foreach (ShipAttachmentMachine item3 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
				{
					if (!((MissionObject)item3).IsDisabled)
					{
						ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)item3, true);
						((UsableMachine)item3).SetIsDisabledForAI(true);
					}
				}
			}
			foreach (ShipAttachmentPointMachine item4 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
			{
				if (!((MissionObject)item4).IsDisabled)
				{
					ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)item4, true);
					((UsableMachine)item4).SetIsDisabledForAI(true);
				}
			}
			if (((List<IDetachment>)(object)_ownerFormation.Detachments).Contains((IDetachment)(object)_ownerShip.ShipPlacementDetachment))
			{
				_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			}
		}
		else
		{
			if (!includeAttachmentMachines)
			{
				return;
			}
			foreach (ShipAttachmentMachine item5 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
			{
				if (!((MissionObject)item5).IsDisabled)
				{
					ModuleExtensions.StopUsingMachine(_ownerFormation, (UsableMachine)(object)item5, true);
					((UsableMachine)item5).SetIsDisabledForAI(true);
				}
			}
		}
	}

	public void FormationLeaveShip()
	{
		if (_ownerFormation != null)
		{
			StopUsingMachines(includeAttachmentMachines: true);
			_ownerFormation = null;
		}
	}

	public bool IsOarsmenLevelLocked()
	{
		return _oarLevelOverridden;
	}

	public void SetOrderOarsmenLevel(int newOarsmenLevel)
	{
		_originalOarsmenLevel = newOarsmenLevel;
		if (!_oarLevelOverridden)
		{
			SetOarsmenLevel(_originalOarsmenLevel);
		}
	}

	private void SetOarsmenLevel(int newOarsmenLevel)
	{
		if (OarsmenLevel == newOarsmenLevel)
		{
			return;
		}
		bool flag = newOarsmenLevel > OarsmenLevel;
		if (flag)
		{
			TickDetachmentsNeeded = true;
		}
		int num = 1;
		if (newOarsmenLevel == 1)
		{
			num = ((MathF.Abs(newOarsmenLevel - OarsmenLevel) > 1) ? 1 : 2);
		}
		int num2 = 0;
		int num3 = int.MaxValue;
		if (flag && OarsmenLevel == 1)
		{
			num3 = (_ownerFormation.Arrangement.UnitCount + _ownerShip.ShipPlacementDetachment.CountOfAgents) / 2;
		}
		for (int i = ((num != 1 && OarsmenLevel + newOarsmenLevel > 1) ? 1 : 0); i < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count && i < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count; i += num)
		{
			if (flag)
			{
				((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i]).SetIsDisabledForAI(false);
				((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i]).SetIsDisabledForAI(false);
				if (newOarsmenLevel == 1)
				{
					num2 += 2;
					if (num2 >= num3)
					{
						break;
					}
				}
				continue;
			}
			Agent pilotAgent;
			if (newOarsmenLevel == 1)
			{
				if (i < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count - 1 && !((UsableMissionObject)((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i]).PilotStandingPoint).HasUser && !((UsableMissionObject)((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i]).PilotStandingPoint).HasAIMovingTo)
				{
					((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i + 1]).SetIsDisabledForAI(true);
					pilotAgent = ((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i + 1]).PilotAgent;
					if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
					}
				}
				if (i < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count - 1 && !((UsableMissionObject)((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i]).PilotStandingPoint).HasUser && !((UsableMissionObject)((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i]).PilotStandingPoint).HasAIMovingTo)
				{
					((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i + 1]).SetIsDisabledForAI(true);
					pilotAgent = ((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i + 1]).PilotAgent;
					if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
					}
				}
			}
			((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i]).SetIsDisabledForAI(true);
			pilotAgent = ((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i]).PilotAgent;
			if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
			{
				pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
			}
			((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i]).SetIsDisabledForAI(true);
			pilotAgent = ((UsableMachine)((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i]).PilotAgent;
			if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
			{
				pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
			}
		}
		OarsmenLevel = newOarsmenLevel;
	}

	public bool GetIsCuttingLoose()
	{
		if (_cutLooseOrderActive)
		{
			return _ownerShip.GetIsAnyBridgeActive();
		}
		return false;
	}

	public void ToggleCutLoose()
	{
		SetCutLoose(!_cutLooseOrderActive);
	}

	public void SetCutLoose(bool enable)
	{
		if (_cutLooseOrderActive == enable)
		{
			return;
		}
		if (enable)
		{
			SetBoardingTargetShip(null);
			foreach (ShipAttachmentMachine item in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
			{
				if (!item.IsShipAttachmentMachineBridged())
				{
					if (((UsableMachine)item).PilotAgent != null)
					{
						((UsableMachine)item).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
					else if (((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent != null)
					{
						((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
				}
			}
			TickDetachmentsNeeded = true;
		}
		_cutLooseOrderActive = enable;
		foreach (ShipAttachmentMachine item2 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
		{
			if (_cutLooseOrderActive)
			{
				if (!item2.IsShipAttachmentMachineBridged())
				{
					if (((UsableMachine)item2).PilotAgent != null)
					{
						((UsableMachine)item2).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
					else
					{
						Agent movingAgent = ((UsableMissionObject)((UsableMachine)item2).PilotStandingPoint).MovingAgent;
						if (movingAgent != null)
						{
							movingAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
						}
					}
				}
				((UsableMachine)item2).SetIsDisabledForAI(false);
			}
			((UsableMachine)item2).SetIsDisabledForAI(!enable);
		}
		foreach (ShipAttachmentPointMachine item3 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
		{
			((UsableMachine)item3).SetIsDisabledForAI(!enable);
		}
		_navalShipsLogic.OnCutLooseOrder(_ownerShip);
	}

	public bool GetIsAttemptingBoarding()
	{
		if (_boardingTargetShip != null)
		{
			return !_ownerShip.SearchShipConnection(_boardingTargetShip, isDirect: true, findEnemy: true, enforceActive: false, acceptNotBridgedConnections: false);
		}
		return false;
	}

	public void SetBoardingTargetShip(MissionShip missionShip)
	{
		if (_boardingTargetShip == missionShip || !IsBoardingAvailable)
		{
			return;
		}
		if (missionShip != null)
		{
			_cutLooseOrderActive = false;
			foreach (ShipAttachmentMachine item in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
			{
				if (item.IsShipAttachmentMachineBridged() || !item.CalculateCanConnectToTargetShip(missionShip))
				{
					if (((UsableMachine)item).PilotAgent != null)
					{
						((UsableMachine)item).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
					else if (((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent != null)
					{
						((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
				}
			}
			foreach (ShipAttachmentPointMachine item2 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
			{
				if (item2.IsShipAttachmentPointBridged())
				{
					if (((UsableMachine)item2).PilotAgent != null)
					{
						((UsableMachine)item2).PilotAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
					else if (((UsableMissionObject)((UsableMachine)item2).PilotStandingPoint).MovingAgent != null)
					{
						((UsableMissionObject)((UsableMachine)item2).PilotStandingPoint).MovingAgent.StopUsingGameObjectMT(true, (StopUsingGameObjectFlags)3);
					}
				}
			}
			TickDetachmentsNeeded = true;
			_navalShipsLogic.OnBoardingOrder(_ownerShip, missionShip);
		}
		foreach (ShipAttachmentMachine item3 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
		{
			if (missionShip != null)
			{
				item3.SetPreferredTargetShip(missionShip);
				((UsableMachine)item3).SetIsDisabledForAI(false);
			}
			else
			{
				item3.SetPreferredTargetShip(null);
				((UsableMachine)item3).SetIsDisabledForAI(true);
			}
		}
		_boardingTargetShip = missionShip;
	}

	public void SetShipStopOrder()
	{
		MovementOrderEnum = ShipMovementOrderEnum.Stop;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		SetStopShipAux();
	}

	public void SetShipMovementOrder(in Vec2 targetPosition)
	{
		MovementOrderEnum = ShipMovementOrderEnum.Move;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		SetTargetPosition(in targetPosition);
	}

	public void SetShipRetreatOrder()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		MovementOrderEnum = ShipMovementOrderEnum.Retreat;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		WorldPosition closestFleePositionForFormation = Mission.Current.GetClosestFleePositionForFormation(_ownerFormation);
		SetTargetPosition(((WorldPosition)(ref closestFleePositionForFormation)).AsVec2);
	}

	public void Tick()
	{
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Invalid comparison between Unknown and I4
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Invalid comparison between Unknown and I4
		if (HasAIController && _ownerShip.AIController.HasTarget && !_ownerShip.AnyActiveFormationTroopOnShip)
		{
			_ownerShip.AIController.ClearTarget();
		}
		if (_ownerFormation == null)
		{
			return;
		}
		if (Mission.Current.IsDeploymentFinished)
		{
			if (_ownerShip.GetIsConnected())
			{
				switch (MovementOrderEnum)
				{
				case ShipMovementOrderEnum.Engage:
					if ((!_autoSelectTargetShip && (TargetShip == null || !_ownerShip.SearchShipConnection(TargetShip, isDirect: true, findEnemy: false, enforceActive: false, acceptNotBridgedConnections: true)) && (_boardingTargetShip == null || !_ownerShip.SearchShipConnection(_boardingTargetShip, isDirect: true, findEnemy: false, enforceActive: false, acceptNotBridgedConnections: true))) || (_autoSelectTargetShip && !_ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true)))
					{
						SetCutLoose(enable: true);
					}
					break;
				case ShipMovementOrderEnum.Move:
				case ShipMovementOrderEnum.Retreat:
				case ShipMovementOrderEnum.StaticOrderCount:
				case ShipMovementOrderEnum.Skirmish:
					if (_boardingTargetShip == null || !_ownerShip.SearchShipConnection(_boardingTargetShip, isDirect: true, findEnemy: false, enforceActive: false, acceptNotBridgedConnections: true))
					{
						SetCutLoose(enable: true);
					}
					break;
				}
			}
			else if (!HasStaticOrder && _orderTimer.Check(true))
			{
				UpdateDynamicMovementOrder();
			}
		}
		CheckAndChangeIndependenceState();
		if (HasAIController)
		{
			DecideOarsmenLevel();
		}
		TickClimbingMachines();
		if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
		{
			if (_ownerShip.GetIsConnectedToEnemyWithSide(out var direction))
			{
				_ownerShip.ShipPlacementDetachment.SetBoarding(isBoarding: true, direction);
			}
			else
			{
				_ownerShip.ShipPlacementDetachment.SetBoarding(isBoarding: false, direction);
				_ownerShip.ShipPlacementDetachment.SetUnderMissileFire(_ownerFormation.QuerySystem.IsUnderRangedAttack);
			}
			if (_ownerShip.ShipPlacementDetachment.IsTickRequired)
			{
				_ownerShip.ShipPlacementDetachment.Tick();
			}
		}
		if (!_ownerShip.IsSinking && (TickDetachmentsNeeded || ((Timer)_detachmentTickTimer).Check(Mission.Current.CurrentTime)))
		{
			ManageShipDetachments();
			((Timer)_detachmentTickTimer).Reset(Mission.Current.CurrentTime);
		}
		if ((_ownerFormation.IsAIControlled && _ownerFormation.IsAIOwned) || !_ownerFormation.IsPlayerTroopInFormation)
		{
			return;
		}
		MovementOrderEnum orderEnum = ((MovementOrder)_ownerFormation.GetReadonlyMovementOrderReference()).OrderEnum;
		if ((int)orderEnum != 2)
		{
			if ((int)orderEnum == 4)
			{
				if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
				{
					_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
				}
			}
			else if (!_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
			{
				_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			}
			else if (_isChargeOrderOverridden && _ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
			{
				SetChargeOrder(applyToPlayerFormation: true);
				_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
				_isChargeOrderOverridden = false;
			}
		}
		else if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
		{
			if (_ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
			{
				_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			}
		}
		else if (!_ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
		{
			_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: true, playersOrder: false);
			_isChargeOrderOverridden = true;
		}
	}

	public void SetShipSkirmishOrder(bool autoTargetClosest = true)
	{
		MissionShip closestEnemyShip = ClosestEnemyShip;
		if (closestEnemyShip != null)
		{
			MovementOrderEnum = ShipMovementOrderEnum.Skirmish;
			_autoSelectTargetShip = autoTargetClosest;
			_orderTimer.Reset();
			UpdateSkirmishOrder(closestEnemyShip);
		}
		else
		{
			SetShipStopOrder();
		}
	}

	public void SetShipFollowOrder(MissionShip shipToFollow, float offsetDistance)
	{
		MovementOrderEnum = ShipMovementOrderEnum.StaticOrderCount;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		Vec2 offsetPosition = default(Vec2);
		((Vec2)(ref offsetPosition))._002Ector(offsetDistance, -15f);
		UpdateFollowOrder(shipToFollow, in offsetPosition);
	}

	public void SetShipMovementOrder(Vec2 targetPosition, in Vec2 targetDirection)
	{
		MovementOrderEnum = ShipMovementOrderEnum.Move;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		SetTargetState(in targetPosition, in targetDirection);
	}

	public void SetShipEngageOrder(bool autoTargetClosest = true)
	{
		MissionShip closestEnemyShip = ClosestEnemyShip;
		if (closestEnemyShip != null)
		{
			MovementOrderEnum = ShipMovementOrderEnum.Engage;
			_autoSelectTargetShip = autoTargetClosest;
			_orderTimer.Reset();
			UpdateEngageOrder(closestEnemyShip);
		}
		else
		{
			SetShipStopOrder();
		}
	}

	public void SetShipEngageOrder(MissionShip shipToEngage)
	{
		MovementOrderEnum = ShipMovementOrderEnum.Engage;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		UpdateEngageOrder(shipToEngage);
	}

	public void SetShipSkirmishOrder(MissionShip shipToSkirmish)
	{
		MovementOrderEnum = ShipMovementOrderEnum.Skirmish;
		_autoSelectTargetShip = false;
		_orderTimer.Reset();
		UpdateSkirmishOrder(shipToSkirmish);
	}

	private void ProjectOrderPositionToBoundaries(ref Vec2 orderPosition)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Mission current = Mission.Current;
		bool flag = false;
		NavalMissionDeploymentPlanningLogic navalMissionDeploymentPlanningLogic = null;
		if (current.IsDeploymentFinished)
		{
			flag = true;
		}
		else if (_ownerShip.Team != null && current.GetDeploymentPlan<NavalMissionDeploymentPlanningLogic>(ref navalMissionDeploymentPlanningLogic))
		{
			if (!((MissionDeploymentPlanningLogic)navalMissionDeploymentPlanningLogic).IsPositionInsideDeploymentBoundaries(_ownerShip.Team, ref orderPosition))
			{
				Vec2 closestDeploymentBoundaryPosition = ((MissionDeploymentPlanningLogic)navalMissionDeploymentPlanningLogic).GetClosestDeploymentBoundaryPosition(_ownerShip.Team, ref orderPosition);
				orderPosition = closestDeploymentBoundaryPosition;
			}
		}
		else
		{
			flag = true;
		}
		if (flag && !Mission.Current.IsPositionInsideBoundaries(orderPosition))
		{
			Vec2 closestBoundaryPosition = Mission.Current.GetClosestBoundaryPosition(orderPosition);
			orderPosition = closestBoundaryPosition;
		}
	}

	private Agent GetNextAgent(ref int currentIndex)
	{
		while (currentIndex >= 0)
		{
			IFormationUnit obj = ((List<IFormationUnit>)(object)_availableUnitList)[currentIndex--];
			Agent val;
			if ((val = (Agent)(object)((obj is Agent) ? obj : null)) != null && val.IsAIControlled && val.IsDetachableFromFormation && val.CanBeAssignedForScriptedMovement())
			{
				return val;
			}
		}
		return null;
	}

	private void UpdateStaticMovementOrder()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (MovementOrderEnum == ShipMovementOrderEnum.Stop)
		{
			SetShipStopOrder();
		}
		else if (MovementOrderEnum == ShipMovementOrderEnum.Move)
		{
			SetShipMovementOrder(_orderGlobalPosition, in _orderGlobalDirection);
		}
		else if (MovementOrderEnum == ShipMovementOrderEnum.Retreat)
		{
			SetShipRetreatOrder();
		}
	}

	public void TickClimbingMachines()
	{
		_ownerShip.ClimbingMachineDetachment.TickClimbingMachines();
	}

	private void UpdateDynamicMovementOrder()
	{
		switch (MovementOrderEnum)
		{
		case ShipMovementOrderEnum.StaticOrderCount:
			if (TargetShip == null)
			{
				SetShipStopOrder();
			}
			else
			{
				UpdateFollowOrder(TargetShip, in _offsetPosition);
			}
			break;
		case ShipMovementOrderEnum.Engage:
			if (_autoSelectTargetShip)
			{
				TrySelectBetterTargetShip();
			}
			if (TargetShip == null)
			{
				SetShipStopOrder();
			}
			else
			{
				UpdateEngageOrder(TargetShip);
			}
			break;
		case ShipMovementOrderEnum.Skirmish:
			if (_autoSelectTargetShip)
			{
				TrySelectBetterTargetShip();
			}
			if (TargetShip == null)
			{
				SetShipStopOrder();
			}
			else
			{
				UpdateSkirmishOrder(TargetShip);
			}
			break;
		}
	}

	private void TrySelectBetterTargetShip(float decisionDistance = 4f)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (TargetShip == null)
		{
			TargetShip = ClosestEnemyShip;
			return;
		}
		MissionShip closestEnemyShip = ClosestEnemyShip;
		if (closestEnemyShip != null)
		{
			MatrixFrame globalFrame = _ownerShip.GlobalFrame;
			Vec2 asVec = ((Vec3)(ref globalFrame.origin)).AsVec2;
			globalFrame = TargetShip.GlobalFrame;
			float num = ((Vec2)(ref asVec)).Distance(((Vec3)(ref globalFrame.origin)).AsVec2);
			globalFrame = closestEnemyShip.GlobalFrame;
			if (((Vec2)(ref asVec)).Distance(((Vec3)(ref globalFrame.origin)).AsVec2) + decisionDistance < num)
			{
				TargetShip = closestEnemyShip;
			}
		}
	}

	private void DecideOarsmenLevel()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		switch (MovementOrderEnum)
		{
		case ShipMovementOrderEnum.Engage:
			SetOrderOarsmenLevel(2);
			break;
		case ShipMovementOrderEnum.Skirmish:
		{
			if (TargetShip == null)
			{
				break;
			}
			WeakGameEntity gameEntity;
			Vec3 globalPosition;
			if (_originalOarsmenLevel != 0)
			{
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)_ownerShip).GameEntity;
				float num = ((Vec3)(ref globalPosition)).DistanceSquared(((WeakGameEntity)(ref gameEntity)).GlobalPosition);
				float num2 = 4356f;
				float num3 = 3240f;
				if (num <= num2 && num >= num3)
				{
					SetOrderOarsmenLevel(0);
				}
			}
			else
			{
				gameEntity = ((ScriptComponentBehavior)TargetShip).GameEntity;
				globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)_ownerShip).GameEntity;
				float num4 = ((Vec3)(ref globalPosition)).DistanceSquared(((WeakGameEntity)(ref gameEntity)).GlobalPosition);
				float num5 = 5184f;
				float num6 = 2304f;
				if (num4 > num5 || num4 < num6)
				{
					SetOrderOarsmenLevel(2);
				}
			}
			break;
		}
		case ShipMovementOrderEnum.Stop:
			SetOrderOarsmenLevel(2);
			break;
		}
	}

	private void UpdateFollowOrder(MissionShip shipToFollow, in Vec2 offsetPosition)
	{
		SetTargetShip(shipToFollow, in offsetPosition);
	}

	private void UpdateSkirmishOrder(MissionShip shipToSkirmish)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = shipToSkirmish.GlobalFrame;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)_ownerShip).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		Vec3 val = ((MatrixFrame)(ref globalFrame)).TransformToLocal(ref globalPosition);
		Vec2 positionOffset = ((Vec3)(ref val)).AsVec2;
		positionOffset = ((Vec2)(ref positionOffset)).Normalized() * 60f;
		SetTargetShip(shipToSkirmish, in positionOffset);
	}

	private void UpdateEngageOrder(MissionShip shipToEngage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = shipToEngage.GlobalFrame;
		Vec3 val = _ownerShip.GlobalFrame.origin - globalFrame.origin;
		Vec2 asVec = ((Vec3)(ref val)).AsVec2;
		Vec2 asVec2 = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		bool flag = ((Vec2)(ref asVec)).DotProduct(((Vec2)(ref asVec2)).RightVec()) > 0f;
		Vec2 positionOffset = default(Vec2);
		((Vec2)(ref positionOffset))._002Ector(flag ? 12f : (-12f), 0f);
		float directionOffset = 0f;
		SetTargetShip(shipToEngage, in positionOffset, directionOffset);
	}

	private void SetStopShipAux()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = _ownerShip.GlobalFrame;
		SetTargetPosition(((Vec3)(ref globalFrame.origin)).AsVec2);
	}

	private void SetTargetPosition(in Vec2 targetPosition)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		TargetShip = null;
		_offsetPosition = Vec2.Zero;
		_offsetDirection = 0f;
		MatrixFrame globalFrame = _ownerShip.GlobalFrame;
		Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Vec2 orderGlobalDirection = ((Vec2)(ref asVec)).Normalized();
		_orderGlobalPosition = targetPosition;
		_orderGlobalDirection = orderGlobalDirection;
		if (!((Vec2)(ref _lastCheckedOrderPosition)).IsValid || ((Vec2)(ref _orderGlobalPosition)).DistanceSquared(_lastCheckedOrderPosition) >= 4f)
		{
			ProjectOrderPositionToBoundaries(ref _orderGlobalPosition);
			_lastCheckedOrderPosition = _orderGlobalPosition;
		}
		if (_navalShipsLogic.IsTeleportingShips)
		{
			TryTeleportShipAux(in _orderGlobalPosition, in _orderGlobalDirection);
		}
		if (HasAIController && _ownerShip.AnyActiveFormationTroopOnShip)
		{
			_ownerShip.AIController.SetTargetState(new NavalState(in _orderGlobalPosition, in _orderGlobalDirection), stopOnArrival: true);
		}
	}

	private void SetTargetState(in Vec2 targetPosition, in Vec2 targetDirection)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		TargetShip = null;
		_offsetPosition = Vec2.Zero;
		_offsetDirection = 0f;
		_orderGlobalPosition = targetPosition;
		_orderGlobalDirection = targetDirection;
		if (!((Vec2)(ref _lastCheckedOrderPosition)).IsValid || ((Vec2)(ref _orderGlobalPosition)).DistanceSquared(_lastCheckedOrderPosition) >= 4f)
		{
			ProjectOrderPositionToBoundaries(ref _orderGlobalPosition);
			_lastCheckedOrderPosition = _orderGlobalPosition;
		}
		if (_navalShipsLogic.IsTeleportingShips)
		{
			TryTeleportShipAux(in _orderGlobalPosition, in _orderGlobalDirection);
		}
		if (HasAIController && _ownerShip.AnyActiveFormationTroopOnShip)
		{
			_ownerShip.AIController.SetTargetState(in _orderGlobalPosition, in _orderGlobalDirection);
		}
	}

	private void SetTargetShip(MissionShip targetShip, in Vec2 positionOffset, float directionOffset = 0f)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		TargetShip = targetShip;
		_offsetPosition = positionOffset;
		directionOffset = MBMath.WrapAngle(directionOffset);
		_offsetDirection = directionOffset;
		MatrixFrame globalFrame = TargetShip.GlobalFrame;
		Vec2 val = ((Vec3)(ref globalFrame.rotation.s)).AsVec2;
		Vec2 val2 = ((Vec2)(ref val)).Normalized();
		val = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Vec2 val3 = ((Vec2)(ref val)).Normalized();
		_orderGlobalPosition = ((Vec3)(ref globalFrame.origin)).AsVec2 + ((Vec2)(ref _offsetPosition)).X * val2 + ((Vec2)(ref _offsetPosition)).Y * val3;
		val = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		_orderGlobalDirection = ((Vec2)(ref val)).Normalized();
		((Vec2)(ref _orderGlobalDirection)).RotateCCW(_offsetDirection);
		if (_navalShipsLogic.IsTeleportingShips)
		{
			Vec2 orderPosition = _orderGlobalPosition;
			Vec2 val4;
			if (!Mission.Current.IsDeploymentFinished)
			{
				Vec2 orderGlobalPosition = _orderGlobalPosition;
				MatrixFrame globalFrame2 = _ownerShip.GlobalFrame;
				val = orderGlobalPosition - ((Vec3)(ref globalFrame2.origin)).AsVec2;
				val4 = ((Vec2)(ref val)).Normalized();
			}
			else
			{
				val4 = _orderGlobalDirection;
			}
			Vec2 direction = val4;
			if (!Mission.Current.IsDeploymentFinished)
			{
				ProjectOrderPositionToBoundaries(ref orderPosition);
			}
			TryTeleportShipAux(in orderPosition, in direction);
		}
		if (HasAIController && _ownerShip.AnyActiveFormationTroopOnShip)
		{
			_ownerShip.AIController.SetTargetShipWithOffset(TargetShip, new NavalVec(in _offsetPosition, _offsetDirection));
		}
	}

	public void ManageShipDetachments()
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Invalid comparison between Unknown and I4
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		foreach (ShipAttachmentMachine item in (List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines)
		{
			if (_boardingTargetShip != null && MissionShip.AreShipsConnected(_ownerShip, _boardingTargetShip) && item.GetBestEnemyAttachment() == null)
			{
				if (((UsableMachine)item).PilotAgent != null && ((UsableMachine)item).PilotAgent.IsAIControlled)
				{
					((UsableMachine)item).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
				}
				else if (((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent != null)
				{
					((UsableMissionObject)((UsableMachine)item).PilotStandingPoint).MovingAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
				}
			}
		}
		Agent captain = _ownerFormation.Captain;
		ShipControllerMachine shipControllerMachine = _ownerShip.ShipControllerMachine;
		if ((((UsableMachine)shipControllerMachine).PilotAgent == null || ((UsableMachine)shipControllerMachine).PilotAgent.IsAIControlled || _navalShipsLogic.IsDeploymentMode) && captain != null && captain.IsAIControlled && (_navalShipsLogic.IsDeploymentMode || ((int)captain.MovementMode == 1 && (!captain.IsDetachedFromFormation || !(captain.Detachment is ClimbingMachineDetachment)))))
		{
			if (captain.IsDetachedFromFormation && (object)captain.CurrentlyUsedGameObject != ((UsableMachine)shipControllerMachine).PilotStandingPoint && (object)captain.HumanAIComponent.GetCurrentlyMovingGameObject() != ((UsableMachine)shipControllerMachine).PilotStandingPoint)
			{
				if (captain.IsUsingGameObject)
				{
					captain.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
				}
				else
				{
					captain.TryAttachToFormation();
				}
			}
			if (((UsableMachine)shipControllerMachine).PilotAgent != null && ((UsableMachine)shipControllerMachine).PilotAgent != captain)
			{
				((UsableMachine)shipControllerMachine).PilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
			}
			if (captain.Detachment == null && !((UsableMachine)shipControllerMachine).IsDisabledForAI)
			{
				((UsableMachine)shipControllerMachine).AddAgentAtSlotIndex(captain, ((UsableMachine)shipControllerMachine).PilotStandingPointSlotIndex);
			}
		}
		if (_ownerFormation.CountOfDetachableNonPlayerUnits > 0)
		{
			_ownerFormation.Arrangement.GetAllUnits(ref _availableUnitList);
			int currentIndex = ((List<IFormationUnit>)(object)_availableUnitList).Count - 1;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (_boardingTargetShip != null)
			{
				while (currentIndex >= 0 && num < ((List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines).Count)
				{
					ShipAttachmentMachine shipAttachmentMachine = ((List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines)[num++];
					if (((UsableMachine)shipAttachmentMachine).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipAttachmentMachine).PilotStandingPoint).HasAIMovingTo && shipAttachmentMachine.CurrentAttachment == null && !((UsableMachine)shipAttachmentMachine).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && shipAttachmentMachine.CalculateCanConnectToTargetShip(_boardingTargetShip) && (!MissionShip.AreShipsConnected(_ownerShip, _boardingTargetShip) || shipAttachmentMachine.GetBestEnemyAttachment() != null))
					{
						Agent nextAgent = GetNextAgent(ref currentIndex);
						if (nextAgent == null)
						{
							break;
						}
						((UsableMachine)shipAttachmentMachine).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipAttachmentMachine).PilotStandingPointSlotIndex);
					}
				}
			}
			else if (_cutLooseOrderActive)
			{
				while (currentIndex >= 0 && num < ((List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines).Count)
				{
					ShipAttachmentMachine shipAttachmentMachine2 = ((List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines)[num++];
					if (shipAttachmentMachine2.IsShipAttachmentMachineBridged() && !((UsableMachine)shipAttachmentMachine2).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && ((UsableMachine)shipAttachmentMachine2).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipAttachmentMachine2).PilotStandingPoint).HasAIMovingTo)
					{
						Agent nextAgent = GetNextAgent(ref currentIndex);
						if (nextAgent == null)
						{
							break;
						}
						((UsableMachine)shipAttachmentMachine2).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipAttachmentMachine2).PilotStandingPointSlotIndex);
					}
				}
				while (currentIndex >= 0 && num2 < ((List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines).Count)
				{
					ShipAttachmentPointMachine shipAttachmentPointMachine = ((List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)[num2++];
					if (shipAttachmentPointMachine.IsShipAttachmentPointBridged() && !((UsableMachine)shipAttachmentPointMachine).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && ((UsableMachine)shipAttachmentPointMachine).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipAttachmentPointMachine).PilotStandingPoint).HasAIMovingTo)
					{
						Agent nextAgent = GetNextAgent(ref currentIndex);
						if (nextAgent == null)
						{
							break;
						}
						((UsableMachine)shipAttachmentPointMachine).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipAttachmentPointMachine).PilotStandingPointSlotIndex);
					}
				}
			}
			if (_ownerShip.ShipSiegeWeapon != null)
			{
				RangedSiegeWeapon shipSiegeWeapon = _ownerShip.ShipSiegeWeapon;
				if (((UsableMachine)shipSiegeWeapon).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipSiegeWeapon).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)shipSiegeWeapon).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
				{
					Agent nextAgent = GetNextAgent(ref currentIndex);
					if (nextAgent != null)
					{
						((UsableMachine)shipSiegeWeapon).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipSiegeWeapon).PilotStandingPointSlotIndex);
					}
				}
			}
			if (((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent == null && !((UsableMissionObject)((UsableMachine)_ownerShip.ShipControllerMachine).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)_ownerShip.ShipControllerMachine).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && (!_ownerShip.IsPlayerShip || Mission.Current.MainAgent == null))
			{
				Agent nextAgent = GetNextAgent(ref currentIndex);
				if (nextAgent != null)
				{
					((UsableMachine)_ownerShip.ShipControllerMachine).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)_ownerShip.ShipControllerMachine).PilotStandingPointSlotIndex);
				}
			}
			while (currentIndex >= 0 && (num3 < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count || num3 < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count))
			{
				if (num3 < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count)
				{
					ShipOarMachine shipOarMachine = ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[num3];
					if (((UsableMachine)shipOarMachine).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipOarMachine).PilotStandingPoint).HasAIMovingTo && !((UsableMissionObject)((UsableMachine)shipOarMachine).PilotStandingPoint).IsDeactivated && !((UsableMachine)shipOarMachine).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
					{
						Agent nextAgent = GetNextAgent(ref currentIndex);
						if (nextAgent == null)
						{
							break;
						}
						((UsableMachine)shipOarMachine).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipOarMachine).PilotStandingPointSlotIndex);
					}
				}
				if (num3 < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count)
				{
					ShipOarMachine shipOarMachine2 = ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[num3];
					if (((UsableMachine)shipOarMachine2).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipOarMachine2).PilotStandingPoint).HasAIMovingTo && !((UsableMissionObject)((UsableMachine)shipOarMachine2).PilotStandingPoint).IsDeactivated && !((UsableMachine)shipOarMachine2).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
					{
						Agent nextAgent = GetNextAgent(ref currentIndex);
						if (nextAgent == null)
						{
							break;
						}
						((UsableMachine)shipOarMachine2).AddAgentAtSlotIndex(nextAgent, ((UsableMachine)shipOarMachine2).PilotStandingPointSlotIndex);
					}
				}
				num3++;
			}
			if (_ownerShip.ShipPlacementDetachment == null || !_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
			{
				return;
			}
			while (currentIndex >= 0 && _ownerShip.ShipPlacementDetachment.HasAvailableSlots)
			{
				Agent nextAgent = GetNextAgent(ref currentIndex);
				if (nextAgent != null)
				{
					_ownerShip.ShipPlacementDetachment.AddAgent(nextAgent);
					continue;
				}
				break;
			}
			return;
		}
		ShipDetachmentPriority shipDetachmentPriority = ShipDetachmentPriority.ConnectionMachine;
		IDetachment val = null;
		bool flag = false;
		if (_cutLooseOrderActive)
		{
			foreach (ShipAttachmentPointMachine item2 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
			{
				if (item2.IsShipAttachmentPointBridged() && !((UsableMachine)item2).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && ((UsableMachine)item2).PilotAgent == null && !((UsableMissionObject)((UsableMachine)item2).PilotStandingPoint).HasAIMovingTo)
				{
					val = (IDetachment)(object)item2;
					break;
				}
			}
			if (val != null)
			{
				goto IL_09e2;
			}
		}
		if (_cutLooseOrderActive || _boardingTargetShip != null)
		{
			foreach (ShipAttachmentMachine item3 in (List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines)
			{
				if (((_cutLooseOrderActive && item3.IsShipAttachmentMachineBridged()) || (_boardingTargetShip != null && item3.CurrentAttachment == null && item3.CalculateCanConnectToTargetShip(_boardingTargetShip) && (!MissionShip.AreShipsConnected(_ownerShip, _boardingTargetShip) || item3.GetBestEnemyAttachment() != null))) && !((UsableMachine)item3).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && ((UsableMachine)item3).PilotAgent == null && !((UsableMissionObject)((UsableMachine)item3).PilotStandingPoint).HasAIMovingTo)
				{
					val = (IDetachment)(object)item3;
					break;
				}
			}
			if (val != null)
			{
				goto IL_09e2;
			}
		}
		shipDetachmentPriority--;
		if (_ownerShip.ShipSiegeWeapon != null)
		{
			RangedSiegeWeapon shipSiegeWeapon2 = _ownerShip.ShipSiegeWeapon;
			if (((UsableMachine)shipSiegeWeapon2).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipSiegeWeapon2).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)shipSiegeWeapon2).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
			{
				val = (IDetachment)(object)shipSiegeWeapon2;
				goto IL_09e2;
			}
		}
		if (val == null)
		{
			shipDetachmentPriority--;
			if (((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent == null && !((UsableMissionObject)((UsableMachine)_ownerShip.ShipControllerMachine).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)_ownerShip.ShipControllerMachine).IsDisabledForBattleSideAI(_ownerFormation.Team.Side) && (!_ownerShip.IsPlayerShip || Mission.Current.MainAgent == null))
			{
				val = (IDetachment)(object)_ownerShip.ShipControllerMachine;
			}
			else
			{
				shipDetachmentPriority--;
				for (int i = 0; i < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count || i < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count; i++)
				{
					if (i < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count)
					{
						ShipOarMachine shipOarMachine3 = ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[i];
						if (((UsableMachine)shipOarMachine3).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipOarMachine3).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)shipOarMachine3).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
						{
							val = (IDetachment)(object)shipOarMachine3;
							break;
						}
					}
					if (i < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count)
					{
						ShipOarMachine shipOarMachine4 = ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[i];
						if (((UsableMachine)shipOarMachine4).PilotAgent == null && !((UsableMissionObject)((UsableMachine)shipOarMachine4).PilotStandingPoint).HasAIMovingTo && !((UsableMachine)shipOarMachine4).IsDisabledForBattleSideAI(_ownerFormation.Team.Side))
						{
							val = (IDetachment)(object)shipOarMachine4;
							break;
						}
					}
				}
				if (val == null)
				{
					shipDetachmentPriority--;
				}
			}
		}
		goto IL_09e2;
		IL_09e2:
		if (shipDetachmentPriority > ShipDetachmentPriority.PlacementDetachment)
		{
			UsableMachine val2;
			int num4 = (((val2 = (UsableMachine)(object)((val is UsableMachine) ? val : null)) != null) ? val2.PilotStandingPointSlotIndex : 0);
			if (_ownerShip.ShipPlacementDetachment.HasAgent)
			{
				Agent val3 = _ownerShip.ShipPlacementDetachment.PickLastAgent();
				val.AddAgentAtSlotIndex(val3, num4);
				return;
			}
			if (shipDetachmentPriority > ShipDetachmentPriority.Oar)
			{
				for (int j = 0; j < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count || j < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count; j++)
				{
					if (j < ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines).Count)
					{
						ShipOarMachine shipOarMachine5 = ((List<ShipOarMachine>)(object)_ownerShip.LeftSideShipOarMachines)[j];
						if (((UsableMachine)shipOarMachine5).PilotAgent != null && ((UsableMachine)shipOarMachine5).PilotAgent.IsAIControlled)
						{
							Agent pilotAgent = ((UsableMachine)shipOarMachine5).PilotAgent;
							pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
							val.AddAgentAtSlotIndex(pilotAgent, num4);
							flag = true;
							break;
						}
					}
					if (j < ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines).Count)
					{
						ShipOarMachine shipOarMachine6 = ((List<ShipOarMachine>)(object)_ownerShip.RightSideShipOarMachines)[j];
						if (((UsableMachine)shipOarMachine6).PilotAgent != null && ((UsableMachine)shipOarMachine6).PilotAgent.IsAIControlled)
						{
							Agent pilotAgent2 = ((UsableMachine)shipOarMachine6).PilotAgent;
							pilotAgent2.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
							val.AddAgentAtSlotIndex(pilotAgent2, num4);
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					return;
				}
				if (shipDetachmentPriority > ShipDetachmentPriority.ControllerMachine && ((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent != null && ((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent.IsAIControlled)
				{
					Agent pilotAgent3 = ((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent;
					pilotAgent3.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
					val.AddAgentAtSlotIndex(pilotAgent3, num4);
					return;
				}
				if (shipDetachmentPriority > ShipDetachmentPriority.SiegeWeapon)
				{
					RangedSiegeWeapon shipSiegeWeapon3 = _ownerShip.ShipSiegeWeapon;
					if (((shipSiegeWeapon3 != null) ? ((UsableMachine)shipSiegeWeapon3).PilotAgent : null) != null && ((UsableMachine)shipSiegeWeapon3).PilotAgent.IsAIControlled)
					{
						Agent pilotAgent4 = ((UsableMachine)shipSiegeWeapon3).PilotAgent;
						pilotAgent4.StopUsingGameObject(true, (StopUsingGameObjectFlags)3);
						val.AddAgentAtSlotIndex(pilotAgent4, num4);
						return;
					}
				}
			}
			TickDetachmentsNeeded = false;
			((Timer)_detachmentTickTimer).Reset(Mission.Current.CurrentTime);
		}
		else
		{
			TickDetachmentsNeeded = false;
			((Timer)_detachmentTickTimer).Reset(Mission.Current.CurrentTime);
		}
	}

	private void TryTeleportShipAux(in Vec2 position, in Vec2 direction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		MatrixFrame globalFrame = _ownerShip.GlobalFrame;
		Vec2 val = position;
		if (!(((Vec2)(ref val)).DistanceSquared(((Vec3)(ref globalFrame.origin)).AsVec2) >= 0.01f))
		{
			val = direction;
			Vec2 asVec = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
			if (!(((Vec2)(ref val)).AngleBetween(((Vec2)(ref asVec)).Normalized()) >= 0.1f))
			{
				return;
			}
		}
		Vec2 val2 = position;
		val = direction;
		_003F val3;
		if (!((Vec2)(ref val)).IsValid)
		{
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)_ownerShip).GameEntity;
			MatrixFrame globalFrame2 = ((WeakGameEntity)(ref gameEntity)).GetGlobalFrame();
			val = ((Vec3)(ref globalFrame2.rotation.f)).AsVec2;
			val3 = ((Vec2)(ref val)).Normalized();
		}
		else
		{
			val3 = direction;
		}
		Vec2 val4 = (Vec2)val3;
		Vec3 origin = ((Vec2)(ref val2)).ToVec3(0f);
		Vec3 val5 = ((Vec2)(ref val4)).ToVec3(0f);
		Vec3 f = ((Vec3)(ref val5)).NormalizedCopy();
		MatrixFrame identity = MatrixFrame.Identity;
		identity.rotation.f = f;
		((Mat3)(ref identity.rotation)).OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
		identity.origin = origin;
		bool anchorShip = _ownerShip.Physics != null && _ownerShip.Physics.IsAnchored;
		_navalShipsLogic.TeleportShip(_ownerShip, identity, checkFreeArea: true, anchorShip);
	}

	private void SetChargeOrder(bool applyToPlayerFormation)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (applyToPlayerFormation || _ownerFormation.PlayerOwner != Mission.Current.MainAgent || !_ownerFormation.HasPlayerControlledTroop)
		{
			_ownerFormation.SetMovementOrder(MovementOrder.MovementOrderCharge);
		}
	}

	public void JoinPlayerFormationToPlacementDetachment(bool isPlayersOrder)
	{
		if (!_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
		{
			_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
		}
		if (isPlayersOrder)
		{
			_isChargeOrderOverridden = false;
		}
	}

	private void CheckAndChangeIndependenceState()
	{
		MissionShip boardingTargetShip = _boardingTargetShip;
		bool flag = boardingTargetShip != null && boardingTargetShip.AnyActiveFormationTroopOnShip && MissionShip.AreShipsConnected(_ownerShip, _boardingTargetShip);
		bool flag2 = flag || _isEnemyOnShip.Value;
		if (!flag2)
		{
			foreach (ShipAttachmentMachine item in (List<ShipAttachmentMachine>)(object)_ownerShip.AttachmentMachines)
			{
				if (item.IsShipAttachmentMachineBridged())
				{
					flag2 = true;
					flag = true;
					break;
				}
				if (!flag2 && ShipAttachmentMachine.DoesShipAttachmentMachineSatisfyOarsmenGetUpCondition(item.CurrentAttachment))
				{
					flag2 = true;
				}
				if (item.IsShipAttachmentMachineConnectedToEnemy())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (ShipAttachmentPointMachine item2 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
				{
					if (item2.IsShipAttachmentPointBridged())
					{
						flag2 = true;
						flag = true;
						break;
					}
					if (!flag2 && ShipAttachmentMachine.DoesShipAttachmentMachineSatisfyOarsmenGetUpCondition(item2.CurrentAttachment))
					{
						flag2 = true;
					}
					if (item2.IsShipAttachmentPointConnectedToEnemy())
					{
						flag = true;
						break;
					}
				}
			}
		}
		switch (_shipIndependenceState)
		{
		case ShipIndependenceState.Independent:
			if (flag || _isEnemyOnShip.Value)
			{
				if (flag2)
				{
					_oarLevelOverridden = true;
					_originalOarsmenLevel = OarsmenLevel;
					SetOarsmenLevel(0);
				}
				((UsableMachine)_ownerShip.ShipControllerMachine).SetIsDisabledForAI(true);
				Agent pilotAgent2 = ((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent;
				if (pilotAgent2 != null && _navalShipsLogic.IsDeploymentMode)
				{
					pilotAgent2.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
				}
				_shipIndependenceState = ShipIndependenceState.Connected;
			}
			if (!_isEnemyOnShip.Value)
			{
				break;
			}
			foreach (ShipAttachmentMachine item3 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
			{
				((UsableMachine)item3).SetIsDisabledForAI(true);
				Agent pilotAgent3 = ((UsableMachine)item3).PilotAgent;
				if (pilotAgent3 != null && _navalShipsLogic.IsDeploymentMode)
				{
					pilotAgent3.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
				}
			}
			foreach (ShipAttachmentPointMachine item4 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
			{
				((UsableMachine)item4).SetIsDisabledForAI(true);
				Agent pilotAgent4 = ((UsableMachine)item4).PilotAgent;
				if (pilotAgent4 != null && _navalShipsLogic.IsDeploymentMode)
				{
					pilotAgent4.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
				}
			}
			_shipIndependenceState = ShipIndependenceState.EnemyOnShip;
			break;
		case ShipIndependenceState.Connected:
			if (_isEnemyOnShip.Value)
			{
				Agent pilotAgent;
				foreach (ShipAttachmentMachine item5 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
				{
					((UsableMachine)item5).SetIsDisabledForAI(true);
					pilotAgent = ((UsableMachine)item5).PilotAgent;
					if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
					}
				}
				foreach (ShipAttachmentPointMachine item6 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
				{
					((UsableMachine)item6).SetIsDisabledForAI(true);
					pilotAgent = ((UsableMachine)item6).PilotAgent;
					if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
					}
				}
				((UsableMachine)_ownerShip.ShipControllerMachine).SetIsDisabledForAI(true);
				pilotAgent = ((UsableMachine)_ownerShip.ShipControllerMachine).PilotAgent;
				if (pilotAgent != null && _navalShipsLogic.IsDeploymentMode)
				{
					pilotAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
				}
				_shipIndependenceState = ShipIndependenceState.EnemyOnShip;
				SetChargeOrder(applyToPlayerFormation: false);
			}
			else if (!flag)
			{
				_shipIndependenceState = ShipIndependenceState.Independent;
				SetOarsmenLevel(_originalOarsmenLevel);
				_oarLevelOverridden = false;
				((UsableMachine)_ownerShip.ShipControllerMachine).SetIsDisabledForAI(false);
			}
			else if (!_oarLevelOverridden && flag2)
			{
				_oarLevelOverridden = true;
				_originalOarsmenLevel = OarsmenLevel;
				SetOarsmenLevel(0);
			}
			break;
		case ShipIndependenceState.EnemyOnShip:
			if (_isEnemyOnShip.Value)
			{
				break;
			}
			if (_cutLooseOrderActive)
			{
				foreach (ShipAttachmentPointMachine item7 in (List<ShipAttachmentPointMachine>)(object)_ownerShip.AttachmentPointMachines)
				{
					((UsableMachine)item7).SetIsDisabledForAI(false);
				}
			}
			if (_cutLooseOrderActive || _boardingTargetShip != null)
			{
				foreach (ShipAttachmentMachine item8 in (List<ShipAttachmentMachine>)(object)_ownerShip.ShipAttachmentMachines)
				{
					((UsableMachine)item8).SetIsDisabledForAI(false);
				}
			}
			_shipIndependenceState = ShipIndependenceState.Connected;
			if (!flag)
			{
				_shipIndependenceState = ShipIndependenceState.Independent;
				SetOarsmenLevel(_originalOarsmenLevel);
				_oarLevelOverridden = false;
				((UsableMachine)_ownerShip.ShipControllerMachine).SetIsDisabledForAI(false);
			}
			break;
		}
		switch (_shipIndependenceState)
		{
		case ShipIndependenceState.Independent:
			if ((_ownerFormation.IsAIControlled || _ownerFormation.IsAIOwned || !_ownerFormation.HasPlayerControlledTroop) && !_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
			{
				_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			}
			_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: false, playersOrder: false);
			break;
		case ShipIndependenceState.Connected:
			if (_ownerFormation.IsAIControlled)
			{
				if (_boardingTargetShip != null && MissionShip.AreShipsConnected(_boardingTargetShip, _ownerShip) && _boardingTargetShip.Formation != null && _ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
				{
					if ((_ownerFormation.IsAIControlled || _ownerFormation.IsAIOwned || !_ownerFormation.HasPlayerControlledTroop) && _ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
					{
						_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
					}
					SetChargeOrder(applyToPlayerFormation: false);
				}
				else
				{
					if ((_ownerFormation.IsAIControlled || _ownerFormation.IsAIOwned || !_ownerFormation.HasPlayerControlledTroop) && !_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
					{
						_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
					}
					_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: false, playersOrder: false);
				}
			}
			else
			{
				if (_ownerFormation.HasPlayerControlledTroop)
				{
					break;
				}
				switch (MovementOrderEnum)
				{
				case ShipMovementOrderEnum.Engage:
					if (!_autoSelectTargetShip)
					{
						if (MissionShip.AreShipsConnected(_ownerShip, TargetShip) && _ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
						{
							if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
							{
								_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
							}
							SetChargeOrder(applyToPlayerFormation: false);
						}
						else
						{
							if (!_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
							{
								_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
							}
							_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: false, playersOrder: false);
						}
					}
					else if (_ownerShip.SearchShipConnection(null, isDirect: true, findEnemy: true, enforceActive: true, acceptNotBridgedConnections: true))
					{
						if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
						{
							_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
						}
						SetChargeOrder(applyToPlayerFormation: false);
					}
					else
					{
						if (!_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
						{
							_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
						}
						_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: false, playersOrder: false);
					}
					break;
				case ShipMovementOrderEnum.Move:
				case ShipMovementOrderEnum.Retreat:
				case ShipMovementOrderEnum.StaticOrderCount:
				case ShipMovementOrderEnum.Skirmish:
					if (_boardingTargetShip == null || !MissionShip.AreShipsConnected(_ownerShip, _boardingTargetShip) || !_boardingTargetShip.AnyActiveFormationTroopOnShip)
					{
						if (!_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
						{
							_ownerFormation.JoinDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
						}
						_ownerShip.SetPositioningOrdersToRallyPoint(applyToPlayerFormation: false, playersOrder: false);
					}
					else
					{
						if (_ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
						{
							_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
						}
						SetChargeOrder(applyToPlayerFormation: false);
					}
					break;
				}
			}
			break;
		case ShipIndependenceState.EnemyOnShip:
			if ((_ownerFormation.IsAIControlled || _ownerFormation.IsAIOwned || !_ownerFormation.HasPlayerControlledTroop) && _ownerShip.ShipPlacementDetachment.IsUsedByFormation(_ownerFormation))
			{
				_ownerFormation.LeaveDetachment((IDetachment)(object)_ownerShip.ShipPlacementDetachment);
			}
			SetChargeOrder(applyToPlayerFormation: false);
			break;
		}
	}

	private void OnShipControllerChanged(MissionShip ship)
	{
		if (_ownerShip == ship)
		{
			if (!_ownerShip.IsAIControlled)
			{
				SetShipStopOrder();
				return;
			}
			if (HasStaticOrder)
			{
				UpdateStaticMovementOrder();
				return;
			}
			_orderTimer.Reset();
			UpdateDynamicMovementOrder();
		}
	}

	internal void OnOwnerShipRemoved()
	{
		_navalShipsLogic.ShipControllerChanged -= OnShipControllerChanged;
		_navalShipsLogic.ShipRemovedEvent -= OnShipRemoved;
	}

	private void OnShipRemoved(MissionShip ship)
	{
		if (ship != _ownerShip && TargetShip == ship)
		{
			TargetShip = null;
		}
		if (_boardingTargetShip == ship)
		{
			_boardingTargetShip = null;
		}
	}
}

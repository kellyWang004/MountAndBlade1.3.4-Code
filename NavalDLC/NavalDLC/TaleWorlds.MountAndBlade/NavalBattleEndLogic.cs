using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade;

public class NavalBattleEndLogic : MissionLogic, IBattleEndLogic
{
	public enum ExitResult
	{
		False,
		NeedsPlayerConfirmation,
		True
	}

	public const float RetreatCheckDuration = 5f;

	private IMissionAgentSpawnLogic _missionSpawnLogic;

	private NavalShipsLogic _navalShipsLogic;

	private NavalAgentsLogic _navalAgentsLogic;

	private bool _notificationsDisabled;

	private MissionTime _enemySideNotYetRetreatingTime;

	private MissionTime _playerSideNotYetRetreatingTime;

	private BasicMissionTimer _checkRetreatingTimer;

	private bool _isPlayerSideRetreating;

	private bool _isEnemySideDepleted;

	private bool _isPlayerSideDepleted;

	private bool _canCheckForEndCondition = true;

	private bool _missionEndedMessageShown;

	private bool _victoryReactionsActivated;

	private bool _victoryReactionsActivatedForRetreating;

	private bool _scoreBoardOpenedOnceOnMissionEnd;

	public bool PlayerVictory
	{
		get
		{
			if (!IsEnemySideRetreating)
			{
				return _isEnemySideDepleted;
			}
			return true;
		}
	}

	public bool EnemyVictory
	{
		get
		{
			if (!_isPlayerSideRetreating)
			{
				return _isPlayerSideDepleted;
			}
			return true;
		}
	}

	public bool IsEnemySideRetreating { get; private set; }

	public override void OnBehaviorInitialize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		((MissionBehavior)this).OnBehaviorInitialize();
		_checkRetreatingTimer = new BasicMissionTimer();
		_missionSpawnLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
		_navalShipsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalShipsLogic>();
		_navalShipsLogic.MissionEndEvent += OnMissionEnd;
		_navalAgentsLogic = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalAgentsLogic>();
	}

	public override void OnMissionTick(float dt)
	{
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		if (!((MissionBehavior)this).Mission.IsDeploymentFinished)
		{
			return;
		}
		if (((MissionBehavior)this).Mission.IsMissionEnding)
		{
			if (_notificationsDisabled)
			{
				_scoreBoardOpenedOnceOnMissionEnd = true;
			}
			if (_missionEndedMessageShown && !_scoreBoardOpenedOnceOnMissionEnd)
			{
				if (_checkRetreatingTimer.ElapsedTime > 7f)
				{
					CheckIsEnemySideRetreatingOrOneSideDepleted();
					_checkRetreatingTimer.Reset();
					if (((MissionBehavior)this).Mission.MissionResult != null && ((MissionBehavior)this).Mission.MissionResult.PlayerDefeated)
					{
						GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost_press_tab_to_view_results", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
					else if (((MissionBehavior)this).Mission.MissionResult != null && ((MissionBehavior)this).Mission.MissionResult.PlayerVictory)
					{
						if (_isEnemySideDepleted)
						{
							GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won_press_tab_to_view_results", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
						}
					}
					else
					{
						GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished_press_tab_to_view_results", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
				}
			}
			else if (_checkRetreatingTimer.ElapsedTime > 3f && !_scoreBoardOpenedOnceOnMissionEnd)
			{
				if (((MissionBehavior)this).Mission.MissionResult != null && ((MissionBehavior)this).Mission.MissionResult.PlayerDefeated)
				{
					if (_isPlayerSideDepleted)
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
					else if (_isPlayerSideRetreating)
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_friendlies_are_fleeing_you_lost", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
				}
				else if (((MissionBehavior)this).Mission.MissionResult != null && ((MissionBehavior)this).Mission.MissionResult.PlayerVictory)
				{
					if (_isEnemySideDepleted)
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
					else if (IsEnemySideRetreating)
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_enemies_are_fleeing_you_won", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
					}
				}
				else
				{
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished", (string)null), 0, (BasicCharacterObject)null, (Equipment)null, "");
				}
				_missionEndedMessageShown = true;
				_checkRetreatingTimer.Reset();
			}
			if (_victoryReactionsActivated)
			{
				return;
			}
			AgentVictoryLogic missionBehavior = ((MissionBehavior)this).Mission.GetMissionBehavior<AgentVictoryLogic>();
			if (missionBehavior != null)
			{
				CheckIsEnemySideRetreatingOrOneSideDepleted();
				if (_isEnemySideDepleted)
				{
					missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(((MissionBehavior)this).Mission.PlayerTeam.Side);
					_victoryReactionsActivated = true;
				}
				else if (_isPlayerSideDepleted)
				{
					missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(((MissionBehavior)this).Mission.PlayerEnemyTeam.Side);
					_victoryReactionsActivated = true;
				}
				else if (IsEnemySideRetreating && !_victoryReactionsActivatedForRetreating)
				{
					missionBehavior.SetTimersOfVictoryReactionsOnRetreat(((MissionBehavior)this).Mission.PlayerTeam.Side);
					_victoryReactionsActivatedForRetreating = true;
				}
				else if (_isPlayerSideRetreating && !_victoryReactionsActivatedForRetreating)
				{
					missionBehavior.SetTimersOfVictoryReactionsOnRetreat(((MissionBehavior)this).Mission.PlayerEnemyTeam.Side);
					_victoryReactionsActivatedForRetreating = true;
				}
			}
		}
		else if (_checkRetreatingTimer.ElapsedTime > 1f)
		{
			CheckIsEnemySideRetreatingOrOneSideDepleted();
			_checkRetreatingTimer.Reset();
		}
	}

	public override bool MissionEnded(ref MissionResult missionResult)
	{
		bool flag = false;
		if (IsEnemySideRetreating || _isEnemySideDepleted)
		{
			missionResult = MissionResult.CreateSuccessful((IMission)(object)((MissionBehavior)this).Mission, IsEnemySideRetreating);
			flag = true;
		}
		else if (_isPlayerSideRetreating || _isPlayerSideDepleted)
		{
			missionResult = MissionResult.CreateDefeated((IMission)(object)((MissionBehavior)this).Mission);
			flag = true;
		}
		if (flag)
		{
			_missionSpawnLogic.StopSpawner((BattleSideEnum)1);
			_missionSpawnLogic.StopSpawner((BattleSideEnum)0);
		}
		return flag;
	}

	public override void OnMissionStateFinalized()
	{
		_navalShipsLogic.MissionEndEvent -= OnMissionEnd;
	}

	private void OnMissionEnd()
	{
		if (!IsEnemySideRetreating)
		{
			return;
		}
		foreach (Agent item in (List<Agent>)(object)((MissionBehavior)this).Mission.PlayerEnemyTeam.ActiveAgents)
		{
			IAgentOriginBase origin = item.Origin;
			if (origin != null)
			{
				origin.SetRouted(true);
			}
		}
		MBList<MissionShip> val = new MBList<MissionShip>();
		_navalShipsLogic.FillTeamShips((TeamSideEnum)2, val);
		MBList<IAgentOriginBase> val2 = new MBList<IAgentOriginBase>();
		foreach (MissionShip item2 in (List<MissionShip>)(object)val)
		{
			_navalAgentsLogic.FillReservedTroopsOfShip(item2, val2);
		}
		foreach (IAgentOriginBase item3 in (List<IAgentOriginBase>)(object)val2)
		{
			item3.SetRouted(true);
		}
	}

	public void ChangeCanCheckForEndCondition(bool canCheckForEndCondition)
	{
		_canCheckForEndCondition = canCheckForEndCondition;
	}

	public ExitResult TryExit()
	{
		if (GameNetwork.IsClientOrReplay)
		{
			return ExitResult.False;
		}
		Agent mainAgent = ((MissionBehavior)this).Mission.MainAgent;
		if ((mainAgent != null && mainAgent.IsActive() && ((MissionBehavior)this).Mission.IsPlayerCloseToAnEnemy(5f)) || (!((MissionBehavior)this).Mission.MissionEnded && (PlayerVictory || EnemyVictory)))
		{
			return ExitResult.False;
		}
		if (!((MissionBehavior)this).Mission.MissionEnded && !IsEnemySideRetreating)
		{
			return ExitResult.NeedsPlayerConfirmation;
		}
		((MissionBehavior)this).Mission.EndMission();
		return ExitResult.True;
	}

	public void SetNotificationDisabled(bool value)
	{
		_notificationsDisabled = value;
	}

	private void CheckIsEnemySideRetreatingOrOneSideDepleted()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		if (!_canCheckForEndCondition)
		{
			return;
		}
		BattleSideEnum side = ((MissionBehavior)this).Mission.PlayerTeam.Side;
		BattleSideEnum oppositeSide = Extensions.GetOppositeSide(side);
		bool flag = false;
		bool flag2 = false;
		foreach (Team item in (List<Team>)(object)Mission.Current.Teams)
		{
			if (item.IsPlayerTeam || item.IsPlayerAlly)
			{
				if (!flag)
				{
					foreach (Formation item2 in (List<Formation>)(object)item.FormationsIncludingEmpty)
					{
						if (item2.CountOfUnits > 0)
						{
							flag = true;
							break;
						}
					}
				}
			}
			else if (item.IsEnemyOf(((MissionBehavior)this).Mission.PlayerTeam) && !flag2)
			{
				foreach (Formation item3 in (List<Formation>)(object)item.FormationsIncludingEmpty)
				{
					if (item3.CountOfUnits > 0)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag && flag2)
			{
				break;
			}
		}
		_isPlayerSideDepleted = !flag || _missionSpawnLogic.IsSideDepleted(side) || CheckAllShipsAgentsNotOnShip(side);
		_isEnemySideDepleted = !flag2 || _missionSpawnLogic.IsSideDepleted(oppositeSide) || CheckAllShipsAgentsNotOnShip(oppositeSide);
		if (_isEnemySideDepleted || _isPlayerSideDepleted)
		{
			return;
		}
		if (((MissionBehavior)this).Mission.MainAgent != null && ((MissionBehavior)this).Mission.MainAgent.IsPlayerControlled && ((MissionBehavior)this).Mission.MainAgent.IsActive())
		{
			_playerSideNotYetRetreatingTime = MissionTime.Now;
		}
		else
		{
			bool flag3 = true;
			foreach (MissionShip item4 in (List<MissionShip>)(object)_navalShipsLogic.AllShips)
			{
				if (item4.Team != null && item4.Team.Side == side && !item4.IsRetreating)
				{
					flag3 = false;
					break;
				}
			}
			if (!flag3)
			{
				_playerSideNotYetRetreatingTime = MissionTime.Now;
			}
		}
		if (((MissionTime)(ref _playerSideNotYetRetreatingTime)).ElapsedSeconds > 5f)
		{
			_isPlayerSideRetreating = true;
		}
		bool flag4 = true;
		foreach (MissionShip item5 in (List<MissionShip>)(object)_navalShipsLogic.AllShips)
		{
			if (item5.Team != null && item5.Team.Side == oppositeSide && !item5.IsRetreating)
			{
				flag4 = false;
				break;
			}
		}
		if (!flag4)
		{
			_enemySideNotYetRetreatingTime = MissionTime.Now;
		}
		if (((MissionTime)(ref _enemySideNotYetRetreatingTime)).ElapsedSeconds > 5f)
		{
			IsEnemySideRetreating = true;
		}
	}

	private bool CheckAllShipsAgentsNotOnShip(BattleSideEnum side)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		foreach (MissionShip item in (List<MissionShip>)(object)_navalShipsLogic.AllShips)
		{
			if (item.Team == null || item.Team.Side != side)
			{
				continue;
			}
			foreach (Agent item2 in (List<Agent>)(object)_navalAgentsLogic.GetActiveAgentsOfShip(item))
			{
				if (item2.GetComponent<AgentNavalComponent>()?.SteppedShip != null)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}
}

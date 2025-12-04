using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace NavalDLC.ViewModelCollection;

public class NavalCustomBattleScoreboardVM : CustomBattleScoreboardVM
{
	private class ScoreboardShipComparer : IComparer<SPScoreboardShipVM>
	{
		public int Compare(SPScoreboardShipVM x, SPScoreboardShipVM y)
		{
			bool isPlayerShip = x.Ship.IsPlayerShip;
			int num = y.Ship.IsPlayerShip.CompareTo(isPlayerShip);
			if (num != 0)
			{
				return num;
			}
			num = y.IsPlayerTeam.CompareTo(x.IsPlayerTeam);
			if (num != 0)
			{
				return num;
			}
			return ResolveEquality(x, y);
		}

		private int ResolveEquality(SPScoreboardShipVM x, SPScoreboardShipVM y)
		{
			return y.Ship.MaxHitPoints.CompareTo(x.Ship.MaxHitPoints);
		}
	}

	private NavalShipsLogic _navalShipsLogic;

	private readonly ScoreboardShipComparer _scoreboardShipComparer = new ScoreboardShipComparer();

	public NavalCustomBattleScoreboardVM()
	{
		SPScoreboardShipVM.GetTooltip = GetShipTooltip;
		((ScoreboardBaseVM)this).IsNavalBattle = true;
	}

	public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
	{
		((CustomBattleScoreboardVM)this).Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
		_navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
	}

	protected override void OnTick(float dt)
	{
		((CustomBattleScoreboardVM)this).OnTick(dt);
		if (_navalShipsLogic != null)
		{
			for (int i = 0; i < ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Attackers.Ships).Count; i++)
			{
				SPScoreboardShipVM val = ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Attackers.Ships)[i];
				ShipAssignment shipAssignment;
				bool flag = _navalShipsLogic.FindAssignmentOfShipOrigin(val.Ship, out shipAssignment);
				val.CurrentHealth = (flag ? shipAssignment.MissionShip.HitPoints : 0f);
			}
			for (int j = 0; j < ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Defenders.Ships).Count; j++)
			{
				SPScoreboardShipVM val2 = ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Defenders.Ships)[j];
				ShipAssignment shipAssignment2;
				bool flag2 = _navalShipsLogic.FindAssignmentOfShipOrigin(val2.Ship, out shipAssignment2);
				val2.CurrentHealth = (flag2 ? shipAssignment2.MissionShip.HitPoints : 0f);
			}
		}
	}

	public override void OnFinalize()
	{
		((ScoreboardBaseVM)this).OnFinalize();
		SPScoreboardShipVM.GetTooltip = null;
	}

	public override void OnDeploymentFinished()
	{
		((ScoreboardBaseVM)this).OnDeploymentFinished();
		UpdateTeamShips();
	}

	private void UpdateTeamShips()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		ShipAssignment shipAssignment;
		for (int num = ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Attackers.Ships).Count - 1; num >= 0; num--)
		{
			if (!_navalShipsLogic.FindAssignmentOfShipOrigin(((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Attackers.Ships)[num].Ship, out shipAssignment))
			{
				((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Attackers.Ships).RemoveAt(num);
			}
		}
		for (int num2 = ((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Defenders.Ships).Count - 1; num2 >= 0; num2--)
		{
			if (!_navalShipsLogic.FindAssignmentOfShipOrigin(((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Defenders.Ships)[num2].Ship, out shipAssignment))
			{
				((Collection<SPScoreboardShipVM>)(object)((ScoreboardBaseVM)this).Defenders.Ships).RemoveAt(num2);
			}
		}
		MBList<MissionShip> val = new MBList<MissionShip>();
		_navalShipsLogic.FillTeamShips(Mission.Current.AttackerTeam.TeamSide, val);
		MBList<MissionShip> val2 = new MBList<MissionShip>();
		if (Mission.Current.AttackerAllyTeam != null)
		{
			_navalShipsLogic.FillTeamShips(Mission.Current.AttackerAllyTeam.TeamSide, val2);
		}
		MBList<MissionShip> val3 = new MBList<MissionShip>();
		_navalShipsLogic.FillTeamShips(Mission.Current.DefenderTeam.TeamSide, val3);
		MBList<MissionShip> val4 = new MBList<MissionShip>();
		if (Mission.Current.DefenderAllyTeam != null)
		{
			_navalShipsLogic.FillTeamShips(Mission.Current.DefenderAllyTeam.TeamSide, val4);
		}
		for (int i = 0; i < ((List<MissionShip>)(object)val).Count; i++)
		{
			MissionShip missionShip = ((List<MissionShip>)(object)val)[i];
			((ScoreboardBaseVM)this).Attackers.GetShipAddIfNotExists(missionShip.ShipOrigin, ((object)missionShip.ShipOrigin.Hull.Type/*cast due to .constrained prefix*/).ToString(), (IBattleCombatant)null, Mission.Current.AttackerTeam.TeamSide);
		}
		for (int j = 0; j < ((List<MissionShip>)(object)val2).Count; j++)
		{
			MissionShip missionShip2 = ((List<MissionShip>)(object)val2)[j];
			((ScoreboardBaseVM)this).Attackers.GetShipAddIfNotExists(missionShip2.ShipOrigin, ((object)missionShip2.ShipOrigin.Hull.Type/*cast due to .constrained prefix*/).ToString(), (IBattleCombatant)null, Mission.Current.AttackerAllyTeam.TeamSide);
		}
		for (int k = 0; k < ((List<MissionShip>)(object)val3).Count; k++)
		{
			MissionShip missionShip3 = ((List<MissionShip>)(object)val3)[k];
			((ScoreboardBaseVM)this).Defenders.GetShipAddIfNotExists(missionShip3.ShipOrigin, ((object)missionShip3.ShipOrigin.Hull.Type/*cast due to .constrained prefix*/).ToString(), (IBattleCombatant)null, Mission.Current.DefenderTeam.TeamSide);
		}
		for (int l = 0; l < ((List<MissionShip>)(object)val4).Count; l++)
		{
			MissionShip missionShip4 = ((List<MissionShip>)(object)val4)[l];
			((ScoreboardBaseVM)this).Defenders.GetShipAddIfNotExists(missionShip4.ShipOrigin, ((object)missionShip4.ShipOrigin.Hull.Type/*cast due to .constrained prefix*/).ToString(), (IBattleCombatant)null, Mission.Current.DefenderAllyTeam.TeamSide);
		}
		((ScoreboardBaseVM)this).Attackers.Ships.Sort((IComparer<SPScoreboardShipVM>)_scoreboardShipComparer);
		((ScoreboardBaseVM)this).Defenders.Ships.Sort((IComparer<SPScoreboardShipVM>)_scoreboardShipComparer);
	}

	private List<TooltipProperty> GetShipTooltip(SPScoreboardShipVM shipVM)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Expected O, but got Unknown
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Expected O, but got Unknown
		IShipOrigin ship = shipVM.Ship;
		List<TooltipProperty> list = new List<TooltipProperty>
		{
			new TooltipProperty(((object)ship.Name).ToString(), string.Empty, 0, false, (TooltipPropertyFlags)4096)
		};
		if (shipVM.IsDestroyed)
		{
			list.Add(new TooltipProperty(string.Empty, ((object)new TextObject("{=w8Yzf0F0}Destroyed", (Dictionary<string, object>)null)).ToString(), -1, false, (TooltipPropertyFlags)0));
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, (TooltipPropertyFlags)0));
		}
		list.Add(new TooltipProperty(((object)new TextObject("{=sqdzHOPe}Class", (Dictionary<string, object>)null)).ToString(), ((object)GameTexts.FindText("str_ship_type", ((object)ship.Hull.Type/*cast due to .constrained prefix*/).ToString().ToLowerInvariant())).ToString(), 0, false, (TooltipPropertyFlags)0));
		MissionShip missionShip = null;
		if (_navalShipsLogic != null && _navalShipsLogic.FindAssignmentOfShipOrigin(ship, out var shipAssignment))
		{
			missionShip = shipAssignment.MissionShip;
		}
		if (missionShip == null)
		{
			string text = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_no_space", (string)null).SetTextVariable("LEFT", (int)ship.HitPoints).SetTextVariable("RIGHT", (int)ship.MaxHitPoints)).ToString();
			list.Add(new TooltipProperty(((object)new TextObject("{=oBbiVeKE}Hit Points", (Dictionary<string, object>)null)).ToString(), text, 0, false, (TooltipPropertyFlags)0));
		}
		else
		{
			string text2 = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_no_space", (string)null).SetTextVariable("LEFT", (int)missionShip.HitPoints).SetTextVariable("RIGHT", (int)ship.MaxHitPoints)).ToString();
			list.Add(new TooltipProperty(((object)new TextObject("{=oBbiVeKE}Hit Points", (Dictionary<string, object>)null)).ToString(), text2, 0, false, (TooltipPropertyFlags)0));
			string text3 = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_no_space", (string)null).SetTextVariable("LEFT", (missionShip.Formation != null) ? missionShip.Formation.CountOfUnits : 0).SetTextVariable("RIGHT", missionShip.CrewSizeOnMainDeck)).ToString();
			list.Add(new TooltipProperty(((object)new TextObject("{=aClquusd}Troop Count", (Dictionary<string, object>)null)).ToString(), text3, 0, false, (TooltipPropertyFlags)0));
		}
		List<ShipSlotAndPieceName> shipSlotAndPieceNames = ship.GetShipSlotAndPieceNames();
		if (shipSlotAndPieceNames.Count > 0)
		{
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, (TooltipPropertyFlags)1024)
			{
				OnlyShowWhenExtended = true
			});
			list.Add(new TooltipProperty(string.Empty, ((object)new TextObject("{=zMvUzdKR}Ship Upgrades", (Dictionary<string, object>)null)).ToString(), -1, false, (TooltipPropertyFlags)0)
			{
				OnlyShowWhenExtended = true
			});
			foreach (ShipSlotAndPieceName item in shipSlotAndPieceNames)
			{
				list.Add(new TooltipProperty(item.SlotName, item.PieceName, 0, false, (TooltipPropertyFlags)0)
				{
					OnlyShowWhenExtended = true
				});
			}
		}
		if (shipSlotAndPieceNames.Count > 0)
		{
			if (Input.IsGamepadActive)
			{
				GameTexts.SetVariable("EXTEND_KEY", ((object)GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", "MapFollowModifier")).ToString());
			}
			else
			{
				GameTexts.SetVariable("EXTEND_KEY", ((object)Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt")).ToString());
			}
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, (TooltipPropertyFlags)0)
			{
				OnlyShowWhenNotExtended = true
			});
			list.Add(new TooltipProperty(string.Empty, ((object)GameTexts.FindText("str_map_tooltip_info", (string)null)).ToString(), -1, false, (TooltipPropertyFlags)0)
			{
				OnlyShowWhenNotExtended = true
			});
		}
		return list;
	}
}

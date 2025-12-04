using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.HUD.ShipMarker;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.MissionViews;

[OverrideView(typeof(NavalMissionShipMarkerUIHandler))]
public class MissionGauntletNavalShipMarker : MissionBattleUIBaseView
{
	private NavalShipMarkersVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private NavalShipTargetSelectionHandler _shipTargetHandler;

	private NavalShipsLogic _navalShipsLogic;

	private MBReadOnlyList<MissionShip> _focusedShipsCache;

	private readonly Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

	private float _fadeOutTimer;

	protected override void OnCreateView()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_dataSource = new NavalShipMarkersVM(((MissionBehavior)this).Mission);
		_gauntletLayer = new GauntletLayer("NavalShipMarker", ((MissionView)this).ViewOrderPriority, false);
		_gauntletLayer.LoadMovie("NavalShipMarker", (ViewModel)(object)_dataSource);
		((ScreenBase)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)(object)_gauntletLayer);
		_shipTargetHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalShipTargetSelectionHandler>();
		_navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		if (_shipTargetHandler != null)
		{
			_shipTargetHandler.OnShipsFocused += OnShipFocusedFromHandler;
		}
	}

	protected override void OnDestroyView()
	{
		if (_shipTargetHandler != null)
		{
			_shipTargetHandler.OnShipsFocused -= OnShipFocusedFromHandler;
		}
		((ScreenBase)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)(object)_gauntletLayer);
		_gauntletLayer = null;
		((ViewModel)_dataSource).OnFinalize();
		_dataSource = null;
	}

	protected override void OnSuspendView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, true);
		}
	}

	protected override void OnResumeView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, false);
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		((MissionBattleUIBaseView)this).OnMissionScreenTick(dt);
		if (((MissionBattleUIBaseView)this).IsViewCreated)
		{
			if ((int)((MissionBehavior)this).Mission.Mode != 6)
			{
				_dataSource.IsEnabled = ((MissionView)this).Input.IsGameKeyDown(5) || ((MissionBehavior)this).Mission.IsOrderMenuOpen;
			}
			_dataSource.IsShipTargetingRelevant = _shipTargetHandler != null && ((MissionBehavior)this).Mission.IsOrderMenuOpen;
			if (_dataSource.IsEnabled)
			{
				_dataSource.RefreshShipMarkers();
				RefreshShipTargetProperties();
				UpdateMarkerPositions(_fadeOutTimer < 0f);
				_fadeOutTimer = 2f;
			}
			else if (_fadeOutTimer >= 0f)
			{
				_dataSource.RefreshShipMarkers();
				_fadeOutTimer -= dt;
				UpdateMarkerPositions(isFirstFrame: false);
			}
		}
	}

	private void UpdateMarkerPositions(bool isFirstFrame)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers).Count; i++)
		{
			NavalShipMarkerItemVM navalShipMarkerItemVM = ((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers)[i];
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			Vec3 val = Vec3.Invalid;
			if (navalShipMarkerItemVM.IsShipActive())
			{
				val = navalShipMarkerItemVM.Ship.GlobalFrame.origin;
			}
			else
			{
				WorldPosition cachedMedianPosition = navalShipMarkerItemVM.Formation.CachedMedianPosition;
				val = ((WorldPosition)(ref cachedMedianPosition)).GetNavMeshVec3();
			}
			if (((Vec3)(ref val)).IsValid)
			{
				MBWindowManager.WorldToScreen(((MissionView)this).MissionScreen.CombatCamera, val + _heightOffset, ref num, ref num2, ref num3);
				if (!MathF.IsValidValue(num3) || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2))
				{
					num = -10000f;
					num2 = -10000f;
					num3 = -1f;
				}
				navalShipMarkerItemVM.WSign = ((!(num3 < 0f)) ? 1 : (-1));
			}
			if (!navalShipMarkerItemVM.IsTargetingAShip && (!((Vec3)(ref val)).IsValid || !MathF.IsValidValue(num3) || num3 < 0f || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2)))
			{
				num = -10000f;
				num2 = -10000f;
				num3 = 0f;
			}
			if (isFirstFrame)
			{
				navalShipMarkerItemVM.ScreenPosition = new Vec2(num, num2);
			}
			else
			{
				navalShipMarkerItemVM.ScreenPosition = Vec2.Lerp(navalShipMarkerItemVM.ScreenPosition, new Vec2(num, num2), 0.9f);
			}
			if (((Vec3)(ref val)).IsValid)
			{
				Agent main = Agent.Main;
				Vec3 position;
				float distance;
				if (main == null || !main.IsActive())
				{
					position = ((MissionView)this).MissionScreen.CombatCamera.Position;
					distance = ((Vec3)(ref position)).Distance(val);
				}
				else
				{
					position = Agent.Main.Position;
					distance = ((Vec3)(ref position)).Distance(val);
				}
				navalShipMarkerItemVM.Distance = distance;
			}
		}
	}

	private unsafe void RefreshShipTargetProperties()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Invalid comparison between Unknown and I4
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		if (!_dataSource.IsShipTargetingRelevant)
		{
			for (int i = 0; i < ((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers).Count; i++)
			{
				((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers)[i].SetTargetedState(isFocused: false, isTargetingAShip: false);
			}
			return;
		}
		List<MissionShip> list = new List<MissionShip>();
		List<Formation> list2 = new List<Formation>();
		Agent main = Agent.Main;
		object obj;
		if (main == null)
		{
			obj = null;
		}
		else
		{
			OrderController playerOrderController = main.Team.PlayerOrderController;
			obj = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null);
		}
		MBReadOnlyList<Formation> val = (MBReadOnlyList<Formation>)obj;
		if (val != null)
		{
			for (int j = 0; j < ((List<Formation>)(object)val).Count; j++)
			{
				MissionShip missionShip = _navalShipsLogic.GetShipAssignment(((List<Formation>)(object)val)[j].Team.TeamSide, ((List<Formation>)(object)val)[j].FormationIndex)?.MissionShip;
				if (missionShip == null)
				{
					continue;
				}
				if (((List<Formation>)(object)val)[j].TargetFormation != null)
				{
					MovementOrder readonlyMovementOrderReference = Unsafe.Read<MovementOrder>((void*)((List<Formation>)(object)val)[j].GetReadonlyMovementOrderReference());
					if ((int)((MovementOrder)(ref readonlyMovementOrderReference)).OrderType == 4 || (int)((MovementOrder)(ref readonlyMovementOrderReference)).OrderType == 12)
					{
						list2.Add(((List<Formation>)(object)val)[j].TargetFormation);
					}
				}
				if (missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Engage && missionShip.ShipOrder.TargetShip != null && !missionShip.ShipOrder.IsAutoSelectingTargetShip)
				{
					list.Add(missionShip.ShipOrder.TargetShip);
				}
			}
		}
		for (int k = 0; k < ((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers).Count; k++)
		{
			NavalShipMarkerItemVM navalShipMarkerItemVM = ((Collection<NavalShipMarkerItemVM>)(object)_dataSource.ShipMarkers)[k];
			if (navalShipMarkerItemVM.TeamType == 2)
			{
				bool isTargetingAShip = list.Contains(navalShipMarkerItemVM.Ship) || list2.Contains(navalShipMarkerItemVM.Formation);
				navalShipMarkerItemVM.SetTargetedState(((List<MissionShip>)(object)_focusedShipsCache)?.Contains(navalShipMarkerItemVM.Ship) ?? false, isTargetingAShip);
			}
		}
	}

	private void OnShipFocusedFromHandler(MBReadOnlyList<MissionShip> focusedShips)
	{
		_focusedShipsCache = focusedShips;
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		if (((MissionBattleUIBaseView)this).IsViewCreated)
		{
			_gauntletLayer.UIContext.ContextAlpha = 0f;
		}
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		if (((MissionBattleUIBaseView)this).IsViewCreated)
		{
			_gauntletLayer.UIContext.ContextAlpha = 1f;
		}
	}
}

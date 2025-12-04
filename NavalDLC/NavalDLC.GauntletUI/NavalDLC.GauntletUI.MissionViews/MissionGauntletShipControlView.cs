using System;
using System.Collections.Generic;
using System.Numerics;
using NavalDLC.Missions;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipInput;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Missions.Interaction;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.MissionViews;

[OverrideView(typeof(MissionShipControlView))]
public class MissionGauntletShipControlView : MissionShipControlView
{
	[Flags]
	public enum ShipControlFeatureFlags
	{
		ShipFocus = 1,
		ShipSelection = 2,
		AttemptBoarding = 4,
		ToggleOarsmen = 8,
		ToggleSails = 0x10,
		CutLoose = 0x20,
		BallistaOrder = 0x40,
		ShootBallista = 0x80,
		ChangeCamera = 0x100
	}

	private GauntletLayer _gauntletLayer;

	private MissionShipControlVM _dataSource;

	private MissionGauntletSingleplayerOrderUIHandler _orderUIHandler;

	private MissionGauntletCrosshair _crosshairView;

	private NavalMissionShipHighlightView _shipHighlightView;

	private MissionShip _playerControlledShip;

	private MissionShip _focusedShip;

	private bool _playerControlledShipHasHybridSails;

	private bool _isAnyBridgeActive;

	private bool _isAnyEnemyShipConnected;

	private const float AttemptBoardingDistance = 50f;

	private const float SelectShipDistance = 300f;

	public ShipControlFeatureFlags SuspendedFeatures { get; private set; }

	public override void OnMissionScreenInitialize()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		((MissionBattleUIBaseView)this).OnMissionScreenInitialize();
		_dataSource = new MissionShipControlVM((IInteractionInterfaceHandler)(object)((MissionBehavior)this).Mission.GetMissionBehavior<MissionGauntletAgentStatus>());
		_gauntletLayer = new GauntletLayer("MissionShipControl", 10, false);
		_gauntletLayer.LoadMovie("MissionShipControl", (ViewModel)(object)_dataSource);
		_orderUIHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
		_shipHighlightView = ((MissionBehavior)this).Mission.GetMissionBehavior<NavalMissionShipHighlightView>();
		_crosshairView = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionGauntletCrosshair>();
		((ScreenLayer)_gauntletLayer).InputRestrictions.SetInputRestrictions(false, (InputUsageMask)0);
		if (!((ScreenLayer)((MissionView)this).MissionScreen.SceneLayer).Input.IsCategoryRegistered(HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory")))
		{
			((ScreenLayer)((MissionView)this).MissionScreen.SceneLayer).Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory"));
		}
		((ScreenBase)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)(object)_gauntletLayer);
		SetControlKeys();
		ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, true);
	}

	public override void OnMissionScreenFinalize()
	{
		((MissionBattleUIBaseView)this).OnMissionScreenFinalize();
		((ViewModel)_dataSource).OnFinalize();
		((ScreenBase)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)(object)_gauntletLayer);
		_dataSource = null;
		_gauntletLayer = null;
	}

	protected override void OnCreateView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, false);
		}
	}

	protected override void OnDestroyView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, true);
		}
	}

	public void SuspendFeature(ShipControlFeatureFlags feature)
	{
		SuspendedFeatures |= feature;
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

	public void ResumeFeature(ShipControlFeatureFlags feature)
	{
		SuspendedFeatures &= ~feature;
	}

	public bool IsFeatureSuspended(ShipControlFeatureFlags feature)
	{
		return (SuspendedFeatures & feature) != 0;
	}

	public override void OnPhotoModeActivated()
	{
		((MissionView)this).OnPhotoModeActivated();
		if (_gauntletLayer != null)
		{
			_gauntletLayer.UIContext.ContextAlpha = 0f;
		}
	}

	public override void OnPhotoModeDeactivated()
	{
		((MissionView)this).OnPhotoModeDeactivated();
		if (_gauntletLayer != null)
		{
			_gauntletLayer.UIContext.ContextAlpha = 1f;
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		((MissionBattleUIBaseView)this).OnMissionScreenTick(dt);
		MissionShip playerControlledShip = _playerControlledShip;
		_playerControlledShip = NavalShipsLogic?.PlayerControlledShip;
		if (_dataSource != null)
		{
			_dataSource.IsControllingShip = _playerControlledShip != null;
		}
		if (_playerControlledShip != null)
		{
			_isAnyBridgeActive = _playerControlledShip.GetIsAnyBridgeActive();
			_isAnyEnemyShipConnected = _playerControlledShip.GetIsConnectedToEnemy();
			RefreshControlKeys();
			UpdateFocusedShip(dt);
			UpdateShipValues(dt);
			TickInput();
		}
		else
		{
			_playerControlledShipHasHybridSails = false;
			_dataSource?.UpdateSelectShipInteraction(canSelectShip: false);
			_dataSource?.UpdateAttemptBoardingInteraction(canAttemptBoarding: false, isBoardingBlocked: false);
			UpdateFocusedShip(dt);
			if (playerControlledShip != null && IsAimingWithRangedWeapon)
			{
				IsAimingWithRangedWeapon = false;
				playerControlledShip.OnSetRangedWeaponControlMode(value: false);
			}
			TickInput();
		}
		if (_dataSource != null)
		{
			_dataSource.IsUsingBallista = base.IsAimingWithRangedWeaponAndAllowed;
			if (base.IsAimingWithRangedWeaponAndAllowed)
			{
				_dataSource.BallistaAmmoCount = ((_playerControlledShip?.ShipSiegeWeapon != null && !((MissionObject)_playerControlledShip.ShipSiegeWeapon).IsDisabled && !((UsableMachine)_playerControlledShip.ShipSiegeWeapon).IsDeactivated) ? _playerControlledShip.ShipSiegeWeapon.AmmoCount : 0);
				_dataSource.IsAmmoCountWarned = _dataSource.BallistaAmmoCount <= 3;
			}
			MissionShipControlVM dataSource = _dataSource;
			MissionShip playerControlledShip2 = _playerControlledShip;
			dataSource.IsCutLooseOrderActive = playerControlledShip2 != null && playerControlledShip2.ShipOrder.GetIsCuttingLoose() && _isAnyBridgeActive;
			_dataSource.IsAttemptBoardingOrderActive = _playerControlledShip?.ShipOrder.GetIsAttemptingBoarding() ?? false;
		}
		if (playerControlledShip == _playerControlledShip)
		{
			return;
		}
		if (_playerControlledShip != null)
		{
			MissionGauntletCrosshair crosshairView = _crosshairView;
			if (crosshairView != null)
			{
				((MissionView)crosshairView).SuspendView();
			}
		}
		else
		{
			MissionGauntletCrosshair crosshairView2 = _crosshairView;
			if (crosshairView2 != null)
			{
				((MissionView)crosshairView2).ResumeView();
			}
		}
	}

	private void UpdateHitPoints()
	{
		if (_dataSource != null)
		{
			if (_playerControlledShip == null)
			{
				_dataSource.ShipHitPoints.IsRelevant = false;
				_dataSource.SailHitPoints.IsRelevant = false;
				_dataSource.FireHitPoints.IsRelevant = false;
				return;
			}
			_dataSource.ShipHitPoints.IsRelevant = true;
			_dataSource.SailHitPoints.IsRelevant = true;
			_dataSource.FireHitPoints.IsRelevant = true;
			_dataSource.ShipHitPoints.ActiveHitPoints = MathF.Round(_playerControlledShip.HitPoints);
			_dataSource.ShipHitPoints.MaxHitPoints = MathF.Round(_playerControlledShip.MaxHealth);
			_dataSource.SailHitPoints.ActiveHitPoints = MathF.Round(_playerControlledShip.SailHitPoints);
			_dataSource.SailHitPoints.MaxHitPoints = MathF.Round(_playerControlledShip.MaxSailHitPoints);
			_dataSource.FireHitPoints.ActiveHitPoints = MathF.Round(_playerControlledShip.FireHitPoints);
			_dataSource.FireHitPoints.MaxHitPoints = MathF.Round(_playerControlledShip.MaxFireHealth);
		}
	}

	public override void OnPreDisplayMissionTick(float dt)
	{
		((MissionBehavior)this).OnPreDisplayMissionTick(dt);
		if (_playerControlledShip != null)
		{
			((MissionBehavior)this).Mission.CameraIsFirstPerson = false;
		}
	}

	private void TickInput()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Expected O, but got Unknown
		if (_playerControlledShip == null)
		{
			Agent main = Agent.Main;
			MissionShip missionShip = ((main == null) ? null : main.GetComponent<AgentNavalComponent>()?.FormationShip);
			bool flag = missionShip?.IsAgentUsingSiegeWeapon(((MissionBehavior)this).Mission.MainAgent) ?? false;
			if (flag != IsAimingWithRangedWeapon)
			{
				IsAimingWithRangedWeapon = flag;
				missionShip?.OnSetRangedWeaponControlMode(IsAimingWithRangedWeapon);
			}
		}
		MissionScreen missionScreen = ((MissionView)this).MissionScreen;
		object obj;
		if (missionScreen == null)
		{
			obj = null;
		}
		else
		{
			SceneLayer sceneLayer = missionScreen.SceneLayer;
			obj = ((sceneLayer != null) ? ((ScreenLayer)sceneLayer).Input : null);
		}
		InputContext val = (InputContext)obj;
		if (val == null || _playerControlledShip == null || ((MissionView)this).MissionScreen.IsPhotoModeEnabled || base.IsDisplayingADialog)
		{
			return;
		}
		if (val.IsHotKeyReleased("ToggleOarsmen") && GetCanToggleOarsmen())
		{
			int num = (_playerControlledShip.ShipOrder.OarsmenLevel + 2) % 3;
			_playerControlledShip.ShipOrder.SetOrderOarsmenLevel(num);
			TextObject val2 = null;
			switch (num)
			{
			case 0:
				val2 = new TextObject("{=RtRNkfMA}Stop using the oars!", (Dictionary<string, object>)null);
				break;
			case 1:
				val2 = new TextObject("{=a7CzRLXb}Use oars in half power!", (Dictionary<string, object>)null);
				break;
			case 2:
				val2 = new TextObject("{=RKthVuaC}Use oars in full power!", (Dictionary<string, object>)null);
				break;
			}
			if (val2 != (TextObject)null)
			{
				DisplayCommandForSelectedFormations(val2);
			}
		}
		if (!((MissionView)this).MissionScreen.IsCheatGhostMode && val.IsHotKeyReleased("ToggleSail") && GetCanToggleSail())
		{
			if (SailControl.IsMax())
			{
				SailControl = SailControl.Min(_playerControlledShipHasHybridSails);
			}
			else
			{
				SailControl = SailControl.Raise(_playerControlledShipHasHybridSails);
			}
			switch (SailControl)
			{
			case SailInput.Raised:
				DisplayCommandForSelectedFormations(new TextObject("{=kWfyfiVA}Furl sails!", (Dictionary<string, object>)null));
				break;
			case SailInput.SquareSailsRaised:
				DisplayCommandForSelectedFormations(new TextObject("{=kGtL9Kea}Furl square sails!", (Dictionary<string, object>)null));
				break;
			case SailInput.Full:
				DisplayCommandForSelectedFormations(new TextObject("{=75VaP7bL}Open sails!", (Dictionary<string, object>)null));
				break;
			}
		}
		if (val.IsHotKeyReleased("CutLoose") && GetCanCutLoose() && !GetIsCutLooseTemporarilyBlocked())
		{
			_playerControlledShip.ShipOrder.SetCutLoose(enable: true);
			DisplayCommandForSelectedFormations(new TextObject("{=siE18G0C}Cut loose!", (Dictionary<string, object>)null));
		}
		if (val.IsHotKeyReleased("ChangeCamera") && GetCanChangeCamera())
		{
			base.ActiveCameraMode = (CameraModes)((int)(base.ActiveCameraMode + 1) % 3);
		}
		if (val.IsHotKeyReleased("SelectShip") && GetCanSelectShip())
		{
			int num2 = _focusedShip.Formation?.Index ?? (-1);
			if (num2 >= 0)
			{
				((GauntletOrderUIHandler)_orderUIHandler).SelectFormationAtIndex(num2);
			}
		}
		if (val.IsHotKeyReleased("AttemptBoarding") && GetCanAttemptBoarding() && !GetIsAttemptBoardingTemporarilyBlocked())
		{
			_playerControlledShip.ShipOrder.SetBoardingTargetShip(_focusedShip);
			DisplayCommandForSelectedFormations(new TextObject("{=6lRn6azK}Attempt boarding!", (Dictionary<string, object>)null));
		}
		if (val.IsHotKeyReleased("ToggleRangedWeaponDirectOrderMode") && GetCanToggleRangedWeaponOrderMode())
		{
			IsAimingWithRangedWeapon = !IsAimingWithRangedWeapon;
			_playerControlledShip.OnSetRangedWeaponControlMode(IsAimingWithRangedWeapon);
		}
		if (val.IsHotKeyReleased("ShootBallista") && GetCanShootBallista())
		{
			_playerControlledShip.ShootBallista();
		}
	}

	private void DisplayCommandForSelectedFormations(TextObject command)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		TextObject val = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", (Dictionary<string, object>)null);
		MissionShip playerControlledShip = _playerControlledShip;
		object obj;
		if (playerControlledShip == null)
		{
			obj = null;
		}
		else
		{
			IShipOrigin shipOrigin = playerControlledShip.ShipOrigin;
			obj = ((shipOrigin != null) ? shipOrigin.Name : null);
		}
		if (obj == null)
		{
			obj = (object)new TextObject("{=wXCM8BnW}Crew", (Dictionary<string, object>)null);
		}
		val.SetTextVariable("STR1", (TextObject)obj);
		val.SetTextVariable("STR2", command);
		InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString()));
	}

	private void UpdateFocusedShip(float dt)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		if ((NativeObject)(object)((MissionBehavior)this).Mission.Scene == (NativeObject)null || _playerControlledShip == null || base.IsDisplayingADialog || MBCommon.IsPaused || IsFeatureSuspended(ShipControlFeatureFlags.ShipFocus))
		{
			_dataSource?.SetTargetedShip(null);
			SetFocusedShip(null);
			return;
		}
		float enemyFocusDistance = 100f;
		float friendlyFocusDistance = 350f;
		MatrixFrame lastFinalRenderCameraFrame = ((MissionBehavior)this).Mission.Scene.LastFinalRenderCameraFrame;
		Vec2 screenCenter = Screen.RealScreenResolution * 0.5f;
		float closestDistance = float.MaxValue;
		float horizontalFocusRange = Screen.RealScreenResolutionWidth / 6f;
		float verticalFocusRange = Screen.RealScreenResolutionHeight / 6f;
		MissionShip closestShip = null;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)_playerControlledShip).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		Vec3 hitScreenPosition = Vec3.Zero;
		for (int i = 0; i < ((List<MissionShip>)(object)NavalShipsLogic.AllShips).Count; i++)
		{
			CheckFocusableShip(((List<MissionShip>)(object)NavalShipsLogic.AllShips)[i], globalPosition, enemyFocusDistance, friendlyFocusDistance, lastFinalRenderCameraFrame, screenCenter, ref hitScreenPosition, ref closestDistance, horizontalFocusRange, verticalFocusRange, ref closestShip, out var directHitFound);
			if (directHitFound)
			{
				break;
			}
		}
		SetFocusedShip(closestShip);
		if (_dataSource != null)
		{
			_dataSource.SetTargetedShip(closestShip, hitScreenPosition.x, hitScreenPosition.y - 110f, hitScreenPosition.z);
			_dataSource.TargetedShipHasAction = GetCanAttemptBoarding() || GetCanSelectShip();
		}
	}

	private void CheckFocusableShip(MissionShip focusableShip, Vec3 playerShipPosition, float enemyFocusDistance, float friendlyFocusDistance, MatrixFrame cameraFrame, Vec2 screenCenter, ref Vec3 hitScreenPosition, ref float closestDistance, float horizontalFocusRange, float verticalFocusRange, ref MissionShip closestShip, out bool directHitFound)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		directHitFound = false;
		if (((MissionObject)focusableShip).IsDisabled || focusableShip.IsSinking || focusableShip == _playerControlledShip)
		{
			return;
		}
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)focusableShip).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		if ((focusableShip.BattleSide == ((MissionBehavior)this).Mission.PlayerEnemyTeam.Side && ((Vec3)(ref globalPosition)).Distance(playerShipPosition) > enemyFocusDistance) || (focusableShip.BattleSide == ((MissionBehavior)this).Mission.PlayerTeam.Side && ((Vec3)(ref globalPosition)).Distance(playerShipPosition) > friendlyFocusDistance))
		{
			return;
		}
		Vec3 shipFocusPosition = GetShipFocusPosition(focusableShip);
		float num = -5000f;
		float num2 = -5000f;
		float num3 = -5000f;
		MBWindowManager.WorldToScreenInsideUsableArea(((MissionView)this).MissionScreen.CombatCamera, shipFocusPosition, ref num, ref num2, ref num3);
		float num4 = 0f;
		gameEntity = ((ScriptComponentBehavior)focusableShip).GameEntity;
		if (((WeakGameEntity)(ref gameEntity)).RayHitEntity(cameraFrame.origin, -cameraFrame.rotation.u, 1000f, ref num4))
		{
			hitScreenPosition = new Vec3(num, num2, num3, -1f);
			closestShip = focusableShip;
			directHitFound = true;
			return;
		}
		Vec2 val = default(Vec2);
		((Vec2)(ref val))._002Ector(num, num2);
		float num5 = ((Vec2)(ref val)).Distance(screenCenter);
		if (num3 > 0f && num5 < closestDistance && MathF.Abs(((Vec2)(ref screenCenter)).X - ((Vec2)(ref val)).X) < horizontalFocusRange && MathF.Abs(((Vec2)(ref screenCenter)).Y - ((Vec2)(ref val)).Y) < verticalFocusRange)
		{
			closestShip = focusableShip;
			closestDistance = num5;
			hitScreenPosition = new Vec3(num, num2, num3, -1f);
		}
	}

	private void SetFocusedShip(MissionShip ship)
	{
		_focusedShip = ship;
		_shipHighlightView?.OnShipFocused(ship);
	}

	private Vec3 GetShipFocusPosition(MissionShip ship)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)ship).GameEntity;
		return ((WeakGameEntity)(ref gameEntity)).GlobalPosition + Vec3.Up * 3f;
	}

	private void UpdateShipValues(float dt)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		if (_playerControlledShip == null || (NativeObject)(object)((MissionBehavior)this).Mission.Scene == (NativeObject)null || _dataSource == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = true;
		bool flag4 = true;
		foreach (MissionSail item in (List<MissionSail>)(object)_playerControlledShip.Sails)
		{
			if ((int)item.SailObject.Type == 1)
			{
				flag = true;
				if (item.TargetSailSetting <= 0f)
				{
					flag4 = false;
				}
			}
			else if ((int)item.SailObject.Type == 0)
			{
				flag2 = true;
				if (item.TargetSailSetting <= 0f)
				{
					flag3 = false;
				}
			}
		}
		_playerControlledShipHasHybridSails = flag && flag2;
		if (_playerControlledShipHasHybridSails)
		{
			if (flag4 && flag3)
			{
				_dataSource.SetSailState(SailInput.Full);
			}
			else if (!flag4 && !flag3)
			{
				_dataSource.SetSailState(SailInput.Raised);
			}
			else
			{
				_dataSource.SetSailState(SailInput.SquareSailsRaised);
			}
		}
		else if (flag)
		{
			if (flag4)
			{
				_dataSource.SetSailState(SailInput.Full);
			}
			else
			{
				_dataSource.SetSailState(SailInput.Raised);
			}
		}
		else if (flag3)
		{
			_dataSource.SetSailState(SailInput.Full);
		}
		else
		{
			_dataSource.SetSailState(SailInput.Raised);
		}
		_dataSource.SetOarsmanLevel(_playerControlledShip.ShipOrder.OarsmenLevel);
		_dataSource.SetSailType(flag, flag2);
		Vec2 val = ((MissionBehavior)this).Mission.Scene.GetGlobalWindStrengthVector();
		Vec2 to = ((Vec2)(ref val)).Normalized();
		MatrixFrame globalFrame = _playerControlledShip.GlobalFrame;
		val = ((Vec3)(ref globalFrame.rotation.f)).AsVec2;
		Vec2 val2 = ((Vec2)(ref val)).Normalized();
		MissionShipControlVM dataSource = _dataSource;
		val = GetProjection(val2, to);
		dataSource.ProjectedWindDirection = ((Vec2)(ref val)).Normalized();
		UpdateHitPoints();
	}

	private static Vec2 GetProjection(Vec2 from, Vec2 to)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vec2 val = ((Vec2)(ref from)).Normalized();
		Vec2 val2 = default(Vec2);
		((Vec2)(ref val2))._002Ector(0f - val.y, val.x);
		return Vec2.op_Implicit(new Vector2(Vec2.DotProduct(to, val), Vec2.DotProduct(to, val2)));
	}

	private void SetControlKeys()
	{
		GameKeyContext category = HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory");
		_dataSource.SetChangeCameraKey(category.GetHotKey("ChangeCamera"));
		_dataSource.SetCutLooseKey(category.GetHotKey("CutLoose"));
		_dataSource.SetToggleOarsmenKey(category.GetHotKey("ToggleOarsmen"));
		_dataSource.SetToggleSailKey(category.GetHotKey("ToggleSail"));
		_dataSource.SetToggleBallistaKey(category.GetHotKey("ToggleRangedWeaponDirectOrderMode"));
	}

	private void RefreshControlKeys()
	{
		if (_dataSource != null)
		{
			if (base.IsDisplayingADialog)
			{
				_dataSource.ChangeCameraKey.IsVisible = false;
				_dataSource.CutLooseKey.IsVisible = false;
				_dataSource.ToggleOarsmenKey.IsVisible = false;
				_dataSource.ToggleSailKey.IsVisible = false;
				_dataSource.ToggleBallistaKey.IsVisible = false;
				_dataSource.UpdateSelectShipInteraction(canSelectShip: false);
				_dataSource.UpdateAttemptBoardingInteraction(canAttemptBoarding: false, isBoardingBlocked: false);
			}
			else
			{
				_dataSource.ChangeCameraKey.IsVisible = GetCanChangeCamera();
				_dataSource.CutLooseKey.IsVisible = GetCanCutLoose();
				_dataSource.CutLooseKey.IsDisabled = GetIsCutLooseTemporarilyBlocked();
				_dataSource.ToggleOarsmenKey.IsVisible = GetCanToggleOarsmen();
				_dataSource.ToggleSailKey.IsVisible = GetCanToggleSail();
				_dataSource.ToggleBallistaKey.IsVisible = GetCanToggleRangedWeaponOrderMode();
				_dataSource.UpdateSelectShipInteraction(GetCanSelectShip());
				_dataSource.UpdateAttemptBoardingInteraction(GetCanAttemptBoarding(), GetIsAttemptBoardingTemporarilyBlocked());
			}
		}
	}

	private bool GetCanAttemptBoarding()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (IsFeatureSuspended(ShipControlFeatureFlags.AttemptBoarding))
		{
			return false;
		}
		if (_focusedShip != null && !_focusedShip.IsConnectionPermanentlyBlocked() && _focusedShip.ShipOrder.IsBoardingAvailable && !_playerControlledShip.GetIsThereActiveBridgeTo(_focusedShip))
		{
			WeakGameEntity gameEntity = ((ScriptComponentBehavior)_focusedShip).GameEntity;
			Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
			gameEntity = ((ScriptComponentBehavior)_playerControlledShip).GameEntity;
			if (((Vec3)(ref globalPosition)).Distance(((WeakGameEntity)(ref gameEntity)).GlobalPosition) <= 50f)
			{
				return !base.IsAimingWithRangedWeaponAndAllowed;
			}
		}
		return false;
	}

	private bool GetIsAttemptBoardingTemporarilyBlocked()
	{
		return _focusedShip?.IsConnectionBlocked() ?? false;
	}

	private bool GetCanChangeCamera()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.ChangeCamera))
		{
			return false;
		}
		return !base.IsAimingWithRangedWeaponAndAllowed;
	}

	private bool GetCanCutLoose()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.CutLoose))
		{
			return false;
		}
		return _isAnyBridgeActive;
	}

	private bool GetIsCutLooseTemporarilyBlocked()
	{
		if (!_playerControlledShip.ShipOrder.GetIsCuttingLoose())
		{
			return _playerControlledShip.IsDisconnectionBlocked();
		}
		return true;
	}

	private bool GetCanSelectShip()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (IsFeatureSuspended(ShipControlFeatureFlags.ShipSelection))
		{
			return false;
		}
		if (_orderUIHandler != null && _focusedShip != null && _focusedShip.Formation != null && _focusedShip.Formation.CountOfUnits > 0)
		{
			Team team = _focusedShip.Team;
			if (team != null && team.IsPlayerTeam && _focusedShip.Formation.PlayerOwner == Agent.Main)
			{
				WeakGameEntity gameEntity = ((ScriptComponentBehavior)_focusedShip).GameEntity;
				Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
				gameEntity = ((ScriptComponentBehavior)_playerControlledShip).GameEntity;
				if (((Vec3)(ref globalPosition)).Distance(((WeakGameEntity)(ref gameEntity)).GlobalPosition) <= 300f)
				{
					return !base.IsAimingWithRangedWeaponAndAllowed;
				}
			}
		}
		return false;
	}

	private bool GetCanToggleOarsmen()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.ToggleOarsmen))
		{
			return false;
		}
		if (!_isAnyEnemyShipConnected)
		{
			return !_playerControlledShip.ShipOrder.IsOarsmenLevelLocked();
		}
		return false;
	}

	private bool GetCanToggleSail()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.ToggleSails))
		{
			return false;
		}
		if (!_isAnyBridgeActive)
		{
			return _playerControlledShip.ShipSailState == MissionShip.SailState.Intact;
		}
		return false;
	}

	private bool GetCanToggleRangedWeaponOrderMode()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.BallistaOrder))
		{
			return false;
		}
		if (_playerControlledShip.ShipSiegeWeapon != null && !((MissionObject)_playerControlledShip.ShipSiegeWeapon).IsDisabled && !((UsableMachine)_playerControlledShip.ShipSiegeWeapon).IsDeactivated)
		{
			return base.IsAimingWithRangedWeaponAllowed;
		}
		return false;
	}

	private bool GetCanShootBallista()
	{
		if (IsFeatureSuspended(ShipControlFeatureFlags.ShootBallista))
		{
			return false;
		}
		if (base.IsAimingWithRangedWeaponAndAllowed && _playerControlledShip.ShipSiegeWeapon != null)
		{
			return ((UsableMachine)_playerControlledShip.ShipSiegeWeapon).UserCountNotInStruckAction > 0;
		}
		return false;
	}
}

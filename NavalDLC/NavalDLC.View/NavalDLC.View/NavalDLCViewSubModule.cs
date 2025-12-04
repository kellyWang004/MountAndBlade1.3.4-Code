using System;
using NavalDLC.HotKeyCategories;
using NavalDLC.View.Map;
using NavalDLC.View.Map.Managers;
using NavalDLC.View.Missions;
using NavalDLC.View.Overlay;
using NavalDLC.View.Permissions;
using NavalDLC.View.VisualOrders;
using SandBox;
using SandBox.View;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.View;

public class NavalDLCViewSubModule : MBSubModuleBase
{
	private NavalShipVisualOrderProvider _shipVisualOrderProvider;

	private NavalTroopVisualOrderProvider _troopVisualOrderProvider;

	private NavalGameMenuOverlayProvider _gameMenuOverlayProvider;

	protected override void OnSubModuleLoad()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleLoad();
		RegisterHotKeyContexts();
		RegisterTooltipTypes();
		_shipVisualOrderProvider = new NavalShipVisualOrderProvider();
		_troopVisualOrderProvider = new NavalTroopVisualOrderProvider();
		VisualOrderFactory.RegisterProvider((VisualOrderProvider)(object)_shipVisualOrderProvider);
		VisualOrderFactory.RegisterProvider((VisualOrderProvider)(object)_troopVisualOrderProvider);
		_gameMenuOverlayProvider = new NavalGameMenuOverlayProvider();
		GameMenuOverlayFactory.RegisterProvider((IGameMenuOverlayProvider)(object)_gameMenuOverlayProvider);
		MissionNameMarkerFactory.DefaultContext.AddProvider<NavalMissionNameMarkerProvider>();
		Input.OnControllerTypeChanged = (Action<ControllerTypes>)Delegate.Combine(Input.OnControllerTypeChanged, new Action<ControllerTypes>(OnControllerTypeChanged));
		NativeOptions.OnNativeOptionChanged = (OnNativeOptionChangedDelegate)Delegate.Combine((Delegate?)(object)NativeOptions.OnNativeOptionChanged, (Delegate?)new OnNativeOptionChangedDelegate(OnNativeOptionChanged));
		ScreenManager.OnPushScreen += new OnPushScreenEvent(OnScreenPushed);
	}

	public override void OnNewGameCreated(Game game, object initializerObject)
	{
		if (game.GameType is Campaign)
		{
			NavalDLCManager.Instance.NavalMapSceneWrapper = new NavalMapSceneWrapper();
		}
	}

	public override void OnAfterGameLoaded(Game game)
	{
		if (game.GameType is Campaign)
		{
			NavalDLCManager.Instance.NavalMapSceneWrapper = new NavalMapSceneWrapper();
		}
	}

	public override void OnGameInitializationFinished(Game game)
	{
		((MBSubModuleBase)this).OnGameInitializationFinished(game);
		NavalPermissionsSystem.OnInitialize();
	}

	public override void OnGameEnd(Game game)
	{
		((MBSubModuleBase)this).OnGameEnd(game);
		NavalPermissionsSystem.OnUnload();
	}

	protected override void OnSubModuleUnloaded()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleUnloaded();
		UnregisterTooltipTypes();
		VisualOrderFactory.UnregisterProvider((VisualOrderProvider)(object)_shipVisualOrderProvider);
		VisualOrderFactory.UnregisterProvider((VisualOrderProvider)(object)_troopVisualOrderProvider);
		GameMenuOverlayFactory.UnregisterProvider((IGameMenuOverlayProvider)(object)_gameMenuOverlayProvider);
		Input.OnControllerTypeChanged = (Action<ControllerTypes>)Delegate.Remove(Input.OnControllerTypeChanged, new Action<ControllerTypes>(OnControllerTypeChanged));
		NativeOptions.OnNativeOptionChanged = (OnNativeOptionChangedDelegate)Delegate.Remove((Delegate?)(object)NativeOptions.OnNativeOptionChanged, (Delegate?)new OnNativeOptionChangedDelegate(OnNativeOptionChanged));
		ScreenManager.OnPushScreen -= new OnPushScreenEvent(OnScreenPushed);
	}

	public override void OnSubModuleDeactivated()
	{
	}

	public override void OnSubModuleActivated()
	{
	}

	private void RegisterTooltipTypes()
	{
		InformationManager.RegisterTooltip<ShipUpgradePiece, PropertyBasedTooltipVM>((Action<PropertyBasedTooltipVM, object[]>)NavalTooltipRefresherCollection.RefreshShipPieceTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Figurehead, PropertyBasedTooltipVM>((Action<PropertyBasedTooltipVM, object[]>)NavalTooltipRefresherCollection.RefreshFigureheadTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<AnchorPoint, PropertyBasedTooltipVM>((Action<PropertyBasedTooltipVM, object[]>)NavalTooltipRefresherCollection.RefreshAnchorPointTooltip, "PropertyBasedTooltip");
		InformationManager.RegisterTooltip<Settlement, PropertyBasedTooltipVM>((Action<PropertyBasedTooltipVM, object[]>)NavalTooltipRefresherCollection.RefreshSettlementTooltip, "PropertyBasedTooltip");
	}

	private void UnregisterTooltipTypes()
	{
		InformationManager.UnregisterTooltip<ShipUpgradePiece>();
		InformationManager.UnregisterTooltip<Figurehead>();
		InformationManager.UnregisterTooltip<AnchorPoint>();
		InformationManager.UnregisterTooltip<Settlement>();
	}

	private void RegisterHotKeyContexts()
	{
		HotKeyManager.RegisterContext((GameKeyContext)(object)new NavalShipControlsHotKeyCategory(), false, false);
		HotKeyManager.RegisterContext((GameKeyContext)(object)new PortHotKeyCategory(), false, false);
		HotKeyManager.RegisterContext((GameKeyContext)(object)new NavalCheatsHotKeyCategory(), false, false);
	}

	private void OnControllerTypeChanged(ControllerTypes newType)
	{
		RegisterHotKeyContexts();
	}

	private void OnNativeOptionChanged(NativeOptionsType optionType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)optionType == 17)
		{
			RegisterHotKeyContexts();
		}
	}

	private void OnScreenPushed(ScreenBase pushedScreen)
	{
		MapScreen val;
		if ((val = (MapScreen)(object)((pushedScreen is MapScreen) ? pushedScreen : null)) != null)
		{
			val.AddMapView<NavalMapAnchorTrackerView>(Array.Empty<object>());
		}
	}

	public override void OnAfterGameInitializationFinished(Game game, object starterObject)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((MBSubModuleBase)this).OnAfterGameInitializationFinished(game, starterObject);
		if (Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
		{
			VisualShipFactory.InitializeShipEntityCache(((MapScene)Campaign.Current.MapSceneWrapper).Scene);
			SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<NavalMobilePartyVisualManager>();
			SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<AnchorVisualManager>();
			SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<StormVisualManager>();
		}
	}
}

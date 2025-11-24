using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.InputSystem;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GameKeyCategory;
using TaleWorlds.MountAndBlade.View.CustomBattle;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.View.Tableaus;
using TaleWorlds.MountAndBlade.View.Tableaus.Thumbnails;
using TaleWorlds.MountAndBlade.View.VisualOrders;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.View;

public class ViewSubModule : MBSubModuleBase
{
	private Dictionary<Tuple<Material, Banner>, Material> _bannerTexturedMaterialCache;

	private GameStateScreenManager _gameStateScreenManager;

	private bool _newGameInitialization;

	private VisualOrderProvider _visualOrderProvider;

	private static ViewSubModule _instance;

	private bool _initialized;

	private DLCInstallationQueryView _dlcInstallationQueryView;

	public static Dictionary<Tuple<Material, Banner>, Material> BannerTexturedMaterialCache
	{
		get
		{
			return _instance._bannerTexturedMaterialCache;
		}
		set
		{
			_instance._bannerTexturedMaterialCache = value;
		}
	}

	public static GameStateScreenManager GameStateScreenManager => _instance._gameStateScreenManager;

	private void InitializeHotKeyManager(bool loadKeys)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		string text = "BannerlordGameKeys.xml";
		HotKeyManager.Initialize(new PlatformFilePath(EngineFilePaths.ConfigsPath, text), !ScreenManager.IsEnterButtonRDown);
		HotKeyManager.RegisterInitialContexts((IEnumerable<GameKeyContext>)new List<GameKeyContext>
		{
			(GameKeyContext)new GenericGameKeyContext(),
			(GameKeyContext)new GenericCampaignPanelsGameKeyCategory("GenericCampaignPanelsGameKeyCategory"),
			(GameKeyContext)new GenericPanelGameKeyCategory("GenericPanelGameKeyCategory"),
			(GameKeyContext)new ArmyManagementHotkeyCategory(),
			(GameKeyContext)new BoardGameHotkeyCategory(),
			(GameKeyContext)new ChatLogHotKeyCategory(),
			(GameKeyContext)new CombatHotKeyCategory(),
			(GameKeyContext)new CraftingHotkeyCategory(),
			(GameKeyContext)new FaceGenHotkeyCategory(),
			(GameKeyContext)new InventoryHotKeyCategory(),
			(GameKeyContext)new PartyHotKeyCategory(),
			(GameKeyContext)new MapHotKeyCategory(),
			(GameKeyContext)new MapNotificationHotKeyCategory(),
			(GameKeyContext)new MissionOrderHotkeyCategory(),
			(GameKeyContext)new OrderOfBattleHotKeyCategory(),
			(GameKeyContext)new MultiplayerHotkeyCategory(),
			(GameKeyContext)new ScoreboardHotKeyCategory(),
			(GameKeyContext)new ConversationHotKeyCategory(),
			(GameKeyContext)new CheatsHotKeyCategory(),
			(GameKeyContext)new PhotoModeHotKeyCategory(),
			(GameKeyContext)new PollHotkeyCategory()
		}, loadKeys);
	}

	private void InitializeBannerVisualManager()
	{
		if (BannerManager.Instance == null)
		{
			BannerManager.Initialize();
			BannerManager.Instance.LoadBannerIcons();
		}
	}

	protected override void OnSubModuleLoad()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_012f: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Expected O, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleLoad();
		_instance = this;
		InitializeHotKeyManager(loadKeys: false);
		InitializeBannerVisualManager();
		CraftedDataViewManager.Initialize();
		_visualOrderProvider = (VisualOrderProvider)(object)new DefaultVisualOrderProvider();
		VisualOrderFactory.RegisterProvider(_visualOrderProvider);
		_gameStateScreenManager = new GameStateScreenManager();
		Module.CurrentModule.GlobalGameStateManager.RegisterListener((IGameStateManagerListener)(object)_gameStateScreenManager);
		MBMusicManager.Create();
		TextObject coreContentDisabledReason = new TextObject("{=V8BXjyYq}Disabled during installation.", (Dictionary<string, object>)null);
		if (Utilities.EditModeEnabled)
		{
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Editor", new TextObject("{=bUh0x6rA}Editor", (Dictionary<string, object>)null), -1, (Action)delegate
			{
				MBInitialScreenBase.OnEditModeEnterPress();
			}, (Func<ValueTuple<bool, TextObject>>)(() => (Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)), (TextObject)null, (Func<bool>)null));
		}
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("CustomBattle", new TextObject("{=4gOGGbeQ}Custom Battle", (Dictionary<string, object>)null), 5000, (Action)CustomBattleFactory.StartCustomBattle, (Func<ValueTuple<bool, TextObject>>)(() => ((bool, TextObject))(false, null)), (TextObject)null, (Func<bool>)(() => CustomBattleFactory.GetProviderCount() == 0)));
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Options", new TextObject("{=NqarFr4P}Options", (Dictionary<string, object>)null), 9998, (Action)delegate
		{
			ScreenManager.PushScreen(ViewCreator.CreateOptionsScreen(fromMainMenu: true));
		}, (Func<ValueTuple<bool, TextObject>>)(() => ((bool, TextObject))(false, null)), (TextObject)null, (Func<bool>)null));
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Credits", new TextObject("{=ODQmOrIw}Credits", (Dictionary<string, object>)null), 9999, (Action)delegate
		{
			ScreenManager.PushScreen(ViewCreator.CreateCreditsScreen());
		}, (Func<ValueTuple<bool, TextObject>>)(() => ((bool, TextObject))(false, null)), (TextObject)null, (Func<bool>)null));
		Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Exit", new TextObject("{=YbpzLHzk}Exit Game", (Dictionary<string, object>)null), 10000, (Action)delegate
		{
			MBInitialScreenBase.DoExitButtonAction();
		}, (Func<ValueTuple<bool, TextObject>>)(() => (Module.CurrentModule.IsOnlyCoreContentEnabled, coreContentDisabledReason)), (TextObject)null, (Func<bool>)null));
		ViewModel.RefreshPropertyAndMethodInfos();
		Module.CurrentModule.ImguiProfilerTick += OnImguiProfilerTick;
		Input.OnControllerTypeChanged = (Action<ControllerTypes>)Delegate.Combine(Input.OnControllerTypeChanged, new Action<ControllerTypes>(OnControllerTypeChanged));
		ScreenManager.OnPushScreen += new OnPushScreenEvent(OnScreenManagerPushScreen);
		NativeOptions.OnNativeOptionChanged = (OnNativeOptionChangedDelegate)Delegate.Combine((Delegate?)(object)NativeOptions.OnNativeOptionChanged, (Delegate?)new OnNativeOptionChangedDelegate(OnNativeOptionChanged));
		EngineController.OnConstrainedStateChanged += OnConstrainedStateChange;
		HyperlinkTexts.IsPlayStationGamepadActive = GetIsPlaystationGamepadActive;
		_dlcInstallationQueryView = new DLCInstallationQueryView();
		_dlcInstallationQueryView.Initialize();
	}

	private void OnModuleStructureChanged()
	{
		ViewModel.RefreshPropertyAndMethodInfos();
	}

	private void OnConstrainedStateChange(bool isConstrained)
	{
		ScreenManager.OnConstrainStateChanged(isConstrained);
	}

	private bool GetIsPlaystationGamepadActive()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		if (Input.IsGamepadActive)
		{
			if ((int)Input.ControllerType != 4)
			{
				return (int)Input.ControllerType == 2;
			}
			return true;
		}
		return false;
	}

	private void OnControllerTypeChanged(ControllerTypes newType)
	{
		ReInitializeHotKeyManager();
	}

	private void OnNativeOptionChanged(NativeOptionsType changedNativeOptionsType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)changedNativeOptionsType == 17)
		{
			ReInitializeHotKeyManager();
		}
	}

	private void ReInitializeHotKeyManager()
	{
		InitializeHotKeyManager(loadKeys: true);
	}

	protected override void OnSubModuleUnloaded()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		_dlcInstallationQueryView?.OnFinalize();
		_dlcInstallationQueryView = null;
		VisualOrderFactory.UnregisterProvider(_visualOrderProvider);
		ThumbnailCacheManager.ClearManager();
		BannerlordTableauManager.ClearManager();
		CraftedDataViewManager.Clear();
		Module.CurrentModule.ImguiProfilerTick -= OnImguiProfilerTick;
		Input.OnControllerTypeChanged = (Action<ControllerTypes>)Delegate.Remove(Input.OnControllerTypeChanged, new Action<ControllerTypes>(OnControllerTypeChanged));
		ScreenManager.OnPushScreen -= new OnPushScreenEvent(OnScreenManagerPushScreen);
		NativeOptions.OnNativeOptionChanged = (OnNativeOptionChangedDelegate)Delegate.Remove((Delegate?)(object)NativeOptions.OnNativeOptionChanged, (Delegate?)new OnNativeOptionChangedDelegate(OnNativeOptionChanged));
		EngineController.OnConstrainedStateChanged -= OnConstrainedStateChange;
		_instance = null;
		((MBSubModuleBase)this).OnSubModuleUnloaded();
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		if (_initialized)
		{
			BannerPersistentTextureCache.Current?.FlushCache();
		}
		if (!_initialized)
		{
			HotKeyManager.LoadAsync();
			BannerlordTableauManager.InitializeCharacterTableauRenderSystem();
			ThumbnailCacheManager.InitializeManager();
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new AvatarThumbnailCache(75));
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new BannerThumbnailCache(100));
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new BannerPersistentTextureCache());
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new BannerEditorTextureCache(5));
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new CharacterThumbnailCache(75));
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new CraftingPieceThumbnailCache(75));
			ThumbnailCacheManager.Current.RegisterThumbnailCache(new ItemThumbnailCache(75));
			_initialized = true;
		}
	}

	protected override void OnNewModuleLoad()
	{
		ViewCreatorManager.CollectTypes();
		ViewModel.RefreshPropertyAndMethodInfos();
		_gameStateScreenManager.CollectTypes();
	}

	protected override void OnApplicationTick(float dt)
	{
		((MBSubModuleBase)this).OnApplicationTick(dt);
		if (Input.DebugInput.IsHotKeyPressed("ToggleUI"))
		{
			MBDebug.DisableUI(new List<string>());
		}
		HotKeyManager.Tick(dt);
		MBMusicManager current = MBMusicManager.Current;
		if (current != null)
		{
			current.Update(dt);
		}
		ThumbnailCacheManager.Current?.Tick(dt);
	}

	protected override void AfterAsyncTickTick(float dt)
	{
		MBMusicManager current = MBMusicManager.Current;
		if (current != null)
		{
			current.Update(dt);
		}
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		MissionWeapon.OnGetWeaponDataHandler = new OnGetWeaponDataDelegate(ItemCollectionElementViewExtensions.OnGetWeaponData);
	}

	public override void OnCampaignStart(Game game, object starterObject)
	{
		Game.Current.GameStateManager.RegisterListener((IGameStateManagerListener)(object)_gameStateScreenManager);
		_newGameInitialization = false;
	}

	public override void OnMultiplayerGameStart(Game game, object starterObject)
	{
		Game.Current.GameStateManager.RegisterListener((IGameStateManagerListener)(object)_gameStateScreenManager);
	}

	public override void OnGameLoaded(Game game, object initializerObject)
	{
		Game.Current.GameStateManager.RegisterListener((IGameStateManagerListener)(object)_gameStateScreenManager);
	}

	public override void OnGameInitializationFinished(Game game)
	{
		((MBSubModuleBase)this).OnGameInitializationFinished(game);
		foreach (ItemObject item in (List<ItemObject>)(object)Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
		{
			if (item.MultiMeshName != "")
			{
				MBUnusedResourceManager.SetMeshUsed(item.MultiMeshName);
			}
			HorseComponent horseComponent = item.HorseComponent;
			if (horseComponent != null)
			{
				foreach (KeyValuePair<string, bool> additionalMeshesName in horseComponent.AdditionalMeshesNameList)
				{
					MBUnusedResourceManager.SetMeshUsed(additionalMeshesName.Key);
				}
			}
			if (item.PrimaryWeapon != null)
			{
				MBUnusedResourceManager.SetMeshUsed(item.HolsterMeshName);
				MBUnusedResourceManager.SetMeshUsed(item.HolsterWithWeaponMeshName);
				MBUnusedResourceManager.SetMeshUsed(item.FlyingMeshName);
				MBUnusedResourceManager.SetBodyUsed(item.BodyName);
				MBUnusedResourceManager.SetBodyUsed(item.HolsterBodyName);
				MBUnusedResourceManager.SetBodyUsed(item.CollisionBodyName);
			}
		}
	}

	public override void BeginGameStart(Game game)
	{
		((MBSubModuleBase)this).BeginGameStart(game);
		Game.Current.BannerVisualCreator = (IBannerVisualCreator)(object)new BannerVisualCreator();
	}

	public override bool DoLoading(Game game)
	{
		if (_newGameInitialization)
		{
			return true;
		}
		_newGameInitialization = true;
		return _newGameInitialization;
	}

	public override void OnGameEnd(Game game)
	{
		MissionWeapon.OnGetWeaponDataHandler = null;
		CraftedDataViewManager.Clear();
	}

	private void OnImguiProfilerTick()
	{
	}

	private void OnScreenManagerPushScreen(ScreenBase pushedScreen)
	{
	}
}

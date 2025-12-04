using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI;

public class NavalDLCGauntletUISubModule : MBSubModuleBase
{
	private bool _initializedLoadingCategory;

	private SpriteCategory _fullBackgroundsCategory;

	protected override void OnApplicationTick(float dt)
	{
		((MBSubModuleBase)this).OnApplicationTick(dt);
		if (!_initializedLoadingCategory)
		{
			_fullBackgroundsCategory = UIResourceManager.LoadSpriteCategory("ui_naval_fullbackgrounds");
			LoadingWindow.InitializeWith<GauntletNavalLoadingWindowManager>();
			_initializedLoadingCategory = true;
		}
	}

	protected override void OnSubModuleLoad()
	{
		((MBSubModuleBase)this).OnSubModuleLoad();
		GauntletGameVersionView.AddModuleVersionInfo("War Sails", NavalVersion.GetApplicationVersionBuildNumber());
	}

	protected override void OnSubModuleUnloaded()
	{
		((MBSubModuleBase)this).OnSubModuleUnloaded();
		SpriteCategory fullBackgroundsCategory = _fullBackgroundsCategory;
		if (fullBackgroundsCategory != null)
		{
			fullBackgroundsCategory.Unload();
		}
		GauntletGameVersionView.RemoveModuleVersionInfo("War Sails");
	}
}

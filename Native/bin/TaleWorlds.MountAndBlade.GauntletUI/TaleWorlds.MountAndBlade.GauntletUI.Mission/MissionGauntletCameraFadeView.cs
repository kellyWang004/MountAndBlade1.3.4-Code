using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission;

[DefaultView]
public class MissionGauntletCameraFadeView : MissionView
{
	private GauntletLayer _layer;

	private BindingListFloatItem _dataSource;

	private MissionCameraFadeView _controller;

	public override void OnMissionScreenInitialize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		base.OnMissionScreenInitialize();
		_dataSource = new BindingListFloatItem(0f);
		_layer = new GauntletLayer("MissionCameraFade", 100000, false);
		_layer.LoadMovie("CameraFade", (ViewModel)(object)_dataSource);
		((ScreenBase)base.MissionScreen).AddLayer((ScreenLayer)(object)_layer);
	}

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		_controller = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionCameraFadeView>();
	}

	public override void OnMissionScreenTick(float dt)
	{
		base.OnMissionScreenTick(dt);
		if (_dataSource != null && _controller != null)
		{
			_dataSource.Item = _controller.FadeAlpha;
		}
	}

	public override void OnMissionScreenFinalize()
	{
		base.OnMissionScreenFinalize();
		((ScreenBase)base.MissionScreen).RemoveLayer((ScreenLayer)(object)_layer);
		_controller = null;
		_dataSource = null;
		_layer = null;
	}
}

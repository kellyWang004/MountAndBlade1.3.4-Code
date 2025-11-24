using SandBox.View.Map;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map;

[OverrideView(typeof(MapCameraFadeView))]
public class GauntletMapCameraFadeView : MapCameraFadeView
{
	private GauntletLayer _layer;

	private BindingListFloatItem _dataSource;

	protected override void CreateLayout()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		base.CreateLayout();
		_dataSource = new BindingListFloatItem(0f);
		_layer = new GauntletLayer("MapCameraFade", 100000, false);
		_layer.LoadMovie("CameraFade", (ViewModel)(object)_dataSource);
		((ScreenBase)base.MapScreen).AddLayer((ScreenLayer)(object)_layer);
	}

	private void Tick(float dt)
	{
		if (_dataSource != null)
		{
			_dataSource.Item = base.FadeAlpha;
		}
	}

	protected override void OnFrameTick(float dt)
	{
		base.OnFrameTick(dt);
		Tick(dt);
	}

	protected override void OnIdleTick(float dt)
	{
		base.OnIdleTick(dt);
		Tick(dt);
	}

	protected override void OnMenuModeTick(float dt)
	{
		base.OnMenuModeTick(dt);
		Tick(dt);
	}

	protected override void OnFinalize()
	{
		base.OnFinalize();
		((ScreenBase)base.MapScreen).RemoveLayer((ScreenLayer)(object)_layer);
		_dataSource = null;
		_layer = null;
	}

	protected override void OnMapConversationStart()
	{
		base.OnMapConversationStart();
		if (_layer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_layer, true);
		}
	}

	protected override void OnMapConversationOver()
	{
		base.OnMapConversationOver();
		if (_layer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_layer, false);
		}
	}
}

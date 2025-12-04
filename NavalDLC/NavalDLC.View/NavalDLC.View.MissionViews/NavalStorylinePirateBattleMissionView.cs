using NavalDLC.Storyline;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews;

public class NavalStorylinePirateBattleMissionView : MissionView
{
	private bool _isInitialized;

	private PirateBattleMissionController _controller;

	private MissionCameraFadeView _cameraFadeViewController;

	private MatrixFrame _cameraFrame = MatrixFrame.Identity;

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		if (!_isInitialized)
		{
			InitializeView();
		}
	}

	private void OnBeginScreenFade(float fadeDuration, float blackScreenDuration)
	{
		_cameraFadeViewController.BeginFadeOutAndIn(fadeDuration, blackScreenDuration, fadeDuration);
	}

	private void OnCameraBearingNeedsUpdate(float direction)
	{
		((MissionView)this).MissionScreen.CameraBearing = direction;
	}

	private void InitializeView()
	{
		_controller = ((MissionBehavior)this).Mission.GetMissionBehavior<PirateBattleMissionController>();
		_cameraFadeViewController = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionCameraFadeView>();
		_isInitialized = _controller != null && _cameraFadeViewController != null;
		if (_controller != null)
		{
			_controller.OnBeginScreenFadeEvent += OnBeginScreenFade;
			_controller.OnCameraBearingNeedsUpdateEvent += OnCameraBearingNeedsUpdate;
			_controller.OnShipsInitializedEvent += OnShipsInitialized;
		}
	}

	private void OnShipsInitialized()
	{
		OnShipsInitializedInternal();
	}

	protected virtual void OnShipsInitializedInternal()
	{
	}
}

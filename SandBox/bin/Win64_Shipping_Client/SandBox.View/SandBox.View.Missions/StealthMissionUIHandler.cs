using SandBox.Objects.Usables;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

public class StealthMissionUIHandler : MissionView
{
	private MissionCameraFadeView _cameraFadeViewController;

	private bool _isInitialized;

	public override void OnMissionScreenTick(float dt)
	{
		((MissionView)this).OnMissionScreenTick(dt);
		if (!_isInitialized)
		{
			InitializeView();
		}
	}

	public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
	{
		((MissionBehavior)this).OnObjectUsed(userAgent, usedObject);
		if (_isInitialized && usedObject is StealthAreaUsePoint)
		{
			CameraFadeInFadeOut(0.5f, 0.5f, 1f);
		}
	}

	private void InitializeView()
	{
		_cameraFadeViewController = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionCameraFadeView>();
		_isInitialized = true;
	}

	private void CameraFadeInFadeOut(float fadeOutTime, float blackTime, float fadeInTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)_cameraFadeViewController.FadeState == 0)
		{
			_cameraFadeViewController.BeginFadeOutAndIn(fadeOutTime, blackTime, fadeInTime);
		}
	}
}

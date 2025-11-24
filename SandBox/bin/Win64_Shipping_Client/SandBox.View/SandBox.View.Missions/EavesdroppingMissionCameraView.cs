using SandBox.Missions;
using TaleWorlds.DotNet;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace SandBox.View.Missions;

public class EavesdroppingMissionCameraView : MissionView
{
	private enum CameraSwitchState
	{
		None,
		ReadyForFadeOut,
		FadeOutAndInStarted,
		WaitingForFadeInToEnd
	}

	private CameraSwitchState _cameraSwitchState;

	private EavesdroppingMissionLogic _eavesdroppingMissionLogic;

	private MissionCameraFadeView _missionCameraFadeView;

	protected virtual void SetPlayerMovementEnabled(bool isPlayerMovementEnabled)
	{
	}

	public override void OnBehaviorInitialize()
	{
		((MissionBehavior)this).OnBehaviorInitialize();
		_cameraSwitchState = CameraSwitchState.None;
		foreach (MissionBehavior missionBehavior in ((MissionBehavior)this).Mission.MissionBehaviors)
		{
			if (missionBehavior is EavesdroppingMissionLogic)
			{
				_eavesdroppingMissionLogic = missionBehavior as EavesdroppingMissionLogic;
			}
			if (missionBehavior is MissionCameraFadeView)
			{
				_missionCameraFadeView = (MissionCameraFadeView)(object)((missionBehavior is MissionCameraFadeView) ? missionBehavior : null);
			}
		}
	}

	public override void OnMissionTick(float dt)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).OnMissionTick(dt);
		if (_eavesdroppingMissionLogic == null)
		{
			return;
		}
		switch (_cameraSwitchState)
		{
		case CameraSwitchState.None:
			if ((_eavesdroppingMissionLogic.EavesdropStarted && (NativeObject)(object)((MissionView)this).MissionScreen.CustomCamera == (NativeObject)null) || (!_eavesdroppingMissionLogic.EavesdropStarted && (NativeObject)(object)((MissionView)this).MissionScreen.CustomCamera != (NativeObject)null))
			{
				if (_eavesdroppingMissionLogic.EavesdropStarted && (NativeObject)(object)((MissionView)this).MissionScreen.CustomCamera == (NativeObject)null)
				{
					SetPlayerMovementEnabled(isPlayerMovementEnabled: false);
				}
				_cameraSwitchState = CameraSwitchState.ReadyForFadeOut;
			}
			break;
		case CameraSwitchState.ReadyForFadeOut:
			_missionCameraFadeView.BeginFadeOutAndIn(0.5f, 0.5f, 0.5f);
			_cameraSwitchState = CameraSwitchState.FadeOutAndInStarted;
			break;
		case CameraSwitchState.FadeOutAndInStarted:
			if ((int)_missionCameraFadeView.FadeState == 2)
			{
				((MissionView)this).MissionScreen.CustomCamera = (((NativeObject)(object)((MissionView)this).MissionScreen.CustomCamera == (NativeObject)null) ? _eavesdroppingMissionLogic.CurrentEavesdroppingCamera : null);
				if ((NativeObject)(object)((MissionView)this).MissionScreen.CustomCamera == (NativeObject)null)
				{
					SetPlayerMovementEnabled(isPlayerMovementEnabled: true);
				}
				_cameraSwitchState = CameraSwitchState.WaitingForFadeInToEnd;
			}
			break;
		case CameraSwitchState.WaitingForFadeInToEnd:
			if ((int)_missionCameraFadeView.FadeState == 0)
			{
				_cameraSwitchState = CameraSwitchState.None;
			}
			break;
		}
	}
}

using System;
using System.Collections.Generic;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Order;
using NavalDLC.ViewModelCollection;
using SandBox.View;
using SandBox.View.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace NavalDLC.View;

[ViewCreatorModule]
public class NavalViews
{
	[ViewMethod("NavalBattle")]
	public static MissionView[] OpenNavalBattleMission(Mission mission)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)(object)new MusicNavalBattleMissionView());
		obj.Add((MissionView)(object)new NavalAmbientShoutsView());
		obj.Add((MissionView)(object)new NavalDeploymentMissionView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add((MissionView)(object)new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a"));
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalOrderOfBattleView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add((MissionView)new MissionCampaignView());
		obj.Add((MissionView)new MissionPreloadView());
		obj.Add((MissionView)(object)new NavalShipsPreloadView());
		return obj.ToArray();
	}

	[ViewMethod("NavalCustomBattle")]
	public static MissionView[] OpenNavalBattleForCustomMission(Mission mission)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalCustomBattleScoreboardVM()),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)(object)new MusicNavalBattleMissionView());
		obj.Add((MissionView)(object)new NavalAmbientShoutsView());
		obj.Add((MissionView)(object)new NavalDeploymentMissionView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add((MissionView)(object)new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a"));
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalOrderOfBattleView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add((MissionView)new MissionCampaignView());
		obj.Add((MissionView)new MissionCustomBattlePreloadView());
		obj.Add((MissionView)(object)new NavalShipsPreloadView());
		return obj.ToArray();
	}

	[ViewMethod("NavalCaptivityBattle")]
	public static MissionView[] OpenNavalCaptivityBattleMission(Mission mission)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		return new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission),
			ViewCreator.CreateMissionAgentStatusUIHandler(mission),
			ViewCreator.CreateMissionMainAgentEquipmentController(mission),
			ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
			ViewCreator.CreateMissionAgentLockVisualizerView(mission),
			(MissionView)new MusicSilencedMissionView(),
			(MissionView)new MissionBoundaryWallView(),
			ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
			ViewCreator.CreateMissionSpectatorControlView(mission),
			ViewCreator.CreatePhotoModeView(),
			NavalViewCreator.CreateMissionShipControlView(mission),
			(MissionView)new MissionItemContourControllerView(),
			(MissionView)new MissionAgentContourControllerView(),
			(MissionView)new MissionConversationCameraView(),
			SandBoxViewCreator.CreateMissionConversationView(mission),
			ViewCreator.CreateMissionLeaveView(),
			(MissionView)(object)new NavalMissionPrepareView(),
			(MissionView)new MissionCampaignView(),
			ViewCreator.CreateMissionHintView(mission),
			ViewCreator.CreateMissionObjectiveView(mission),
			SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
			NavalViewCreator.CreateCaptivityMissionView(mission),
			NavalViewCreator.CreateNavalMissionCaptureShipView(mission)
		}.ToArray();
	}

	[ViewMethod("BlockedEstuary")]
	public static MissionView[] OpenNavalSetPieceBattleMission(Mission mission)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		return new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission),
			ViewCreator.CreateMissionObjectiveView(mission),
			ViewCreator.CreateMissionAgentStatusUIHandler(mission),
			ViewCreator.CreateMissionMainAgentEquipmentController(mission),
			ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
			ViewCreator.CreateMissionAgentLockVisualizerView(mission),
			(MissionView)new MusicSilencedMissionView(),
			NavalViewCreator.CreateNavalShipMarkerUIHandler(mission),
			ViewCreator.CreateMissionBoundaryCrossingView(),
			(MissionView)new MissionBoundaryWallView(),
			ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
			ViewCreator.CreateMissionSpectatorControlView(mission),
			ViewCreator.CreatePhotoModeView(),
			NavalViewCreator.CreateMissionShipControlView(mission),
			(MissionView)new MissionItemContourControllerView(),
			(MissionView)new MissionAgentContourControllerView(),
			(MissionView)new MissionConversationCameraView(),
			SandBoxViewCreator.CreateMissionConversationView(mission),
			ViewCreator.CreateMissionLeaveView(),
			SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
			(MissionView)(object)new NavalMissionPrepareView(),
			(MissionView)(object)new BlockedEstuaryView(),
			NavalViewCreator.CreateNavalMissionCaptureShipView(mission),
			(MissionView)new MissionCampaignView()
		}.ToArray();
	}

	[ViewMethod("NavalStorylinePirateBattle")]
	public static MissionView[] OpenNavalStorylinePirateBattleMission(Mission mission)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalStorylinePirateBattleScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)new MusicSilencedMissionView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add(NavalViewCreator.CreatePirateBattleMissionView(mission));
		obj.Add((MissionView)new MissionConversationCameraView());
		obj.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
		obj.Add(ViewCreator.CreateMissionLeaveView());
		obj.Add((MissionView)new MissionCampaignView());
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionObjectiveView(mission));
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		return obj.ToArray();
	}

	[ViewMethod("HelpAnAllySetPieceBattle")]
	public static MissionView[] OpenHelpAnAllySetPieceBattle(Mission mission)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			NavalViewCreator.CreateHelpingAnAllyMissionView(),
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)new MusicSilencedMissionView());
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add(ViewCreator.CreateMissionObjectiveView((Mission)null));
		obj.Add((MissionView)new MissionConversationCameraView());
		obj.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
		obj.Add(ViewCreator.CreateMissionLeaveView());
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		return obj.ToArray();
	}

	[ViewMethod("NavalStorylineQuest5SetPieceBattleMission")]
	public static MissionView[] OpenNavalStorylineQuest5SetPieceBattleMission(Mission mission)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionObjectiveView((Mission)null),
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission),
			ViewCreator.CreateMissionAgentStatusUIHandler(mission),
			ViewCreator.CreateMissionMainAgentEquipmentController(mission),
			ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
			ViewCreator.CreateMissionAgentLockVisualizerView(mission),
			(MissionView)new MusicSilencedMissionView(),
			ViewCreator.CreateMissionBoundaryCrossingView(),
			(MissionView)new MissionBoundaryWallView(),
			ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
			ViewCreator.CreateMissionSpectatorControlView(mission),
			ViewCreator.CreatePhotoModeView(),
			(MissionView)new MissionItemContourControllerView(),
			(MissionView)new MissionAgentContourControllerView(),
			NavalViewCreator.CreateMissionShipControlView(mission),
			(MissionView)(object)new NavalMissionPrepareView(),
			NavalViewCreator.CreateNavalShipMarkerUIHandler(mission),
			NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission),
			(MissionView)(object)new NavalMissionShipHighlightView(),
			(MissionView)new MissionFormationTargetSelectionHandler(),
			(MissionView)new MusicStealthMissionView(),
			(MissionView)new MissionCampaignView(),
			(MissionView)new MissionConversationCameraView(),
			SandBoxViewCreator.CreateMissionConversationView(mission),
			ViewCreator.CreateMissionLeaveView(),
			NavalViewCreator.CreateQuest5SetPieceBattleMissionView(mission),
			NavalViewCreator.CreateQuest5SetPieceBattleBossFightCameraView(mission),
			NavalViewCreator.CreateQuest5SetPieceBattleInteriorConversationCameraView(mission),
			SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
			SandBoxViewCreator.CreateMissionAgentAlarmStateView(mission),
			(MissionView)(object)new NavalOrderTroopPlacer(null)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		return obj.ToArray();
	}

	[ViewMethod("NavalFinalConversationMission")]
	public static MissionView[] OpenNavalFinalConversationMission(Mission mission)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		return new List<MissionView>
		{
			(MissionView)new MissionCampaignView(),
			(MissionView)new MissionConversationCameraView(),
			SandBoxViewCreator.CreateMissionConversationView(mission),
			ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission),
			(MissionView)new MissionSingleplayerViewHandler(),
			ViewCreator.CreateMissionAgentStatusUIHandler(mission),
			ViewCreator.CreateMissionMainAgentEquipmentController(mission),
			ViewCreator.CreateMissionAgentLockVisualizerView(mission),
			(MissionView)new MusicSilencedMissionView(),
			SandBoxViewCreator.CreateMissionBarterView(),
			ViewCreator.CreateMissionLeaveView(),
			SandBoxViewCreator.CreateBoardGameView(),
			SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
			(MissionView)new MissionItemContourControllerView(),
			(MissionView)new MissionAgentContourControllerView(),
			(MissionView)new MissionCampaignBattleSpectatorView(),
			ViewCreator.CreatePhotoModeView(),
			ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
			(MissionView)(object)new NavalFinalConversationMissionView()
		}.ToArray();
	}

	[ViewMethod("NavalStorylineWoundedBeastBattle")]
	public static MissionView[] OpenNavalStorylineWoundedBeastBattleMission(Mission mission)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionObjectiveView((Mission)null),
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)new MusicSilencedMissionView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionLeaveView());
		obj.Add((MissionView)(object)new WoundedBeastView());
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		return obj.ToArray();
	}

	[ViewMethod("FloatingFortressSetPieceBattleMission")]
	public static MissionView[] OpenFloatingFortressSetPieceBattleMission(Mission mission)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		List<MissionView> obj = new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission)
		};
		MissionView item = NavalViewCreator.CreateNavalOrderUIHandler(mission);
		obj.Add(item);
		obj.Add((MissionView)new MissionFormationTargetSelectionHandler());
		obj.Add((MissionView)(object)new NavalOrderTroopPlacer(null));
		obj.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
		obj.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
		obj.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
		obj.Add((MissionView)new MusicSilencedMissionView());
		obj.Add(ViewCreator.CreateMissionBoundaryCrossingView());
		obj.Add((MissionView)new MissionBoundaryWallView());
		obj.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
		obj.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
		obj.Add(ViewCreator.CreatePhotoModeView());
		obj.Add((MissionView)new MissionItemContourControllerView());
		obj.Add((MissionView)new MissionAgentContourControllerView());
		obj.Add(NavalViewCreator.CreateMissionShipControlView(mission));
		obj.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
		obj.Add((MissionView)(object)new NavalMissionShipHighlightView());
		obj.Add((MissionView)new MissionConversationCameraView());
		obj.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
		obj.Add(ViewCreator.CreateMissionLeaveView());
		obj.Add(NavalViewCreator.CreateFloatingFortressView());
		obj.Add(ViewCreator.CreateMissionObjectiveView((Mission)null));
		obj.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
		obj.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
		obj.Add((MissionView)(object)new NavalMissionPrepareView());
		obj.Add((MissionView)new MissionCampaignView());
		return obj.ToArray();
	}

	[ViewMethod("NavalStorylineAlleyFight")]
	public static MissionView[] OpenNavalStorylineAlleyFight(Mission mission)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		return new List<MissionView>
		{
			ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
			ViewCreator.CreateMissionAgentLabelUIHandler(mission),
			ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardBaseVM)(object)new NavalStorylineAlleyFightScoreboardVM(null)),
			ViewCreator.CreateOptionsUIHandler(),
			ViewCreator.CreateMissionMainAgentEquipDropView(mission),
			(MissionView)new MissionFormationTargetSelectionHandler(),
			ViewCreator.CreateMissionAgentStatusUIHandler(mission),
			ViewCreator.CreateMissionMainAgentEquipmentController(mission),
			ViewCreator.CreateMissionAgentLockVisualizerView(mission),
			(MissionView)new MusicSilencedMissionView(),
			(MissionView)(object)new NavalStorylineAlleyFightCinematicView(),
			ViewCreatorManager.CreateMissionView<MissionHintView>(false, mission, Array.Empty<object>()),
			ViewCreator.CreateMissionBoundaryCrossingView(),
			ViewCreator.CreatePhotoModeView(),
			(MissionView)new MissionBoundaryWallView(),
			ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
			ViewCreator.CreateMissionSpectatorControlView(mission),
			(MissionView)new MissionItemContourControllerView(),
			(MissionView)new MissionAgentContourControllerView(),
			(MissionView)new MissionConversationCameraView(),
			SandBoxViewCreator.CreateMissionConversationView(mission),
			ViewCreator.CreateMissionLeaveView()
		}.ToArray();
	}
}

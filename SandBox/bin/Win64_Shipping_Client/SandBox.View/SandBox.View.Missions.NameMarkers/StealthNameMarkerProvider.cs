using System.Collections.Generic;
using SandBox.Missions.MissionLogics;
using SandBox.Objects.Usables;
using SandBox.ViewModelCollection;
using SandBox.ViewModelCollection.Missions.NameMarker;
using SandBox.ViewModelCollection.Missions.NameMarker.Targets.Hideout;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions;

namespace SandBox.View.Missions.NameMarkers;

public class StealthNameMarkerProvider : MissionNameMarkerProvider
{
	private StealthAreaMissionLogic _stealthAreaMissionLogic;

	protected override void OnInitialize(Mission mission)
	{
		base.OnInitialize(mission);
		_stealthAreaMissionLogic = mission.GetMissionBehavior<StealthAreaMissionLogic>();
	}

	protected override void OnDestroy(Mission mission)
	{
		base.OnDestroy(mission);
		_stealthAreaMissionLogic = null;
	}

	public override void CreateMarkers(List<MissionNameMarkerTargetBaseVM> markers)
	{
		CreateStealthAreaMarkers(markers);
	}

	private void CreateStealthAreaMarkers(List<MissionNameMarkerTargetBaseVM> markers)
	{
		if (_stealthAreaMissionLogic == null)
		{
			return;
		}
		Mission current = Mission.Current;
		if (current == null)
		{
			return;
		}
		if (Agent.Main != null)
		{
			foreach (StealthAreaUsePoint item3 in MBExtensions.FindAllWithType<StealthAreaUsePoint>((IEnumerable<MissionObject>)Mission.Current.ActiveMissionObjects))
			{
				if (((UsableMissionObject)item3).IsUsableByAgent(Agent.Main))
				{
					MissionStealthAreaUsePointNameMarkerTargetVM item = new MissionStealthAreaUsePointNameMarkerTargetVM(item3);
					markers.Add(item);
				}
			}
		}
		AgentReadOnlyList allAgents = current.AllAgents;
		for (int i = 0; i < ((List<Agent>)(object)allAgents).Count; i++)
		{
			Agent val = ((List<Agent>)(object)allAgents)[i];
			if (SandBoxUIHelper.CanAgentBeAlarmed(val))
			{
				StealthAreaMissionLogic stealthAreaMissionLogic = _stealthAreaMissionLogic;
				if (stealthAreaMissionLogic != null && stealthAreaMissionLogic.IsSentry(val))
				{
					MissionStealthSentryNameMarkerTargetVM item2 = new MissionStealthSentryNameMarkerTargetVM(val);
					markers.Add(item2);
				}
			}
		}
	}
}

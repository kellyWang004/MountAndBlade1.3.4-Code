using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace StoryMode.GameComponents;

public class StoryModeTargetScoreCalculatingModel : TargetScoreCalculatingModel
{
	public override float TravelingToAssignmentFactor => ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.TravelingToAssignmentFactor;

	public override float BesiegingFactor => ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.BesiegingFactor;

	public override float AssaultingTownFactor => ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.AssaultingTownFactor;

	public override float RaidingFactor => ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.RaidingFactor;

	public override float DefendingFactor => ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.DefendingFactor;

	public override float GetPatrollingFactor(bool isNavalPatrolling)
	{
		return ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.GetPatrollingFactor(isNavalPatrolling);
	}

	public override float CalculatePatrollingScoreForSettlement(Settlement settlement, bool isFromPort, MobileParty mobileParty)
	{
		return ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.CalculatePatrollingScoreForSettlement(settlement, isFromPort, mobileParty);
	}

	public override float CurrentObjectiveValue(MobileParty mobileParty)
	{
		return ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.CurrentObjectiveValue(mobileParty);
	}

	public override float GetTargetScoreForFaction(Settlement targetSettlement, ArmyTypes missionType, MobileParty mobileParty, float ourStrength)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((int)missionType == 1 && targetSettlement != null && ((MBObjectBase)targetSettlement).StringId == "village_ES3_2" && TutorialPhase.Instance != null && !TutorialPhase.Instance.IsCompleted)
		{
			return 0f;
		}
		return ((MBGameModel<TargetScoreCalculatingModel>)this).BaseModel.GetTargetScoreForFaction(targetSettlement, missionType, mobileParty, ourStrength);
	}
}

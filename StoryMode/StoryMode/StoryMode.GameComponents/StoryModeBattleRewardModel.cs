using System.Collections.Generic;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace StoryMode.GameComponents;

public class StoryModeBattleRewardModel : BattleRewardModel
{
	public override int CalculateGoldLossAfterDefeat(Hero partyLeaderHero)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculateGoldLossAfterDefeat(partyLeaderHero);
	}

	public override ExplainedNumber CalculateInfluenceGain(PartyBase party, float influenceValueOfBattle, float contributionShare)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculateInfluenceGain(party, influenceValueOfBattle, contributionShare);
	}

	public override ExplainedNumber CalculateMoraleChangeOnRoundVictory(PartyBase party, MapEventSide partySide, BattleSideEnum roundWinner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculateMoraleChangeOnRoundVictory(party, partySide, roundWinner);
	}

	public override ExplainedNumber CalculateMoraleGainVictory(PartyBase party, float renownValueOfBattle, float contributionShare, MapEvent battle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculateMoraleGainVictory(party, renownValueOfBattle, contributionShare, battle);
	}

	public override int CalculatePlunderedGoldAmountFromDefeatedParty(PartyBase defeatedParty)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculatePlunderedGoldAmountFromDefeatedParty(defeatedParty);
	}

	public override ExplainedNumber CalculateRenownGain(PartyBase party, float renownValueOfBattle, float contributionShare)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (TutorialPhase.Instance != null && !TutorialPhase.Instance.IsCompleted && party == PartyBase.MainParty)
		{
			return default(ExplainedNumber);
		}
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.CalculateRenownGain(party, renownValueOfBattle, contributionShare);
	}

	public override float CalculateShipDamageAfterDefeat(Ship ship)
	{
		return 0f;
	}

	public override MBReadOnlyList<KeyValuePair<Ship, MapEventParty>> DistributeDefeatedPartyShipsAmongWinners(MapEvent mapEvent, MBReadOnlyList<Ship> shipsToLoot, MBReadOnlyList<MapEventParty> winnerParties)
	{
		return new MBReadOnlyList<KeyValuePair<Ship, MapEventParty>>();
	}

	public override float GetAITradePenalty()
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetAITradePenalty();
	}

	public override float GetBannerLootChanceFromDefeatedHero(Hero defeatedHero)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetBannerLootChanceFromDefeatedHero(defeatedHero);
	}

	public override ItemObject GetBannerRewardForWinningMapEvent(MapEvent mapEvent)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetBannerRewardForWinningMapEvent(mapEvent);
	}

	public override float GetExpectedLootedItemValueFromCasualty(Hero winnerPartyLeaderHero, CharacterObject casualtyCharacter)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetExpectedLootedItemValueFromCasualty(winnerPartyLeaderHero, casualtyCharacter);
	}

	public override Figurehead GetFigureheadLoot(MBReadOnlyList<MapEventParty> defeatedParties, PartyBase defeatedSideLeaderParty)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetFigureheadLoot(defeatedParties, defeatedSideLeaderParty);
	}

	public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootCasualtyChances(MBReadOnlyList<MapEventParty> winnerParties, PartyBase defeatedParty)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootCasualtyChances(winnerParties, defeatedParty);
	}

	public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootedItemFromTroop(character, targetValue);
	}

	public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootGoldChances(MBReadOnlyList<MapEventParty> winnerParties)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootGoldChances(winnerParties);
	}

	public override MBList<KeyValuePair<MapEventParty, float>> GetLootItemChancesForWinnerParties(MBReadOnlyList<MapEventParty> winnerParties, PartyBase defeatedParty)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootItemChancesForWinnerParties(winnerParties, defeatedParty);
	}

	public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootMemberChancesForWinnerParties(MBReadOnlyList<MapEventParty> winnerParties)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootMemberChancesForWinnerParties(winnerParties);
	}

	public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootPrisonerChances(MBReadOnlyList<MapEventParty> winnerParties, TroopRosterElement prisonerElement)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetLootPrisonerChances(winnerParties, prisonerElement);
	}

	public override float GetMainPartyMemberScatterChance()
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetMainPartyMemberScatterChance();
	}

	public override int GetPlayerGainedRelationAmount(MapEvent mapEvent, Hero hero)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetPlayerGainedRelationAmount(mapEvent, hero);
	}

	public override float GetShipSiegeEngineHitMoraleEffect(Ship ship, SiegeEngineType siegeEngineType)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetShipSiegeEngineHitMoraleEffect(ship, siegeEngineType);
	}

	public override float GetSunkenShipMoraleEffect(PartyBase shipOwner, Ship ship)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetSunkenShipMoraleEffect(shipOwner, ship);
	}

	public override MBReadOnlyList<MapEventParty> GetWinnerPartiesThatCanPlunderGoldFromShips(MBReadOnlyList<MapEventParty> winnerParties)
	{
		return ((MBGameModel<BattleRewardModel>)this).BaseModel.GetWinnerPartiesThatCanPlunderGoldFromShips(winnerParties);
	}
}

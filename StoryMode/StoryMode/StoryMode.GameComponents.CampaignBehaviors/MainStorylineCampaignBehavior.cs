using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Helpers;
using StoryMode.Quests.PlayerClanQuests;
using StoryMode.StoryModeObjects;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace StoryMode.GameComponents.CampaignBehaviors;

public class MainStorylineCampaignBehavior : CampaignBehaviorBase
{
	public override void RegisterEvents()
	{
		CampaignEvents.CanHeroDieEvent.AddNonSerializedListener((object)this, (ReferenceAction<Hero, KillCharacterActionDetail, bool>)CanHeroDie);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener((object)this, (Action<Clan, Kingdom, Kingdom, ChangeKingdomActionDetail, bool>)OnClanChangedKingdom);
		CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener((object)this, (Action)OnGameLoadFinished);
	}

	private void OnGameLoadFinished()
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		if (Clan.PlayerClan.Kingdom != null && !Clan.PlayerClan.IsUnderMercenaryService)
		{
			Clan.PlayerClan.IsNoble = true;
		}
		int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
		if (StoryModeHeroes.LittleSister.Age < (float)heroComesOfAge)
		{
			if (!StoryModeHeroes.LittleSister.IsDisabled && !StoryModeHeroes.LittleSister.IsNotSpawned)
			{
				DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
			}
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.6", 0))
			{
				AgingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AgingCampaignBehavior>();
				if (campaignBehavior != null)
				{
					FieldInfo field = typeof(AgingCampaignBehavior).GetField("_heroesYoungerThanHeroComesOfAge", BindingFlags.Instance | BindingFlags.NonPublic);
					Dictionary<Hero, int> dictionary = (Dictionary<Hero, int>)field.GetValue(campaignBehavior);
					if (!dictionary.ContainsKey(StoryModeHeroes.LittleSister))
					{
						dictionary.Add(StoryModeHeroes.LittleSister, (int)StoryModeHeroes.LittleSister.Age);
						field.SetValue(campaignBehavior, dictionary);
					}
				}
			}
		}
		else if (!StoryModeHeroes.LittleSister.IsDisabled && (!StoryModeManager.Current.MainStoryLine.FamilyRescued || Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest))))
		{
			DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
		}
		else if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.6", 0) && StoryModeManager.Current.MainStoryLine.FamilyRescued && StoryModeHeroes.LittleSister.IsNotSpawned)
		{
			HeroHelper.SpawnHeroForTheFirstTime(StoryModeHeroes.LittleSister, GetSettlementForRelativeSpawn(StoryModeHeroes.LittleSister));
		}
		if (StoryModeHeroes.LittleBrother.Age < (float)heroComesOfAge)
		{
			if (!StoryModeHeroes.LittleBrother.IsDisabled && !StoryModeHeroes.LittleBrother.IsNotSpawned)
			{
				DisableHeroAction.Apply(StoryModeHeroes.LittleBrother);
			}
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.6", 0))
			{
				AgingCampaignBehavior campaignBehavior2 = Campaign.Current.GetCampaignBehavior<AgingCampaignBehavior>();
				if (campaignBehavior2 != null)
				{
					FieldInfo field2 = typeof(AgingCampaignBehavior).GetField("_heroesYoungerThanHeroComesOfAge", BindingFlags.Instance | BindingFlags.NonPublic);
					Dictionary<Hero, int> dictionary2 = (Dictionary<Hero, int>)field2.GetValue(campaignBehavior2);
					if (!dictionary2.ContainsKey(StoryModeHeroes.LittleBrother))
					{
						dictionary2.Add(StoryModeHeroes.LittleBrother, (int)StoryModeHeroes.LittleBrother.Age);
						field2.SetValue(campaignBehavior2, dictionary2);
					}
				}
			}
		}
		else if (!StoryModeHeroes.LittleBrother.IsDisabled && (!StoryModeManager.Current.MainStoryLine.FamilyRescued || Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest))))
		{
			DisableHeroAction.Apply(StoryModeHeroes.LittleBrother);
		}
		else if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.6", 0) && StoryModeManager.Current.MainStoryLine.FamilyRescued && StoryModeHeroes.LittleBrother.IsNotSpawned)
		{
			HeroHelper.SpawnHeroForTheFirstTime(StoryModeHeroes.LittleBrother, GetSettlementForRelativeSpawn(StoryModeHeroes.LittleBrother));
		}
		EquipmentElement equipmentElement;
		if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.2.0", 0))
		{
			FirstPhase instance = FirstPhase.Instance;
			if (instance != null && instance.AllPiecesCollected)
			{
				ItemObject val = ((GameType)Campaign.Current).ObjectManager.GetObject<ItemObject>("dragon_banner");
				bool flag = false;
				foreach (ItemRosterElement item2 in MobileParty.MainParty.ItemRoster)
				{
					ItemRosterElement current = item2;
					equipmentElement = ((ItemRosterElement)(ref current)).EquipmentElement;
					if (((EquipmentElement)(ref equipmentElement)).Item == val)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					StoryModeManager.Current.MainStoryLine.FirstPhase?.MergeDragonBanner();
				}
			}
		}
		ApplicationVersion lastLoadedGameVersion = MBSaveLoad.LastLoadedGameVersion;
		if (!((ApplicationVersion)(ref lastLoadedGameVersion)).IsOlderThan(ApplicationVersion.FromString("v1.2.9.35367", 0)))
		{
			return;
		}
		List<EquipmentElement> list = new List<EquipmentElement>();
		foreach (ItemRosterElement item3 in MobileParty.MainParty.ItemRoster)
		{
			ItemRosterElement current2 = item3;
			equipmentElement = ((ItemRosterElement)(ref current2)).EquipmentElement;
			ItemObject item = ((EquipmentElement)(ref equipmentElement)).Item;
			string text = ((item != null) ? ((MBObjectBase)item).StringId : null);
			equipmentElement = ((ItemRosterElement)(ref current2)).EquipmentElement;
			if (!((EquipmentElement)(ref equipmentElement)).IsQuestItem)
			{
				switch (text)
				{
				case "dragon_banner_center":
				case "dragon_banner_dragonhead":
				case "dragon_banner_handle":
				case "dragon_banner":
					list.Add(((ItemRosterElement)(ref current2)).EquipmentElement);
					break;
				}
			}
		}
		if (!list.Any())
		{
			return;
		}
		foreach (EquipmentElement item4 in list)
		{
			EquipmentElement current3 = item4;
			MobileParty.MainParty.ItemRoster.AddToCounts(current3, -1);
			MobileParty.MainParty.ItemRoster.AddToCounts(new EquipmentElement(((EquipmentElement)(ref current3)).Item, (ItemModifier)null, (ItemObject)null, true), 1);
		}
	}

	private Settlement GetSettlementForRelativeSpawn(Hero hero)
	{
		if (!hero.HomeSettlement.OwnerClan.IsAtWarWith(Clan.PlayerClan.MapFaction))
		{
			return hero.HomeSettlement;
		}
		if (!Extensions.IsEmpty<Settlement>((IEnumerable<Settlement>)Clan.PlayerClan.MapFaction.Settlements))
		{
			return Extensions.GetRandomElement<Settlement>(Clan.PlayerClan.MapFaction.Settlements);
		}
		foreach (Settlement item in (List<Settlement>)(object)Settlement.All)
		{
			if (!item.MapFaction.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				return item;
			}
		}
		return ((SettlementComponent)Extensions.GetRandomElement<Village>(Village.All)).Settlement;
	}

	private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomActionDetail detail, bool showNotification = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		if (clan == Clan.PlayerClan && newKingdom != null && ((int)detail == 7 || (int)detail == 1))
		{
			Clan.PlayerClan.IsNoble = true;
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void CanHeroDie(Hero hero, KillCharacterActionDetail causeOfDeath, ref bool result)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		if ((hero == StoryModeHeroes.Radagos && StoryModeManager.Current.MainStoryLine.TutorialPhase.IsCompleted && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RescueFamilyQuestBehavior.RescueFamilyQuest)) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(RebuildPlayerClanQuest)) && (int)causeOfDeath == 6) || (int)causeOfDeath == 7)
		{
			result = true;
		}
		else if (hero.IsSpecial && hero != StoryModeHeroes.RadagosHenchman && !StoryModeManager.Current.MainStoryLine.IsCompleted)
		{
			result = false;
		}
	}
}

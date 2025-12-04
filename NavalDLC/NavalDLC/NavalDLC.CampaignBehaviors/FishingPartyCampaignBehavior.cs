using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors;

public class FishingPartyCampaignBehavior : CampaignBehaviorBase
{
	private ItemObject Fish;

	private int[] _invalidFishingTerrainTypes;

	private const float FishingZoneThreatClosenessDistanceSquared = 16f;

	private const float MinDistanceForInteractionSquared = 0.01f;

	private const int MinFishCountToDropOff = 5;

	private const float MinFishingTimeInHours = 8f;

	private const float MaxFishingTimeInHours = 10f;

	private const float MinRoamingTimeInDays = 1f;

	private const float MaxRoamingTimeInDays = 3f;

	private const float MaxFishToCatchPerHour = 1f;

	private float _maxDistanceBetweenPointsInFishingSpots => Campaign.Current.GetAverageDistanceBetweenClosestTwoTownsWithNavigationType((NavigationType)2) * 1.5f;

	private float _minDistanceBetweenPointsInFishingSpots => _maxDistanceBetweenPointsInFishingSpots * 0.1f;

	private bool CanHaveFishingParties(Village village)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (village.VillageType == DefaultVillageTypes.Fisherman && ((SettlementComponent)village).Settlement.Culture.FishingPartyTemplate != null)
		{
			CampaignVec2 val = village.DropOffLocation();
			return ((CampaignVec2)(ref val)).IsValid();
		}
		return false;
	}

	private bool CanCreateFishingParties(Village village)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if ((int)village.VillageState == 0 && ((SettlementComponent)village).Settlement.Party.MapEvent == null && village.Hearth > (float)Campaign.Current.Models.PartySizeLimitModel.MinimumNumberOfVillagersAtVillagerParty && CanHaveFishingParties(village) && GetIdealFishingPartyCount(village) > ((List<FishingPartyComponent>)(object)village.FishingParties()).Count)
		{
			int num = 0;
			for (int i = 0; i < ((SettlementComponent)village).Owner.ItemRoster.Count; i++)
			{
				int num2 = num;
				ItemRosterElement val = ((SettlementComponent)village).Owner.ItemRoster[i];
				num = num2 + ((ItemRosterElement)(ref val)).Amount;
			}
			result = num < village.GetWarehouseCapacity();
		}
		return result;
	}

	private float EndingRoamingChance(FishingPartyComponent fishingParty)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = 9f;
		CampaignTime roamingStartTime = fishingParty.RoamingStartTime;
		float elapsedHoursUntilNow = ((CampaignTime)(ref roamingStartTime)).ElapsedHoursUntilNow;
		if (elapsedHoursUntilNow < 1f * (float)CampaignTime.HoursInDay - num * 0.5f)
		{
			return 0f;
		}
		if (elapsedHoursUntilNow > 3f * (float)CampaignTime.HoursInDay)
		{
			return 1f;
		}
		int num2 = MathF.Round((3f * (float)CampaignTime.HoursInDay - elapsedHoursUntilNow) / num);
		return 1f / (float)(num2 + 1);
	}

	private float EndingFishingChance(FishingPartyComponent fishingParty)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		CampaignTime fishingWaitStartTime = fishingParty.FishingWaitStartTime;
		float elapsedHoursUntilNow = ((CampaignTime)(ref fishingWaitStartTime)).ElapsedHoursUntilNow;
		if (elapsedHoursUntilNow < 7.5f)
		{
			return 0f;
		}
		if (elapsedHoursUntilNow > 10f)
		{
			return 1f;
		}
		int num = MathF.Round(10f - elapsedHoursUntilNow);
		return 1f / (float)(num + 1);
	}

	private int GetIdealFishingPartySize(Village village)
	{
		return Campaign.Current.Models.PartySizeLimitModel.GetIdealVillagerPartySize(village);
	}

	private int GetIdealFishingPartyCount(Village village)
	{
		int num = Math.Min(2, village.GetHearthLevel() + 1);
		Hero governor = village.Bound.Town.Governor;
		if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.TheCorsairsEdge))
		{
			num += MathF.Round(NavalPerks.Shipmaster.TheCorsairsEdge.SecondaryBonus);
		}
		return num;
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnNewGameCreated);
		CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener((object)this, (Action<MobileParty>)OnHourlyTickParty);
		CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener((object)this, (Action<Settlement>)OnDailySettlementTick);
		CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnGameEarlyLoaded);
	}

	private void InitializeCachedData()
	{
		Fish = MBObjectManager.Instance.GetObject<ItemObject>("fish");
		MBList<int> val = Extensions.ToMBList<int>(Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType((NavigationType)2));
		((List<int>)(object)val).Add(11);
		((List<int>)(object)val).Add(22);
		((List<int>)(object)val).Add(25);
		_invalidFishingTerrainTypes = ((List<int>)(object)val).ToArray();
	}

	private void OnGameEarlyLoaded(CampaignGameStarter starter)
	{
		InitializeCachedData();
	}

	private void OnDailySettlementTick(Settlement settlement)
	{
		Village village = settlement.Village;
		if (village != null && MBRandom.RandomFloat > 0.5f && CanCreateFishingParties(village))
		{
			MobileParty val = FishingPartyComponent.CreateFishingParty(((MBObjectBase)settlement.OwnerClan).StringId + "_1", village);
			village.Hearth = MathF.Max(0f, village.Hearth - (float)((val.MemberRoster.TotalManCount + 1) / 2));
			StartRoaming(val.PartyComponent as FishingPartyComponent);
		}
	}

	private void OnNewGameCreated(CampaignGameStarter starter)
	{
		InitializeCachedData();
		foreach (Village item in (List<Village>)(object)Village.All)
		{
			if (CanHaveFishingParties(item))
			{
				OnDailySettlementTick(((SettlementComponent)item).Settlement);
			}
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void TryReinforceParty(FishingPartyComponent fishingParty)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (((PartyComponent)fishingParty).HomeSettlement.Party.MapEvent != null || (int)((VillagerPartyComponent)fishingParty).Village.VillageState != 0)
		{
			return;
		}
		int num = GetIdealFishingPartySize(((VillagerPartyComponent)fishingParty).Village) - ((PartyComponent)fishingParty).MobileParty.MemberRoster.TotalManCount;
		if (num > (int)((VillagerPartyComponent)fishingParty).Village.Hearth)
		{
			num = (int)((VillagerPartyComponent)fishingParty).Village.Hearth;
		}
		if (num > 0)
		{
			if (num > (int)((PartyComponent)fishingParty).HomeSettlement.Village.Hearth)
			{
				num = (int)((PartyComponent)fishingParty).HomeSettlement.Village.Hearth;
			}
			Village village = ((PartyComponent)fishingParty).HomeSettlement.Village;
			village.Hearth -= (float)((num + 1) / 2);
			CharacterObject character = Extensions.GetRandomElement<PartyTemplateStack>(((PartyComponent)fishingParty).HomeSettlement.Culture.FishingPartyTemplate.Stacks).Character;
			((PartyComponent)fishingParty).MobileParty.MemberRoster.AddToCounts(character, num, false, 0, 0, true, -1);
		}
		foreach (Ship item in (List<Ship>)(object)((PartyComponent)fishingParty).MobileParty.Ships)
		{
			if (item.HitPoints < item.MaxHitPoints)
			{
				RepairShipAction.ApplyForFree(item);
			}
		}
	}

	private void CatchFish(FishingPartyComponent fishingParty)
	{
		MobileParty mobileParty = ((PartyComponent)fishingParty).MobileParty;
		int num = 0;
		float num2 = 1f * MBRandom.RandomFloat;
		Hero governor = ((VillagerPartyComponent)fishingParty).Village.Bound.Town.Governor;
		if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.MasterAngler))
		{
			num2 += num2 * NavalPerks.Shipmaster.MasterAngler.SecondaryBonus;
		}
		num = MBRandom.RoundRandomized(num2);
		int num3 = (int)((mobileParty.TotalWeightCarried - (float)mobileParty.InventoryCapacity) / Fish.Weight);
		if (num3 > num)
		{
			num = num3;
		}
		if (num > 0)
		{
			mobileParty.ItemRoster.AddToCounts(Fish, num);
		}
	}

	private void OnHourlyTickParty(MobileParty party)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Invalid comparison between Unknown and I4
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (party.MapEvent != null || !(party.PartyComponent is FishingPartyComponent fishingPartyComponent))
		{
			return;
		}
		CampaignVec2 val;
		if (party.ShortTermBehavior != party.DefaultBehavior)
		{
			fishingPartyComponent.IsFishing = false;
			if (party.IsFleeing() && party.ShortTermTargetParty != null)
			{
				val = party.ShortTermTargetParty.Position;
				if (((CampaignVec2)(ref val)).DistanceSquared(party.TargetPosition) < 16f)
				{
					StartRoaming(fishingPartyComponent);
				}
			}
		}
		else if ((int)party.DefaultBehavior == 0)
		{
			if (fishingPartyComponent.IsRoaming)
			{
				if (fishingPartyComponent.IsFishing)
				{
					CatchFish(fishingPartyComponent);
					if (MBRandom.RandomFloat < EndingFishingChance(fishingPartyComponent))
					{
						if (MBRandom.RandomFloat < EndingRoamingChance(fishingPartyComponent))
						{
							StartDropOff(fishingPartyComponent);
						}
						else
						{
							GoToNewFishingPoint(fishingPartyComponent);
						}
					}
				}
				else if (MBRandom.RandomFloat < EndingRoamingChance(fishingPartyComponent) && ((PartyComponent)fishingPartyComponent).MobileParty.ItemRoster.Count > 5)
				{
					StartDropOff(fishingPartyComponent);
				}
				else
				{
					GoToNewFishingPoint(fishingPartyComponent);
				}
			}
			else
			{
				StartDropOff(fishingPartyComponent);
			}
		}
		else
		{
			if ((int)party.DefaultBehavior != 9)
			{
				return;
			}
			val = party.Position - party.TargetPosition;
			if (!(((CampaignVec2)(ref val)).LengthSquared < 0.01f))
			{
				return;
			}
			party.SetMoveModeHold();
			val = party.Position - ((VillagerPartyComponent)fishingPartyComponent).Village.DropOffLocation();
			if (((CampaignVec2)(ref val)).LengthSquared < 0.01f)
			{
				OnDropOff(fishingPartyComponent);
				if (((List<FishingPartyComponent>)(object)((VillagerPartyComponent)fishingPartyComponent).Village.FishingParties()).Count > GetIdealFishingPartyCount(((VillagerPartyComponent)fishingPartyComponent).Village) && !party.IsVisible)
				{
					DestroyPartyAction.Apply((PartyBase)null, party);
				}
				else
				{
					StartRoaming(fishingPartyComponent);
				}
			}
			else if (!fishingPartyComponent.IsRoaming)
			{
				Debug.FailedAssert("fishing ship not roaming nor dropping off", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\FishingPartyCampaignBehavior.cs", "OnHourlyTickParty", 407);
				StartDropOff(fishingPartyComponent);
			}
			else
			{
				fishingPartyComponent.IsFishing = true;
			}
		}
	}

	private void OnDropOff(FishingPartyComponent fishingParty)
	{
		TryReinforceParty(fishingParty);
		((SettlementComponent)((VillagerPartyComponent)fishingParty).Village).Settlement.ItemRoster.Add((IEnumerable<ItemRosterElement>)((PartyComponent)fishingParty).MobileParty.ItemRoster);
		((PartyComponent)fishingParty).MobileParty.ItemRoster.Clear();
		Town town = ((VillagerPartyComponent)fishingParty).Village.Bound.Town;
		Hero governor = town.Governor;
		if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.TheHelmsmansShield))
		{
			town.Prosperity += NavalPerks.Shipmaster.TheHelmsmansShield.SecondaryBonus;
		}
		Hero governor2 = town.Governor;
		if (governor2 != null && governor2.GetPerkValue(NavalPerks.Shipmaster.RavenEye))
		{
			town.Loyalty += NavalPerks.Shipmaster.RavenEye.SecondaryBonus;
		}
	}

	private void StartRoaming(FishingPartyComponent fishingParty)
	{
		fishingParty.IsRoaming = true;
		fishingParty.IsFishing = false;
		GoToNewFishingPoint(fishingParty);
	}

	private void StartDropOff(FishingPartyComponent fishingParty)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		fishingParty.IsFishing = false;
		fishingParty.IsRoaming = false;
		CampaignVec2 val = ((VillagerPartyComponent)fishingParty).Village.DropOffLocation();
		((PartyComponent)fishingParty).MobileParty.SetMoveGoToPoint(val, (NavigationType)2);
	}

	private void GoToNewFishingPoint(FishingPartyComponent fishingParty)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		fishingParty.IsFishing = false;
		CampaignVec2 val = ((VillagerPartyComponent)fishingParty).Village.DropOffLocation();
		int num = 20;
		do
		{
			val = NavigationHelper.FindReachablePointAroundPosition(((VillagerPartyComponent)fishingParty).Village.DropOffLocation(), _invalidFishingTerrainTypes, _maxDistanceBetweenPointsInFishingSpots, _minDistanceBetweenPointsInFishingSpots, true);
			num--;
		}
		while (num > 0 && ((CampaignVec2)(ref val)).Distance(((PartyComponent)fishingParty).MobileParty.Position) < _minDistanceBetweenPointsInFishingSpots);
		((PartyComponent)fishingParty).MobileParty.SetMoveGoToPoint(val, (NavigationType)2);
	}

	[CommandLineArgumentFunction("show_drop_off", "campaign")]
	public static string show_drop_off(List<string> strings)
	{
		return ((object)((SettlementComponent)Extensions.MinBy<Village, float>((IEnumerable<Village>)Village.All, (Func<Village, float>)delegate(Village v)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			CampaignVec2 position = ((SettlementComponent)v).Settlement.Position;
			return ((CampaignVec2)(ref position)).DistanceSquared(MobileParty.MainParty.Position);
		})).Name).ToString();
	}
}

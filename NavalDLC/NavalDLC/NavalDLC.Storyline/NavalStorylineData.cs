using System.Collections.Generic;
using Helpers;
using NavalDLC.Storyline.CampaignBehaviors;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline;

public class NavalStorylineData
{
	public enum NavalStorylineStage
	{
		None = -1,
		Act1,
		Act2,
		Act3Quest1,
		Act3Quest2,
		Act3SpeakToSailors,
		Act3Quest4,
		Act3Quest5,
		Act3SpeakToGunnarAndSister
	}

	private const string NaukosStringId = "naval_storyline_naukos";

	private const string GangradirStringId = "naval_storyline_gangradir";

	private const string ValmissaStringId = "naval_storyline_valmissa";

	private const string BjolgurStringId = "naval_storyline_bjolgur";

	private const string PurigStringId = "naval_storyline_northerner";

	private const string EmiraAlFahdaStringId = "naval_storyline_emira_al_fahda";

	private const string LaharStringId = "naval_storyline_lahar";

	private const string PrusasStringId = "naval_storyline_crusas";

	public const string NavalStoryLineOutOfTownMenuId = "naval_storyline_outside_town";

	public const string NavalStoryLineEncounterBlockingMenuId = "naval_storyline_encounter_blocking";

	public const string NavalStoryLineVirtualPortMenuId = "naval_storyline_virtualport";

	public const string NavalStoryLineEncounterMeetingMenuId = "naval_storyline_encounter_meeting";

	public const string NavalStoryLineEncounterMenuId = "naval_storyline_encounter";

	public const string NavalStoryLineJoinEncounterMenuId = "naval_storyline_join_encounter";

	private const string HomeSettlementStringId = "town_V8";

	private const string Act3Quest1TargetSettlementStringId = "town_N1";

	private const string Act3Quest2TargetSettlementStringId = "town_A1";

	private const string Act3Quest3TargetSettlementStringId = "town_S3";

	private const string Act3Quest4TargetSettlementStringId = "town_V7";

	public const string GunnarsVillageStringId = "castle_village_N7_2";

	public const string InquireAtOsticanCharacterSpawnPointTag = "sp_storyline_npc";

	private Hero _naukos;

	private Hero _gangradir;

	private Hero _valmissa;

	private Hero _bjolgur;

	private Hero _purig;

	private Hero _lahar;

	private Hero _emiraAlFahda;

	private Hero _prusas;

	private Banner _corsairBanner;

	private Settlement _homeSettlement;

	private Settlement _act3Quest1TargetSettlement;

	private Settlement _act3Quest2TargetSettlement;

	private Settlement _act3Quest3TargetSettlement;

	private Settlement _act3Quest4TargetSettlement;

	public static Hero Naukos => NavalDLCManager.Instance.NavalStorylineData._naukos;

	public static Hero Gangradir => NavalDLCManager.Instance.NavalStorylineData._gangradir;

	public static Hero Valmissa => NavalDLCManager.Instance.NavalStorylineData._valmissa;

	public static Hero Bjolgur => NavalDLCManager.Instance.NavalStorylineData._bjolgur;

	public static Hero Purig => NavalDLCManager.Instance.NavalStorylineData._purig;

	public static Hero Lahar => NavalDLCManager.Instance.NavalStorylineData._lahar;

	public static Hero EmiraAlFahda => NavalDLCManager.Instance.NavalStorylineData._emiraAlFahda;

	public static Hero Prusas => NavalDLCManager.Instance.NavalStorylineData._prusas;

	public static Settlement HomeSettlement => NavalDLCManager.Instance.NavalStorylineData._homeSettlement;

	public static Settlement Act3Quest1TargetSettlement => NavalDLCManager.Instance.NavalStorylineData._act3Quest1TargetSettlement;

	public static Settlement Act3Quest2TargetSettlement => NavalDLCManager.Instance.NavalStorylineData._act3Quest2TargetSettlement;

	public static Settlement Act3Quest3TargetSettlement => NavalDLCManager.Instance.NavalStorylineData._act3Quest3TargetSettlement;

	public static Settlement Act3Quest4TargetSettlement => NavalDLCManager.Instance.NavalStorylineData._act3Quest4TargetSettlement;

	public static Banner CorsairBanner => NavalDLCManager.Instance.NavalStorylineData._corsairBanner;

	public void Initialize()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		if (!IsNavalStorylineCanceled())
		{
			CacheNavalStorylineSettlements();
			CreateStorylineHero("naval_storyline_naukos", out _naukos);
			CreateStorylineHero("naval_storyline_gangradir", out _gangradir);
			CreateStorylineHero("naval_storyline_valmissa", out _valmissa);
			CreateStorylineHero("naval_storyline_bjolgur", out _bjolgur);
			CreateStorylineHero("naval_storyline_northerner", out _purig);
			CreateStorylineHero("naval_storyline_lahar", out _lahar);
			CreateStorylineHero("naval_storyline_emira_al_fahda", out _emiraAlFahda);
			CreateStorylineHero("naval_storyline_crusas", out _prusas);
			_corsairBanner = new Banner("11.97.166.1528.1528.764.764.1.0.0.500.35.171.555.555.764.764.0.0.0.167.35.171.350.350.764.764.0.0.0");
		}
	}

	private void CacheNavalStorylineSettlements()
	{
		_homeSettlement = Settlement.Find("town_V8");
		_act3Quest1TargetSettlement = Settlement.Find("town_N1");
		_act3Quest2TargetSettlement = Settlement.Find("town_A1");
		_act3Quest3TargetSettlement = Settlement.Find("town_S3");
		_act3Quest4TargetSettlement = Settlement.Find("town_V7");
	}

	private void CreateStorylineHero(string stringId, out Hero hero)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		hero = Campaign.Current.CampaignObjectManager.Find<Hero>(stringId);
		if (hero == null)
		{
			CharacterObject val = MBObjectManager.Instance.GetObject<CharacterObject>(stringId);
			HeroCreator.CreateBasicHero(stringId, val, ref hero, true);
			hero.SetName(((BasicCharacterObject)val).Name, ((BasicCharacterObject)val).Name);
			CampaignTime randomBirthDayForAge = HeroHelper.GetRandomBirthDayForAge(((BasicCharacterObject)val).Age);
			hero.SetBirthDay(randomBirthDayForAge);
			hero.SetNewOccupation((Occupation)31);
			if (((MBObjectBase)val.Culture).StringId == "aserai")
			{
				hero.BornSettlement = Act3Quest2TargetSettlement;
			}
			else
			{
				hero.BornSettlement = HomeSettlement;
			}
		}
		hero.CharacterObject.SetTransferableInPartyScreen(false);
	}

	public static bool IsNavalStorylineHero(Hero hero)
	{
		if (hero != Gangradir && hero != Valmissa && hero != Purig && hero != Bjolgur && hero != Naukos && hero != Lahar && hero != EmiraAlFahda)
		{
			return hero == Prusas;
		}
		return true;
	}

	public static void StartNavalStoryline()
	{
		((QuestBase)new InquireAtOstican()).StartQuest();
	}

	public static bool IsStorylineActivationPossible()
	{
		if (!Campaign.Current.IsMainHeroDisguised)
		{
			return MobileParty.MainParty.Army == null;
		}
		return false;
	}

	public static void ActivateNavalStoryline()
	{
		NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
		if (campaignBehavior != null && !campaignBehavior.IsNavalStorylineActive())
		{
			campaignBehavior.ChangeNavalStorylineActivity(activity: true);
		}
	}

	public static void DeactivateNavalStoryline()
	{
		NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
		if (campaignBehavior != null && campaignBehavior.IsNavalStorylineActive())
		{
			campaignBehavior.ChangeNavalStorylineActivity(activity: false);
		}
	}

	public static bool IsMainPartyAllowed()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		AccessDetails val = default(AccessDetails);
		Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterSettlement(Settlement.CurrentSettlement, ref val);
		if ((int)val.AccessLevel == 2 && !Clan.PlayerClan.MapFaction.IsAtWarWith(Settlement.CurrentSettlement.MapFaction))
		{
			if (Settlement.CurrentSettlement.SiegeEvent != null)
			{
				if (!Settlement.CurrentSettlement.SiegeEvent.IsBlockadeActive)
				{
					return MobileParty.MainParty.HasNavalNavigationCapability;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public static bool IsTutorialSkipped()
	{
		return Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>()?.IsTutorialSkipped() ?? false;
	}

	public static bool IsNavalStoryLineActive()
	{
		return Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>()?.IsNavalStorylineActive() ?? false;
	}

	public static bool HasCompletedLast(NavalStorylineStage stage)
	{
		NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
		if (campaignBehavior != null)
		{
			return stage == campaignBehavior.GetNavalStorylineStage();
		}
		return false;
	}

	public static NavalStorylineStage GetStorylineStage()
	{
		return Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>()?.GetNavalStorylineStage() ?? NavalStorylineStage.None;
	}

	public static bool IsNavalStorylineCanceled()
	{
		return Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>()?.GetIsNavalStorylineCanceled() ?? true;
	}

	public static void OnStorylineProgress(NavalStorylineQuestBase navalQuest)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ActivateNavalStoryline();
		FadeToBlack();
		if (navalQuest.Template != null)
		{
			MobileParty.MainParty.InitializeMobilePartyAtPosition(navalQuest.Template, MobileParty.MainParty.Position);
			foreach (Ship item in (List<Ship>)(object)MobileParty.MainParty.Ships)
			{
				item.IsTradeable = false;
				item.IsUsedByQuest = true;
			}
		}
		AddGangradirToMainParty();
		GiveProvisionsToPlayer();
		MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
		PlayerEncounter.Finish(true);
	}

	private static void GiveProvisionsToPlayer()
	{
		Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>()?.GiveProvisionsToPlayer();
	}

	public static void AddGangradirToMainParty()
	{
		Gangradir.Heal(Gangradir.MaxHitPoints, false);
		MobileParty.MainParty.MemberRoster.AddToCounts(Gangradir.CharacterObject, 1, false, 0, 0, true, -1);
	}

	public static void TeleportMainHeroAndGangradirBackToBase()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		FadeToBlack();
		if (MobileParty.MainParty.CurrentSettlement != HomeSettlement)
		{
			MobileParty.MainParty.Position = (MobileParty.MainParty.HasNavalNavigationCapability ? HomeSettlement.PortPosition : HomeSettlement.GatePosition);
			MobileParty.MainParty.IsCurrentlyAtSea = MobileParty.MainParty.HasNavalNavigationCapability;
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Finish(true);
			}
			if (MobileParty.MainParty.IsInRaftState)
			{
				RaftStateChangeAction.DeactivateRaftStateForParty(MobileParty.MainParty);
			}
			if (Hero.MainHero.IsPrisoner)
			{
				EndCaptivityAction.ApplyByReleasedAfterBattle(Hero.MainHero);
			}
			if (MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.SetPosition(new CampaignVec2(Vec2.Invalid, false));
			}
			EncounterManager.StartSettlementEncounter(MobileParty.MainParty, HomeSettlement);
		}
		Gangradir.Heal(Gangradir.MaxHitPoints, false);
		EnterSettlementAction.ApplyForCharacterOnly(Gangradir, HomeSettlement);
		GameMenu.ActivateGameMenu("naval_storyline_outside_town");
		MobileParty.MainParty.SetMoveModeHold();
		GameState activeState = GameStateManager.Current.ActiveState;
		MapState val;
		if ((val = (MapState)(object)((activeState is MapState) ? activeState : null)) != null)
		{
			val.Handler.TeleportCameraToMainParty();
		}
	}

	private static void FadeToBlack()
	{
		GameState activeState = Game.Current.GameStateManager.ActiveState;
		MapState val;
		if ((val = (MapState)(object)((activeState is MapState) ? activeState : null)) != null)
		{
			val.OnFadeInAndOut(0.1f, 0.5f, 0.35f);
		}
	}

	public static MissionInitializerRecord GetNavalMissionInitializerTemplate(string sceneName)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		MissionInitializerRecord result = default(MissionInitializerRecord);
		((MissionInitializerRecord)(ref result))._002Ector(sceneName);
		result.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
		result.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
		result.DecalAtlasGroup = 2;
		return result;
	}
}

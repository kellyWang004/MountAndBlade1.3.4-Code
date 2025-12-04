using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GameComponents;

public class NavalAgentStatCalculateModel : AgentStatCalculateModel
{
	private Dictionary<Agent, Figurehead> _agentFigureHeadSpawnMap = new Dictionary<Agent, Figurehead>();

	public override float GetDifficultyModifier()
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetDifficultyModifier();
	}

	public override bool CanAgentRideMount(Agent agent, Agent targetMount)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.CanAgentRideMount(agent, targetMount);
	}

	private void ApplyFigureheadBonuses(Agent agent, AgentDrivenProperties agentDrivenProperties, Figurehead figureHead)
	{
		float effectAmount = figureHead.EffectAmount;
		if (figureHead == DefaultFigureheads.Hawk || figureHead == DefaultFigureheads.Boar)
		{
			_agentFigureHeadSpawnMap.Add(agent, figureHead);
		}
		else if (figureHead == DefaultFigureheads.Raven)
		{
			agentDrivenProperties.ThrowingWeaponDamageMultiplierBonus += effectAmount;
		}
		else if (figureHead == DefaultFigureheads.SaberToothTiger)
		{
			agentDrivenProperties.ArmorPenetrationMultiplierCrossbow += effectAmount;
			agentDrivenProperties.ArmorPenetrationMultiplierBow += effectAmount;
		}
		else if (figureHead == DefaultFigureheads.Oxen)
		{
			agent.HealthLimit += effectAmount;
			agent.Health += effectAmount;
		}
	}

	public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
	{
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
		agentDrivenProperties.AiShooterErrorWoRangeUpdate = 0f;
		NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		if (missionBehavior == null)
		{
			return;
		}
		object obj;
		if (agent == null)
		{
			obj = null;
		}
		else
		{
			IAgentOriginBase origin = agent.Origin;
			obj = ((origin != null) ? origin.BattleCombatant : null);
		}
		PartyBase val = (PartyBase)((obj is PartyBase) ? obj : null);
		MobileParty obj2 = ((val != null && val.IsMobile) ? val.MobileParty : null);
		object obj3;
		if (obj2 == null)
		{
			obj3 = null;
		}
		else
		{
			Army army = obj2.Army;
			if (army == null)
			{
				obj3 = null;
			}
			else
			{
				MobileParty leaderParty = army.LeaderParty;
				if (leaderParty == null)
				{
					obj3 = null;
				}
				else
				{
					Hero leaderHero = leaderParty.LeaderHero;
					obj3 = ((leaderHero != null) ? leaderHero.CharacterObject : null);
				}
			}
		}
		CharacterObject val2 = (CharacterObject)obj3;
		Ship val3 = ((val == null || ((List<Ship>)(object)val.Ships).Count <= 0) ? null : ((val != null) ? val.FlagShip : null));
		Figurehead val4 = ((val3 != null) ? val3.Figurehead : null);
		bool flag = val2 != null && val2.GetPerkValue(NavalPerks.Shipmaster.Commodore) && val3 != null && val4 != null;
		if (flag)
		{
			ApplyFigureheadBonuses(agent, agentDrivenProperties, val4);
		}
		foreach (MissionShip item in (List<MissionShip>)(object)missionBehavior.AllShips)
		{
			IShipOrigin shipOrigin = item.ShipOrigin;
			Ship val5 = (Ship)(object)((shipOrigin is Ship) ? shipOrigin : null);
			if ((!flag || val5 != val3) && item.GetIsAgentOnShip(agent))
			{
				Figurehead val6 = ((val5 != null) ? val5.Figurehead : null);
				if (val6 != null)
				{
					ApplyFigureheadBonuses(agent, agentDrivenProperties, val6);
				}
				agentDrivenProperties.MeleeWeaponDamageMultiplierBonus += item.ShipOrigin.CrewMeleeDamageFactor;
				break;
			}
		}
	}

	public override void InitializeMissionEquipment(Agent agent)
	{
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.InitializeMissionEquipment(agent);
		if (Mission.Current.IsNavalBattle && agent.IsHuman)
		{
			BasicCharacterObject character = agent.Character;
			CharacterObject val = (CharacterObject)(object)((character is CharacterObject) ? character : null);
			if (val != null)
			{
				Formation formation = agent.Formation;
				object obj;
				if (formation == null)
				{
					obj = null;
				}
				else
				{
					Agent captain = formation.Captain;
					obj = ((captain != null) ? captain.Character : null);
				}
				CharacterObject val2 = (CharacterObject)((obj is CharacterObject) ? obj : null);
				if (val == val2)
				{
					val2 = null;
				}
				MissionEquipment equipment = agent.Equipment;
				ExplainedNumber val5 = default(ExplainedNumber);
				ExplainedNumber val6 = default(ExplainedNumber);
				ExplainedNumber val7 = default(ExplainedNumber);
				for (int i = 0; i < 5; i++)
				{
					EquipmentIndex val3 = (EquipmentIndex)i;
					MissionWeapon val4 = equipment[val3];
					if (((MissionWeapon)(ref val4)).IsEmpty)
					{
						continue;
					}
					WeaponComponentData currentUsageItem = ((MissionWeapon)(ref val4)).CurrentUsageItem;
					if (currentUsageItem == null)
					{
						continue;
					}
					if (currentUsageItem.IsShield)
					{
						((ExplainedNumber)(ref val5))._002Ector((float)((MissionWeapon)(ref val4)).HitPoints, false, (TextObject)null);
						PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.Forceful, val, true, ref val5, false);
						int num = MathF.Round(((ExplainedNumber)(ref val5)).ResultNumber);
						if (num != ((MissionWeapon)(ref val4)).HitPoints)
						{
							equipment.SetHitPointsOfSlot(val3, (short)num, true);
						}
					}
					if (currentUsageItem.IsConsumable && currentUsageItem.RelevantSkill != null)
					{
						((ExplainedNumber)(ref val6))._002Ector(0f, false, (TextObject)null);
						if (currentUsageItem.RelevantSkill == DefaultSkills.Throwing && val2 != null && val2.GetPerkValue(NavalPerks.Boatswain.WellStocked))
						{
							((ExplainedNumber)(ref val6)).Add(NavalPerks.Boatswain.WellStocked.SecondaryBonus, (TextObject)null, (TextObject)null);
						}
						int num2 = MathF.Round(((ExplainedNumber)(ref val6)).ResultNumber);
						((ExplainedNumber)(ref val7))._002Ector((float)(((MissionWeapon)(ref val4)).Amount + num2), false, (TextObject)null);
						if ((currentUsageItem.RelevantSkill == DefaultSkills.Bow || currentUsageItem.RelevantSkill == DefaultSkills.Crossbow || currentUsageItem.RelevantSkill == DefaultSkills.Throwing) && val2 != null && val2.GetPerkValue(NavalPerks.Boatswain.WellStocked))
						{
							((ExplainedNumber)(ref val7)).AddFactor(NavalPerks.Boatswain.WellStocked.PrimaryBonus, (TextObject)null);
						}
						if (val2 != null && val2.GetPerkValue(NavalPerks.Boatswain.ShipwrightsInsight))
						{
							((ExplainedNumber)(ref val7)).AddFactor(NavalPerks.Boatswain.ShipwrightsInsight.SecondaryBonus, (TextObject)null);
						}
						int num3 = MathF.Round(((ExplainedNumber)(ref val7)).ResultNumber);
						if (num3 != ((MissionWeapon)(ref val4)).Amount)
						{
							equipment.SetAmountOfSlot(val3, (short)num3, true);
						}
					}
				}
			}
		}
		NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		if (missionBehavior == null)
		{
			return;
		}
		foreach (MissionShip item in (List<MissionShip>)(object)missionBehavior.AllShips)
		{
			if (!item.GetIsAgentOnShip(agent))
			{
				continue;
			}
			bool flag = MathF.Abs(item.ShipOrigin.CrewShieldHitPointsFactor) > 1E-05f;
			bool flag2 = item.ShipOrigin.AdditionalArcherQuivers != 0;
			bool flag3 = item.ShipOrigin.AdditionalThrowingWeaponStack != 0;
			if (!(flag || flag2 || flag3))
			{
				continue;
			}
			for (EquipmentIndex val8 = (EquipmentIndex)0; (int)val8 < 4; val8 = (EquipmentIndex)(val8 + 1))
			{
				MissionWeapon val9 = agent.Equipment[val8];
				if (((MissionWeapon)(ref val9)).IsEmpty)
				{
					continue;
				}
				val9 = agent.Equipment[val8];
				WeaponComponentData weaponComponentDataForUsage = ((MissionWeapon)(ref val9)).GetWeaponComponentDataForUsage(0);
				if (weaponComponentDataForUsage.IsShield)
				{
					if (flag)
					{
						MissionEquipment equipment2 = agent.Equipment;
						EquipmentIndex val10 = val8;
						val9 = agent.Equipment[val8];
						equipment2.SetHitPointsOfSlot(val10, (short)((float)((MissionWeapon)(ref val9)).ModifiedMaxHitPoints * (1f + item.ShipOrigin.CrewShieldHitPointsFactor)), true);
						flag = false;
					}
				}
				else
				{
					if (!weaponComponentDataForUsage.IsConsumable)
					{
						continue;
					}
					if (weaponComponentDataForUsage.IsRangedWeapon)
					{
						if (flag3)
						{
							MissionEquipment equipment3 = agent.Equipment;
							EquipmentIndex val11 = val8;
							val9 = agent.Equipment[val8];
							equipment3.SetAmountOfSlot(val11, (short)(((MissionWeapon)(ref val9)).ModifiedMaxAmount * (1 + item.ShipOrigin.AdditionalThrowingWeaponStack)), false);
							flag3 = false;
						}
					}
					else if (weaponComponentDataForUsage.IsAmmo && flag2)
					{
						MissionEquipment equipment4 = agent.Equipment;
						EquipmentIndex val12 = val8;
						val9 = agent.Equipment[val8];
						equipment4.SetAmountOfSlot(val12, (short)(((MissionWeapon)(ref val9)).ModifiedMaxAmount * (1 + item.ShipOrigin.AdditionalArcherQuivers)), false);
						flag2 = false;
					}
				}
			}
		}
	}

	public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.UpdateAgentStats(agent, agentDrivenProperties);
		_ = agent.Character;
		if (!Mission.Current.IsNavalBattle || !agent.IsHuman)
		{
			return;
		}
		UpdateNavalHumanStats(agent, agentDrivenProperties);
		MissionShip missionShip = agent.GetComponent<AgentNavalComponent>()?.SteppedShip;
		if (missionShip != null)
		{
			IShipOrigin shipOrigin = missionShip.ShipOrigin;
			IShipOrigin obj = ((shipOrigin is Ship) ? shipOrigin : null);
			Figurehead val = ((obj != null) ? ((Ship)obj).Figurehead : null);
			if (val != null && val == DefaultFigureheads.Siren)
			{
				BattleSideEnum side = agent.Team.Side;
				Team team = missionShip.Team;
				if ((BattleSideEnum?)side != ((team != null) ? new BattleSideEnum?(team.Side) : ((BattleSideEnum?)null)))
				{
					agentDrivenProperties.DamageMultiplierBonus = val.EffectAmount;
				}
			}
		}
		if (_agentFigureHeadSpawnMap.TryGetValue(agent, out var value))
		{
			float effectAmount = value.EffectAmount;
			if (value == DefaultFigureheads.Hawk)
			{
				agentDrivenProperties.WeaponInaccuracy *= 1f - effectAmount;
			}
			else if (value == DefaultFigureheads.Boar)
			{
				effectAmount += 1f;
				agentDrivenProperties.ArmorHead *= effectAmount;
				agentDrivenProperties.ArmorTorso *= effectAmount;
				agentDrivenProperties.ArmorArms *= effectAmount;
				agentDrivenProperties.ArmorLegs *= effectAmount;
			}
		}
	}

	private void UpdateNavalHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Invalid comparison between Unknown and I4
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Invalid comparison between Unknown and I4
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Invalid comparison between Unknown and I4
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Invalid comparison between Unknown and I4
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Invalid comparison between Unknown and I4
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val2 = default(ExplainedNumber);
		((ExplainedNumber)(ref val2))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val3 = default(ExplainedNumber);
		((ExplainedNumber)(ref val3))._002Ector(0.2f, false, (TextObject)null);
		ExplainedNumber val4 = default(ExplainedNumber);
		((ExplainedNumber)(ref val4))._002Ector(0.2f, false, (TextObject)null);
		ExplainedNumber val5 = default(ExplainedNumber);
		((ExplainedNumber)(ref val5))._002Ector(0.2f, false, (TextObject)null);
		((ExplainedNumber)(ref val)).LimitMin(0f);
		((ExplainedNumber)(ref val2)).LimitMin(0f);
		((ExplainedNumber)(ref val3)).LimitMin(0f);
		((ExplainedNumber)(ref val4)).LimitMin(0f);
		((ExplainedNumber)(ref val5)).LimitMin(0f);
		int effectiveSkill = ((AgentStatCalculateModel)this).GetEffectiveSkill(agent, NavalSkills.Mariner);
		SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref val, effectiveSkill);
		SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref val2, effectiveSkill);
		SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref val3, effectiveSkill);
		SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref val4, effectiveSkill);
		SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref val5, effectiveSkill);
		Equipment spawnEquipment = agent.SpawnEquipment;
		agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum();
		agentDrivenProperties.ArmorTorso = spawnEquipment.GetHumanBodyArmorSum();
		agentDrivenProperties.ArmorLegs = spawnEquipment.GetLegArmorSum();
		agentDrivenProperties.ArmorArms = spawnEquipment.GetArmArmorSum();
		agentDrivenProperties.OffhandWeaponDefendSpeedMultiplier = 1f;
		BasicCharacterObject character = agent.Character;
		CharacterObject val6 = (CharacterObject)(object)((character is CharacterObject) ? character : null);
		MissionEquipment equipment = agent.Equipment;
		float num = equipment.GetTotalWeightOfWeapons();
		float effectiveArmorEncumbrance = ((AgentStatCalculateModel)this).GetEffectiveArmorEncumbrance(agent, spawnEquipment);
		_ = agent.Monster.Weight;
		EquipmentIndex primaryWieldedItemIndex = agent.GetPrimaryWieldedItemIndex();
		EquipmentIndex offhandWieldedItemIndex = agent.GetOffhandWieldedItemIndex();
		int effectiveSkill2 = ((AgentStatCalculateModel)this).GetEffectiveSkill(agent, DefaultSkills.Athletics);
		Formation formation = agent.Formation;
		object obj;
		if (formation == null)
		{
			obj = null;
		}
		else
		{
			Agent captain = formation.Captain;
			obj = ((captain != null) ? captain.Character : null);
		}
		CharacterObject val7 = (CharacterObject)((obj is CharacterObject) ? obj : null);
		Formation formation2 = agent.Formation;
		if (((formation2 != null) ? formation2.Captain : null) == agent)
		{
			val7 = null;
		}
		PerkHelper.AddPerkBonusForCharacter(NavalPerks.Shipmaster.WindRider, val6, true, ref val4, false);
		PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.WindRider, val7, ref val4);
		PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.RollingThunder, val6, true, ref val3, false);
		MissionWeapon val8;
		if ((int)primaryWieldedItemIndex != -1)
		{
			val8 = equipment[primaryWieldedItemIndex];
			ItemObject item = ((MissionWeapon)(ref val8)).Item;
			WeaponComponent weaponComponent = item.WeaponComponent;
			if (weaponComponent != null)
			{
				ItemTypeEnum itemType = weaponComponent.GetItemType();
				bool flag = false;
				if (val6 != null)
				{
					bool flag2 = (int)itemType == 9 && val6.GetPerkValue(Bow.RangersSwiftness);
					bool flag3 = (int)itemType == 10 && val6.GetPerkValue(Crossbow.LooseAndMove);
					flag = flag2 || flag3;
				}
				if (!flag)
				{
					float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
					num += 4f * MathF.Sqrt(realWeaponLength) * item.Weight;
				}
			}
		}
		if ((int)offhandWieldedItemIndex != -1)
		{
			val8 = equipment[offhandWieldedItemIndex];
			ItemObject item2 = ((MissionWeapon)(ref val8)).Item;
			WeaponComponentData primaryWeapon = item2.PrimaryWeapon;
			if (primaryWeapon != null && Extensions.HasAnyFlag<WeaponFlags>(primaryWeapon.WeaponFlags, (WeaponFlags)268435456) && (val6 == null || !val6.GetPerkValue(OneHanded.ShieldBearer)))
			{
				num += 1.5f * item2.Weight;
			}
		}
		agentDrivenProperties.WeaponsEncumbrance = num;
		agentDrivenProperties.ArmorEncumbrance = effectiveArmorEncumbrance;
		EquipmentIndex primaryWieldedItemIndex2 = agent.GetPrimaryWieldedItemIndex();
		object obj2;
		if ((int)primaryWieldedItemIndex2 == -1)
		{
			obj2 = null;
		}
		else
		{
			val8 = equipment[primaryWieldedItemIndex2];
			obj2 = ((MissionWeapon)(ref val8)).CurrentUsageItem;
		}
		WeaponComponentData val9 = (WeaponComponentData)obj2;
		if (val9 != null && val9.IsRangedWeapon && !val6.IsNavalSoldier())
		{
			PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.RollingThunder, val6, true, ref val2, false);
			float num2 = 1f + ((ExplainedNumber)(ref val2)).ResultNumber;
			agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= num2;
			agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= num2;
			agentDrivenProperties.AiShooterErrorWoRangeUpdate = ((ExplainedNumber)(ref val3)).ResultNumber;
			agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians *= 1f + ((ExplainedNumber)(ref val)).ResultNumber;
		}
		ExplainedNumber val10 = default(ExplainedNumber);
		((ExplainedNumber)(ref val10))._002Ector(0.7f, false, (TextObject)null);
		ExplainedNumber val11 = default(ExplainedNumber);
		((ExplainedNumber)(ref val11))._002Ector(0.7f, false, (TextObject)null);
		SkillHelper.AddSkillBonusForSkillLevel(DefaultSkillEffects.AthleticsSpeedFactor, ref val10, effectiveSkill2);
		SkillHelper.AddSkillBonusForSkillLevel(DefaultSkillEffects.AthleticsSpeedFactor, ref val11, 300);
		agentDrivenProperties.MaxSpeedMultiplier *= 1f - ((ExplainedNumber)(ref val4)).ResultNumber;
		if (!val6.IsNavalSoldier())
		{
			agentDrivenProperties.DamageMultiplierBonus = 0f - ((ExplainedNumber)(ref val5)).ResultNumber;
		}
		else
		{
			agentDrivenProperties.DamageMultiplierBonus = 0f;
		}
		if (val6 != null)
		{
			SetNavalPerksAndEffectsOnAgent(agent, val6, agentDrivenProperties, val9);
		}
	}

	public override int GetEffectiveSkill(Agent agent, SkillObject skill)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetEffectiveSkill(agent, skill);
	}

	public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetWeaponDamageMultiplier(agent, weapon);
	}

	public override float GetEquipmentStealthBonus(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetEquipmentStealthBonus(agent);
	}

	public override float GetSneakAttackMultiplier(Agent agent, WeaponComponentData weapon)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetSneakAttackMultiplier(agent, weapon);
	}

	public override float GetKnockBackResistance(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetKnockBackResistance(agent);
	}

	public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = (StrikeType)(-1))
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetKnockDownResistance(agent, strikeType);
	}

	public override float GetDismountResistance(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetDismountResistance(agent);
	}

	public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetWeaponInaccuracy(agent, weapon, weaponSkill);
	}

	public override float GetInteractionDistance(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetInteractionDistance(agent);
	}

	public override float GetMaxCameraZoom(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetMaxCameraZoom(agent);
	}

	public override string GetMissionDebugInfoForAgent(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetMissionDebugInfoForAgent(agent);
	}

	public override float GetEffectiveMaxHealth(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetEffectiveMaxHealth(agent);
	}

	public override float GetEnvironmentSpeedFactor(Agent agent)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetEnvironmentSpeedFactor(agent);
	}

	public override float GetBreatheHoldMaxDuration(Agent agent, float baseBreatheHoldMaxDuration)
	{
		if (agent.IsHuman)
		{
			Formation formation = agent.Formation;
			object obj;
			if (formation == null)
			{
				obj = null;
			}
			else
			{
				Agent captain = formation.Captain;
				obj = ((captain != null) ? captain.Character : null);
			}
			CharacterObject val = (CharacterObject)((obj is CharacterObject) ? obj : null);
			BasicCharacterObject character = agent.Character;
			BasicCharacterObject obj2 = ((character is CharacterObject) ? character : null);
			float num = ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetBreatheHoldMaxDuration(agent, baseBreatheHoldMaxDuration);
			if ((object)obj2 == val)
			{
				val = null;
			}
			if (!((CharacterObject)(object)obj2).IsNavalSoldier())
			{
				num -= 10f;
			}
			if (agent.GetBaseArmorEffectivenessForBodyPart((BoneBodyPartType)2) > 10f)
			{
				num -= 10f;
			}
			ExplainedNumber val2 = default(ExplainedNumber);
			((ExplainedNumber)(ref val2))._002Ector(num, false, (TextObject)null);
			if (Mission.Current.IsNavalBattle && val != null)
			{
				PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.OldSaltsTouch, val, ref val2);
			}
			return ((ExplainedNumber)(ref val2)).ResultNumber;
		}
		return 1E+09f;
	}

	private void SetNavalPerksAndEffectsOnAgent(Agent agent, CharacterObject agentCharacter, AgentDrivenProperties agentDrivenProperties, WeaponComponentData equippedWeaponComponent)
	{
		Formation formation = agent.Formation;
		object obj;
		if (formation == null)
		{
			obj = null;
		}
		else
		{
			Agent captain = formation.Captain;
			obj = ((captain != null) ? captain.Character : null);
		}
		CharacterObject val = (CharacterObject)((obj is CharacterObject) ? obj : null);
		Formation formation2 = agent.Formation;
		if (((formation2 != null) ? formation2.Captain : null) == agent)
		{
			val = null;
		}
		bool flag = equippedWeaponComponent != null && equippedWeaponComponent.IsMeleeWeapon;
		if (equippedWeaponComponent != null && flag)
		{
			ExplainedNumber val2 = default(ExplainedNumber);
			((ExplainedNumber)(ref val2))._002Ector(agentDrivenProperties.HandlingMultiplier, false, (TextObject)null);
			PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.PiratesProwess, agentCharacter, true, ref val2, false);
			agentDrivenProperties.HandlingMultiplier = ((ExplainedNumber)(ref val2)).ResultNumber;
		}
		float num = 0f;
		float num2 = 0f;
		bool flag2 = false;
		if (val != null)
		{
			if (agentCharacter.Tier <= 3 && val.GetPerkValue(NavalPerks.Boatswain.SpecialArrows))
			{
				num += NavalPerks.Boatswain.SpecialArrows.PrimaryBonus;
				flag2 = true;
			}
			if (flag2)
			{
				float num3 = 1f + num2;
				agentDrivenProperties.ArmorHead = MathF.Max(0f, (agentDrivenProperties.ArmorHead + num) * num3);
				agentDrivenProperties.ArmorTorso = MathF.Max(0f, (agentDrivenProperties.ArmorTorso + num) * num3);
				agentDrivenProperties.ArmorArms = MathF.Max(0f, (agentDrivenProperties.ArmorArms + num) * num3);
				agentDrivenProperties.ArmorLegs = MathF.Max(0f, (agentDrivenProperties.ArmorLegs + num) * num3);
			}
		}
	}
}

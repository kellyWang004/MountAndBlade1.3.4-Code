using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ComponentInterfaces;

public class NavalCustomBattleAgentStatCalculateModel : AgentStatCalculateModel
{
	public override float GetDifficultyModifier()
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetDifficultyModifier();
	}

	public override bool CanAgentRideMount(Agent agent, Agent targetMount)
	{
		return ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.CanAgentRideMount(agent, targetMount);
	}

	public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
	{
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
		NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		if (missionBehavior == null)
		{
			return;
		}
		foreach (MissionShip item in (List<MissionShip>)(object)missionBehavior.AllShips)
		{
			if (item.GetIsAgentOnShip(agent))
			{
				agentDrivenProperties.MeleeWeaponDamageMultiplierBonus += item.ShipOrigin.CrewMeleeDamageFactor;
				break;
			}
		}
	}

	public override void InitializeMissionEquipment(Agent agent)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Invalid comparison between Unknown and I4
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.InitializeMissionEquipment(agent);
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
			for (EquipmentIndex val = (EquipmentIndex)0; (int)val < 4; val = (EquipmentIndex)(val + 1))
			{
				MissionWeapon val2 = agent.Equipment[val];
				if (((MissionWeapon)(ref val2)).IsEmpty)
				{
					continue;
				}
				val2 = agent.Equipment[val];
				WeaponComponentData weaponComponentDataForUsage = ((MissionWeapon)(ref val2)).GetWeaponComponentDataForUsage(0);
				if (weaponComponentDataForUsage.IsShield)
				{
					if (flag)
					{
						MissionEquipment equipment = agent.Equipment;
						EquipmentIndex val3 = val;
						val2 = agent.Equipment[val];
						equipment.SetHitPointsOfSlot(val3, (short)((float)((MissionWeapon)(ref val2)).ModifiedMaxHitPoints * (1f + item.ShipOrigin.CrewShieldHitPointsFactor)), true);
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
							MissionEquipment equipment2 = agent.Equipment;
							EquipmentIndex val4 = val;
							val2 = agent.Equipment[val];
							equipment2.SetAmountOfSlot(val4, (short)(((MissionWeapon)(ref val2)).ModifiedMaxAmount * (1 + item.ShipOrigin.AdditionalThrowingWeaponStack)), false);
							flag3 = false;
						}
					}
					else if (weaponComponentDataForUsage.IsAmmo && flag2)
					{
						MissionEquipment equipment3 = agent.Equipment;
						EquipmentIndex val5 = val;
						val2 = agent.Equipment[val];
						equipment3.SetAmountOfSlot(val5, (short)(((MissionWeapon)(ref val2)).ModifiedMaxAmount * (1 + item.ShipOrigin.AdditionalArcherQuivers)), false);
						flag2 = false;
					}
				}
			}
		}
	}

	public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
	{
		((MBGameModel<AgentStatCalculateModel>)this).BaseModel.UpdateAgentStats(agent, agentDrivenProperties);
		if (Mission.Current.IsNavalBattle && agent.IsHuman)
		{
			UpdateNavalHumanStats(agent, agentDrivenProperties);
		}
	}

	private void UpdateNavalHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Invalid comparison between Unknown and I4
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Invalid comparison between Unknown and I4
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Invalid comparison between Unknown and I4
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val2 = default(ExplainedNumber);
		((ExplainedNumber)(ref val2))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val3 = default(ExplainedNumber);
		((ExplainedNumber)(ref val3))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val4 = default(ExplainedNumber);
		((ExplainedNumber)(ref val4))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val5 = default(ExplainedNumber);
		((ExplainedNumber)(ref val5))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val6 = default(ExplainedNumber);
		((ExplainedNumber)(ref val6))._002Ector(0.3f, false, (TextObject)null);
		ExplainedNumber val7 = default(ExplainedNumber);
		((ExplainedNumber)(ref val7))._002Ector(0.3f, false, (TextObject)null);
		((ExplainedNumber)(ref val)).LimitMin(0f);
		((ExplainedNumber)(ref val2)).LimitMin(0f);
		((ExplainedNumber)(ref val3)).LimitMin(0f);
		((ExplainedNumber)(ref val4)).LimitMin(0f);
		((ExplainedNumber)(ref val5)).LimitMin(0f);
		((ExplainedNumber)(ref val6)).LimitMin(0f);
		((ExplainedNumber)(ref val7)).LimitMin(0f);
		Equipment spawnEquipment = agent.SpawnEquipment;
		agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum();
		agentDrivenProperties.ArmorTorso = spawnEquipment.GetHumanBodyArmorSum();
		agentDrivenProperties.ArmorLegs = spawnEquipment.GetLegArmorSum();
		agentDrivenProperties.ArmorArms = spawnEquipment.GetArmArmorSum();
		agentDrivenProperties.OffhandWeaponDefendSpeedMultiplier = 1f;
		BasicCharacterObject character = agent.Character;
		MissionEquipment equipment = agent.Equipment;
		float num = equipment.GetTotalWeightOfWeapons();
		float effectiveArmorEncumbrance = ((AgentStatCalculateModel)this).GetEffectiveArmorEncumbrance(agent, spawnEquipment);
		int weight = agent.Monster.Weight;
		EquipmentIndex primaryWieldedItemIndex = agent.GetPrimaryWieldedItemIndex();
		EquipmentIndex offhandWieldedItemIndex = agent.GetOffhandWieldedItemIndex();
		((AgentStatCalculateModel)this).GetEffectiveSkill(agent, DefaultSkills.Athletics);
		Formation formation = agent.Formation;
		if (formation != null)
		{
			Agent captain = formation.Captain;
			if (captain != null)
			{
				_ = captain.Character;
			}
		}
		Formation formation2 = agent.Formation;
		if (formation2 != null)
		{
			_ = formation2.Captain;
		}
		else
			_ = null;
		MissionWeapon val8;
		if ((int)primaryWieldedItemIndex != -1)
		{
			val8 = equipment[primaryWieldedItemIndex];
			ItemObject item = ((MissionWeapon)(ref val8)).Item;
			WeaponComponent weaponComponent = item.WeaponComponent;
			if (weaponComponent != null)
			{
				weaponComponent.GetItemType();
				if (0 == 0)
				{
					float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
					num += 4f * MathF.Sqrt(realWeaponLength) * item.Weight;
				}
			}
		}
		if ((int)offhandWieldedItemIndex != -1)
		{
			val8 = equipment[offhandWieldedItemIndex];
			WeaponComponentData primaryWeapon = ((MissionWeapon)(ref val8)).Item.PrimaryWeapon;
			if (primaryWeapon != null)
			{
				Extensions.HasAnyFlag<WeaponFlags>(primaryWeapon.WeaponFlags, (WeaponFlags)268435456);
			}
		}
		agentDrivenProperties.WeaponsEncumbrance = num;
		agentDrivenProperties.ArmorEncumbrance = effectiveArmorEncumbrance;
		float num2 = effectiveArmorEncumbrance + num;
		EquipmentIndex primaryWieldedItemIndex2 = agent.GetPrimaryWieldedItemIndex();
		object obj;
		if ((int)primaryWieldedItemIndex2 == -1)
		{
			obj = null;
		}
		else
		{
			val8 = equipment[primaryWieldedItemIndex2];
			obj = ((MissionWeapon)(ref val8)).CurrentUsageItem;
		}
		WeaponComponentData val9 = (WeaponComponentData)obj;
		if (val9 != null)
		{
			_ = val9.IsRangedWeapon;
		}
		ExplainedNumber val10 = default(ExplainedNumber);
		((ExplainedNumber)(ref val10))._002Ector(0.7f, false, (TextObject)null);
		ExplainedNumber val11 = default(ExplainedNumber);
		((ExplainedNumber)(ref val11))._002Ector(0.7f, false, (TextObject)null);
		ExplainedNumber val12 = default(ExplainedNumber);
		((ExplainedNumber)(ref val12))._002Ector(0.2f, false, (TextObject)null);
		((ExplainedNumber)(ref val12)).LimitMin(0f);
		float num3 = ((ExplainedNumber)(ref val12)).ResultNumber * num2 / (float)weight;
		float num4 = MBMath.ClampFloat(((ExplainedNumber)(ref val10)).ResultNumber - num3, 0.1f, ((ExplainedNumber)(ref val11)).ResultNumber);
		agentDrivenProperties.MaxSpeedMultiplier = ((AgentStatCalculateModel)this).GetEnvironmentSpeedFactor(agent) * num4;
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
		float breatheHoldMaxDuration = ((MBGameModel<AgentStatCalculateModel>)this).BaseModel.GetBreatheHoldMaxDuration(agent, baseBreatheHoldMaxDuration);
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector(breatheHoldMaxDuration, false, (TextObject)null);
		return ((ExplainedNumber)(ref val)).ResultNumber;
	}
}

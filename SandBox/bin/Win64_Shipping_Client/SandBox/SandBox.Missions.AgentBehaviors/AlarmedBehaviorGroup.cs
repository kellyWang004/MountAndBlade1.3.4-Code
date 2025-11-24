using System;
using System.Collections.Generic;
using SandBox.Missions.MissionLogics;
using SandBox.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.AgentBehaviors;

public class AlarmedBehaviorGroup : AgentBehaviorGroup
{
	public const float SafetyDistance = 15f;

	public const float SafetyDistanceSquared = 225f;

	private readonly MissionFightHandler _missionFightHandler;

	public bool DisableCalmDown;

	private readonly BasicMissionTimer _alarmedTimer;

	private readonly BasicMissionTimer _checkCalmDownTimer;

	public bool DoNotCheckForAlarmFactorIncrease;

	public bool DoNotIncreaseAlarmFactorDueToSeeingOrHearingTheEnemy;

	private bool _canMoveWhenCautious = true;

	private readonly MissionTimer _lastSuspiciousPositionTimer;

	private readonly MissionTimer _alarmYellTimer;

	private readonly List<Agent> _ignoredAgentsForAlarm;

	private MissionTime _lastAlarmTriggerTime;

	public float AlarmFactor { get; private set; }

	public AlarmedBehaviorGroup(AgentNavigator navigator, Mission mission)
		: base(navigator, mission)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		_alarmedTimer = new BasicMissionTimer();
		_checkCalmDownTimer = new BasicMissionTimer();
		_missionFightHandler = base.Mission.GetMissionBehavior<MissionFightHandler>();
		_lastSuspiciousPositionTimer = new MissionTimer(10f);
		_alarmYellTimer = new MissionTimer(10f);
		_ignoredAgentsForAlarm = new List<Agent>(0);
		_lastAlarmTriggerTime = MissionTime.Zero;
		base.Mission.OnAddSoundAlarmFactorToAgents += new OnAddSoundAlarmFactorToAgentsDelegate(OnAddSoundAlarmFactor);
	}

	public void SetCanMoveWhenCautious(bool value)
	{
		_canMoveWhenCautious = value;
	}

	private void UpdateAgentAlarmState(float dt)
	{
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Invalid comparison between Unknown and I4
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Invalid comparison between Unknown and I4
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		Vec3 val3;
		if (!base.OwnerAgent.IsAlarmed())
		{
			bool flag = base.OwnerAgent.IsAIAtMoveDestination();
			if ((!base.OwnerAgent.IsCautious() || flag) && ((MissionTime)(ref _lastAlarmTriggerTime)).ElapsedSeconds > 2f)
			{
				float alarmFactor = AlarmFactor;
				AlarmFactor = Math.Max(0f, AlarmFactor - (base.OwnerAgent.IsPatrollingCautious() ? 0.1f : (_canMoveWhenCautious ? 0.15f : 0.25f)) * dt);
				if (alarmFactor >= 1f && AlarmFactor < 1f)
				{
					AlarmFactor = 0.3f;
				}
			}
			bool hasVisualOnEnemy = false;
			bool hasVisualOnCorpse = false;
			if (!DoNotCheckForAlarmFactorIncrease)
			{
				Vec3 val;
				if (!base.OwnerAgent.IsHuman || !base.OwnerAgent.AgentVisuals.IsValid())
				{
					val = base.OwnerAgent.LookDirection;
				}
				else
				{
					MatrixFrame frame = base.OwnerAgent.Frame;
					ref Mat3 rotation = ref frame.rotation;
					MatrixFrame boneEntitialFrame = base.OwnerAgent.AgentVisuals.GetBoneEntitialFrame(base.OwnerAgent.Monster.HeadLookDirectionBoneIndex, true);
					val = ((Mat3)(ref rotation)).TransformToParent(ref boneEntitialFrame.rotation.f);
				}
				Vec3 val2 = val;
				val3 = Vec3.CrossProduct(Vec3.Up, val2);
				val2 = ((Vec3)(ref val2)).RotateAboutAnArbitraryVector(((Vec3)(ref val3)).NormalizedCopy(), 0.2f);
				foreach (Agent item in (List<Agent>)(object)base.OwnerAgent.Mission.AllAgents)
				{
					float num = 0f;
					float num2 = 0f;
					AgentState state = item.State;
					bool flag2 = item.AgentVisuals.IsValid();
					if ((int)state == 5 || (int)state == 2 || (int)state == 0 || !flag2)
					{
						continue;
					}
					AgentFlag agentFlags = item.GetAgentFlags();
					bool flag3 = _ignoredAgentsForAlarm.IndexOf(item) >= 0;
					if (item != base.OwnerAgent && Extensions.HasAllFlags<AgentFlag>(agentFlags, (AgentFlag)2056) && ((!item.IsActive() && !flag3) || (item.IsActive() && (item.IsAlarmed() || (item.IsPatrollingCautious() && !flag3 && item.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().AlarmFactor > AlarmFactor + 0.1f) || base.OwnerAgent.IsEnemyOf(item)))))
					{
						if (!DoNotIncreaseAlarmFactorDueToSeeingOrHearingTheEnemy)
						{
							int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(item, DefaultSkills.Roguery);
							float equipmentStealthBonus = MissionGameModels.Current.AgentStatCalculateModel.GetEquipmentStealthBonus(item);
							float sneakingNoiseMultiplier = Math.Max(0f, 1f - ((float)effectiveSkill * 0.0001f + equipmentStealthBonus * 0.002f));
							num += GetSoundFactor(item, sneakingNoiseMultiplier);
						}
						num2 += GetVisualFactor(val2, item, ref hasVisualOnCorpse, ref hasVisualOnEnemy);
						float num3 = num + num2;
						if (num3 > 0f && (!hasVisualOnEnemy || !DoNotIncreaseAlarmFactorDueToSeeingOrHearingTheEnemy))
						{
							AlarmFactor += num3 * dt * Campaign.Current.Models.DifficultyModel.GetStealthDifficultyMultiplier();
							_lastAlarmTriggerTime = MissionTime.Now;
						}
						Vec2 val4;
						if (AlarmFactor >= 1f && base.OwnerAgent.IsAlarmStateNormal())
						{
							base.OwnerAgent.SetAlarmState((AIStateFlag)1);
							WorldPosition lastSuspiciousPosition = item.GetWorldPosition();
							Vec2 asVec = ((WorldPosition)(ref lastSuspiciousPosition)).AsVec2;
							val3 = base.OwnerAgent.Position;
							val4 = ((Vec3)(ref val3)).AsVec2 - ((WorldPosition)(ref lastSuspiciousPosition)).AsVec2;
							((WorldPosition)(ref lastSuspiciousPosition)).SetVec2(asVec + ((Vec2)(ref val4)).Normalized() * 2f);
							SetAILastSuspiciousPositionHelper(in lastSuspiciousPosition, checkNavMeshForCorrection: true);
							_lastSuspiciousPositionTimer.Reset();
						}
						else if (num3 > 0f && (base.OwnerAgent.IsCautious() || base.OwnerAgent.IsPatrollingCautious()) && _lastSuspiciousPositionTimer.Check(true))
						{
							WorldPosition lastSuspiciousPosition2 = item.GetWorldPosition();
							Vec2 asVec2 = ((WorldPosition)(ref lastSuspiciousPosition2)).AsVec2;
							val3 = base.OwnerAgent.Position;
							val4 = ((Vec3)(ref val3)).AsVec2 - ((WorldPosition)(ref lastSuspiciousPosition2)).AsVec2;
							((WorldPosition)(ref lastSuspiciousPosition2)).SetVec2(asVec2 + ((Vec2)(ref val4)).Normalized() * 2f);
							SetAILastSuspiciousPositionHelper(in lastSuspiciousPosition2, checkNavMeshForCorrection: true);
						}
						if (num2 > 0f && base.OwnerAgent.IsPatrollingCautious() && (!item.IsActive() || (!item.IsEnemyOf(base.OwnerAgent) && !item.IsAlarmed())))
						{
							_ignoredAgentsForAlarm.Add(item);
						}
					}
				}
			}
			if (AlarmFactor >= 2f && hasVisualOnEnemy)
			{
				base.OwnerAgent.SetAlarmState((AIStateFlag)3);
				_alarmYellTimer.Set(-3f);
			}
			else if (_canMoveWhenCautious && AlarmFactor >= 2f && base.OwnerAgent.IsCautious() && hasVisualOnCorpse)
			{
				base.OwnerAgent.SetAlarmState((AIStateFlag)2);
			}
			else if (AlarmFactor < 0.0001f)
			{
				base.OwnerAgent.SetAlarmState((AIStateFlag)0);
			}
			for (int num4 = _ignoredAgentsForAlarm.Count - 1; num4 >= 0; num4--)
			{
				Agent val5 = _ignoredAgentsForAlarm[num4];
				if (val5.IsActive() && (val5.IsAlarmStateNormal() || val5.IsAlarmed()))
				{
					_ignoredAgentsForAlarm.RemoveAt(num4);
				}
			}
			AlarmFactor = Math.Min(AlarmFactor, 2f);
		}
		else if (_alarmYellTimer.Check(true))
		{
			base.OwnerAgent.MakeVoice(VoiceType.Yell, (CombatVoiceNetworkPredictionType)2);
			Mission mission = base.OwnerAgent.Mission;
			Agent ownerAgent = base.OwnerAgent;
			val3 = base.OwnerAgent.Position + new Vec3(0f, 0f, base.OwnerAgent.GetEyeGlobalHeight(), -1f);
			mission.AddSoundAlarmFactorToAgents(ownerAgent, ref val3, 10f);
		}
	}

	private void SetAILastSuspiciousPositionHelper(in WorldPosition lastSuspiciousPosition, bool checkNavMeshForCorrection)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (_canMoveWhenCautious)
		{
			base.OwnerAgent.SetAILastSuspiciousPosition(lastSuspiciousPosition, checkNavMeshForCorrection);
			return;
		}
		WorldPosition worldPosition = base.OwnerAgent.GetWorldPosition();
		Vec2 asVec = ((WorldPosition)(ref worldPosition)).AsVec2;
		WorldPosition val = lastSuspiciousPosition;
		Vec2 asVec2 = ((WorldPosition)(ref val)).AsVec2;
		Vec3 position = base.OwnerAgent.Position;
		Vec2 val2 = asVec2 - ((Vec3)(ref position)).AsVec2;
		((WorldPosition)(ref worldPosition)).SetVec2(asVec + ((Vec2)(ref val2)).Normalized() * 0.1f);
		base.OwnerAgent.SetAILastSuspiciousPosition(worldPosition, false);
	}

	public void AddIgnoredAgentsForAlarm(Agent agent)
	{
		_ignoredAgentsForAlarm.Add(agent);
	}

	private float GetSoundFactor(Agent currentAgent, float sneakingNoiseMultiplier)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Invalid comparison between Unknown and I4
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected I4, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		Vec3 val = currentAgent.Velocity;
		if (((Vec3)(ref val)).LengthSquared > 0.010000001f)
		{
			Vec3 val2 = currentAgent.Position + new Vec3(0f, 0f, currentAgent.GetEyeGlobalHeight(), -1f) - (base.OwnerAgent.Position + new Vec3(0f, 0f, currentAgent.GetEyeGlobalHeight(), -1f));
			float num = ((Vec3)(ref val2)).Normalize();
			val = currentAgent.AverageVelocity;
			float num2 = 200f * Math.Min(1f, ((Vec3)(ref val)).Length / currentAgent.GetMaximumForwardUnlimitedSpeed());
			Scene scene = currentAgent.Mission.Scene;
			val = currentAgent.Position;
			float waterLevelAtPosition = scene.GetWaterLevelAtPosition(((Vec3)(ref val)).AsVec2, !GameNetwork.IsMultiplayer, true);
			if (waterLevelAtPosition > currentAgent.Position.z)
			{
				num2 *= 4f;
			}
			if (currentAgent.HasMount)
			{
				num2 *= 2f;
			}
			else if ((int)currentAgent.State == 1 && currentAgent.AgentVisuals.IsValid())
			{
				HumanWalkingMovementMode movementMode = currentAgent.AgentVisuals.GetMovementMode();
				switch (movementMode - 1)
				{
				case 0:
					num2 *= 0.7f;
					break;
				case 1:
					num2 *= ((waterLevelAtPosition > currentAgent.Position.z) ? 0.6f : 0.1f);
					break;
				case 2:
					num2 *= ((waterLevelAtPosition > currentAgent.Position.z) ? 0.2f : 0f);
					break;
				}
			}
			num2 *= sneakingNoiseMultiplier;
			num2 /= 20f + num * num * 2.5f;
			if (num2 > 0.25f)
			{
				return num2;
			}
		}
		return 0f;
	}

	public float GetVisualFactor(Vec3 usedGlobalLookDirection, Agent currentAgent, ref bool hasVisualOnCorpse, ref bool hasVisualOnEnemy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		Vec3 val = currentAgent.Position + new Vec3(0f, 0f, currentAgent.GetEyeGlobalHeight(), -1f) - (base.OwnerAgent.Position + new Vec3(0f, 0f, currentAgent.GetEyeGlobalHeight(), -1f));
		float num = 0f;
		if (Vec3.DotProduct(val, usedGlobalLookDirection) > 0f)
		{
			float distance = ((Vec3)(ref val)).Normalize();
			Vec3 velocity = currentAgent.Velocity;
			bool currentAgentHasSpeed = ((Vec3)(ref velocity)).LengthSquared > 0.010000001f;
			float equipmentStealthBonus = MissionGameModels.Current.AgentStatCalculateModel.GetEquipmentStealthBonus(currentAgent);
			float visualStrength = GetVisualStrength(val, usedGlobalLookDirection, currentAgent, currentAgentHasSpeed, distance, equipmentStealthBonus);
			if (visualStrength > 0.1f)
			{
				bool isDayTime = base.OwnerAgent.Mission.Scene.IsDayTime;
				Vec3 position = currentAgent.Position;
				List<GameEntity> list = new List<GameEntity>();
				currentAgent.AgentVisuals.GetEntity().Scene.GetAllEntitiesWithScriptComponent<StealthIndoorLightingArea>(ref list);
				float num2 = (isDayTime ? 0.7f : 0.2f);
				float num3 = (isDayTime ? 1f : 0.3f);
				foreach (GameEntity item in list)
				{
					StealthIndoorLightingArea firstScriptOfType = item.GetFirstScriptOfType<StealthIndoorLightingArea>();
					if (((VolumeBox)firstScriptOfType).IsPointIn(position))
					{
						num2 = firstScriptOfType.AmbientLightStrength;
						num3 = firstScriptOfType.SunMoonLightStrength;
						break;
					}
				}
				float visualStrengthOfAgentVisual = base.OwnerAgent.AgentVisuals.GetVisualStrengthOfAgentVisual(currentAgent.AgentVisuals, base.OwnerAgent.Mission, num2, num3, base.OwnerAgent.Index);
				visualStrength *= visualStrengthOfAgentVisual;
				if (visualStrength > 0.35f)
				{
					num += visualStrength;
					if (!currentAgent.IsActive())
					{
						hasVisualOnCorpse = true;
					}
					else if (base.OwnerAgent.IsEnemyOf(currentAgent))
					{
						hasVisualOnEnemy = true;
						if (currentAgent != Agent.Main && Agent.Main != null && currentAgent.IsFriendOf(Agent.Main))
						{
							num *= 0.5f;
						}
					}
				}
			}
		}
		return num;
	}

	private float GetVisualStrength(Vec3 positionDifferenceDirection, Vec3 usedGlobalLookDirection, Agent currentAgent, bool currentAgentHasSpeed, float distance, float equipmentStealthBonus)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Invalid comparison between Unknown and I4
		float num = 1.0995574f;
		float num2 = MathF.PI / 4f;
		Vec3 val = ((Vec3)(ref usedGlobalLookDirection)).CrossProductWithUp();
		val = ((Vec3)(ref val)).NormalizedCopy();
		Mat3 val2 = new Mat3(ref val, ref usedGlobalLookDirection, ref Vec3.Up);
		val2.u = Vec3.CrossProduct(val2.s, val2.f);
		Vec3 val3 = ((Mat3)(ref val2)).TransformToLocal(ref positionDifferenceDirection);
		float num3 = MathF.Atan2(val3.z, val3.x);
		float num4 = MathF.Acos(MBMath.ClampFloat(val3.y, 0f, 1f));
		float num5 = default(float);
		float num6 = default(float);
		MathF.SinCos(num3, ref num5, ref num6);
		float num7 = num * num2 / MathF.Sqrt(num * num * num5 * num5 + num2 * num2 * num6 * num6);
		float num8 = ((num4 >= num7) ? 0f : Math.Min(1f, 0.25f + (num7 - num4) / num7));
		float num9 = 2f;
		num8 *= num8;
		if (!currentAgent.HasMount)
		{
			CapsuleData collisionCapsule = currentAgent.CollisionCapsule;
			if (!(distance <= ((CapsuleData)(ref collisionCapsule)).Radius * 5f))
			{
				if (currentAgent.AgentVisuals.IsValid() && currentAgent.CrouchMode)
				{
					num8 *= 0.45f;
					num9 = 5f;
				}
				goto IL_0127;
			}
		}
		num8 *= 6.5f;
		goto IL_0127;
		IL_0127:
		if (!currentAgentHasSpeed)
		{
			num8 *= 0.85f;
		}
		else if ((int)currentAgent.State != 1)
		{
			num8 *= 0.85f;
		}
		float num10 = Math.Max(0f, 1f - equipmentStealthBonus * 0.0025f);
		num8 *= 750f * num10;
		return num8 / (10f + distance * distance / num9);
	}

	public void ResetAlarmFactor()
	{
		AlarmFactor = 0f;
	}

	private void AddAlarmFactor(float addedAlarmFactor, Agent suspiciousAgent)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		AlarmFactor += addedAlarmFactor;
		_lastAlarmTriggerTime = MissionTime.Now;
		if (AlarmFactor >= 1f && base.OwnerAgent.IsAlarmStateNormal())
		{
			base.OwnerAgent.SetAlarmState((AIStateFlag)1);
			if (suspiciousAgent != null)
			{
				SetAILastSuspiciousPositionHelper(suspiciousAgent.GetWorldPosition(), checkNavMeshForCorrection: true);
			}
			else
			{
				SetAILastSuspiciousPositionHelper(base.OwnerAgent.GetWorldPosition(), checkNavMeshForCorrection: false);
			}
			_lastSuspiciousPositionTimer.Reset();
		}
		else if ((base.OwnerAgent.IsCautious() || base.OwnerAgent.IsPatrollingCautious()) && _lastSuspiciousPositionTimer.Check(true))
		{
			if (suspiciousAgent != null)
			{
				SetAILastSuspiciousPositionHelper(suspiciousAgent.GetWorldPosition(), checkNavMeshForCorrection: true);
			}
			else
			{
				SetAILastSuspiciousPositionHelper(base.OwnerAgent.GetWorldPosition(), checkNavMeshForCorrection: false);
			}
		}
	}

	public void AddAlarmFactor(float addedAlarmFactor, in WorldPosition suspiciousPosition)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		AlarmFactor += addedAlarmFactor;
		_lastAlarmTriggerTime = MissionTime.Now;
		if (AlarmFactor >= 1f && base.OwnerAgent.IsAlarmStateNormal())
		{
			base.OwnerAgent.SetAlarmState((AIStateFlag)1);
			SetAILastSuspiciousPositionHelper(in suspiciousPosition, checkNavMeshForCorrection: true);
			_lastSuspiciousPositionTimer.Reset();
		}
		else if ((base.OwnerAgent.IsCautious() || base.OwnerAgent.IsPatrollingCautious()) && _lastSuspiciousPositionTimer.Check(true))
		{
			SetAILastSuspiciousPositionHelper(in suspiciousPosition, checkNavMeshForCorrection: true);
		}
	}

	public override void Tick(float dt, bool isSimulation)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (base.Mission.AllowAiTicking && base.OwnerAgent.IsAIControlled)
		{
			HandleMissiles(dt);
			if (Extensions.HasAllFlags<AgentFlag>(base.OwnerAgent.GetAgentFlags(), (AgentFlag)81920))
			{
				UpdateAgentAlarmState(dt);
			}
		}
		if (!base.IsActive)
		{
			return;
		}
		if (base.ScriptedBehavior != null)
		{
			if (!base.ScriptedBehavior.IsActive)
			{
				DisableAllBehaviors();
				base.ScriptedBehavior.IsActive = true;
			}
		}
		else
		{
			float num = 0f;
			int num2 = -1;
			for (int i = 0; i < Behaviors.Count; i++)
			{
				float availability = Behaviors[i].GetAvailability(isSimulation);
				if (availability > num)
				{
					num = availability;
					num2 = i;
				}
			}
			if (num > 0f && num2 != -1 && !Behaviors[num2].IsActive)
			{
				DisableAllBehaviors();
				Behaviors[num2].IsActive = true;
			}
		}
		TickActiveBehaviors(dt, isSimulation);
	}

	private void TickActiveBehaviors(float dt, bool isSimulation)
	{
		foreach (AgentBehavior behavior in Behaviors)
		{
			if (behavior.IsActive)
			{
				behavior.Tick(dt, isSimulation);
			}
		}
	}

	public override float GetScore(bool isSimulation)
	{
		if (base.OwnerAgent.IsAlarmed() || base.OwnerAgent.IsPatrollingCautious() || base.OwnerAgent.IsCautious())
		{
			if (!DisableCalmDown && _alarmedTimer.ElapsedTime > 10f && _checkCalmDownTimer.ElapsedTime > 1f)
			{
				_checkCalmDownTimer.Reset();
				if (!IsNearDanger())
				{
					base.OwnerAgent.DisableScriptedMovement();
				}
			}
			return 1f;
		}
		if (IsNearDanger())
		{
			AlarmAgent(base.OwnerAgent);
			return 1f;
		}
		return 0f;
	}

	private bool IsNearDanger()
	{
		float distanceSquared;
		Agent closestAlarmSource = GetClosestAlarmSource(out distanceSquared);
		if (closestAlarmSource != null)
		{
			if (!(distanceSquared < 225f))
			{
				return Navigator.CanSeeAgent(closestAlarmSource);
			}
			return true;
		}
		return false;
	}

	public Agent GetClosestAlarmSource(out float distanceSquared)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		distanceSquared = float.MaxValue;
		if (_missionFightHandler == null || !_missionFightHandler.IsThereActiveFight())
		{
			return null;
		}
		Agent result = null;
		foreach (Agent dangerSource in _missionFightHandler.GetDangerSources(base.OwnerAgent))
		{
			Vec3 position = dangerSource.Position;
			float num = ((Vec3)(ref position)).DistanceSquared(base.OwnerAgent.Position);
			if (num < distanceSquared)
			{
				distanceSquared = num;
				result = dangerSource;
			}
		}
		return result;
	}

	public static void AlarmAgent(Agent agent)
	{
		agent.SetWatchState((WatchState)2);
	}

	protected override void OnActivate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		TextObject val = new TextObject("{=!}{p0} {p1} activate alarmed behavior group.", (Dictionary<string, object>)null);
		val.SetTextVariable("p0", base.OwnerAgent.Name);
		val.SetTextVariable("p1", base.OwnerAgent.Index);
		_alarmedTimer.Reset();
		_checkCalmDownTimer.Reset();
		base.OwnerAgent.DisableScriptedMovement();
		base.OwnerAgent.ClearTargetFrame();
		Navigator.SetItemsVisibility(isVisible: false);
		if (CampaignMission.Current.Location != null)
		{
			LocationCharacter locationCharacter = CampaignMission.Current.Location.GetLocationCharacter(base.OwnerAgent.Origin);
			if (locationCharacter != null && locationCharacter.ActionSetCode != locationCharacter.AlarmedActionSetCode)
			{
				AnimationSystemData val2 = MonsterExtensions.FillAnimationSystemData(locationCharacter.GetAgentBuildData().AgentMonster, MBGlobals.GetActionSet(locationCharacter.AlarmedActionSetCode), ((BasicCharacterObject)locationCharacter.Character).GetStepSize(), false);
				base.OwnerAgent.SetActionSet(ref val2);
			}
		}
		if (Navigator.MemberOfAlley != null || MissionFightHandler.IsAgentAggressive(base.OwnerAgent))
		{
			DisableCalmDown = true;
		}
	}

	private void HandleMissiles(float dt)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		foreach (Missile item in (List<Missile>)(object)base.Mission.MissilesList)
		{
			Vec3 position = ((MBMissile)item).GetPosition();
			Vec3 velocity = ((MBMissile)item).GetVelocity();
			float num = ((Vec3)(ref velocity)).Length / 20f + 0.1f;
			float num2 = 0.1f;
			float num3 = 20f;
			float num4 = MathF.Sqrt(num * num / num2 - num3);
			if (!base.OwnerAgent.IsAlarmed() && base.OwnerAgent.IsActive() && base.OwnerAgent.IsAIControlled && Extensions.HasAnyFlag<AgentFlag>(base.OwnerAgent.GetAgentFlags(), (AgentFlag)65536) && base.OwnerAgent.RiderAgent == null && base.OwnerAgent != item.ShooterAgent)
			{
				Vec3 position2 = base.OwnerAgent.Position;
				position2.z += base.OwnerAgent.GetEyeGlobalHeight();
				Vec3 val = position + velocity;
				Vec3 closestPointOnLineSegmentToPoint = MBMath.GetClosestPointOnLineSegmentToPoint(ref position, ref val, ref position2);
				float num5 = ((Vec3)(ref closestPointOnLineSegmentToPoint)).DistanceSquared(position2);
				if (num5 < num4 * num4)
				{
					AddAlarmFactor(num * num / (num3 + num5) * dt, item.ShooterAgent);
				}
			}
		}
	}

	private void OnAddSoundAlarmFactor(Agent alarmCreatorAgent, in Vec3 soundPosition, float soundLevelSquareRoot)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (GameNetwork.IsClientOrReplay)
		{
			return;
		}
		float num = 0.7f;
		float num2 = 20f;
		float num3 = MathF.Sqrt(soundLevelSquareRoot * soundLevelSquareRoot / num - num2);
		if (base.OwnerAgent.IsActive() && !base.OwnerAgent.IsAlarmed() && base.OwnerAgent.IsAIControlled && Extensions.HasAnyFlag<AgentFlag>(base.OwnerAgent.GetAgentFlags(), (AgentFlag)65536) && base.OwnerAgent.RiderAgent == null && base.OwnerAgent != alarmCreatorAgent)
		{
			Vec3 position = base.OwnerAgent.Position;
			position.z += base.OwnerAgent.GetEyeGlobalHeight();
			Vec3 val = soundPosition;
			float num4 = ((Vec3)(ref val)).DistanceSquared(position);
			if (num4 < num3 * num3)
			{
				this.AddAlarmFactor(soundLevelSquareRoot * soundLevelSquareRoot / (num2 + num4), in ILSpyHelper_AsRefReadOnly(new WorldPosition(base.Mission.Scene, soundPosition)));
			}
		}
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	public override void OnAgentRemoved(Agent agent)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (agent == base.OwnerAgent)
		{
			base.Mission.OnAddSoundAlarmFactorToAgents -= new OnAddSoundAlarmFactorToAgentsDelegate(OnAddSoundAlarmFactor);
		}
	}

	protected override void OnDeactivate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		base.OnDeactivate();
		if (base.OwnerAgent.IsActive())
		{
			EquipmentIndex offhandWieldedItemIndex = base.OwnerAgent.GetOffhandWieldedItemIndex();
			if ((int)offhandWieldedItemIndex != -1 && (int)offhandWieldedItemIndex != 4)
			{
				base.Mission.AddTickAction((MissionTickAction)0, base.OwnerAgent, 1, 0);
			}
			base.Mission.AddTickAction((MissionTickAction)0, base.OwnerAgent, 0, 3);
			base.OwnerAgent.SetWatchState((WatchState)0);
			base.OwnerAgent.ResetLookAgent();
			base.OwnerAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			base.OwnerAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, (AnimFlags)0, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
		}
	}

	public override void ForceThink(float inSeconds)
	{
	}
}

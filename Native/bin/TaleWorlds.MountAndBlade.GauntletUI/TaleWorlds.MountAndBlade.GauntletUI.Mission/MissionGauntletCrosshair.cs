using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission;

[OverrideView(typeof(MissionCrosshair))]
public class MissionGauntletCrosshair : MissionBattleUIBaseView
{
	private GauntletLayer _layer;

	private CrosshairVM _dataSource;

	private GauntletMovieIdentifier _movie;

	private double[] _targetGadgetOpacities = new double[4];

	protected override void OnCreateView()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		CombatLogManager.OnGenerateCombatLog += new OnPrintCombatLogHandler(OnCombatLogGenerated);
		_dataSource = new CrosshairVM();
		_layer = new GauntletLayer("MissionCrosshair", 1, false);
		_movie = _layer.LoadMovie("Crosshair", (ViewModel)(object)_dataSource);
		if ((int)((MissionBehavior)this).Mission.Mode != 1 && (int)((MissionBehavior)this).Mission.Mode != 9)
		{
			((ScreenBase)base.MissionScreen).AddLayer((ScreenLayer)(object)_layer);
		}
	}

	protected override void OnDestroyView()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		CombatLogManager.OnGenerateCombatLog -= new OnPrintCombatLogHandler(OnCombatLogGenerated);
		if ((int)((MissionBehavior)this).Mission.Mode != 1 && (int)((MissionBehavior)this).Mission.Mode != 9)
		{
			((ScreenBase)base.MissionScreen).RemoveLayer((ScreenLayer)(object)_layer);
		}
		_dataSource = null;
		_movie = null;
		_layer = null;
	}

	protected override void OnSuspendView()
	{
		if (_layer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_layer, true);
		}
	}

	protected override void OnResumeView()
	{
		if (_layer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_layer, false);
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Invalid comparison between Unknown and I4
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Invalid comparison between Unknown and I4
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Invalid comparison between Unknown and I4
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Expected I4, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Invalid comparison between Unknown and I4
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Invalid comparison between Unknown and I4
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Expected I4, but got Unknown
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Expected I4, but got Unknown
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Invalid comparison between Unknown and I4
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Invalid comparison between Unknown and I4
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Invalid comparison between Unknown and I4
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Invalid comparison between Unknown and I4
		base.OnMissionScreenTick(dt);
		if (((MissionBehavior)this).DebugInput.IsKeyReleased((InputKey)63) && base.IsViewCreated)
		{
			OnDestroyView();
			OnCreateView();
		}
		if (!base.IsViewCreated)
		{
			return;
		}
		_dataSource.IsVisible = GetShouldCrosshairBeVisible();
		bool flag = true;
		bool isTargetInvalid = false;
		for (int i = 0; i < _targetGadgetOpacities.Length; i++)
		{
			_targetGadgetOpacities[i] = 0.0;
		}
		if (GetShouldArrowsBeVisible())
		{
			_dataSource.CrosshairType = BannerlordConfig.CrosshairType;
			Agent mainAgent = ((MissionBehavior)this).Mission.MainAgent;
			double num = base.MissionScreen.CameraViewAngle * (MathF.PI / 180f);
			double num2 = 2.0 * Math.Tan((double)(mainAgent.CurrentAimingError + mainAgent.CurrentAimingTurbulance) * (0.5 / Math.Tan(num * 0.5)));
			_dataSource.SetProperties(num2, (double)(1f + (base.MissionScreen.CombatCamera.HorizontalFov - MathF.PI / 2f) / (MathF.PI / 2f)));
			WeaponInfo wieldedWeaponInfo = mainAgent.GetWieldedWeaponInfo((HandIndex)0);
			Vec3 lookDirection = mainAgent.LookDirection;
			Vec2 val = ((Vec3)(ref lookDirection)).AsVec2;
			float rotationInRadians = ((Vec2)(ref val)).RotationInRadians;
			val = mainAgent.GetMovementDirection();
			float num3 = MBMath.WrapAngle(rotationInRadians - ((Vec2)(ref val)).RotationInRadians);
			if (((WeaponInfo)(ref wieldedWeaponInfo)).IsValid && ((WeaponInfo)(ref wieldedWeaponInfo)).IsRangedWeapon && BannerlordConfig.DisplayTargetingReticule)
			{
				ActionCodeType currentActionType = mainAgent.GetCurrentActionType(1);
				MissionWeapon wieldedWeapon = mainAgent.WieldedWeapon;
				if (((MissionWeapon)(ref wieldedWeapon)).ReloadPhaseCount > 1 && ((MissionWeapon)(ref wieldedWeapon)).IsReloading && (int)currentActionType == 18)
				{
					StackArray10FloatFloatTuple reloadPhases = default(StackArray10FloatFloatTuple);
					ActionIndexCache itemUsageReloadActionCode = MBItem.GetItemUsageReloadActionCode(((MissionWeapon)(ref wieldedWeapon)).CurrentUsageItem.ItemUsage, 9, mainAgent.HasMount, -1, mainAgent.GetIsLeftStance(), mainAgent.IsLookDirectionLow);
					FillReloadDurationsFromActions(ref reloadPhases, ((MissionWeapon)(ref wieldedWeapon)).ReloadPhaseCount, mainAgent, itemUsageReloadActionCode);
					float num4 = mainAgent.GetCurrentActionProgress(1);
					ActionIndexCache currentAction = mainAgent.GetCurrentAction(1);
					if (currentAction != ActionIndexCache.act_none)
					{
						float num5 = 1f - MBActionSet.GetActionBlendOutStartProgress(mainAgent.ActionSet, ref currentAction);
						num4 += num5;
					}
					float animationParameter = MBAnimation.GetAnimationParameter2(mainAgent.AgentVisuals.GetSkeleton().GetAnimationAtChannel(1));
					bool flag2 = num4 > animationParameter;
					float item = (flag2 ? 1f : (num4 / animationParameter));
					short reloadPhase = ((MissionWeapon)(ref wieldedWeapon)).ReloadPhase;
					for (int j = 0; j < reloadPhase; j++)
					{
						((StackArray10FloatFloatTuple)(ref reloadPhases))[j] = (1f, ((StackArray10FloatFloatTuple)(ref reloadPhases))[j].Item2);
					}
					if (!flag2)
					{
						((StackArray10FloatFloatTuple)(ref reloadPhases))[(int)reloadPhase] = (item, ((StackArray10FloatFloatTuple)(ref reloadPhases))[(int)reloadPhase].Item2);
						_dataSource.SetReloadProperties(ref reloadPhases, (int)((MissionWeapon)(ref wieldedWeapon)).ReloadPhaseCount);
					}
					flag = false;
				}
				if ((int)currentActionType == 15)
				{
					Vec2 bodyRotationConstraint = mainAgent.GetBodyRotationConstraint(1);
					isTargetInvalid = ((MissionBehavior)this).Mission.MainAgent.MountAgent != null && !MBMath.IsBetween(num3, bodyRotationConstraint.x, bodyRotationConstraint.y) && (bodyRotationConstraint.x < -0.1f || bodyRotationConstraint.y > 0.1f);
				}
			}
			else if (!((WeaponInfo)(ref wieldedWeaponInfo)).IsValid || ((WeaponInfo)(ref wieldedWeaponInfo)).IsMeleeWeapon)
			{
				ActionCodeType currentActionType2 = mainAgent.GetCurrentActionType(1);
				UsageDirection currentActionDirection = mainAgent.GetCurrentActionDirection(1);
				if (BannerlordConfig.DisplayAttackDirection && ((int)currentActionType2 == 19 || MBMath.IsBetween((int)currentActionType2, 1, 15)))
				{
					if ((int)currentActionType2 == 19)
					{
						UsageDirection attackDirection = mainAgent.AttackDirection;
						switch ((int)attackDirection)
						{
						case 0:
							_targetGadgetOpacities[0] = 0.7;
							break;
						case 3:
							_targetGadgetOpacities[1] = 0.7;
							break;
						case 1:
							_targetGadgetOpacities[2] = 0.7;
							break;
						case 2:
							_targetGadgetOpacities[3] = 0.7;
							break;
						}
					}
					else
					{
						isTargetInvalid = true;
						switch (currentActionDirection - 4)
						{
						case 0:
							_targetGadgetOpacities[0] = 0.7;
							break;
						case 3:
							_targetGadgetOpacities[1] = 0.7;
							break;
						case 1:
							_targetGadgetOpacities[2] = 0.7;
							break;
						case 2:
							_targetGadgetOpacities[3] = 0.7;
							break;
						}
					}
				}
				else if (BannerlordConfig.DisplayAttackDirection)
				{
					UsageDirection val2 = mainAgent.PlayerAttackDirection();
					if ((int)val2 >= 0 && (int)val2 < 4)
					{
						if ((int)val2 == 0)
						{
							_targetGadgetOpacities[0] = 0.7;
						}
						else if ((int)val2 == 3)
						{
							_targetGadgetOpacities[1] = 0.7;
						}
						else if ((int)val2 == 1)
						{
							_targetGadgetOpacities[2] = 0.7;
						}
						else if ((int)val2 == 2)
						{
							_targetGadgetOpacities[3] = 0.7;
						}
					}
				}
			}
		}
		if (flag)
		{
			StackArray10FloatFloatTuple val3 = default(StackArray10FloatFloatTuple);
			_dataSource.SetReloadProperties(ref val3, 0);
		}
		_dataSource.SetArrowProperties(_targetGadgetOpacities[0], _targetGadgetOpacities[1], _targetGadgetOpacities[2], _targetGadgetOpacities[3]);
		_dataSource.IsTargetInvalid = isTargetInvalid;
	}

	protected virtual bool GetShouldArrowsBeVisible()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		if (!base.IsViewSuspended && ((MissionBehavior)this).Mission.MainAgent != null && (int)((MissionBehavior)this).Mission.Mode != 1 && (int)((MissionBehavior)this).Mission.Mode != 9 && (int)((MissionBehavior)this).Mission.Mode != 6 && !base.MissionScreen.IsViewingCharacter() && !IsMissionScreenUsingCustomCamera())
		{
			return !ScreenManager.GetMouseVisibility();
		}
		return false;
	}

	protected virtual bool GetShouldCrosshairBeVisible()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (GetShouldArrowsBeVisible() && BannerlordConfig.DisplayTargetingReticule)
		{
			MissionWeapon wieldedWeapon = ((MissionBehavior)this).Mission.MainAgent.WieldedWeapon;
			if (!((MissionWeapon)(ref wieldedWeapon)).IsEmpty)
			{
				wieldedWeapon = ((MissionBehavior)this).Mission.MainAgent.WieldedWeapon;
				if (((MissionWeapon)(ref wieldedWeapon)).CurrentUsageItem.IsRangedWeapon)
				{
					wieldedWeapon = ((MissionBehavior)this).Mission.MainAgent.WieldedWeapon;
					if ((int)((MissionWeapon)(ref wieldedWeapon)).CurrentUsageItem.WeaponClass == 17)
					{
						wieldedWeapon = ((MissionBehavior)this).Mission.MainAgent.WieldedWeapon;
						return !((MissionWeapon)(ref wieldedWeapon)).IsReloading;
					}
					return true;
				}
			}
		}
		return false;
	}

	private bool IsMissionScreenUsingCustomCamera()
	{
		return (NativeObject)(object)base.MissionScreen.CustomCamera != (NativeObject)null;
	}

	private void OnCombatLogGenerated(CombatLogData logData)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		bool isAttackerAgentMine = logData.IsAttackerAgentMine;
		bool flag = !logData.IsVictimAgentSameAsAttackerAgent && !logData.IsFriendlyFire;
		bool flag2 = logData.IsAttackerAgentHuman && (int)logData.BodyPartHit == 0;
		if (isAttackerAgentMine && flag && ((CombatLogData)(ref logData)).TotalDamage > 0)
		{
			CrosshairVM dataSource = _dataSource;
			if (dataSource != null)
			{
				dataSource.ShowHitMarker(logData.IsFatalDamage, flag2);
			}
		}
	}

	private void FillReloadDurationsFromActions(ref StackArray10FloatFloatTuple reloadPhases, int reloadPhaseCount, Agent mainAgent, ActionIndexCache reloadAction)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < reloadPhaseCount; i++)
		{
			if (reloadAction != ActionIndexCache.act_none)
			{
				float num2 = MBAnimation.GetAnimationParameter2(MBActionSet.GetAnimationIndexOfAction(mainAgent.ActionSet, ref reloadAction)) * MBActionSet.GetActionAnimationDuration(mainAgent.ActionSet, ref reloadAction);
				((StackArray10FloatFloatTuple)(ref reloadPhases))[i] = (((StackArray10FloatFloatTuple)(ref reloadPhases))[i].Item1, num2);
				if (num2 > num)
				{
					num = num2;
				}
				reloadAction = MBActionSet.GetActionAnimationContinueToAction(mainAgent.ActionSet, ref reloadAction);
			}
		}
		if (num > 1E-05f)
		{
			for (int j = 0; j < reloadPhaseCount; j++)
			{
				((StackArray10FloatFloatTuple)(ref reloadPhases))[j] = (((StackArray10FloatFloatTuple)(ref reloadPhases))[j].Item1, ((StackArray10FloatFloatTuple)(ref reloadPhases))[j].Item2 / num);
			}
		}
	}

	public override void OnPhotoModeActivated()
	{
		base.OnPhotoModeActivated();
		if (base.IsViewCreated)
		{
			_layer.UIContext.ContextAlpha = 0f;
		}
	}

	public override void OnPhotoModeDeactivated()
	{
		base.OnPhotoModeDeactivated();
		if (base.IsViewCreated)
		{
			_layer.UIContext.ContextAlpha = 1f;
		}
	}
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;

[OverrideView(typeof(MissionFormationMarkerUIHandler))]
public class MissionGauntletFormationMarker : MissionBattleUIBaseView
{
	private MissionFormationMarkerVM _dataSource;

	private GauntletLayer _gauntletLayer;

	private MissionFormationTargetSelectionHandler _formationTargetHandler;

	private MBReadOnlyList<Formation> _focusedFormationsCache;

	private readonly Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

	private float _fadeOutTimer;

	protected override void OnCreateView()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		_dataSource = new MissionFormationMarkerVM(((MissionBehavior)this).Mission);
		_gauntletLayer = new GauntletLayer("MissionFormationMarker", ViewOrderPriority, false);
		_gauntletLayer.LoadMovie("FormationMarker", (ViewModel)(object)_dataSource);
		((ScreenBase)base.MissionScreen).AddLayer((ScreenLayer)(object)_gauntletLayer);
		_formationTargetHandler = ((MissionBehavior)this).Mission.GetMissionBehavior<MissionFormationTargetSelectionHandler>();
		if (_formationTargetHandler != null)
		{
			_formationTargetHandler.OnFormationFocused += OnFormationFocusedFromHandler;
		}
	}

	protected override void OnDestroyView()
	{
		if (_formationTargetHandler != null)
		{
			_formationTargetHandler.OnFormationFocused -= OnFormationFocusedFromHandler;
		}
		((ScreenBase)base.MissionScreen).RemoveLayer((ScreenLayer)(object)_gauntletLayer);
		_gauntletLayer = null;
		((ViewModel)_dataSource).OnFinalize();
		_dataSource = null;
	}

	protected override void OnSuspendView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, true);
		}
	}

	protected override void OnResumeView()
	{
		if (_gauntletLayer != null)
		{
			ScreenManager.SetSuspendLayer((ScreenLayer)(object)_gauntletLayer, false);
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		base.OnMissionScreenTick(dt);
		if (base.IsViewCreated)
		{
			if ((int)((MissionBehavior)this).Mission.Mode != 6)
			{
				_dataSource.IsEnabled = base.Input.IsGameKeyDown(5) || ((MissionBehavior)this).Mission.IsOrderMenuOpen;
			}
			_dataSource.IsFormationTargetRelevant = _formationTargetHandler != null && ((MissionBehavior)this).Mission.IsOrderMenuOpen;
			if (_dataSource.IsEnabled)
			{
				_dataSource.RefreshFormationMarkers();
				RefreshTargetProperties();
				UpdateMarkerPositions(_fadeOutTimer < 0f);
				_fadeOutTimer = 2f;
			}
			else if (_fadeOutTimer >= 0f)
			{
				_fadeOutTimer -= dt;
				UpdateMarkerPositions(isFirstFrame: false);
			}
		}
	}

	private void UpdateMarkerPositions(bool isFirstFrame)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets).Count; i++)
		{
			MissionFormationMarkerTargetVM val = ((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets)[i];
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			WorldPosition cachedMedianPosition = val.Formation.CachedMedianPosition;
			if (((WorldPosition)(ref cachedMedianPosition)).IsValid)
			{
				MBWindowManager.WorldToScreen(base.MissionScreen.CombatCamera, ((WorldPosition)(ref cachedMedianPosition)).GetGroundVec3() + _heightOffset, ref num, ref num2, ref num3);
				if (!MathF.IsValidValue(num3) || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2))
				{
					num = -10000f;
					num2 = -10000f;
					num3 = -1f;
				}
				val.WSign = ((!(num3 < 0f)) ? 1 : (-1));
			}
			if (!val.IsTargetingAFormation && (!((WorldPosition)(ref cachedMedianPosition)).IsValid || !MathF.IsValidValue(num3) || num3 < 0f || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2)))
			{
				num = -10000f;
				num2 = -10000f;
				num3 = 0f;
			}
			if (isFirstFrame)
			{
				val.ScreenPosition = new Vec2(num, num2);
			}
			else
			{
				val.ScreenPosition = Vec2.Lerp(val.ScreenPosition, new Vec2(num, num2), 0.9f);
			}
			Vec3 position = base.MissionScreen.CombatCamera.Position;
			val.Distance = ((Vec3)(ref position)).Distance(((WorldPosition)(ref cachedMedianPosition)).GetGroundVec3());
		}
	}

	private unsafe void RefreshTargetProperties()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		if (!_dataSource.IsFormationTargetRelevant)
		{
			for (int i = 0; i < ((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets).Count; i++)
			{
				((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets)[i].SetTargetedState(false, false);
			}
			return;
		}
		List<Formation> list = new List<Formation>();
		Agent main = Agent.Main;
		object obj;
		if (main == null)
		{
			obj = null;
		}
		else
		{
			OrderController playerOrderController = main.Team.PlayerOrderController;
			obj = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null);
		}
		MBReadOnlyList<Formation> val = (MBReadOnlyList<Formation>)obj;
		if (val != null)
		{
			for (int j = 0; j < ((List<Formation>)(object)val).Count; j++)
			{
				if (((List<Formation>)(object)val)[j].TargetFormation != null)
				{
					MovementOrder readonlyMovementOrderReference = Unsafe.Read<MovementOrder>((void*)((List<Formation>)(object)val)[j].GetReadonlyMovementOrderReference());
					if ((int)((MovementOrder)(ref readonlyMovementOrderReference)).OrderType == 4 || (int)((MovementOrder)(ref readonlyMovementOrderReference)).OrderType == 12)
					{
						list.Add(((List<Formation>)(object)val)[j].TargetFormation);
					}
				}
			}
		}
		for (int k = 0; k < ((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets).Count; k++)
		{
			MissionFormationMarkerTargetVM val2 = ((Collection<MissionFormationMarkerTargetVM>)(object)_dataSource.Targets)[k];
			if (val2.TeamType == 2)
			{
				bool flag = list.Contains(val2.Formation);
				val2.SetTargetedState(((List<Formation>)(object)_focusedFormationsCache)?.Contains(val2.Formation) ?? false, flag);
			}
		}
	}

	private void OnFormationFocusedFromHandler(MBReadOnlyList<Formation> focusedFormations)
	{
		_focusedFormationsCache = focusedFormations;
	}

	public override void OnPhotoModeActivated()
	{
		base.OnPhotoModeActivated();
		if (base.IsViewCreated)
		{
			_gauntletLayer.UIContext.ContextAlpha = 0f;
		}
	}

	public override void OnPhotoModeDeactivated()
	{
		base.OnPhotoModeDeactivated();
		if (base.IsViewCreated)
		{
			_gauntletLayer.UIContext.ContextAlpha = 1f;
		}
	}
}

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.MissionViews;

public class MissionItemContourControllerView : MissionView
{
	private const float SceneItemQueryFreq = 1f;

	private readonly WeakGameEntity[] _tempPickableEntities = (WeakGameEntity[])(object)new WeakGameEntity[128];

	private readonly UIntPtr[] _pickableItemsId = new UIntPtr[128];

	private readonly List<GameEntity> _contourItems = new List<GameEntity>();

	private GameEntity _focusedGameEntity;

	private IFocusable _currentFocusedObject;

	private bool _isContourAppliedToAllItems;

	private bool _isContourAppliedToFocusedItem;

	private readonly uint _nonFocusedDefaultContourColor;

	private readonly uint _nonFocusedAmmoContourColor;

	private readonly uint _nonFocusedThrowableContourColor;

	private readonly uint _nonFocusedBannerContourColor;

	private readonly uint _focusedContourColor;

	private float _lastItemQueryTime;

	private static bool IsAllowedByOption
	{
		get
		{
			if (BannerlordConfig.HideBattleUI)
			{
				return GameNetwork.IsMultiplayer;
			}
			return true;
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		base.OnMissionScreenTick(dt);
		if (!IsAllowedByOption)
		{
			return;
		}
		if (Agent.Main != null && base.MissionScreen.InputManager.IsGameKeyDown(5))
		{
			RemoveContourFromAllItems();
			PopulateContourListWithNearbyItems();
			ApplyContourToAllItems();
			_lastItemQueryTime = ((MissionBehavior)this).Mission.CurrentTime;
		}
		else
		{
			RemoveContourFromAllItems();
			_contourItems.Clear();
		}
		if (_isContourAppliedToAllItems)
		{
			float currentTime = ((MissionBehavior)this).Mission.CurrentTime;
			if (currentTime - _lastItemQueryTime > 1f)
			{
				RemoveContourFromAllItems();
				PopulateContourListWithNearbyItems();
				_lastItemQueryTime = currentTime;
			}
		}
	}

	public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		((MissionBehavior)this).OnFocusGained(agent, focusableObject, isInteractable);
		if (!(IsAllowedByOption && focusableObject != _currentFocusedObject && isInteractable))
		{
			return;
		}
		_currentFocusedObject = focusableObject;
		UsableMissionObject val;
		if ((val = (UsableMissionObject)(object)((focusableObject is UsableMissionObject) ? focusableObject : null)) != null)
		{
			SpawnedItemEntity val2;
			if ((val2 = (SpawnedItemEntity)(object)((val is SpawnedItemEntity) ? val : null)) != null)
			{
				_focusedGameEntity = GameEntity.CreateFromWeakEntity(((ScriptComponentBehavior)val2).GameEntity);
			}
			else if (!string.IsNullOrEmpty(((object)val.ActionMessage).ToString()) && !string.IsNullOrEmpty(((object)val.DescriptionMessage).ToString()))
			{
				_focusedGameEntity = GameEntity.CreateFromWeakEntity(((ScriptComponentBehavior)val).GameEntity);
			}
			else
			{
				UsableMachine usableMachineFromPoint = GetUsableMachineFromPoint(val);
				if (usableMachineFromPoint != null)
				{
					_focusedGameEntity = GameEntity.CreateFromWeakEntity(((ScriptComponentBehavior)usableMachineFromPoint).GameEntity);
				}
			}
		}
		AddContourToFocusedItem();
	}

	public override void OnFocusLost(Agent agent, IFocusable focusableObject)
	{
		((MissionBehavior)this).OnFocusLost(agent, focusableObject);
		if (IsAllowedByOption)
		{
			RemoveContourFromFocusedItem();
			_currentFocusedObject = null;
			_focusedGameEntity = null;
		}
	}

	private void PopulateContourListWithNearbyItems()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		_contourItems.Clear();
		float num = (GameNetwork.IsSessionActive ? 1f : 3f);
		Agent main = Agent.Main;
		float num2 = main.GetMaximumForwardUnlimitedSpeed() * num;
		Vec3 val = main.Position - new Vec3(num2, num2, 1f, -1f);
		Vec3 val2 = main.Position + new Vec3(num2, num2, 2.5f, -1f);
		int num3 = ((MissionBehavior)this).Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref val, ref val2, _tempPickableEntities, _pickableItemsId);
		Vec3 position = base.MissionScreen.CombatCamera.Position;
		Vec3 position2 = main.Position;
		Vec3 val3 = new Vec3(position.x, position.y, 0f, -1f);
		float num4 = ((Vec3)(ref val3)).Distance(new Vec3(position2.x, position2.y, 0f, -1f));
		Vec3 val4 = position * (1f - num4) + (position + base.MissionScreen.CombatCamera.Direction) * num4;
		float num5 = default(float);
		WeakGameEntity val9 = default(WeakGameEntity);
		WeakGameEntity val10 = default(WeakGameEntity);
		for (int i = 0; i < num3; i++)
		{
			WeakGameEntity val5 = _tempPickableEntities[i];
			SpawnedItemEntity firstScriptOfType = ((WeakGameEntity)(ref val5)).GetFirstScriptOfType<SpawnedItemEntity>();
			if (firstScriptOfType == null)
			{
				continue;
			}
			Vec3 val6 = ((WeakGameEntity)(ref val5)).ComputeGlobalPhysicsBoundingBoxCenter();
			val3 = val6 - val4;
			Vec3 val7 = ((Vec3)(ref val3)).NormalizedCopy();
			Vec3 origin = ((WeakGameEntity)(ref val5)).GetBodyWorldTransform().origin;
			val3 = origin - val4;
			Vec3 val8 = ((Vec3)(ref val3)).NormalizedCopy();
			if ((!((MissionBehavior)this).Mission.Scene.RayCastForClosestEntityOrTerrain(val4 + val7 * 0.2f, val6, ref num5, ref val9, 0.2f, (BodyFlags)79617) || !((WeakGameEntity)(ref val9)).IsValid || !(val9 == val5)) && (!((MissionBehavior)this).Mission.Scene.RayCastForClosestEntityOrTerrain(val4 + val8 * 0.2f, origin, ref num5, ref val10, 0.2f, (BodyFlags)79617) || !((WeakGameEntity)(ref val10)).IsValid || !(val10 == val5)))
			{
				continue;
			}
			if (firstScriptOfType.IsBanner())
			{
				if (MissionGameModels.Current.BattleBannerBearersModel.IsInteractableFormationBanner(firstScriptOfType, main))
				{
					_contourItems.Add(GameEntity.CreateFromWeakEntity(val5));
				}
			}
			else
			{
				_contourItems.Add(GameEntity.CreateFromWeakEntity(val5));
			}
		}
		int num6 = ((MissionBehavior)this).Mission.Scene.SelectEntitiesInBoxWithScriptComponent<UsableMachine>(ref val, ref val2, _tempPickableEntities, _pickableItemsId);
		for (int j = 0; j < num6; j++)
		{
			WeakGameEntity val11 = _tempPickableEntities[j];
			UsableMachine firstScriptOfType2 = ((WeakGameEntity)(ref val11)).GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType2 != null && !((MissionObject)firstScriptOfType2).IsDisabled)
			{
				WeakGameEntity validStandingPointForAgentWithoutDistanceCheck = firstScriptOfType2.GetValidStandingPointForAgentWithoutDistanceCheck(main);
				IFocusable val12;
				if (((WeakGameEntity)(ref validStandingPointForAgentWithoutDistanceCheck)).IsValid && !(((WeakGameEntity)(ref validStandingPointForAgentWithoutDistanceCheck)).GetFirstScriptOfType<UsableMissionObject>() is SpawnedItemEntity) && (val12 = (IFocusable)/*isinst with value type is only supported in some contexts*/) != null && val12 is UsableMissionObject)
				{
					_contourItems.Add(GameEntity.CreateFromWeakEntity(val11));
				}
			}
		}
	}

	private void ApplyContourToAllItems()
	{
		if (_isContourAppliedToAllItems)
		{
			return;
		}
		foreach (GameEntity contourItem in _contourItems)
		{
			uint nonFocusedColor = GetNonFocusedColor(contourItem);
			uint value = ((contourItem == _focusedGameEntity) ? _focusedContourColor : nonFocusedColor);
			contourItem.SetContourColor((uint?)value, true);
		}
		_isContourAppliedToAllItems = true;
	}

	private uint GetNonFocusedColor(GameEntity entity)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		SpawnedItemEntity firstScriptOfType = entity.GetFirstScriptOfType<SpawnedItemEntity>();
		object obj;
		if (firstScriptOfType == null)
		{
			obj = null;
		}
		else
		{
			MissionWeapon weaponCopy = firstScriptOfType.WeaponCopy;
			obj = ((MissionWeapon)(ref weaponCopy)).Item;
		}
		object obj2 = obj;
		WeaponComponentData val = ((obj2 != null) ? ((ItemObject)obj2).PrimaryWeapon : null);
		object obj3 = obj;
		ItemTypeEnum? val2 = ((obj3 != null) ? new ItemTypeEnum?(((ItemObject)obj3).ItemType) : ((ItemTypeEnum?)null));
		if (obj != null && ((ItemObject)obj).HasBannerComponent)
		{
			return _nonFocusedBannerContourColor;
		}
		if ((val != null && val.IsAmmo) || val2 == (ItemTypeEnum?)5 || val2 == (ItemTypeEnum?)6 || val2 == (ItemTypeEnum?)7 || val2 == (ItemTypeEnum?)20)
		{
			return _nonFocusedAmmoContourColor;
		}
		if (val2 == (ItemTypeEnum?)12)
		{
			return _nonFocusedThrowableContourColor;
		}
		return _nonFocusedDefaultContourColor;
	}

	private void RemoveContourFromAllItems()
	{
		if (!_isContourAppliedToAllItems)
		{
			return;
		}
		foreach (GameEntity contourItem in _contourItems)
		{
			if (_focusedGameEntity == (GameEntity)null || contourItem != _focusedGameEntity)
			{
				contourItem.SetContourColor((uint?)null, true);
			}
		}
		_isContourAppliedToAllItems = false;
	}

	private void AddContourToFocusedItem()
	{
		if (_focusedGameEntity != (GameEntity)null && !_isContourAppliedToFocusedItem)
		{
			_focusedGameEntity.SetContourColor((uint?)_focusedContourColor, true);
			_isContourAppliedToFocusedItem = true;
		}
	}

	private void RemoveContourFromFocusedItem()
	{
		if (_focusedGameEntity != (GameEntity)null && _isContourAppliedToFocusedItem)
		{
			if (_contourItems.Contains(_focusedGameEntity))
			{
				_focusedGameEntity.SetContourColor((uint?)_nonFocusedDefaultContourColor, true);
			}
			else
			{
				_focusedGameEntity.SetContourColor((uint?)null, true);
			}
			_isContourAppliedToFocusedItem = false;
		}
	}

	private UsableMachine GetUsableMachineFromPoint(UsableMissionObject standingPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity val = ((ScriptComponentBehavior)standingPoint).GameEntity;
		while (((WeakGameEntity)(ref val)).IsValid && !((WeakGameEntity)(ref val)).HasScriptOfType<UsableMachine>())
		{
			val = ((WeakGameEntity)(ref val)).Parent;
		}
		if (((WeakGameEntity)(ref val)).IsValid)
		{
			UsableMachine firstScriptOfType = ((WeakGameEntity)(ref val)).GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType != null)
			{
				return firstScriptOfType;
			}
		}
		return null;
	}

	public MissionItemContourControllerView()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Color val = new Color(0.85f, 0.85f, 0.85f, 1f);
		_nonFocusedDefaultContourColor = ((Color)(ref val)).ToUnsignedInteger();
		val = new Color(0f, 0.73f, 1f, 1f);
		_nonFocusedAmmoContourColor = ((Color)(ref val)).ToUnsignedInteger();
		val = new Color(0.051f, 0.988f, 0.18f, 1f);
		_nonFocusedThrowableContourColor = ((Color)(ref val)).ToUnsignedInteger();
		val = new Color(0.521f, 0.988f, 0.521f, 1f);
		_nonFocusedBannerContourColor = ((Color)(ref val)).ToUnsignedInteger();
		val = new Color(1f, 0.84f, 0.35f, 1f);
		_focusedContourColor = ((Color)(ref val)).ToUnsignedInteger();
		base._002Ector();
	}
}

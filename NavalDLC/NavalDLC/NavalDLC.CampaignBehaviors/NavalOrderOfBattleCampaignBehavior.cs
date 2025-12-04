using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace NavalDLC.CampaignBehaviors;

public class NavalOrderOfBattleCampaignBehavior : CampaignBehaviorBase
{
	public class NavalOrderOfBattleFormationData
	{
		[SaveableField(1)]
		public readonly Hero Captain;

		[SaveableField(2)]
		public readonly Ship Ship;

		[SaveableField(3)]
		public readonly DeploymentFormationClass FormationClass;

		[SaveableField(4)]
		public readonly Dictionary<FormationFilterType, bool> Filters;

		public NavalOrderOfBattleFormationData(Hero captain, Ship ship, DeploymentFormationClass formationClass, Dictionary<FormationFilterType, bool> filters)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			Captain = captain;
			Ship = ship;
			FormationClass = formationClass;
			Filters = new Dictionary<FormationFilterType, bool>();
			foreach (FormationFilterType key in filters.Keys)
			{
				Filters.Add(key, filters[key]);
			}
		}
	}

	private List<NavalOrderOfBattleFormationData> _navalBattleFormationInfos;

	public NavalOrderOfBattleCampaignBehavior()
	{
		_navalBattleFormationInfos = new List<NavalOrderOfBattleFormationData>();
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnShipDestroyedEvent.AddNonSerializedListener((object)this, (Action<PartyBase, Ship, ShipDestroyDetail>)OnShipDestroyed);
		CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener((object)this, (Action<Hero>)OnHeroUnregistered);
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<List<NavalOrderOfBattleFormationData>>("_navalBattleFormationInfos", ref _navalBattleFormationInfos);
	}

	public NavalOrderOfBattleFormationData GetFormationDataAtIndex(int formationIndex)
	{
		if (_navalBattleFormationInfos.Count > formationIndex)
		{
			return _navalBattleFormationInfos[formationIndex];
		}
		return null;
	}

	public void SetFormationInfos(List<NavalOrderOfBattleFormationData> formationInfos)
	{
		_navalBattleFormationInfos = new List<NavalOrderOfBattleFormationData>(formationInfos);
	}

	private void OnShipDestroyed(PartyBase owner, Ship ship, ShipDestroyDetail detail)
	{
		for (int num = _navalBattleFormationInfos.Count - 1; num >= 0; num--)
		{
			if (_navalBattleFormationInfos[num].Ship == ship)
			{
				_navalBattleFormationInfos.RemoveAt(num);
			}
		}
	}

	private void OnHeroUnregistered(Hero hero)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		for (int num = _navalBattleFormationInfos.Count - 1; num >= 0; num--)
		{
			NavalOrderOfBattleFormationData navalOrderOfBattleFormationData = _navalBattleFormationInfos[num];
			if (navalOrderOfBattleFormationData.Captain == hero)
			{
				_navalBattleFormationInfos[num] = new NavalOrderOfBattleFormationData(null, navalOrderOfBattleFormationData.Ship, navalOrderOfBattleFormationData.FormationClass, navalOrderOfBattleFormationData.Filters);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace NavalDLC.ViewModelCollection.Order;

public class NavalMissionOrderVM : MissionOrderVM
{
	private List<ClassConfiguration> _classData;

	public NavalMissionOrderVM(OrderController orderController, bool isDeployment, bool isMultiplayer)
		: base(orderController, isDeployment, isMultiplayer)
	{
		((ViewModel)this).RefreshValues();
	}

	protected override MissionOrderTroopControllerVM CreateTroopController(OrderController orderController)
	{
		return (MissionOrderTroopControllerVM)(object)new NavalMissionOrderTroopControllerVM((MissionOrderVM)(object)this, ((MissionOrderVM)this).IsDeployment, (Action)base.OnTransferFinished);
	}

	public void OnClassesSet(List<ClassConfiguration> classData)
	{
		_classData = classData;
		(((MissionOrderVM)this).TroopController as NavalMissionOrderTroopControllerVM).OnClassesSet(_classData);
	}

	public override void OnOrderLayoutTypeChanged()
	{
		((MissionOrderVM)this).OnOrderLayoutTypeChanged();
		(((MissionOrderVM)this).TroopController as NavalMissionOrderTroopControllerVM).OnClassesSet(_classData);
	}

	protected override void HighlightAllTroops()
	{
		if (((IEnumerable<OrderTroopItemVM>)((MissionOrderVM)this).TroopController.TroopList).Count((OrderTroopItemVM t) => ((OrderSubjectVM)t).IsSelectable) == 1)
		{
			((OrderSubjectVM)((IEnumerable<OrderTroopItemVM>)((MissionOrderVM)this).TroopController.TroopList).FirstOrDefault((Func<OrderTroopItemVM, bool>)((OrderTroopItemVM t) => ((OrderSubjectVM)t).IsSelectable))).IsSelectionHighlightActive = true;
			return;
		}
		foreach (OrderTroopItemVM item in (List<OrderTroopItemVM>)(object)((MissionOrderVM)this).TroopController.TroopList)
		{
			if (((OrderSubjectVM)item).IsSelectable && !NavalDLCHelpers.IsPlayerCaptainOfFormationShip(item.Formation))
			{
				((OrderSubjectVM)item).IsSelectionHighlightActive = true;
			}
		}
	}
}

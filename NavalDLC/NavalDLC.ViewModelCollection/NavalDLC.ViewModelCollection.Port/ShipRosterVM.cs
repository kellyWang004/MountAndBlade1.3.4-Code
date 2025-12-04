using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port;

public class ShipRosterVM : ViewModel
{
	private class PortShipVMComparer : IComparer<ShipItemVM>
	{
		private readonly MBReadOnlyList<Ship> _orderedShipsList;

		public PortShipVMComparer(MBReadOnlyList<Ship> orderedShipsList)
		{
			_orderedShipsList = orderedShipsList;
		}

		public int Compare(ShipItemVM x, ShipItemVM y)
		{
			int num = ((List<Ship>)(object)_orderedShipsList).IndexOf(x.Ship);
			int value = ((List<Ship>)(object)_orderedShipsList).IndexOf(y.Ship);
			return num.CompareTo(value);
		}
	}

	private TextObject _rosterName;

	private readonly Action _onSelected;

	private bool _hasAnyShips;

	private bool _hasMultipleShips;

	private bool _hasOwnerCharacter;

	private bool _isSelected;

	private string _name;

	private string _hasNoShipsText;

	private string _shipCountText;

	private string _weightText;

	private string _troopCountText;

	private bool _isWeightDangerous;

	private bool _isTroopCountDangerous;

	private MBBindingList<ShipItemVM> _ships;

	private CharacterImageIdentifierVM _ownerVisual;

	private HintViewModel _tooltip;

	public PartyBase Owner { get; private set; }

	[DataSourceProperty]
	public bool HasAnyShips
	{
		get
		{
			return _hasAnyShips;
		}
		set
		{
			if (value != _hasAnyShips)
			{
				_hasAnyShips = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "HasAnyShips");
			}
		}
	}

	[DataSourceProperty]
	public bool HasMultipleShips
	{
		get
		{
			return _hasMultipleShips;
		}
		set
		{
			if (value != _hasMultipleShips)
			{
				_hasMultipleShips = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "HasMultipleShips");
			}
		}
	}

	[DataSourceProperty]
	public bool HasOwnerCharacter
	{
		get
		{
			return _hasOwnerCharacter;
		}
		set
		{
			if (value != _hasOwnerCharacter)
			{
				_hasOwnerCharacter = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "HasOwnerCharacter");
			}
		}
	}

	[DataSourceProperty]
	public bool IsSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			if (value != _isSelected)
			{
				_isSelected = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsSelected");
			}
		}
	}

	[DataSourceProperty]
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			if (value != _name)
			{
				_name = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "Name");
			}
		}
	}

	[DataSourceProperty]
	public string HasNoShipsText
	{
		get
		{
			return _hasNoShipsText;
		}
		set
		{
			if (value != _hasNoShipsText)
			{
				_hasNoShipsText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "HasNoShipsText");
			}
		}
	}

	[DataSourceProperty]
	public string ShipCountText
	{
		get
		{
			return _shipCountText;
		}
		set
		{
			if (value != _shipCountText)
			{
				_shipCountText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ShipCountText");
			}
		}
	}

	[DataSourceProperty]
	public string WeightText
	{
		get
		{
			return _weightText;
		}
		set
		{
			if (value != _weightText)
			{
				_weightText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "WeightText");
			}
		}
	}

	[DataSourceProperty]
	public string TroopCountText
	{
		get
		{
			return _troopCountText;
		}
		set
		{
			if (value != _troopCountText)
			{
				_troopCountText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "TroopCountText");
			}
		}
	}

	[DataSourceProperty]
	public bool IsWeightDangerous
	{
		get
		{
			return _isWeightDangerous;
		}
		set
		{
			if (value != _isWeightDangerous)
			{
				_isWeightDangerous = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsWeightDangerous");
			}
		}
	}

	[DataSourceProperty]
	public bool IsTroopCountDangerous
	{
		get
		{
			return _isTroopCountDangerous;
		}
		set
		{
			if (value != _isTroopCountDangerous)
			{
				_isTroopCountDangerous = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsTroopCountDangerous");
			}
		}
	}

	[DataSourceProperty]
	public MBBindingList<ShipItemVM> Ships
	{
		get
		{
			return _ships;
		}
		set
		{
			if (value != _ships)
			{
				_ships = value;
				((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<ShipItemVM>>(value, "Ships");
			}
		}
	}

	[DataSourceProperty]
	public CharacterImageIdentifierVM OwnerCharacterVisual
	{
		get
		{
			return _ownerVisual;
		}
		set
		{
			if (value != _ownerVisual)
			{
				_ownerVisual = value;
				((ViewModel)this).OnPropertyChangedWithValue<CharacterImageIdentifierVM>(value, "OwnerCharacterVisual");
			}
		}
	}

	[DataSourceProperty]
	public HintViewModel Tooltip
	{
		get
		{
			return _tooltip;
		}
		set
		{
			if (value != _tooltip)
			{
				_tooltip = value;
				((ViewModel)this).OnPropertyChangedWithValue<HintViewModel>(value, "Tooltip");
			}
		}
	}

	public ShipRosterVM(Action onSelected)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		_onSelected = onSelected;
		Ships = new MBBindingList<ShipItemVM>();
		Tooltip = new HintViewModel();
		((ViewModel)this).RefreshValues();
	}

	public override void RefreshValues()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		((ViewModel)this).RefreshValues();
		Name = ((object)_rosterName)?.ToString();
		HasNoShipsText = ((object)new TextObject("{=vfXHD89T}No ships available", (Dictionary<string, object>)null)).ToString();
		ShipCountText = ((object)new TextObject("{=nx9Pk1ca}{AMOUNT} {?AMOUNT==1}ship{?}ships{\\?}", (Dictionary<string, object>)null).SetTextVariable("AMOUNT", ((Collection<ShipItemVM>)(object)Ships).Count)).ToString();
		if (HasOwnerCharacter)
		{
			MobileParty mobileParty = Owner.MobileParty;
			float num = ((mobileParty != null) ? mobileParty.TotalWeightCarried : 0f);
			float num2 = ((IEnumerable<ShipItemVM>)_ships).Sum((ShipItemVM x) => x.Ship.InventoryCapacity);
			WeightText = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_no_space", (string)null).SetTextVariable("LEFT", (int)num).SetTextVariable("RIGHT", (int)num2)).ToString();
			IsWeightDangerous = num > num2;
			int numberOfAllMembers = Owner.NumberOfAllMembers;
			int num3 = ((IEnumerable<ShipItemVM>)_ships).Sum((ShipItemVM x) => x.Ship.TotalCrewCapacity);
			TroopCountText = ((object)GameTexts.FindText("str_LEFT_over_RIGHT_no_space", (string)null).SetTextVariable("LEFT", numberOfAllMembers).SetTextVariable("RIGHT", num3)).ToString();
			IsTroopCountDangerous = numberOfAllMembers > num3;
		}
		else
		{
			WeightText = string.Empty;
			TroopCountText = string.Empty;
			IsWeightDangerous = false;
			IsTroopCountDangerous = false;
		}
		if (!HasAnyShips)
		{
			Tooltip.HintText = new TextObject("{=vfXHD89T}No ships available", (Dictionary<string, object>)null);
		}
		else if (IsWeightDangerous || IsTroopCountDangerous)
		{
			Tooltip.HintText = new TextObject("{=qSRbt9qc}Over the carrying limit, sailing speed will be negatively affected!", (Dictionary<string, object>)null);
		}
		else
		{
			Tooltip.HintText = null;
		}
		Ships.ApplyActionOnAllItems((Action<ShipItemVM>)delegate(ShipItemVM s)
		{
			((ViewModel)s).RefreshValues();
		});
	}

	public void SetRosterName(TextObject rosterName)
	{
		_rosterName = rosterName;
		((ViewModel)this).RefreshValues();
	}

	public void SetRosterOwner(PartyBase owner)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		Owner = owner;
		HasOwnerCharacter = Owner != null && Owner.LeaderHero != null;
		CharacterImageIdentifierVM ownerCharacterVisual = OwnerCharacterVisual;
		if (ownerCharacterVisual != null)
		{
			((ViewModel)ownerCharacterVisual).OnFinalize();
		}
		if (HasOwnerCharacter)
		{
			OwnerCharacterVisual = new CharacterImageIdentifierVM(CharacterCode.CreateFrom((BasicCharacterObject)(object)Owner.LeaderHero.CharacterObject));
		}
		else
		{
			OwnerCharacterVisual = null;
		}
		((ViewModel)this).RefreshValues();
	}

	public void RefreshShips(MBReadOnlyList<ShipItemVM> removedShips, MBReadOnlyList<ShipItemVM> addedShips, MBReadOnlyList<Ship> orderedShipsList)
	{
		for (int i = 0; i < ((List<ShipItemVM>)(object)removedShips).Count; i++)
		{
			((Collection<ShipItemVM>)(object)Ships).Remove(((List<ShipItemVM>)(object)removedShips)[i]);
		}
		for (int j = 0; j < ((List<ShipItemVM>)(object)addedShips).Count; j++)
		{
			((Collection<ShipItemVM>)(object)Ships).Add(((List<ShipItemVM>)(object)addedShips)[j]);
		}
		Ships.Sort((IComparer<ShipItemVM>)new PortShipVMComparer(orderedShipsList));
		HasAnyShips = ((Collection<ShipItemVM>)(object)Ships).Count > 0;
		HasMultipleShips = ((Collection<ShipItemVM>)(object)Ships).Count > 1;
		((ViewModel)this).RefreshValues();
	}

	public void ExecuteSelectRoster()
	{
		_onSelected?.Invoke();
	}

	public override void OnFinalize()
	{
		((ViewModel)this).OnFinalize();
		foreach (ShipItemVM item in (Collection<ShipItemVM>)(object)Ships)
		{
			((ViewModel)item).OnFinalize();
		}
		((Collection<ShipItemVM>)(object)Ships).Clear();
		CharacterImageIdentifierVM ownerCharacterVisual = OwnerCharacterVisual;
		if (ownerCharacterVisual != null)
		{
			((ViewModel)ownerCharacterVisual).OnFinalize();
		}
		OwnerCharacterVisual = null;
	}
}

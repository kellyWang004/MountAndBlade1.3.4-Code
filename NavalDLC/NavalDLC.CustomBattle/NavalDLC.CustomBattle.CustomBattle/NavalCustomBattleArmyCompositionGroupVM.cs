using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CustomBattle.CustomBattle;

public class NavalCustomBattleArmyCompositionGroupVM : ViewModel
{
	public int[] CompositionValues;

	private bool _updatingSliders;

	private BasicCultureObject _selectedCulture;

	private int _minimumSafeTroopCount;

	private float _cachedArmySizeRatio;

	private readonly MBReadOnlyList<SkillObject> _allSkills = Game.Current.ObjectManager.GetObjectTypeList<SkillObject>();

	private readonly List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();

	private NavalCustomBattleArmyCompositionItemVM _meleeInfantryComposition;

	private NavalCustomBattleArmyCompositionItemVM _rangedInfantryComposition;

	private int _armySize;

	private int _maxArmySize;

	private int _minArmySize;

	private string _armySizeTitle;

	private string _warningText;

	private bool _isWarned;

	[DataSourceProperty]
	public NavalCustomBattleArmyCompositionItemVM MeleeInfantryComposition
	{
		get
		{
			return _meleeInfantryComposition;
		}
		set
		{
			if (value != _meleeInfantryComposition)
			{
				_meleeInfantryComposition = value;
				((ViewModel)this).OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "MeleeInfantryComposition");
			}
		}
	}

	[DataSourceProperty]
	public NavalCustomBattleArmyCompositionItemVM RangedInfantryComposition
	{
		get
		{
			return _rangedInfantryComposition;
		}
		set
		{
			if (value != _rangedInfantryComposition)
			{
				_rangedInfantryComposition = value;
				((ViewModel)this).OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "RangedInfantryComposition");
			}
		}
	}

	[DataSourceProperty]
	public string ArmySizeTitle
	{
		get
		{
			return _armySizeTitle;
		}
		set
		{
			if (value != _armySizeTitle)
			{
				_armySizeTitle = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ArmySizeTitle");
			}
		}
	}

	[DataSourceProperty]
	public string WarningText
	{
		get
		{
			return _warningText;
		}
		set
		{
			if (value != _warningText)
			{
				_warningText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "WarningText");
			}
		}
	}

	[DataSourceProperty]
	public bool IsWarned
	{
		get
		{
			return _isWarned;
		}
		set
		{
			if (value != _isWarned)
			{
				_isWarned = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsWarned");
			}
		}
	}

	[DataSourceProperty]
	public int ArmySize
	{
		get
		{
			return _armySize;
		}
		set
		{
			value = (int)MathF.Clamp((float)value, (float)MinArmySize, (float)MaxArmySize);
			if (_armySize != value)
			{
				_armySize = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "ArmySize");
				_cachedArmySizeRatio = (float)(value - MinArmySize) / (float)(MaxArmySize - MinArmySize);
				UpdateIsWarned();
			}
		}
	}

	[DataSourceProperty]
	public int MaxArmySize
	{
		get
		{
			return _maxArmySize;
		}
		set
		{
			if (_maxArmySize != value)
			{
				_maxArmySize = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "MaxArmySize");
			}
		}
	}

	[DataSourceProperty]
	public int MinArmySize
	{
		get
		{
			return _minArmySize;
		}
		set
		{
			if (_minArmySize != value)
			{
				_minArmySize = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "MinArmySize");
			}
		}
	}

	public NavalCustomBattleArmyCompositionGroupVM(NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp)
	{
		foreach (BasicCharacterObject item in ((IEnumerable<BasicCharacterObject>)Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>()).Where((BasicCharacterObject c) => c.IsSoldier && !c.IsObsolete))
		{
			_allCharacterObjects.Add(item);
		}
		CompositionValues = new int[2];
		CompositionValues[0] = 50;
		CompositionValues[1] = 50;
		MeleeInfantryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry, _allCharacterObjects, _allSkills, UpdateSliders, troopTypeSelectionPopUp, CompositionValues);
		RangedInfantryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry, _allCharacterObjects, _allSkills, UpdateSliders, troopTypeSelectionPopUp, CompositionValues);
		_cachedArmySizeRatio = 0.75f;
		UpdateTroopCountLimits(1, BannerlordConfig.MaxBattleSize, 1);
		((ViewModel)this).RefreshValues();
	}

	public override void RefreshValues()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		((ViewModel)this).RefreshValues();
		ArmySizeTitle = ((object)new TextObject("{=EQLbYxec}Crew Count", (Dictionary<string, object>)null)).ToString();
		WarningText = ((object)new TextObject("{=nkIeNadI}Ships may be undercrewed!", (Dictionary<string, object>)null)).ToString();
		((ViewModel)MeleeInfantryComposition).RefreshValues();
		((ViewModel)RangedInfantryComposition).RefreshValues();
	}

	public void SetCurrentSelectedCulture(BasicCultureObject selectedCulture)
	{
		if (_selectedCulture != selectedCulture)
		{
			MeleeInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
			RangedInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
			_selectedCulture = selectedCulture;
		}
	}

	private void UpdateSliders(int value, int changedSliderIndex)
	{
		if (!_updatingSliders)
		{
			_updatingSliders = true;
			int[] array = new int[2]
			{
				CompositionValues[0],
				CompositionValues[1]
			};
			array[changedSliderIndex] = value;
			array[(changedSliderIndex + 1) % 2] = 100 - value;
			SetArmyCompositionValue(0, array[0], MeleeInfantryComposition);
			SetArmyCompositionValue(1, array[1], RangedInfantryComposition);
			_updatingSliders = false;
		}
	}

	private void SetArmyCompositionValue(int index, int value, NavalCustomBattleArmyCompositionItemVM composition)
	{
		CompositionValues[index] = value;
		composition.RefreshCompositionValue();
	}

	public void ExecuteRandomize()
	{
		int num = MBRandom.RandomInt(100);
		MeleeInfantryComposition.ExecuteRandomize(num);
		RangedInfantryComposition.ExecuteRandomize(100 - num);
	}

	public void UpdateTroopCountLimits(int minTroopCount, int maxTroopCount, int minimumSafeTroopCount)
	{
		float cachedArmySizeRatio = _cachedArmySizeRatio;
		MinArmySize = MathF.Max(1, minTroopCount);
		MaxArmySize = MathF.Min(BannerlordConfig.MaxBattleSize, maxTroopCount);
		if (MaxArmySize < MinArmySize)
		{
			Debug.FailedAssert("Max army size is less than min army size!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleArmyCompositionGroupVM.cs", "UpdateTroopCountLimits", 110);
			MaxArmySize = MinArmySize;
		}
		_minimumSafeTroopCount = minimumSafeTroopCount;
		ArmySize = MathF.Round(MathF.Lerp((float)MinArmySize, (float)MaxArmySize, cachedArmySizeRatio, 1E-05f));
		_cachedArmySizeRatio = cachedArmySizeRatio;
		UpdateIsWarned();
	}

	private void UpdateIsWarned()
	{
		IsWarned = ArmySize < _minimumSafeTroopCount;
	}
}

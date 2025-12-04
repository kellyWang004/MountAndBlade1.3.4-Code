using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port;

public class ShipStatVM : ViewModel
{
	private readonly TextObject _nameTextObj;

	private bool _isBonusBeneficial;

	private string _statId;

	private string _name;

	private string _valueText;

	private string _bonusValueText;

	private HintViewModel _hint;

	[DataSourceProperty]
	public bool IsBonusBeneficial
	{
		get
		{
			return _isBonusBeneficial;
		}
		set
		{
			if (value != _isBonusBeneficial)
			{
				_isBonusBeneficial = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsBonusBeneficial");
			}
		}
	}

	[DataSourceProperty]
	public string StatId
	{
		get
		{
			return _statId;
		}
		set
		{
			if (value != _statId)
			{
				_statId = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "StatId");
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
	public string ValueText
	{
		get
		{
			return _valueText;
		}
		set
		{
			if (value != _valueText)
			{
				_valueText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ValueText");
			}
		}
	}

	[DataSourceProperty]
	public string BonusValueText
	{
		get
		{
			return _bonusValueText;
		}
		set
		{
			if (value != _bonusValueText)
			{
				_bonusValueText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "BonusValueText");
			}
		}
	}

	[DataSourceProperty]
	public HintViewModel Hint
	{
		get
		{
			return _hint;
		}
		set
		{
			if (value != _hint)
			{
				_hint = value;
				((ViewModel)this).OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
			}
		}
	}

	public ShipStatVM(string statId, TextObject name, string value, string bonusValue = "", bool isBonusBeneficial = true)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		_nameTextObj = name;
		ValueText = value;
		BonusValueText = bonusValue;
		IsBonusBeneficial = isBonusBeneficial;
		StatId = statId;
		Hint = new HintViewModel();
		((ViewModel)this).RefreshValues();
	}

	public override void RefreshValues()
	{
		((ViewModel)this).RefreshValues();
		Name = ((object)_nameTextObj).ToString();
		Hint.HintText = GameTexts.FindText("str_ship_stat_explanation", StatId);
	}
}

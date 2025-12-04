using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.ViewModelCollection.Port;

public class ShipFigureheadVM : ShipUpgradePieceBaseVM
{
	public Ship EquippedShip;

	public readonly Figurehead Figurehead;

	private readonly IViewDataTracker _viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();

	public ShipFigureheadVM(Figurehead figurehead, Action<ShipUpgradePieceBaseVM> onSelected)
		: base(onSelected)
	{
		Figurehead = figurehead;
		base.Price = 0;
		base.UpgradePieceTier = ShipUpgradePieceTier.Diamond;
		base.Identifier = ((MBObjectBase)figurehead).StringId;
		((ViewModel)this).RefreshValues();
	}

	public override void RefreshValues()
	{
		base.RefreshValues();
		base.Name = ((object)((PropertyObject)Figurehead).Name).ToString();
	}

	protected override PropertyBasedTooltipVM GetProperties()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		object[] array = new object[1] { Figurehead };
		PropertyBasedTooltipVM val = new PropertyBasedTooltipVM(typeof(Figurehead), array);
		if (!TextObject.IsNullOrEmpty(((PropertyObject)Figurehead).Description))
		{
			TooltipProperty item = new TooltipProperty(((object)((PropertyObject)Figurehead).Description).ToString(), string.Empty, 0, false, (TooltipPropertyFlags)0);
			((Collection<TooltipProperty>)(object)val.TooltipPropertyList).Insert(0, item);
			item = new TooltipProperty(" ", " ", 0, false, (TooltipPropertyFlags)0);
			((Collection<TooltipProperty>)(object)val.TooltipPropertyList).Insert(1, item);
		}
		if (EquippedShip != null)
		{
			val.AddProperty(" ", " ", 0, (TooltipPropertyFlags)0);
			val.AddProperty(((object)new TextObject("{=bQzObjHj}Attached ship", (Dictionary<string, object>)null)).ToString(), ((object)EquippedShip.Name).ToString(), 0, (TooltipPropertyFlags)0);
		}
		else if (base.IsDisabled)
		{
			val.AddProperty(" ", " ", 0, (TooltipPropertyFlags)0);
			val.AddProperty(((object)new TextObject("{=4RUs8Cfu}Not unlocked", (Dictionary<string, object>)null)).ToString(), string.Empty, 0, (TooltipPropertyFlags)0);
		}
		return val;
	}

	public override void ExecuteInspectBegin()
	{
		base.ExecuteInspectBegin();
		if (base.IsUnexamined)
		{
			_viewDataTracker.OnFigureheadExamined(Figurehead);
		}
	}

	public override void Update()
	{
		base.Update();
		base.IsUnexamined = !base.IsDisabled && _viewDataTracker.UnexaminedFigureheads.Contains(Figurehead);
	}
}

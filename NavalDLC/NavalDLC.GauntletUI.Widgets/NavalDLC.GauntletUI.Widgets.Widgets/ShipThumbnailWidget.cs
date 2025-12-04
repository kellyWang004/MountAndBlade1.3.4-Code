using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Widgets.Widgets;

public class ShipThumbnailWidget : Widget
{
	private readonly BrushWidget _childWidget;

	private bool _shouldUpdateSprite;

	private Vec2 _previousSize;

	private string _prefabId;

	private Brush _spriteBrush;

	private Brush _styleBrush;

	[Editor(false)]
	public string PrefabId
	{
		get
		{
			return _prefabId;
		}
		set
		{
			if (_prefabId != value)
			{
				_prefabId = value;
				((PropertyOwnerObject)this).OnPropertyChanged<string>(value, "PrefabId");
				_shouldUpdateSprite = true;
			}
		}
	}

	[Editor(false)]
	public Brush SpriteBrush
	{
		get
		{
			return _spriteBrush;
		}
		set
		{
			if (_spriteBrush != value)
			{
				_spriteBrush = value;
				((PropertyOwnerObject)this).OnPropertyChanged<Brush>(value, "SpriteBrush");
				_shouldUpdateSprite = true;
			}
		}
	}

	[Editor(false)]
	public Brush StyleBrush
	{
		get
		{
			return _styleBrush;
		}
		set
		{
			if (_styleBrush != value)
			{
				_styleBrush = value;
				((PropertyOwnerObject)this).OnPropertyChanged<Brush>(value, "StyleBrush");
				_childWidget.Brush = value;
				_shouldUpdateSprite = true;
			}
		}
	}

	public ShipThumbnailWidget(UIContext context)
		: base(context)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		((Widget)this).ClipContents = true;
		((Widget)this).DoNotPassEventsToChildren = true;
		((Widget)this).UpdateChildrenStates = true;
		_childWidget = new BrushWidget(context)
		{
			IsVisible = false,
			WidthSizePolicy = (SizePolicy)0,
			HeightSizePolicy = (SizePolicy)0,
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		((Widget)this).AddChild((Widget)(object)_childWidget);
	}

	protected override void OnLateUpdate(float dt)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		((Widget)this).OnLateUpdate(dt);
		if (Vec2.op_Implicit(((Widget)this).Size) != _previousSize)
		{
			_previousSize = Vec2.op_Implicit(((Widget)this).Size);
			_shouldUpdateSprite = true;
		}
		if (_shouldUpdateSprite && (((Widget)this).Size.X != 0f || (int)((Widget)this).WidthSizePolicy == 2) && (((Widget)this).Size.Y != 0f || (int)((Widget)this).HeightSizePolicy == 2))
		{
			_shouldUpdateSprite = false;
			UpdateSprite();
		}
	}

	private void UpdateSprite()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Invalid comparison between Unknown and I4
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Invalid comparison between Unknown and I4
		object obj;
		if (!string.IsNullOrEmpty(PrefabId))
		{
			Brush spriteBrush = SpriteBrush;
			if (spriteBrush == null)
			{
				obj = null;
			}
			else
			{
				BrushLayer layer = spriteBrush.GetLayer(PrefabId);
				obj = ((layer != null) ? layer.Sprite : null);
			}
		}
		else
		{
			obj = null;
		}
		if (obj == null)
		{
			Brush spriteBrush2 = SpriteBrush;
			if (spriteBrush2 == null)
			{
				obj = null;
			}
			else
			{
				BrushLayer defaultLayer = spriteBrush2.DefaultLayer;
				obj = ((defaultLayer != null) ? defaultLayer.Sprite : null);
			}
		}
		Sprite val = (Sprite)obj;
		_childWidget.Brush.DefaultLayer.Sprite = val;
		if (val != null)
		{
			((Widget)_childWidget).IsVisible = true;
			float num;
			float num2;
			if ((int)((Widget)this).WidthSizePolicy == 2 && (int)((Widget)this).HeightSizePolicy == 2)
			{
				num = val.Width;
				num2 = val.Height;
			}
			else if ((int)((Widget)this).WidthSizePolicy == 2)
			{
				num2 = ((Widget)this).Size.Y * ((Widget)this)._inverseScaleToUse;
				num = num2 * (float)val.Width / (float)val.Height;
			}
			else if ((int)((Widget)this).HeightSizePolicy == 2)
			{
				num = ((Widget)this).Size.X * ((Widget)this)._inverseScaleToUse;
				num2 = num * (float)val.Height / (float)val.Width;
			}
			else
			{
				float num3 = ((Widget)this).Size.X * ((Widget)this)._inverseScaleToUse;
				float num4 = ((Widget)this).Size.Y * ((Widget)this)._inverseScaleToUse;
				float val2 = num3 / (float)val.Width;
				float val3 = num4 / (float)val.Height;
				float num5 = Math.Max(val2, val3);
				num = (float)val.Width * num5;
				num2 = (float)val.Height * num5;
			}
			((Widget)_childWidget).SuggestedWidth = num;
			((Widget)_childWidget).SuggestedHeight = num2;
		}
		else
		{
			((Widget)_childWidget).IsVisible = false;
		}
	}
}

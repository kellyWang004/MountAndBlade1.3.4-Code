using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.HotKeyCategories;

public class NavalShipControlsHotKeyCategory : GameKeyContext
{
	public const string CategoryId = "NavalShipControlsHotKeyCategory";

	public const string AccelerationAxis = "MovementAxisY";

	public const string TurnAxis = "MovementAxisX";

	public const string ToggleOarsmen = "ToggleOarsmen";

	public const string ToggleSail = "ToggleSail";

	public const string CutLoose = "CutLoose";

	public const string ChangeCamera = "ChangeCamera";

	public const string SelectShip = "SelectShip";

	public const string AttemptBoarding = "AttemptBoarding";

	public const string DelegateCommand = "DelegateCommand";

	public const string ToggleRangedWeaponOrderMode = "ToggleRangedWeaponDirectOrderMode";

	public const string ShootBallista = "ShootBallista";

	public NavalShipControlsHotKeyCategory()
		: base("NavalShipControlsHotKeyCategory", 0, (GameKeyContextType)0)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		GameAxisKey val = ((GameKeyContext)GenericGameKeyContext.Current).RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("MovementAxisY"));
		GameAxisKey val2 = ((GameKeyContext)GenericGameKeyContext.Current).RegisteredGameAxisKeys.First((GameAxisKey g) => g.Id.Equals("MovementAxisX"));
		((GameKeyContext)this).RegisterGameAxisKey(val, true);
		((GameKeyContext)this).RegisterGameAxisKey(val2, true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("ToggleSail", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)44),
			new Key((InputKey)240)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("ToggleOarsmen", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)45),
			new Key((InputKey)241)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("CutLoose", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)45),
			new Key((InputKey)241)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("ChangeCamera", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)46),
			new Key((InputKey)243)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("SelectShip", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)18),
			new Key((InputKey)252)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("AttemptBoarding", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)19),
			new Key((InputKey)253)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("DelegateCommand", "NavalShipControlsHotKeyCategory", (InputKey)34, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("ToggleRangedWeaponDirectOrderMode", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)225),
			new Key((InputKey)254)
		}, (Modifiers)0, (Modifiers)0), true);
		((GameKeyContext)this).RegisterHotKey(new HotKey("ShootBallista", "NavalShipControlsHotKeyCategory", new List<Key>
		{
			new Key((InputKey)224),
			new Key((InputKey)255)
		}, (Modifiers)0, (Modifiers)0), true);
	}
}

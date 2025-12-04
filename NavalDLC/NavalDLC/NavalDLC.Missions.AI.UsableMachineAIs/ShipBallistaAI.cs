using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs;

public sealed class ShipBallistaAI : BallistaAI
{
	private bool _canAiUpdateAim = true;

	private bool _isUnderDirectControl;

	public ShipBallistaAI(Ballista ballista)
		: base(ballista)
	{
		_isUnderDirectControl = false;
	}

	protected override void UpdateAim(RangedSiegeWeapon rangedSiegeWeapon, float dt)
	{
		if (!_isUnderDirectControl && _canAiUpdateAim)
		{
			((RangedSiegeWeaponAi)this).UpdateAim(rangedSiegeWeapon, dt);
		}
	}

	public void SetCanAiUpdateAim(bool canAiUpdateAim)
	{
		_canAiUpdateAim = canAiUpdateAim;
	}

	public void SetIsUnderDirectControl(bool value)
	{
		_isUnderDirectControl = value;
	}
}

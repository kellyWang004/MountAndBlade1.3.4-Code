using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC;

public class ShipAssignment
{
	private MissionShip _missionShip;

	private MissionShipObject _missionShipObject;

	private IShipOrigin _shipOrigin;

	public TeamSideEnum TeamSide { get; private set; }

	public FormationClass FormationIndex { get; private set; }

	public MissionShipObject MissionShipObject => _missionShipObject;

	public IShipOrigin ShipOrigin => _shipOrigin;

	public MissionShip MissionShip => _missionShip;

	public Formation Formation
	{
		get
		{
			if (_missionShip == null)
			{
				return null;
			}
			return _missionShip.Formation;
		}
	}

	public bool IsSet
	{
		get
		{
			if (_shipOrigin != null)
			{
				return _missionShipObject != null;
			}
			return false;
		}
	}

	public bool HasMissionShip
	{
		get
		{
			if (IsSet)
			{
				return MissionShip != null;
			}
			return false;
		}
	}

	internal void Set(IShipOrigin shipOrigin)
	{
		_shipOrigin = shipOrigin;
		IShipOrigin shipOrigin2 = _shipOrigin;
		if (!string.IsNullOrEmpty((shipOrigin2 != null) ? shipOrigin2.OriginShipId : null))
		{
			_missionShipObject = MBObjectManager.Instance.GetObject<MissionShipObject>(_shipOrigin.OriginShipId);
		}
		_missionShip = null;
	}

	internal void RemoveShip()
	{
		_missionShip = null;
	}

	internal void Clear()
	{
		_shipOrigin = null;
		_missionShipObject = null;
		_missionShip = null;
	}

	internal void SetMissionShip(MissionShip missionShip)
	{
		_missionShip = missionShip;
		_shipOrigin = missionShip.ShipOrigin;
		_missionShipObject = missionShip.MissionShipObject;
	}

	internal static ShipAssignment Create(TeamSideEnum teamSide, FormationClass formationIndex, IShipOrigin shipOrigin = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return new ShipAssignment(teamSide, formationIndex, shipOrigin);
	}

	private ShipAssignment(TeamSideEnum teamSide, FormationClass formationIndex, IShipOrigin shipOrigin = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		_shipOrigin = shipOrigin;
		TeamSide = teamSide;
		FormationIndex = formationIndex;
		if (shipOrigin != null)
		{
			Set(shipOrigin);
			return;
		}
		_shipOrigin = null;
		_missionShipObject = null;
		_missionShip = null;
	}
}

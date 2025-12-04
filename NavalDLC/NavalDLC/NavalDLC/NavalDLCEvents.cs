using NavalDLC.Map;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC;

public class NavalDLCEvents : CampaignEventReceiver
{
	private readonly MbEvent<PartyBase, NavalStorylinePartyData> _isPartyQuestPartyEvent = new MbEvent<PartyBase, NavalStorylinePartyData>();

	private readonly MbEvent<bool> _onNavalStorylineActivityChangedEvent = new MbEvent<bool>();

	private readonly MbEvent _onGangradirSavedEvent = new MbEvent();

	private readonly MbEvent _onNavalStorylineCanceledEvent = new MbEvent();

	private readonly MbEvent _onNavalStorylineTutorialSkippedEvent = new MbEvent();

	private readonly MbEvent<Storm> _onStormCreatedEvent = new MbEvent<Storm>();

	public static NavalDLCEvents Instance => NavalDLCManager.Instance.NavalDLCEvents;

	public static IMbEvent<PartyBase, NavalStorylinePartyData> IsNavalQuestPartyEvent => (IMbEvent<PartyBase, NavalStorylinePartyData>)(object)Instance._isPartyQuestPartyEvent;

	public static IMbEvent<bool> OnNavalStorylineActivityChangedEvent => (IMbEvent<bool>)(object)Instance._onNavalStorylineActivityChangedEvent;

	public static IMbEvent OnGangradirSavedEvent => (IMbEvent)(object)Instance._onGangradirSavedEvent;

	public static IMbEvent OnNavalStorylineCanceledEvent => (IMbEvent)(object)Instance._onNavalStorylineCanceledEvent;

	public static IMbEvent OnNavalStorylineTutorialSkippedEvent => (IMbEvent)(object)Instance._onNavalStorylineTutorialSkippedEvent;

	public static IMbEvent<Storm> OnStormCreatedEvent => (IMbEvent<Storm>)(object)Instance._onStormCreatedEvent;

	public override void RemoveListeners(object obj)
	{
		_onNavalStorylineActivityChangedEvent.ClearListeners(obj);
		_isPartyQuestPartyEvent.ClearListeners(obj);
		_onGangradirSavedEvent.ClearListeners(obj);
		_onNavalStorylineCanceledEvent.ClearListeners(obj);
		_onStormCreatedEvent.ClearListeners(obj);
		_onNavalStorylineTutorialSkippedEvent.ClearListeners(obj);
	}

	public void IsNavalQuestParty(PartyBase party, NavalStorylinePartyData result)
	{
		Instance._isPartyQuestPartyEvent.Invoke(party, result);
	}

	public void OnNavalStorylineActivityChanged(bool activity)
	{
		Instance._onNavalStorylineActivityChangedEvent.Invoke(activity);
	}

	public void OnGangradirSaved()
	{
		Instance._onGangradirSavedEvent.Invoke();
	}

	public void OnNavalStorylineCanceled()
	{
		Instance._onNavalStorylineCanceledEvent.Invoke();
	}

	public void OnNavalStorylineTutorialSkipped()
	{
		Instance._onNavalStorylineTutorialSkippedEvent.Invoke();
	}

	public void OnStormCreated(Storm storm)
	{
		Instance._onStormCreatedEvent.Invoke(storm);
	}
}

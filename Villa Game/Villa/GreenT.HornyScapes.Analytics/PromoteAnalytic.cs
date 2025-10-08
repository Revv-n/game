using StripClub.Model.Cards;
using StripClub.UI.Collections.Promote;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Analytics;

public class PromoteAnalytic : AnalyticWindow<PromoteWindow>
{
	[Inject]
	private CardsCollection _cardsCollection;

	private readonly CompositeDisposable _onNewStream = new CompositeDisposable();

	private void OnEnable()
	{
		if (window.Card != null)
		{
			TrackPromote();
		}
	}

	private void OnDisable()
	{
		_onNewStream?.Clear();
	}

	private void TrackPromote()
	{
		_onNewStream?.Clear();
		_cardsCollection.GetPromoteOrDefault(window.Card).Level.Skip(1).Subscribe(SendPromoteEvent).AddTo(_onNewStream);
	}

	private void SendPromoteEvent(int level)
	{
		PromoteAmplitudeEvent analyticsEvent = new PromoteAmplitudeEvent(window.Card, level);
		amplitude.AddEvent(analyticsEvent);
	}
}

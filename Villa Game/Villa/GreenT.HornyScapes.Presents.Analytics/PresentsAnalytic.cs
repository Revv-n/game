using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Characters;

namespace GreenT.HornyScapes.Presents.Analytics;

public class PresentsAnalytic : BaseAnalytic
{
	private readonly CharacterManager _characterManager;

	public PresentsAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, CharacterManager characterManager)
		: base(amplitude)
	{
		_characterManager = characterManager;
	}

	public void SendSpendEvent(string presentId, int amount, int relationshipID)
	{
		ICharacter character = _characterManager.Collection.First((ICharacter x) => x.RelationsipId == relationshipID);
		PresentSpendAnalyticEvent analyticsEvent = new PresentSpendAnalyticEvent(presentId, amount, character.ID);
		amplitude.AddEvent(analyticsEvent);
	}

	public void SendReceivedEvent(string presentId, int amount, CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		PresentReceivedAnalyticEvent analyticsEvent = new PresentReceivedAnalyticEvent(presentId, amount, CurrencyAmplitudeAnalytic.Source[sourceType]);
		amplitude.AddEvent(analyticsEvent);
	}
}

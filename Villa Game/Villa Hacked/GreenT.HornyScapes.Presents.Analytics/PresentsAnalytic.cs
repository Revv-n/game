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
		PresentSpendAnalyticEvent presentSpendAnalyticEvent = new PresentSpendAnalyticEvent(presentId, amount, character.ID);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)presentSpendAnalyticEvent);
	}

	public void SendReceivedEvent(string presentId, int amount, CurrencyAmplitudeAnalytic.SourceType sourceType)
	{
		PresentReceivedAnalyticEvent presentReceivedAnalyticEvent = new PresentReceivedAnalyticEvent(presentId, amount, CurrencyAmplitudeAnalytic.Source[sourceType]);
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent((AmplitudeEvent)presentReceivedAnalyticEvent);
	}
}

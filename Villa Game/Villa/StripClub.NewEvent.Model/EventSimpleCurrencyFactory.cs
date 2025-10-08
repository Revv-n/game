using StripClub.NewEvent.Data;

namespace StripClub.NewEvent.Model;

public class EventSimpleCurrencyFactory
{
	private readonly IEventDataSaver eventDataSaver;

	private readonly IEventWallet wallet;

	public EventSimpleCurrencyFactory(IEventDataSaver eventDataSaver, IEventWallet wallet)
	{
		this.eventDataSaver = eventDataSaver;
		this.wallet = wallet;
	}

	public EventSimpleCurrency Create(EventCurrencyType type, int value)
	{
		EventSimpleCurrency eventSimpleCurrency = CreateCurrency(type, value);
		wallet.ForceSet(eventSimpleCurrency);
		eventDataSaver.Add(eventSimpleCurrency);
		return eventSimpleCurrency;
	}

	protected EventSimpleCurrency CreateCurrency(EventCurrencyType type, int value)
	{
		return new EventSimpleCurrency(type, value);
	}
}

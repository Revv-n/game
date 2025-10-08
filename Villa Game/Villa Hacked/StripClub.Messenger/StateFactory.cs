using GreenT.Data;
using Zenject;

namespace StripClub.Messenger;

public class StateFactory : IFactory<MessengerState>, IFactory
{
	private readonly IMessengerManager messenger;

	private readonly ISaver saver;

	public StateFactory(IMessengerManager messenger, ISaver saver)
	{
		this.messenger = messenger;
		this.saver = saver;
	}

	public MessengerState Create()
	{
		MessengerState messengerState = new MessengerState(messenger);
		saver.Add(messengerState);
		return messengerState;
	}
}

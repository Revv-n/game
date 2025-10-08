using GreenT.HornyScapes.Messenger;
using StripClub.Extensions;
using StripClub.Messenger;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class PlayerResponseSaveEvent : SaveEvent
{
	private IMessengerManager messenger;

	private GameStarter gameStarter;

	[Inject]
	public void Init(IMessengerManager messenger, GameStarter gameStarter)
	{
		this.messenger = messenger;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		(from _message in (from _arg in gameStarter.IsGameActive.First((bool _isActive) => _isActive).ContinueWith(messenger.OnUpdate)
				select _arg.Message).OfType<ChatMessage, PlayerChatMessage>()
			where _message.State.Contains(ChatMessage.MessageState.Delivered) && _message.State.Contains(ChatMessage.MessageState.Passed)
			select _message).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

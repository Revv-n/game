using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<PlayerChatMessage>(Observable.Where<PlayerChatMessage>(Observable.OfType<ChatMessage, PlayerChatMessage>(Observable.Select<MessengerUpdateArgs, ChatMessage>(Observable.ContinueWith<bool, MessengerUpdateArgs>(Observable.First<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _isActive) => _isActive)), messenger.OnUpdate), (Func<MessengerUpdateArgs, ChatMessage>)((MessengerUpdateArgs _arg) => _arg.Message))), (Func<PlayerChatMessage, bool>)((PlayerChatMessage _message) => _message.State.Contains(ChatMessage.MessageState.Delivered) && _message.State.Contains(ChatMessage.MessageState.Passed))), (Action<PlayerChatMessage>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

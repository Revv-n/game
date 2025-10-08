using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MergeCore;
using Merge;
using StripClub.Messenger;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Messenger;

public class MessengerNotifyController : IDisposable, IInitializable
{
	private IMessengerManager messengerManager;

	private readonly GameStarter gameStarter;

	private Subject<bool> onHasPlayerResponse = new Subject<bool>();

	private List<PlayerChatMessage> waitAnswers = new List<PlayerChatMessage>();

	private List<ResponseOption> canAnswerToCharacter = new List<ResponseOption>();

	private CompositeDisposable checkResponseStream = new CompositeDisposable();

	private IDisposable checkStream;

	public IObservable<bool> OnHasPlayerResponse => Observable.AsObservable<bool>((IObservable<bool>)onHasPlayerResponse);

	public bool HasPlayerResponse => canAnswerToCharacter.Any();

	public MessengerNotifyController(IMessengerManager messengerManager, GameStarter gameStarter)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		this.messengerManager = messengerManager;
		this.gameStarter = gameStarter;
	}

	public void Initialize()
	{
		SubscribeOnLoading(gameStarter);
	}

	private void SubscribeOnLoading(GameStarter gameStarter)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.SkipWhile<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => !x)), (Action<bool>)NotifyResponseStates), (ICollection<IDisposable>)checkResponseStream);
	}

	private void NotifyResponseStates(bool isOn)
	{
		Controller<GameItemController>.Instance.OnItemCreated -= OnItemChanged;
		Controller<GameItemController>.Instance.OnItemRemoved -= OnItemChanged;
		Controller<GameItemController>.Instance.OnItemTakenFromSomethere -= OnItemChanged;
		checkStream?.Dispose();
		if (!isOn)
		{
			return;
		}
		foreach (Dialogue dialogue in messengerManager.GetDialogues())
		{
			CheckPlayerResponse(dialogue);
		}
		checkStream = ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.Where<MessengerUpdateArgs>(messengerManager.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Dialogue != null)), (Action<MessengerUpdateArgs>)delegate(MessengerUpdateArgs _args)
		{
			CheckPlayerResponse(_args.Dialogue);
		});
		Controller<GameItemController>.Instance.OnItemCreated += OnItemChanged;
		Controller<GameItemController>.Instance.OnItemRemoved += OnItemChanged;
		Controller<GameItemController>.Instance.OnItemTakenFromSomethere += OnItemChanged;
	}

	private void OnItemChanged(GameItem obj)
	{
		foreach (PlayerChatMessage waitAnswer in waitAnswers)
		{
			CheckOptionsWithMergeItems(waitAnswer);
			onHasPlayerResponse.OnNext(HasPlayerResponse);
		}
	}

	private void CheckPlayerResponse(Dialogue dialogue)
	{
		if (IsLastPlayerMessage(out var _playerChatMessage2))
		{
			if (!waitAnswers.Contains(_playerChatMessage2))
			{
				waitAnswers.Add(_playerChatMessage2);
			}
			CheckOptionsWithMergeItems(_playerChatMessage2);
			onHasPlayerResponse.OnNext(HasPlayerResponse);
		}
		bool IsLastPlayerMessage(out PlayerChatMessage _playerChatMessage)
		{
			_playerChatMessage = null;
			if (dialogue.IsComplete)
			{
				return false;
			}
			if (!(dialogue.LastMessage is PlayerChatMessage playerChatMessage))
			{
				return false;
			}
			_playerChatMessage = playerChatMessage;
			return true;
		}
	}

	private bool CheckOptionsWithMergeItems(PlayerChatMessage _playerChatMessage)
	{
		bool flag = false;
		foreach (ResponseOption getAvailableOption in _playerChatMessage.GetAvailableOptions)
		{
			if (getAvailableOption.NecessaryItems.Any())
			{
				flag = IsOptionReady(getAvailableOption);
				if (flag && !canAnswerToCharacter.Contains(getAvailableOption))
				{
					ListenAnswer(_playerChatMessage);
					canAnswerToCharacter.Add(getAvailableOption);
				}
				else if (!flag && canAnswerToCharacter.Contains(getAvailableOption))
				{
					canAnswerToCharacter.Remove(getAvailableOption);
				}
			}
		}
		return flag;
	}

	private void ListenAnswer(PlayerChatMessage _playerChatMessage)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<ChatMessage>(_playerChatMessage.OnUpdate, (Action<ChatMessage>)delegate(ChatMessage _message)
		{
			foreach (ResponseOption getAvailableOption in (_message as PlayerChatMessage).GetAvailableOptions)
			{
				if (canAnswerToCharacter.Contains(getAvailableOption))
				{
					canAnswerToCharacter.Remove(getAvailableOption);
				}
			}
			waitAnswers.Remove(_playerChatMessage);
			onHasPlayerResponse.OnNext(canAnswerToCharacter.Any());
		}), (ICollection<IDisposable>)checkResponseStream);
	}

	private bool IsOptionReady(ResponseOption responseOption)
	{
		bool flag = true;
		foreach (IItemLot necessaryItem in responseOption.NecessaryItems)
		{
			flag = flag && necessaryItem.CheckIsEnough();
		}
		return flag;
	}

	public void Dispose()
	{
		if (Controller<GameItemController>.Instance != null)
		{
			Controller<GameItemController>.Instance.OnItemCreated -= OnItemChanged;
			Controller<GameItemController>.Instance.OnItemRemoved -= OnItemChanged;
			Controller<GameItemController>.Instance.OnItemTakenFromSomethere -= OnItemChanged;
		}
		checkStream?.Dispose();
		onHasPlayerResponse.Dispose();
		checkResponseStream.Dispose();
	}
}

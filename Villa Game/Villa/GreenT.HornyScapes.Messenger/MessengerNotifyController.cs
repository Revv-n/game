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

	public IObservable<bool> OnHasPlayerResponse => onHasPlayerResponse.AsObservable();

	public bool HasPlayerResponse => canAnswerToCharacter.Any();

	public MessengerNotifyController(IMessengerManager messengerManager, GameStarter gameStarter)
	{
		this.messengerManager = messengerManager;
		this.gameStarter = gameStarter;
	}

	public void Initialize()
	{
		SubscribeOnLoading(gameStarter);
	}

	private void SubscribeOnLoading(GameStarter gameStarter)
	{
		gameStarter.IsGameActive.SkipWhile((bool x) => !x).Subscribe(NotifyResponseStates).AddTo(checkResponseStream);
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
		checkStream = messengerManager.OnUpdate.Where((MessengerUpdateArgs _args) => _args.Dialogue != null).Subscribe(delegate(MessengerUpdateArgs _args)
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
		_playerChatMessage.OnUpdate.Subscribe(delegate(ChatMessage _message)
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
		}).AddTo(checkResponseStream);
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

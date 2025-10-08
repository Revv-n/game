using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Cheats;

[MementoHolder]
public class JewelsObserverTemp : IDisposable, ISavableState
{
	[Serializable]
	public class JewelsObserverMemento : Memento
	{
		public string History { get; }

		public JewelsObserverMemento(JewelsObserverTemp taskObserver)
			: base(taskObserver)
		{
			History = taskObserver.History;
		}
	}

	private GameStarter _gameStarter;

	private TaskManagerCluster _taskManagerCluster;

	private CurrencyProcessor _currencyProcessor;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private const string _saveKey = "jewelsTask.observer.key";

	private Task _lastTask;

	public string History { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public string UniqueKey()
	{
		return "jewelsTask.observer.key";
	}

	public Memento SaveState()
	{
		return new JewelsObserverMemento(this);
	}

	public void LoadState(Memento memento)
	{
		JewelsObserverMemento jewelsObserverMemento = (JewelsObserverMemento)memento;
		History = jewelsObserverMemento.History;
	}

	public JewelsObserverTemp(GameStarter gameStarter, TaskManagerCluster taskManagerCluster, CurrencyProcessor currencyProcessor, ISaver saver)
	{
		_gameStarter = gameStarter;
		_taskManagerCluster = taskManagerCluster;
		_currencyProcessor = currencyProcessor;
		saver.Add(this);
		_gameStarter.IsGameReadyToStart.Where((bool ready) => ready).Subscribe(delegate
		{
			StartWork();
		}).AddTo(_disposables);
	}

	private void StartWork()
	{
		_taskManagerCluster[ContentType.Main].Tasks.Where((Task task) => !task.IsRewarded).ToObservable().SelectMany((Task task) => task.OnUpdate)
			.Subscribe(delegate(Task updatedTask)
			{
				OnTaskUpdated(updatedTask);
			})
			.AddTo(_disposables);
		_currencyProcessor.GetAddStream(CurrencyType.Jewel, delegate(int amount)
		{
			PlayerGetJewel(amount);
		}).AddTo(_disposables);
	}

	private void OnTaskUpdated(Task task)
	{
		if (!task.IsRewarded || !IsRewardJewel(task.Reward) || _lastTask == task)
		{
			return;
		}
		string text = $"[{DateTime.Now:HH:mm:ss}] Player done task: {task} and get reward: ";
		foreach (CurrencyLinkedContent allCurrencyReward in GetAllCurrencyRewards(task.Reward))
		{
			text += $"{allCurrencyReward.Currency}, quantity {allCurrencyReward.Quantity}";
		}
		History = History + text + "\n";
		_lastTask = task;
	}

	private bool IsRewardJewel(LinkedContent linkedContent)
	{
		foreach (CurrencyLinkedContent allCurrencyReward in GetAllCurrencyRewards(linkedContent))
		{
			if (allCurrencyReward.Currency == CurrencyType.Jewel)
			{
				return true;
			}
		}
		return false;
	}

	private List<CurrencyLinkedContent> GetAllCurrencyRewards(LinkedContent linkedContent)
	{
		List<CurrencyLinkedContent> list = new List<CurrencyLinkedContent>();
		for (CurrencyLinkedContent next = linkedContent.GetNext<CurrencyLinkedContent>(checkThis: true); next != null; next = next.GetNext<CurrencyLinkedContent>())
		{
			list.Add(next);
		}
		return list;
	}

	private void PlayerGetJewel(int amount)
	{
		History += $"[{DateTime.Now:HH:mm:ss}] Player's wallet get {amount} jewel(s)\n\n";
	}

	void IDisposable.Dispose()
	{
		_disposables.Dispose();
	}
}

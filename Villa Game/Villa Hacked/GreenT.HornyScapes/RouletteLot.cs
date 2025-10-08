using System;
using System.Collections.Generic;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes;

[MementoHolder]
public abstract class RouletteLot : IPurchasable, ISavableState
{
	public struct RewardSettings
	{
		public LinkedContent DefaultReward { get; }

		public LinkedContent UniqueReward { get; }

		public RewardSettings(LinkedContent defaultReward, LinkedContent uniqueReward)
		{
			DefaultReward = defaultReward;
			UniqueReward = uniqueReward;
		}
	}

	[Serializable]
	public class RouletteMemento : Memento
	{
		public int RollCount;

		public bool IsViewed;

		public int OverallRollCount;

		public RouletteMemento(RouletteLot lot)
			: base(lot)
		{
			Save(lot);
		}

		public Memento Save(RouletteLot lot)
		{
			RollCount = lot._rollCount;
			OverallRollCount = lot._overallRollCount;
			IsViewed = lot.IsViewed;
			return this;
		}
	}

	protected readonly RouletteDropService _rouletteDropService;

	protected readonly IPurchaseProcessor _purchaseProcessor;

	protected readonly SignalBus _signalBus;

	protected int _overallRollCount;

	protected int _rollCount;

	private const int PURCHASE_10X_LEFT_AMOUNT = 9;

	private const int COUNT_10X = 10;

	public int OverallRollCount => _overallRollCount;

	public int ID { get; }

	public ILocker Locker { get; }

	public bool IsViewed { get; set; }

	public bool Wholesale { get; set; }

	public string ShopSource { get; }

	public bool IsLastMainReward { get; private set; }

	public int Attempts { get; private set; }

	public Price<int> CurrentPrice { get; protected set; }

	public List<bool> IsMainReward { get; private set; }

	public List<int> AttemptsForAnalytics { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	protected RouletteLot(int id, ILocker locker, IPurchaseProcessor purchaseProcessor, string shopSource, RouletteDropService rouletteDropService, SignalBus signalBus)
	{
		ID = id;
		Locker = locker;
		ShopSource = shopSource;
		_rouletteDropService = rouletteDropService;
		_purchaseProcessor = purchaseProcessor;
		_signalBus = signalBus;
		IsMainReward = new List<bool>();
		AttemptsForAnalytics = new List<int>();
	}

	public virtual void SendPurchaseNotification(bool wholesale)
	{
		_signalBus.Fire<RouletteLotBoughtSignal>(new RouletteLotBoughtSignal(this, wholesale));
	}

	public virtual bool Purchase()
	{
		int rollCount = _rollCount;
		int attempts = Attempts;
		IsMainReward.Clear();
		AttemptsForAnalytics.Clear();
		LinkedContent linkedContent;
		if (Wholesale)
		{
			IsLastMainReward = _rouletteDropService.TryGetContent(_rollCount, out linkedContent);
			LinkedContent linkedContent2 = linkedContent;
			UpdateRollCount();
			_overallRollCount += 10;
			AttemptsForAnalytics.Add(Attempts);
			IsMainReward.Add(IsLastMainReward);
			for (int i = 0; i < 9; i++)
			{
				IsLastMainReward = _rouletteDropService.TryGetContent(_rollCount, out var linkedContent3);
				linkedContent2.Insert(linkedContent3);
				linkedContent2 = linkedContent3;
				UpdateRollCount();
				AttemptsForAnalytics.Add(Attempts);
				IsMainReward.Add(IsLastMainReward);
			}
		}
		else
		{
			IsLastMainReward = _rouletteDropService.TryGetContent(_rollCount, out linkedContent);
			UpdateRollCount();
			_overallRollCount++;
		}
		if (!_purchaseProcessor.TryBuy(linkedContent, CurrentPrice))
		{
			_rollCount = rollCount;
			Attempts = attempts;
			return false;
		}
		return true;
	}

	private void UpdateRollCount()
	{
		int num2 = (Attempts = _rollCount + 1);
		_rollCount = ((!IsLastMainReward) ? num2 : 0);
	}

	public abstract string UniqueKey();

	public virtual Memento SaveState()
	{
		return new RouletteMemento(this);
	}

	public virtual void LoadState(Memento memento)
	{
		RouletteMemento rouletteMemento = (RouletteMemento)memento;
		_rollCount = rouletteMemento.RollCount;
		_overallRollCount = rouletteMemento.OverallRollCount;
		IsViewed = rouletteMemento.IsViewed;
	}
}

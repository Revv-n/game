using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Booster.Effect;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace StripClub.Model.Shop;

[MementoHolder]
public class SummonLot : ValuableLot<int>, ISavableState
{
	[Serializable]
	public class SummonMemento : Memento
	{
		[field: SerializeField]
		public DateTime LastBuy { get; private set; }

		public SummonMemento(SummonLot lot)
			: base(lot)
		{
			LastBuy = lot.LastPurchase;
		}
	}

	public struct RewardSettings
	{
		public LinkedContent Reward { get; }

		public LinkedContent FirstPurchaseReward { get; }

		public Price<int> Price { get; }

		public RewardSettings(LinkedContent reward, LinkedContent firstPurchaseReward, Price<int> price)
		{
			Reward = reward;
			FirstPurchaseReward = firstPurchaseReward;
			Price = price;
		}
	}

	public readonly ContentType TaskType;

	private string uniqueKey;

	private const string localizationBase = "content.shop.summon.{0}";

	private const string title = ".title";

	private const string description = ".desc";

	private const string dropChances = ".drop_chances.title";

	private LinkedContent currentContent;

	private Price<int> currentPrice;

	private readonly TimeSpan? _initialUnlockTime;

	private readonly Subject<Unit> _onUnlockTimeUpdate = new Subject<Unit>();

	public override string LocalizationKey => $"content.shop.summon.{base.ID}";

	public string TitleKey => LocalizationKey + ".title";

	public string DescriptionKey => LocalizationKey + ".desc";

	public string DropChancesTitleKey => LocalizationKey + ".drop_chances.title";

	public Stickers Stickers { get; }

	public RewardSettings SingleRewardSettings { get; }

	public RewardSettings WholesaleRewardSettings { get; }

	public Func<DateTime> GetTime { get; set; }

	public DateTime LastPurchase { get; private set; }

	public override bool IsFree
	{
		get
		{
			if (!IsFreeAlways)
			{
				return IsNowFreeByTime();
			}
			return true;
		}
	}

	public Dictionary<int, decimal> FakeChances { get; }

	public string ViewName { get; }

	public bool CurrentWholesale { get; protected set; }

	public override LinkedContent Content => currentContent;

	public ContentSource Source { get; }

	public BoosterIncrementBonus Bonus { get; private set; }

	public bool IsFreeAlways { get; private set; }

	public bool IsX10Disabled { get; private set; }

	public override Price<int> Price => currentPrice;

	public override string UniqueKey()
	{
		return uniqueKey;
	}

	public SummonLot(SummonMapper mapper, RewardSettings singleReward, RewardSettings wholesaleReward, ILocker locker, EqualityLocker<int> countLocker, IPurchaseProcessor purchaseProcessor, Func<DateTime> getTime)
		: base(mapper.id, mapper.monetization_id, mapper.tab_id, mapper.position, mapper.buy_times, locker, countLocker, purchaseProcessor, mapper.source)
	{
		Source = mapper.content_source;
		TaskType = mapper.task_type;
		base.Received = 0;
		Stickers = (Stickers)0;
		if (mapper.hot)
		{
			Stickers |= Stickers.Hot;
		}
		if (mapper.best)
		{
			Stickers |= Stickers.Best;
		}
		SingleRewardSettings = singleReward;
		WholesaleRewardSettings = wholesaleReward;
		if (mapper.free_unlock_time.HasValue)
		{
			_initialUnlockTime = TimeSpan.FromSeconds(mapper.free_unlock_time.Value);
		}
		LastPurchase = default(DateTime);
		FakeChances = new Dictionary<int, decimal>();
		for (int i = 0; i != mapper.drop_chances.Length; i++)
		{
			FakeChances[i] = mapper.drop_chances[i];
		}
		ViewName = mapper.view_type;
		GetTime = getTime;
		uniqueKey = "lot.summon." + base.ID;
		IsFreeAlways = SingleRewardSettings.Price.Value == 0;
		IsX10Disabled = WholesaleRewardSettings.Price == null;
	}

	public override void Initialize()
	{
		base.Initialize();
		LastPurchase = DateTime.MinValue;
	}

	public void Setup(bool wholesale)
	{
		RewardSettings rewardSettings = (wholesale ? WholesaleRewardSettings : SingleRewardSettings);
		currentContent = (IsFirstReceived() ? rewardSettings.FirstPurchaseReward : rewardSettings.Reward);
		currentPrice = ((!wholesale && IsFree) ? Price<int>.Free : rewardSettings.Price);
		CurrentWholesale = wholesale;
	}

	public bool IsFirstReceived()
	{
		return base.Received == 0;
	}

	public void SetBonus(BoosterIncrementBonus bonus)
	{
		Bonus = bonus;
		_onUnlockTimeUpdate.OnNext(default(Unit));
	}

	public void ResetBonus()
	{
		Bonus = null;
		_onUnlockTimeUpdate.OnNext(default(Unit));
	}

	public override GreenT.Data.Memento SaveState()
	{
		return new SummonMemento(this);
	}

	public override void LoadState(GreenT.Data.Memento memento)
	{
		base.LoadState(memento);
		SummonMemento summonMemento = (SummonMemento)memento;
		LastPurchase = summonMemento.LastBuy;
	}

	public override bool Purchase()
	{
		if (!base.Purchase())
		{
			return false;
		}
		if (IsFree && CurrentWholesale)
		{
			return true;
		}
		LastPurchase = GetTime();
		return true;
	}

	public Price<int> GetSinglePrice()
	{
		if (IsFree)
		{
			return Price<int>.Free;
		}
		return SingleRewardSettings.Price;
	}

	public IObservable<Unit> OnUnlockTimeUpdate()
	{
		return _onUnlockTimeUpdate.AsObservable();
	}

	public TimeSpan? GetUnlockTime()
	{
		if (Bonus == null)
		{
			return _initialUnlockTime;
		}
		return Bonus.ApplyTimer.TimeLeft;
	}

	private bool IsNowFreeByTime()
	{
		DateTime value = Bonus?.LastApplied ?? LastPurchase;
		TimeSpan? unlockTime = GetUnlockTime();
		return value + unlockTime <= GetTime();
	}
}

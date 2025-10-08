using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.MergeStore;

[MementoHolder]
public class StoreSection : IDisposable, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public ItemSave[] Items;

		public TimeSpan TimeLeft { get; }

		public DateTime SaveTime { get; }

		public Memento(StoreSection section)
			: base(section)
		{
			TimeLeft = section._refreshTimer.TimeLeft;
			Items = section._items.Select((Item mergeStoreItem) => new ItemSave(mergeStoreItem)).ToArray();
			SaveTime = section._clock.GetTime();
		}
	}

	public readonly int ID;

	private readonly string _saveKey;

	private readonly IClock _clock;

	private readonly GenericTimer _refreshTimer;

	private IDisposable _timeStream;

	private Item[] _items = Array.Empty<Item>();

	public readonly ContentType ContentType;

	public readonly SectionType Type;

	public readonly int RefreshTime;

	public readonly int RefreshPrice;

	public readonly CurrencyType CurrencyType;

	public readonly IReadOnlyCollection<int> DiscountChances;

	public readonly int ItemsCount;

	public readonly int EnergyThreshold;

	public readonly int EnergyLowerTierChance;

	public readonly int SaleTierDifference;

	public readonly int[] RarityChance;

	private readonly Subject<StoreSection> _onClear = new Subject<StoreSection>();

	private readonly Subject<SectionRefreshRequest> _onRequestRefresh = new Subject<SectionRefreshRequest>();

	private readonly Subject<StoreSection> _onRefresh = new Subject<StoreSection>();

	public GenericTimer RefreshTimer => _refreshTimer;

	public IReadOnlyCollection<IItem> ShopItems => (IReadOnlyCollection<IItem>)(object)_items;

	public IObservable<StoreSection> OnClear => (IObservable<StoreSection>)_onClear;

	public IObservable<SectionRefreshRequest> OnRequestRefresh => Observable.AsObservable<SectionRefreshRequest>((IObservable<SectionRefreshRequest>)_onRequestRefresh);

	public IObservable<StoreSection> OntRefresh => Observable.AsObservable<StoreSection>((IObservable<StoreSection>)_onRefresh);

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public StoreSection(int id, string saveKey, GenericTimer refreshTimer, IClock clock, ContentType contentType, SectionType type, int refreshTime, int refreshPrice, CurrencyType refreshCurrency, int[] discountChances, int itemsCount, int energyThreshold, int energyLowerTierChance, int saleTierDifference, int[] rarityChance)
	{
		ID = id;
		_clock = clock ?? throw new ArgumentNullException("clock");
		ContentType = contentType;
		Type = type;
		RefreshTime = refreshTime;
		RefreshPrice = refreshPrice;
		CurrencyType = refreshCurrency;
		DiscountChances = (IReadOnlyCollection<int>)(object)discountChances;
		ItemsCount = itemsCount;
		_saveKey = saveKey;
		_refreshTimer = refreshTimer;
		EnergyThreshold = energyThreshold;
		EnergyLowerTierChance = energyLowerTierChance;
		SaleTierDifference = saleTierDifference;
		RarityChance = rarityChance;
	}

	public void Initialization()
	{
		if (!_refreshTimer.IsActive.Value)
		{
			LaunchTimers();
		}
	}

	public void Refresh(Item[] items)
	{
		ClearItems();
		SetItems(items);
		LaunchTimers();
		_onRefresh.OnNext(this);
	}

	public StoreSection Clear()
	{
		_refreshTimer.Stop();
		_refreshTimer?.Dispose();
		_timeStream?.Dispose();
		ClearItems();
		return this;
	}

	public Item[] GetItems()
	{
		return _items;
	}

	public IItem GetItem(int index)
	{
		return _items[index];
	}

	private void SetItems(Item[] items)
	{
		_items = items;
	}

	private void LaunchTimers()
	{
		_timeStream?.Dispose();
		_refreshTimer.Clear();
		if (RefreshTime > 0)
		{
			InitSectionTimer();
		}
	}

	private void ClearItems()
	{
		if (_items != null)
		{
			Item[] items = _items;
			for (int i = 0; i < items.Length; i++)
			{
				items[i].Clear();
			}
			_items = Array.Empty<Item>();
		}
	}

	private void FreeRequestRefresh()
	{
		_onRequestRefresh.OnNext(SectionRefreshRequest.GetFree(this));
	}

	private void InitSectionTimer()
	{
		StartTimer(TimeSpan.FromSeconds(RefreshTime));
	}

	private void StartTimer(TimeSpan timeSpan)
	{
		_timeStream?.Dispose();
		_refreshTimer.Start(timeSpan);
		_timeStream = ObservableExtensions.Subscribe<GenericTimer>(_refreshTimer.OnTimeIsUp, (Action<GenericTimer>)delegate
		{
			FreeRequestRefresh();
			_refreshTimer.Start(TimeSpan.FromSeconds(RefreshTime));
		});
	}

	public void Dispose()
	{
		_timeStream?.Dispose();
		_refreshTimer?.Dispose();
	}

	public string UniqueKey()
	{
		return _saveKey;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (!(memento is Memento memento2))
		{
			return;
		}
		TimeSpan timeSpan = _clock.GetTime() - memento2.SaveTime;
		TimeSpan timeSpan2 = ((memento2.TimeLeft > TimeSpan.Zero) ? (memento2.TimeLeft - timeSpan) : TimeSpan.Zero);
		if (!(timeSpan2 <= TimeSpan.Zero))
		{
			StartTimer(timeSpan2);
			SetItems(memento2.Items.Select((ItemSave saveItem) => new Item(saveItem)).ToArray());
		}
	}

	public void TryRefresh()
	{
		_onRequestRefresh.OnNext(SectionRefreshRequest.Get(this));
	}
}

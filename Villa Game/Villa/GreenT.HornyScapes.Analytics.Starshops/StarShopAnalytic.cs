using System;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.StarShop;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Analytics.Starshops;

[MementoHolder]
public class StarShopAnalytic : BaseEntityAnalytic<StarShopItem>, ISavableState
{
	[Serializable]
	public class StarShopAnalyticMemento : Memento
	{
		public int LastCompleted;

		public StarShopAnalyticMemento(StarShopAnalytic analytic)
			: base(analytic)
		{
			LastCompleted = analytic.LastCompletedId;
		}
	}

	private StarShopManager starShopManager;

	private readonly ICurrencyProcessor currencies;

	private readonly GameStarter gameStarter;

	private int lastCompletedId = -1;

	public int LastCompletedId
	{
		get
		{
			if (lastCompletedId != -1)
			{
				return lastCompletedId;
			}
			StarShopItem[] source = starShopManager.Collection.Where((StarShopItem _starShop) => _starShop.State == EntityStatus.Rewarded).ToArray();
			if (source.Any())
			{
				lastCompletedId = source.Max((StarShopItem _starShop) => _starShop.ID);
			}
			return lastCompletedId;
		}
	}

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public StarShopAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, StarShopManager starShopManager, ICurrencyProcessor currencies, GameStarter gameStarter)
		: base(amplitude)
	{
		this.starShopManager = starShopManager;
		this.currencies = currencies;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		ClearStreams();
		OnGameStart().Subscribe(delegate
		{
			TrackStarShops();
		}).AddTo(onNewStream);
		IObservable<bool> OnGameStart()
		{
			return gameStarter.IsGameActive.Where((bool x) => x);
		}
	}

	private void TrackStarShops()
	{
		SubscribeOnNewAddToTrack();
		SubscribeOnNoRewardedItems();
	}

	private void SubscribeOnNoRewardedItems()
	{
		StarShopItem[] array = starShopManager.Collection.Where((StarShopItem _item) => _item.State != EntityStatus.Rewarded).ToArray();
		foreach (StarShopItem step in array)
		{
			AddToTrack(step);
		}
	}

	private void SubscribeOnNewAddToTrack()
	{
		starShopManager.OnNew.Subscribe(AddToTrack).AddTo(onNewStream);
	}

	private void AddToTrack(StarShopItem step)
	{
		if (!itemsStreams.ContainsKey(step.ID))
		{
			IDisposable value = step.OnUpdate.Where(IsValid).Subscribe(SendEventByPass);
			itemsStreams.Add(step.ID, value);
		}
	}

	protected override bool IsValid(StarShopItem entity)
	{
		return entity.State == EntityStatus.Rewarded;
	}

	public override void SendEventByPass(StarShopItem tuple)
	{
		lastCompletedId = tuple.ID;
		amplitude.AddEvent(new StarShopAmplitudeEventOld(tuple));
		FreeStream(tuple.ID);
	}

	public string UniqueKey()
	{
		return "StarShop.Analytic";
	}

	public Memento SaveState()
	{
		return new StarShopAnalyticMemento(this);
	}

	public void LoadState(Memento memento)
	{
		StarShopAnalyticMemento starShopAnalyticMemento = (StarShopAnalyticMemento)memento;
		lastCompletedId = starShopAnalyticMemento.LastCompleted;
	}
}

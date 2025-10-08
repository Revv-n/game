using System;
using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class CurrencyAmplitudeAnalytic
{
	public enum SourceType
	{
		None,
		Summon,
		Event,
		Bought,
		Task,
		LevelUp,
		MergeSell,
		MergeCollect,
		MergeRechargeEnergy,
		StarShop,
		SignUp,
		Promote,
		MergeRechargeClickSpawner,
		MergeRechargeChest,
		MergeRechargeBubble,
		MergeRechargeLocker,
		BoughtCellInventory,
		Sellout,
		Roulette,
		EightSlotsMinievents,
		OfferMinievents,
		BundlesMinievents,
		BattlePass,
		Duplicate,
		GoldenTicket,
		MiniEventRating,
		SpawnMergeItem,
		MergeRechargeEventEnergy,
		MergeShop,
		RefreshMergeShop,
		MergePoints,
		Bubble,
		MiniChainBundles,
		Banner,
		Relationship,
		Present,
		EventBattlePass,
		Promocode
	}

	public static Dictionary<SourceType, string> Source = new Dictionary<SourceType, string>
	{
		{
			SourceType.None,
			"none"
		},
		{
			SourceType.Summon,
			"summon"
		},
		{
			SourceType.Event,
			"event"
		},
		{
			SourceType.Bought,
			"bought"
		},
		{
			SourceType.Task,
			"task"
		},
		{
			SourceType.LevelUp,
			"level_up"
		},
		{
			SourceType.MergeSell,
			"merge_sell"
		},
		{
			SourceType.MergeCollect,
			"merge_collect"
		},
		{
			SourceType.MergeRechargeEnergy,
			"merge_recharge_energy"
		},
		{
			SourceType.MergeRechargeEventEnergy,
			"event_recharge_energy"
		},
		{
			SourceType.StarShop,
			"star_shop"
		},
		{
			SourceType.SignUp,
			"sign_up"
		},
		{
			SourceType.Promote,
			"promote"
		},
		{
			SourceType.MergeRechargeClickSpawner,
			"merge_recharge_click_spawner"
		},
		{
			SourceType.MergeRechargeChest,
			"merge_recharge_chest"
		},
		{
			SourceType.MergeRechargeBubble,
			"merge_recharge_bubble"
		},
		{
			SourceType.MergeRechargeLocker,
			"merge_recharge_locker"
		},
		{
			SourceType.BoughtCellInventory,
			"bought_cell_inventory"
		},
		{
			SourceType.Sellout,
			"sellout"
		},
		{
			SourceType.Duplicate,
			"duplicate"
		},
		{
			SourceType.Roulette,
			"roulette"
		},
		{
			SourceType.EightSlotsMinievents,
			"8SlotsMinievents"
		},
		{
			SourceType.OfferMinievents,
			"offerMinievents"
		},
		{
			SourceType.BundlesMinievents,
			"bundlesMinievents"
		},
		{
			SourceType.BattlePass,
			"bp"
		},
		{
			SourceType.GoldenTicket,
			"golden_ticket"
		},
		{
			SourceType.MiniEventRating,
			"mini_event_rating"
		},
		{
			SourceType.SpawnMergeItem,
			"spawn_merge_item"
		},
		{
			SourceType.MergeShop,
			"merge_shop"
		},
		{
			SourceType.RefreshMergeShop,
			"refresh_merge_shop"
		},
		{
			SourceType.MergePoints,
			"merge_points"
		},
		{
			SourceType.Bubble,
			"bubble"
		},
		{
			SourceType.MiniChainBundles,
			"mini_chain_bundles"
		},
		{
			SourceType.Banner,
			"banner"
		},
		{
			SourceType.Relationship,
			"relationships"
		},
		{
			SourceType.Present,
			"present"
		},
		{
			SourceType.EventBattlePass,
			"event_bp"
		},
		{
			SourceType.Promocode,
			"promocode"
		}
	};

	private Subject<EnergyRecievedAnalyticData> onEnergyRecieved = new Subject<EnergyRecievedAnalyticData>();

	private Subject<EnergySpendAnalyticData> onEnergySpend = new Subject<EnergySpendAnalyticData>();

	private readonly IAmplitudeSender<AmplitudeEvent> amplitude;

	private readonly ICurrencyProcessor currencies;

	private readonly PlayerExperience playerExp;

	private readonly CurrencyAnalyticFactory currencyAnalyticFactory;

	public IObservable<EnergyRecievedAnalyticData> OnEnergyRecieved => (IObservable<EnergyRecievedAnalyticData>)onEnergyRecieved;

	public IObservable<EnergySpendAnalyticData> OnEnergySpend => (IObservable<EnergySpendAnalyticData>)onEnergySpend;

	public CurrencyAmplitudeAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude, ICurrencyProcessor currencies, PlayerExperience playerExp, CurrencyAnalyticFactory currencyAnalyticFactory)
	{
		this.amplitude = amplitude;
		this.currencies = currencies;
		this.playerExp = playerExp;
		this.currencyAnalyticFactory = currencyAnalyticFactory;
	}

	public void SendReceivedAllEvent(LinkedContent linkedContent)
	{
		while (linkedContent != null)
		{
			SendReceivedOneEvent(linkedContent);
			linkedContent = linkedContent.Next();
		}
	}

	public void SendReceivedOneEvent(LinkedContent linkedContent)
	{
		if (linkedContent is CurrencyLinkedContent content)
		{
			SendReceivedEvent(content);
		}
	}

	private void SendReceivedEvent(CurrencyLinkedContent content)
	{
		SendReceivedEvent(content.Currency, content.Quantity, content.AnalyticData.SourceType, content.CompositeIdentificator);
	}

	public void SendReceivedEventDefault(CurrencyType type, int diff, SourceType source, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		diff = ((diff > 0) ? diff : (-diff));
		AmplitudeEvent amplitudeEvent = CreateEvent(type, diff, source, compositeIdentificator);
		if (amplitudeEvent != null)
		{
			SendEvent(amplitudeEvent);
		}
	}

	public void SendSpendEventDefault(CurrencyType type, int diff, SourceType source, ContentType contentType, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		diff = ((diff > 0) ? (-diff) : diff);
		AmplitudeEvent amplitudeEvent = CreateEvent(type, diff, source, compositeIdentificator, contentType);
		if (amplitudeEvent != null)
		{
			SendEvent(amplitudeEvent);
		}
	}

	public void SendReceivedEvent(CurrencyType type, int diff, SourceType source, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (type == CurrencyType.Energy || type == CurrencyType.EventEnergy)
		{
			SendReceivedEventEnergy(type, diff, source, compositeIdentificator);
		}
		else
		{
			SendReceivedEventDefault(type, diff, source, compositeIdentificator);
		}
	}

	public void SendSpentEvent(CurrencyType type, int diff, SourceType source, ContentType contentType, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		if (type == CurrencyType.Energy || type == CurrencyType.EventEnergy)
		{
			SendSpendEventEnergy(type, diff, source, contentType, compositeIdentificator);
		}
		else
		{
			SendSpendEventDefault(type, diff, source, contentType, compositeIdentificator);
		}
	}

	private void SendReceivedEventEnergy(CurrencyType type, int diff, SourceType source, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		onEnergyRecieved.OnNext(new EnergyRecievedAnalyticData(type, diff, source, compositeIdentificator));
	}

	private void SendSpendEventEnergy(CurrencyType type, int diff, SourceType source, ContentType contentType, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator))
	{
		onEnergySpend.OnNext(new EnergySpendAnalyticData(type, diff, source, compositeIdentificator, contentType));
	}

	private AmplitudeEvent CreateEvent(CurrencyType type, int diff, SourceType source, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator), ContentType contentType = ContentType.Main)
	{
		if (!Source.ContainsKey(source))
		{
			return null;
		}
		return currencyAnalyticFactory.Create(type, diff, Source[source], compositeIdentificator, contentType);
	}

	private void SendEvent(AmplitudeEvent _event)
	{
		((IAnalyticSender<AmplitudeEvent>)(object)amplitude).AddEvent(_event);
	}
}

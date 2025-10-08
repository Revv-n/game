using System;
using System.Collections.Generic;
using GreenT;
using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using JetBrains.Annotations;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace StripClub.Model.Shop;

[MementoHolder]
public class SubscriptionLot : ValuableLot<decimal>, ISavableState, IPaymentID
{
	[Serializable]
	public class SubscriptionLotMemento : Memento
	{
		[field: SerializeField]
		public DateTime LastBuy { get; private set; }

		public SubscriptionLotMemento(SubscriptionLot lot)
			: base(lot)
		{
			LastBuy = lot.LastPurchase;
		}

		public virtual GreenT.Data.Memento Save(SubscriptionLot lot)
		{
			Save((Lot)lot);
			LastBuy = lot.LastPurchase;
			return this;
		}
	}

	public class ViewSettings
	{
		private readonly BundlesProviderBase _bundlesProvider;

		public List<int> Focus { get; }

		public string PrefabKey { get; }

		public string CharacterKey { get; }

		public ContentSource ContentSource { get; }

		public bool IsCharacterSpriteOverridden => !string.IsNullOrEmpty(CharacterKey);

		public ViewSettings(BundlesProviderBase bundlesProvider, ContentSource contentSource, string view_prefab, string view_parameters)
		{
			_bundlesProvider = bundlesProvider;
			ContentSource = contentSource;
			PrefabKey = view_prefab;
			CharacterKey = string.Empty;
			Focus = new List<int>();
			if (string.IsNullOrEmpty(view_parameters))
			{
				return;
			}
			string[] array = view_parameters.Split(';', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':', StringSplitOptions.None);
				string text = array2[0];
				if (!(text == "girl"))
				{
					if (!(text == "focus"))
					{
						throw new ArgumentOutOfRangeException("Wrong view_parameter <b>key:\"" + array2[0] + "\"</b>").LogException();
					}
					Focus.Add(int.Parse(array2[1]));
				}
				else
				{
					CharacterKey = array2[1];
				}
			}
		}

		public Sprite GetGirlSprite()
		{
			return _bundlesProvider.TryFindInConcreteBundle<Sprite>(ContentSource, CharacterKey);
		}

		public GameObject GetBackground()
		{
			return _bundlesProvider.TryFindInConcreteBundle<GameObject>(ContentSource, PrefabKey);
		}

		public ListBundleViewData GetBundleData()
		{
			return _bundlesProvider.TryFindInConcreteBundle<ListBundleViewData>(ContentSource, PrefabKey);
		}
	}

	public readonly MonetizationRequestData Data;

	private readonly string uniqueKey;

	public int? ExtensionID { get; }

	public LotFeatures Features { get; }

	public string PaymentID { get; }

	public override Price<decimal> Price { get; }

	public Price<decimal> OldPrice { get; }

	public override bool IsFree => Price.Value == 0m;

	public override string LocalizationKey { get; }

	[CanBeNull]
	public LinkedContent BoosterReward { get; }

	[CanBeNull]
	public LinkedContent RechargeReward { get; }

	public LinkedContent ImmediateReward { get; }

	public override LinkedContent Content { get; }

	public ViewSettings Settings { get; }

	public string NameKey { get; }

	public string TitleKey { get; }

	public string DescriptionKey { get; }

	public Func<DateTime> GetTime { get; set; }

	public DateTime LastPurchase { get; private set; }

	public override string UniqueKey()
	{
		return uniqueKey;
	}

	public SubscriptionLot(SubscriptionLotMapper mapper, ViewSettings viewSettings, ILocker locker, LinkedContent reward, LinkedContent immediateReward, LinkedContent boosterReward, LinkedContent rechargeReward, EqualityLocker<int> countLocker, IPurchaseProcessor purchaseProcessor, decimal price, Func<DateTime> getTime, MonetizationRequestData data, CurrencyType currencyType, CompositeIdentificator compositeIdentificator)
		: base(mapper.id, mapper.monetization_id, mapper.tab_id, mapper.position, mapper.buy_times, locker, countLocker, purchaseProcessor, mapper.source)
	{
		ExtensionID = mapper.extension_id;
		LocalizationKey = $"content.shop.subscription.{base.ID}";
		Features = new LotFeatures(mapper.hot, mapper.best, mapper.sale, mapper.sale_value);
		PaymentID = mapper.lot_id;
		Price = ((price != 0m) ? new Price<decimal>(price, currencyType, compositeIdentificator) : Price<decimal>.Free);
		if (mapper.prev_price.HasValue)
		{
			OldPrice = new Price<decimal>(mapper.prev_price.Value, currencyType, compositeIdentificator);
		}
		Content = reward;
		ImmediateReward = immediateReward;
		BoosterReward = boosterReward;
		RechargeReward = rechargeReward;
		Settings = viewSettings;
		NameKey = mapper.bundle_name;
		TitleKey = mapper.bundle_title;
		DescriptionKey = mapper.bundle_descr;
		LastPurchase = DateTime.MinValue;
		GetTime = getTime;
		Data = data;
		uniqueKey = $"lot.subscription.{base.ID}";
	}

	public override void Initialize()
	{
		base.Initialize();
		LastPurchase = DateTime.MinValue;
	}

	public override bool Purchase()
	{
		if (!base.Purchase())
		{
			return false;
		}
		LastPurchase = GetTime();
		return true;
	}

	public override GreenT.Data.Memento SaveState()
	{
		return new SubscriptionLotMemento(this);
	}

	public override void LoadState(GreenT.Data.Memento memento)
	{
		base.LoadState(memento);
		SubscriptionLotMemento subscriptionLotMemento = (SubscriptionLotMemento)memento;
		LastPurchase = subscriptionLotMemento.LastBuy;
	}
}

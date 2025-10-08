using System;
using System.Collections.Generic;
using GreenT;
using GreenT.AssetBundles;
using GreenT.Data;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Monetization.Webgl;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using UniRx;
using UnityEngine;

namespace StripClub.Model.Shop;

[MementoHolder]
public class BundleLot : ValuableLot<decimal>, ISavableState, IPaymentID
{
	[Serializable]
	public class BundleLotMemento : Memento
	{
		[field: SerializeField]
		public DateTime LastBuy { get; private set; }

		public BundleLotMemento(BundleLot lot)
			: base(lot)
		{
			LastBuy = lot.LastReceivedChangeDate;
		}

		public virtual GreenT.Data.Memento Save(BundleLot lot)
		{
			Save((Lot)lot);
			LastBuy = lot.LastReceivedChangeDate;
			return this;
		}
	}

	public class ViewSettings
	{
		private readonly AssetProvider _assetProvider;

		private readonly BundlesProviderBase _bundlesProvider;

		private readonly ContentSource _contentSource;

		public string PrefabKey { get; }

		public string CharacterKey { get; }

		public List<string> Focus { get; }

		public bool IsCharacterSpriteOverridden => !string.IsNullOrEmpty(CharacterKey);

		public ViewSettings(AssetProvider assetProvider, BundlesProviderBase bundlesProvider, ContentSource contentSource, string view_prefab, string view_parameters)
		{
			_assetProvider = assetProvider;
			_contentSource = contentSource;
			CharacterKey = string.Empty;
			Focus = new List<string>();
			PrefabKey = view_prefab;
			_bundlesProvider = bundlesProvider;
			if (string.IsNullOrEmpty(view_parameters))
			{
				return;
			}
			string[] array = view_parameters.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(':');
				string text = array2[0];
				if (!(text == "girl"))
				{
					if (!(text == "focus"))
					{
						throw new ArgumentOutOfRangeException("Wrong view_parameter <b>key:\"" + array2[0] + "\"</b>").LogException();
					}
					Focus.Add(array2[1]);
				}
				else
				{
					CharacterKey = array2[1];
				}
			}
		}

		public Sprite GetGirlSprite()
		{
			return _assetProvider.FindBundleImageOrFake(_contentSource, CharacterKey, AssetResolveType.Fake);
		}

		public GameObject GetBackground()
		{
			return _bundlesProvider.TryFindInConcreteBundle<GameObject>(ContentSource.Background, PrefabKey);
		}

		public ListBundleViewData GetBundleData()
		{
			return _bundlesProvider.TryFindInConcreteBundle<ListBundleViewData>(_contentSource, PrefabKey);
		}
	}

	private readonly bool _resettable;

	private readonly string[] _resetAfterEnd;

	private ShopBundleMapper _mapper;

	private string uniqueKey;

	public readonly MonetizationRequestData Data;

	private readonly Subject<BundleLot> _onUpdated = new Subject<BundleLot>();

	public LotFeatures Features { get; }

	public string PaymentID { get; }

	public override Price<decimal> Price { get; }

	public Price<decimal> OldPrice { get; }

	public override LinkedContent Content { get; }

	public override bool IsFree => Price.Value == 0m;

	public override string LocalizationKey { get; }

	public ViewSettings Settings { get; }

	public string NameKey { get; }

	public string TitleKey { get; }

	public string DescriptionKey { get; }

	public int GoToBankTab { get; }

	public Func<DateTime> GetTime { get; set; }

	public DateTime LastReceivedChangeDate { get; private set; }

	public IObservable<BundleLot> OnUpdated => _onUpdated;

	public override string UniqueKey()
	{
		return uniqueKey;
	}

	public BundleLot(ShopBundleMapper mapper, ViewSettings viewSettings, LinkedContent reward, ILocker locker, EqualityLocker<int> countLocker, IPurchaseProcessor purchaseProcessor, decimal price, Func<DateTime> getTime, MonetizationRequestData data, CurrencyType currencyType, CompositeIdentificator compositeIdentificator)
		: base(mapper.id, mapper.monetization_id, mapper.tab_id, mapper.position, mapper.buy_times, locker, countLocker, purchaseProcessor, mapper.source)
	{
		LocalizationKey = "content.shop.bundle." + base.ID;
		Features = new LotFeatures(mapper.hot, mapper.best, mapper.sale, mapper.sale_value);
		PaymentID = mapper.lot_id;
		Price = ((price != 0m) ? new Price<decimal>(price, currencyType, compositeIdentificator) : Price<decimal>.Free);
		if (mapper.prev_price.HasValue)
		{
			OldPrice = new Price<decimal>(mapper.prev_price.Value, currencyType, compositeIdentificator);
		}
		Content = reward;
		Settings = viewSettings;
		NameKey = mapper.bundle_name;
		TitleKey = mapper.bundle_title;
		DescriptionKey = mapper.bundle_descr;
		GoToBankTab = mapper.go_to_banktab;
		LastReceivedChangeDate = DateTime.MinValue;
		GetTime = getTime;
		Data = data;
		uniqueKey = "lot.bundle." + base.ID;
		_resettable = mapper.resettable;
		_resetAfterEnd = mapper.reset_after;
		_mapper = mapper;
	}

	public override void Initialize()
	{
		base.Initialize();
		LastReceivedChangeDate = DateTime.MinValue;
	}

	public override bool Purchase()
	{
		if (!base.Purchase())
		{
			return false;
		}
		LastReceivedChangeDate = GetTime();
		_onUpdated?.OnNext(this);
		return true;
	}

	public override GreenT.Data.Memento SaveState()
	{
		return new BundleLotMemento(this);
	}

	public override void LoadState(GreenT.Data.Memento memento)
	{
		BundleLotMemento bundleLotMemento = (BundleLotMemento)memento;
		LastReceivedChangeDate = bundleLotMemento.LastBuy;
		base.LoadState(memento);
	}

	public void ForceReset()
	{
		base.Received = 0;
		LastReceivedChangeDate = GetTime();
		_onUpdated?.OnNext(this);
	}

	protected override void TryResetDailyInfo()
	{
		if (_resettable)
		{
			base.TryResetDailyInfo();
			DateTime date = LastReceivedChangeDate.Date;
			DateTime minValue = DateTime.MinValue;
			if (!(date == minValue.Date) && !(date >= GetTime().Date))
			{
				ForceReset();
			}
		}
	}

	public BundleLot Clone()
	{
		return new BundleLot(_mapper, Settings, Content, base.Locker, base.CountLocker, purchaseProcessor, Price.Value, GetTime, Data, Price.Currency, Price.CompositeIdentificator);
	}

	public BundleLot CloneWithAccessibleLocker()
	{
		AccessibleLocker accessibleLocker = new AccessibleLocker();
		accessibleLocker.Initialize();
		return new BundleLot(_mapper, Settings, Content, accessibleLocker, base.CountLocker, purchaseProcessor, Price.Value, GetTime, Data, Price.Currency, Price.CompositeIdentificator);
	}
}

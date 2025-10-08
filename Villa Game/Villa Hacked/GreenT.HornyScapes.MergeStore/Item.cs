using System;
using GreenT.Types;
using Merge;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeStore;

[Serializable]
public class Item : IItem
{
	private readonly ReactiveProperty<bool> _purchased = new ReactiveProperty<bool>();

	private readonly Subject<Item> _onClear = new Subject<Item>();

	private readonly Subject<ItemBuyRequest> _purchasedRequest = new Subject<ItemBuyRequest>();

	public string Id { get; }

	public GIKey ItemKey { get; }

	public int BasePrice { get; }

	public int SalePrice { get; }

	public CurrencyType CurrencyType { get; }

	public int Sale { get; }

	public int SaleID { get; }

	public int Amount { get; }

	public ContentType ContentType { get; }

	public int SaleTierDifference { get; set; }

	public SectionType Section { get; }

	public IReactiveProperty<bool> Purchased => (IReactiveProperty<bool>)(object)_purchased;

	public IObservable<IItem> OnClear => (IObservable<IItem>)_onClear;

	public IObservable<ItemBuyRequest> PurchasedRequest => (IObservable<ItemBuyRequest>)_purchasedRequest;

	public void TryBuy()
	{
		throw new NotImplementedException();
	}

	public Item(ItemSave saveItem)
		: this(saveItem.Id, saveItem.ItemKey, saveItem.BsePrice, saveItem.SalePrice, saveItem.CurrencyType, saveItem.Sale, saveItem.SaleID, saveItem.Amount, saveItem.ContentType, saveItem.SaleTierDifference, saveItem.Section, saveItem.Purchased)
	{
	}

	public Item(string id, GIKey itemKey, int bsePrice, int salePrice, CurrencyType currencyType, int sale, int saleID, int amount, ContentType contentType, int saleTierDifference, SectionType section, bool purchased = false)
	{
		Id = id;
		ItemKey = itemKey;
		BasePrice = bsePrice;
		SalePrice = salePrice;
		CurrencyType = currencyType;
		Sale = sale;
		SaleID = saleID;
		Amount = amount;
		_purchased.Value = purchased;
		ContentType = contentType;
		SaleTierDifference = saleTierDifference;
		Section = section;
	}

	public void TryBuy(ButtonPosition buttonPosition)
	{
		if (!_purchased.Value)
		{
			_purchasedRequest.OnNext(new ItemBuyRequest(this, buttonPosition));
		}
	}

	public void Buy()
	{
		_purchased.Value = true;
	}

	public void Clear()
	{
		_onClear.OnNext(this);
	}
}

using System;
using GreenT.Types;
using Merge;
using StripClub.Model;

namespace GreenT.HornyScapes.MergeStore;

[Serializable]
public class ItemSave
{
	public string Id;

	public GIKey ItemKey;

	public int BsePrice;

	public int SalePrice;

	public CurrencyType CurrencyType;

	public int Sale;

	public int SaleID;

	public int Amount;

	public bool Purchased;

	public ContentType ContentType;

	public int SaleTierDifference;

	public SectionType Section;

	public ItemSave(Item storeItem)
	{
		Id = storeItem.Id;
		ItemKey = storeItem.ItemKey;
		BsePrice = storeItem.BasePrice;
		SalePrice = storeItem.SalePrice;
		CurrencyType = storeItem.CurrencyType;
		Sale = storeItem.Sale;
		SaleID = storeItem.SaleID;
		Amount = storeItem.Amount;
		Purchased = storeItem.Purchased.Value;
		ContentType = storeItem.ContentType;
		SaleTierDifference = storeItem.SaleTierDifference;
		Section = storeItem.Section;
	}
}

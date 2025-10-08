using System;
using GreenT.Types;
using Merge;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.MergeStore;

public interface IItem
{
	string Id { get; }

	GIKey ItemKey { get; }

	int BasePrice { get; }

	int SalePrice { get; }

	CurrencyType CurrencyType { get; }

	int Sale { get; }

	int SaleID { get; }

	int Amount { get; }

	ContentType ContentType { get; }

	int SaleTierDifference { get; set; }

	IReactiveProperty<bool> Purchased { get; }

	IObservable<IItem> OnClear { get; }

	void TryBuy(ButtonPosition buttonPosition);
}

using System;
using StripClub.Model;
using StripClub.Model.Data;
using TMPro;

namespace GreenT.HornyScapes.MergeStore;

public class RefreshSectionButtonView : ButtonViewBase<StoreSection>
{
	protected override void SetPrice()
	{
		TMP_Text price = _price;
		int refreshPrice = base.Source.RefreshPrice;
		price.text = refreshPrice.ToString();
	}

	protected override Cost GetPrice()
	{
		return new Cost(base.Source.RefreshPrice, base.Source.CurrencyType);
	}

	protected override CurrencyType GetCurrencyType()
	{
		return base.Source.CurrencyType;
	}

	protected override int GetTargetPrice()
	{
		return base.Source.RefreshPrice;
	}

	protected override IObservable<StoreSection> GetOnClear()
	{
		return base.Source.OnClear;
	}

	protected override void EmitBuyRequest()
	{
		base.Source.TryRefresh();
	}
}

using System;
using StripClub.Model;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.MergeStore;

public class RefreshSectionButtonView : ButtonViewBase<StoreSection>
{
	protected override void SetPrice()
	{
		_price.text = base.Source.RefreshPrice.ToString();
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

using System;
using StripClub.Model;
using StripClub.Model.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace GreenT.HornyScapes.MergeStore;

public class SaleItemButtonView : ButtonViewBase<IItem>
{
	[FormerlySerializedAs("_priceSale")]
	[SerializeField]
	private TMP_Text _oldPrice;

	[SerializeField]
	private GameObject _saleRoot;

	private ButtonPosition _position;

	protected override void SetPrice()
	{
		if (_saleRoot != null && _oldPrice != null)
		{
			_oldPrice.text = base.Source.BasePrice.ToString();
			_saleRoot.gameObject.SetActive(base.Source.BasePrice != base.Source.SalePrice);
		}
		_price.text = base.Source.SalePrice.ToString();
	}

	public void SetPosition(ButtonPosition position)
	{
		_position = position;
	}

	protected override Cost GetPrice()
	{
		return new Cost(base.Source.SalePrice, base.Source.CurrencyType);
	}

	protected override CurrencyType GetCurrencyType()
	{
		return base.Source.CurrencyType;
	}

	protected override int GetTargetPrice()
	{
		return base.Source.SalePrice;
	}

	protected override IObservable<IItem> GetOnClear()
	{
		return base.Source.OnClear;
	}

	protected override void EmitBuyRequest()
	{
		base.Source.TryBuy(_position);
	}
}

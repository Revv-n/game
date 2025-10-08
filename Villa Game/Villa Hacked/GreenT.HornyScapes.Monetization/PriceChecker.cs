using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UnityEngine;

namespace GreenT.HornyScapes.Monetization;

public class PriceChecker
{
	private readonly IRegionPriceResolver _priceResolver;

	private readonly string _priceType = "real:0";

	public PriceChecker(IRegionPriceResolver priceResolver)
	{
		_priceResolver = priceResolver;
	}

	public void ValidatePrice(ShopBundleMapper mapper)
	{
		if (!(mapper.price_resource != _priceType))
		{
			Validate(mapper.GetType(), mapper.id, mapper.price, mapper.lot_id);
		}
	}

	public void ValidatePrice(GemShopMapper mapper)
	{
		if (!(mapper.price_resource != _priceType))
		{
			Validate(mapper.GetType(), mapper.id, mapper.price, mapper.lot_id);
		}
	}

	public void ValidatePrice(SubscriptionLotMapper mapper)
	{
		if (!(mapper.price_resource != _priceType))
		{
			Validate(mapper.GetType(), mapper.id, mapper.price, mapper.lot_id);
		}
	}

	public void ValidatePrice(Type mapperType, int id, decimal price, string lot_id)
	{
		Validate(mapperType, id, price, lot_id);
	}

	private void Validate(Type mapperType, int id, decimal price, string lot_id)
	{
		if (_priceResolver.Prices.All((KeyValuePair<string, decimal> pair) => pair.Key != lot_id))
		{
			Debug.LogError("non-existent or not added to validator \"lot_id\". lot_id = " + lot_id);
		}
		else if (_priceResolver.Prices.All((KeyValuePair<string, decimal> pair) => pair.Value != price))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("The price must be one of: ");
			string value = string.Join(", ", _priceResolver.Prices.Values.OrderBy((decimal k) => k));
			stringBuilder.Append(value);
			Debug.LogError($"\"price\" is incorrectly specified in {mapperType} id = {id} | price = {price}\n" + stringBuilder);
		}
		else if (_priceResolver.Prices[lot_id] != price)
		{
			Debug.LogError($"\"lot_id\" is incorrectly specified in {mapperType} id = {id} | price = {price} | lot_id = {lot_id}");
		}
	}
}

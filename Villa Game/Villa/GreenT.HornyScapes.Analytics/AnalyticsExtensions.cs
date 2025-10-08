using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Monetization;
using StripClub.Model.Shop;
using UnityEngine.Networking;

namespace GreenT.HornyScapes.Analytics;

public static class AnalyticsExtensions
{
	private static readonly Dictionary<Type, string> keyPostfixItemId = new Dictionary<Type, string>
	{
		{
			typeof(GemShopLot),
			"_G"
		},
		{
			typeof(BundleLot),
			"_B"
		},
		{
			typeof(SummonLot),
			"_S"
		},
		{
			typeof(SubscriptionLot),
			"_SUB"
		}
	};

	public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		return new UnityWebRequestAwaiter(asyncOp);
	}

	public static string GetItemIdWithPostfix(SteamPaymentData data, Lot lot)
	{
		return GetItemIdWithPostfix(data.item_id, lot);
	}

	public static string GetItemIdWithPostfix(PaymentIntentData data, Lot lot)
	{
		return GetItemIdWithPostfix(data.ItemID.ToString(), lot);
	}

	public static string GetItemIdWithPostfix(Transaction data, Lot lot)
	{
		return GetItemIdWithPostfix(data.monetizationID.ToString(), lot);
	}

	private static string GetItemIdWithPostfix(string itemId, Lot lot)
	{
		string postfix = GetPostfix(lot);
		return itemId + postfix;
	}

	public static string GetPostfix(Lot lot)
	{
		Type type = lot.GetType();
		if (!keyPostfixItemId.ContainsKey(type))
		{
			throw new Exception().SendException($"Can't get  analytic postfix for {lot.ID} type {lot.GetType().Name}");
		}
		return keyPostfixItemId[lot.GetType()];
	}
}

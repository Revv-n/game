using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes;

public class PurchaseProcessor : IPurchaseProcessor
{
	private readonly PlayerStats playerStats;

	private readonly ContentStorageProvider contentStorageProvider;

	private readonly ICurrencyProcessor currencyProcessor;

	public PurchaseProcessor(ICurrencyProcessor currencyProcessor, PlayerStats playerStats, ContentStorageProvider contentStorageProvider)
	{
		this.currencyProcessor = currencyProcessor;
		this.playerStats = playerStats;
		this.contentStorageProvider = contentStorageProvider;
	}

	public bool TryBuy<T>(LinkedContent content, Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		if (!price.Currency.IsRealCurrency() && !SpendPlayersMoney(price))
		{
			return false;
		}
		if (IsPurchaseForRealCurrency(price))
		{
			IncrementNumberPurchasesRealCurrency();
		}
		if (content is LootboxLinkedContent lootboxLinkedContent)
		{
			contentStorageProvider.TrySetStoredContent(content, lootboxLinkedContent.Lootbox.ContentType);
		}
		else
		{
			contentStorageProvider.TrySetStoredContent(content);
		}
		if (contentStorageProvider.HasStoredContent())
		{
			contentStorageProvider.AddToPlayer();
		}
		return true;
	}

	public void AddAllContentToPlayer(LinkedContent content)
	{
		content?.AddToPlayer();
	}

	private bool SpendPlayersMoney<T>(Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		if (PriceIsFree(price))
		{
			return true;
		}
		if (!TryParse(price.Value, out var value))
		{
			return false;
		}
		if (value < 0)
		{
			return NegativeLogException(value);
		}
		if (!currencyProcessor.IsEnough(price.Currency, value, price.CompositeIdentificator) || !currencyProcessor.TrySpent(price.Currency, value, price.CompositeIdentificator))
		{
			return false;
		}
		return true;
	}

	private bool TryParse<T>(T target, out int value)
	{
		if (int.TryParse(target.ToString(), out value))
		{
			return true;
		}
		string name = GetType().Name;
		T val = target;
		new ArgumentException(name + ": Can't convert price:\"" + val?.ToString() + "\" to int").LogException();
		return false;
	}

	private void IncrementNumberPurchasesRealCurrency()
	{
		ReactiveProperty<int> paymentCount = playerStats.PaymentCount;
		int value = paymentCount.Value + 1;
		paymentCount.Value = value;
	}

	private static bool IsPurchaseForRealCurrency<T>(Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		if (price.Currency.IsRealCurrency())
		{
			return !PriceIsFree(price);
		}
		return false;
	}

	private static bool PriceIsFree<T>(Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		return price.Equals(Price<T>.Free);
	}

	private static bool NegativeLogException(int value)
	{
		new ArgumentException($"Negative values cannot be used | value = {value}").LogException();
		return false;
	}
}

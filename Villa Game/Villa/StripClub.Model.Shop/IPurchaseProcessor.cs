using System;

namespace StripClub.Model.Shop;

public interface IPurchaseProcessor
{
	bool TryBuy<T>(LinkedContent content, Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable;

	void AddAllContentToPlayer(LinkedContent content);
}

namespace GreenT.Multiplier;

public class AdjustedMultiplierDictionary<TKey, KEntity> : MultiplierDictionaryFacade<TKey, KEntity>, IMultiplierTotalContainer<TKey>, IMultiplierContainer<TKey> where KEntity : ICompositeMultiplier, new()
{
	public KEntity Total { get; }

	public AdjustedMultiplierDictionary()
	{
		Total = new KEntity();
	}

	public KEntity TotalByKey(TKey key)
	{
		KEntity result = new KEntity();
		result.Add(Total);
		result.Add(base[key]);
		return result;
	}

	public void AddToTotal(IMultiplier multiplier)
	{
		Total.Add(multiplier);
	}

	public void RemoveFromTotal(IMultiplier multiplier)
	{
		Total.Remove(multiplier);
	}
}

namespace StripClub.Model;

public class SumPriceAverageLocker : Locker
{
	protected readonly decimal from;

	protected readonly decimal to;

	public SumPriceAverageLocker(decimal from, decimal to)
	{
		this.from = from;
		this.to = to;
	}

	public void Set(decimal current)
	{
		isOpen.Value = current >= from && current < to;
	}
}

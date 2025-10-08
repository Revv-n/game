namespace GreenT.Multiplier;

public class PercentMultiplier : Multiplier
{
	public PercentMultiplier(double value)
		: base(value)
	{
	}

	public override void Set(double value)
	{
		value = ConvertPercent(value);
		base.Set(value);
	}

	private double ConvertPercent(double values)
	{
		return (100.0 - values) / 100.0;
	}
}

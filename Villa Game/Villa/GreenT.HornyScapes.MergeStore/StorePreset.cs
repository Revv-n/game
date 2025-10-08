namespace GreenT.HornyScapes.MergeStore;

public class StorePreset
{
	public readonly StoreSection Regular;

	public readonly StoreSection Premium;

	public StorePreset(StoreSection regular, StoreSection premium)
	{
		Regular = regular;
		Premium = premium;
	}
}

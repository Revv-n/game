namespace GreenT.Multiplier;

public class PlayerMultipliers
{
	public readonly TotalMultiplier Total;

	public readonly IOrderedCompositeMultiplier ClickPower;

	public readonly ICompositeMultiplier KeyDropChance;

	public readonly IOrderedCompositeMultiplier Prestige;

	public readonly IOrderedCompositeMultiplier InitSoftMoneyMultiplier;

	public PlayerMultipliers(TotalMultiplier total, IOrderedCompositeMultiplier clickPower, ICompositeMultiplier keyDropChance, IOrderedCompositeMultiplier prestige, IOrderedCompositeMultiplier initSoftMoneyMultiplier)
	{
		ClickPower = clickPower;
		Total = total;
		KeyDropChance = keyDropChance;
		Prestige = prestige;
		InitSoftMoneyMultiplier = initSoftMoneyMultiplier;
	}
}

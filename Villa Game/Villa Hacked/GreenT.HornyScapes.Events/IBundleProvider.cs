namespace GreenT.HornyScapes.Events;

public interface IBundleProvider<out TBundleData> : IRewardHolder where TBundleData : IBundleData
{
	TBundleData Bundle { get; }
}

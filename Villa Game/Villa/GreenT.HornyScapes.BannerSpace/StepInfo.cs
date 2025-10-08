namespace GreenT.HornyScapes.BannerSpace;

public struct StepInfo
{
	public readonly int Step;

	public readonly int LegendaryStep;

	public readonly int MainStep;

	public StepInfo(BannerSaveData bannerSaveData)
	{
		Step = bannerSaveData.Step;
		LegendaryStep = bannerSaveData.LegendaryStep;
		MainStep = bannerSaveData.MainStep;
	}
}

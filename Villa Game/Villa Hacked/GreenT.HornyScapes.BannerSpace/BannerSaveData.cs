using System;

namespace GreenT.HornyScapes.BannerSpace;

[Serializable]
public class BannerSaveData
{
	public const int MIN_STEP = 1;

	public string Source;

	public int Step;

	public int LegendaryStep;

	public int MainStep;

	public BannerSaveData(string source)
	{
		Source = source;
		ResetMainStep();
		ResetLegendaryStep();
	}

	public void ResetMainStep()
	{
		MainStep = 1;
	}

	public void ResetLegendaryStep()
	{
		LegendaryStep = 1;
	}

	public void AddStep()
	{
		Step++;
	}

	public void AddLegendaryStep()
	{
		LegendaryStep++;
	}

	public void AddMainStep()
	{
		MainStep++;
	}
}

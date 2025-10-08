namespace GreenT.Steam.Achievements.Goals;

public class MergeData
{
	public readonly AchievementDTO Achievement;

	public readonly int TargetValue;

	public MergeData(AchievementDTO achievement, int targetValue)
	{
		Achievement = achievement;
		TargetValue = targetValue;
	}
}

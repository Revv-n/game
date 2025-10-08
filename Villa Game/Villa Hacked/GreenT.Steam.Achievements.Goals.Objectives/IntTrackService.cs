namespace GreenT.Steam.Achievements.Goals.Objectives;

public class IntTrackService : TrackService<int>
{
	public IntTrackService(AchievementService achievementService, AchievementDTO achievement, string statsKey, int targetValue)
		: base(achievementService, achievement, statsKey, targetValue)
	{
	}
}

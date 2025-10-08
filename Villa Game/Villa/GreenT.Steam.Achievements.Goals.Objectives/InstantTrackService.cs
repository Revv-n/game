namespace GreenT.Steam.Achievements.Goals.Objectives;

public class InstantTrackService : TrackService<bool>
{
	public InstantTrackService(AchievementService achievementService, AchievementDTO achievement)
		: base(achievementService, achievement, "", targetValue: true)
	{
	}
}

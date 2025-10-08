using StripClub.Model;

namespace GreenT.Steam.Achievements.Goals;

public class FinishTutorialTrackService : LockerTrackService
{
	public FinishTutorialTrackService(AchievementService achievementService, AchievementDTO achievement, ILocker locker)
		: base(achievementService, achievement, locker)
	{
	}
}

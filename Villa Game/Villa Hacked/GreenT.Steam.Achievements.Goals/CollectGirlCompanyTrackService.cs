using StripClub.Model;

namespace GreenT.Steam.Achievements.Goals;

public class CollectGirlCompanyTrackService : LockerTrackService
{
	public CollectGirlCompanyTrackService(AchievementService achievementService, AchievementDTO achievement, ILocker locker)
		: base(achievementService, achievement, locker)
	{
	}
}

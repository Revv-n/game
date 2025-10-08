using StripClub.Model;

namespace GreenT.Steam.Achievements.Goals;

public class FinishRoomTrackService : LockerTrackService
{
	public FinishRoomTrackService(AchievementService achievementService, AchievementDTO achievement, ILocker locker)
		: base(achievementService, achievement, locker)
	{
	}
}

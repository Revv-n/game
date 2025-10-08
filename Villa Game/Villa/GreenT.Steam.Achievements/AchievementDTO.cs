namespace GreenT.Steam.Achievements;

public class AchievementDTO
{
	public AchievementIdType AchievementIdType;

	public string Name;

	public string Description;

	public bool Achieved;

	public AchievementDTO(AchievementIdType achievementIdType, string name, string desc)
	{
		AchievementIdType = achievementIdType;
		Name = name;
		Description = desc;
		Achieved = false;
	}
}

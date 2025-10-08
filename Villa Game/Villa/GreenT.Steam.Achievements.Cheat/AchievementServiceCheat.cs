using System.Linq;
using Steamworks;
using UnityEngine;
using Zenject;

namespace GreenT.Steam.Achievements.Cheat;

public class AchievementServiceCheat : MonoBehaviour
{
	[Header("Runtime only")]
	public AchievementIdType idType;

	public StatsType statsType;

	private AchievementService _achievementService;

	private AchievementProvider _provider;

	[Inject]
	private void Init(AchievementService achievementService, AchievementProvider provider)
	{
		_achievementService = achievementService;
		_provider = provider;
	}

	[EditorButton]
	public void PrintIntStat()
	{
		SteamUserStats.GetStat(statsType.ToString(), out int pData);
		Debug.Log($"{statsType.ToString()} value = {pData}");
	}

	[EditorButton]
	public void UnlockSelectedAchievement()
	{
		AchievementDTO achievementDto = _provider.Collection.FirstOrDefault((AchievementDTO _ach) => _ach.AchievementIdType == idType);
		_achievementService.UnlockAchievement(achievementDto);
	}

	[EditorButton]
	public void ClearAll()
	{
		SteamUserStats.ResetAllStats(bAchievementsToo: true);
	}

	[EditorButton]
	public void ClearSelectedAchievement()
	{
		SteamUserStats.ClearAchievement(idType.ToString());
	}
}

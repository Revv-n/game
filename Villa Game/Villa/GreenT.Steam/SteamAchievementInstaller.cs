using GreenT.Steam.Achievements;
using GreenT.Steam.Achievements.Callbacks;
using GreenT.Steam.Achievements.Goals;
using Zenject;

namespace GreenT.Steam;

public class SteamAchievementInstaller : Installer<SteamAchievementInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<AchievementEntryPoint>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AchievementService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AchievementProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AchievementCallbackProvider>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<AchievementStats>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TrackersFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<BindCallbackService>().AsSingle();
		BindSingletonTrackService();
	}

	private void BindSingletonTrackService()
	{
		base.Container.BindInterfacesAndSelfTo<MergeTrackService>().AsSingle().WithArguments(StatsType.STAT_MERGE.ToString());
	}
}

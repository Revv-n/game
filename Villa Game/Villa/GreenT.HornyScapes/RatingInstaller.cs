using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Settings;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RatingInstaller : Installer<RatingInstaller>
{
	public override void InstallBindings()
	{
		BindRequests();
		BindServices();
		BindFactories();
		BindInitializers();
		BindManagers();
	}

	private void BindManagers()
	{
		base.Container.BindInterfacesAndSelfTo<TournamentPointsManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PowerManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MatchmakingManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingDataManager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingControllerManager>().AsSingle();
	}

	private void BindInitializers()
	{
		base.Container.BindInterfacesAndSelfTo<TournamentPointsStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<TournamentPointsMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PowerStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<PowerMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MatchmakingStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<MatchmakingMapper>>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingStructureInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<RatingMapper>>().AsSingle();
	}

	private void BindServices()
	{
		base.Container.BindInterfacesAndSelfTo<RatingService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingRewardService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<TournamentPointsStorage>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingPlayerPowerUpdateController>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingAnalytic>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.BindInterfacesAndSelfTo<MatchmakingFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<RatingDataFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MiniEventRatingControllerFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventRatingControllerFactory>().AsSingle();
	}

	private void BindRequests()
	{
		base.Container.Bind<AppIdHolder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<FakeScoreboardRequest>().AsSingle();
		base.Container.BindPostRequest<LeaderboardRequest>(PostRequestType.GetLeaderboard);
		base.Container.BindPostRequest<LeaderboardAddPointsRequest>(PostRequestType.AddLeaderboardPoints);
		base.Container.BindPostRequest<RegistrationRequest>(PostRequestType.LeaderboardRegistration);
		base.Container.Bind<string>().FromResolveGetter((IProjectSettings _settings) => (_settings.RequestUrlResolver as RequestSettings).RatingAppName).WhenInjectedInto<AppIdHolder>();
	}
}

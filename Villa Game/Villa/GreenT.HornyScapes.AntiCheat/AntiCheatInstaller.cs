using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.AntiCheat;

public sealed class AntiCheatInstaller : Installer<AntiCheatInstaller>
{
	public override void InstallBindings()
	{
		BindRequests();
	}

	private void BindRequests()
	{
		base.Container.BindPostRequest<UpdateCheatingStatusRequest>(PostRequestType.CheaterRequest);
		base.Container.BindInterfacesAndSelfTo<CheatEngineSearchService>().AsSingle();
		Bind<CheaterAnalytic>();
	}

	private void Bind<T>() where T : BaseAnalytic
	{
		base.Container.BindInterfacesAndSelfTo<T>().AsSingle().OnInstantiated(delegate(InjectContext context, T obj)
		{
			base.Container.Resolve<AnalyticSystemManager>().Add(obj);
		})
			.NonLazy();
	}
}

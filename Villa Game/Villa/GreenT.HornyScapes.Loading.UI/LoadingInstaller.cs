using GreenT.HornyScapes.UI;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Loading.UI;

public class LoadingInstaller : MonoInstaller<LoadingInstaller>
{
	[SerializeField]
	private LoadingScreenFactory loadingScreenFactory;

	public override void InstallBindings()
	{
		base.Container.Bind<LoadingScreenFactory>().FromInstance(loadingScreenFactory).AsTransient();
		base.Container.Bind(typeof(ILoadingScreen), typeof(LoadingProgressBar)).To<LoadingProgressBar>().FromResolveGetter((LoadingScreenFactory _factory) => _factory.Create())
			.AsSingle()
			.NonLazy();
		base.Container.Bind<SwitchingPhrases>().FromResolveGetter((LoadingProgressBar _loadingScreen) => _loadingScreen.gameObject.GetComponent<SwitchingPhrases>());
	}
}

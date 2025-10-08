using System;
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
		((FromBinderGeneric<LoadingScreenFactory>)(object)((MonoInstallerBase)this).Container.Bind<LoadingScreenFactory>()).FromInstance(loadingScreenFactory).AsTransient();
		((NonLazyBinder)((ConcreteBinderNonGeneric)((MonoInstallerBase)this).Container.Bind(new Type[2]
		{
			typeof(ILoadingScreen),
			typeof(LoadingProgressBar)
		})).To<LoadingProgressBar>().FromResolveGetter<LoadingScreenFactory, LoadingProgressBar>((Func<LoadingScreenFactory, LoadingProgressBar>)((LoadingScreenFactory _factory) => _factory.Create())).AsSingle()).NonLazy();
		((FromBinderGeneric<SwitchingPhrases>)(object)((MonoInstallerBase)this).Container.Bind<SwitchingPhrases>()).FromResolveGetter<LoadingProgressBar>((Func<LoadingProgressBar, SwitchingPhrases>)((LoadingProgressBar _loadingScreen) => _loadingScreen.gameObject.GetComponent<SwitchingPhrases>()));
	}
}

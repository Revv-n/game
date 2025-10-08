using GreenT.HornyScapes;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class RouletteLotViewInstaller : MonoInstaller<RouletteLotViewInstaller>
{
	[SerializeField]
	private RouletteBankLotViewFactory _viewFactory;

	[SerializeField]
	private Transform _viewManagerRoot;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteBankLotViewFactory>().FromInstance((object)_viewFactory);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RouletteBankSummonViewManager>()).FromComponentOn(_viewManagerRoot.gameObject).AsSingle();
	}
}

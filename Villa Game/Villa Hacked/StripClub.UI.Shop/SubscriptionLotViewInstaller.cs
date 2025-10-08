using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionLotViewInstaller : MonoInstaller
{
	[SerializeField]
	private SubscriptionLotViewFactory _viewFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionLotViewFactory>().FromInstance((object)_viewFactory);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionLotView.Manager>()).FromNewComponentOn(((Component)this).gameObject).AsSingle();
	}
}

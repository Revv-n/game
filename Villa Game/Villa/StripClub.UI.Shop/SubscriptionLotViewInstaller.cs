using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SubscriptionLotViewInstaller : MonoInstaller
{
	[SerializeField]
	private SubscriptionLotViewFactory _viewFactory;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SubscriptionLotViewFactory>().FromInstance(_viewFactory);
		base.Container.BindInterfacesAndSelfTo<SubscriptionLotView.Manager>().FromNewComponentOn(base.gameObject).AsSingle();
	}
}

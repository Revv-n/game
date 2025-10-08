using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class LotViewInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private ContainerViewFactory viewFactory;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<ContainerViewFactory>().FromInstance(viewFactory);
		base.Container.BindInterfacesAndSelfTo<LotView.LotContainerManager>().FromNewComponentOn(base.gameObject).AsSingle();
	}
}

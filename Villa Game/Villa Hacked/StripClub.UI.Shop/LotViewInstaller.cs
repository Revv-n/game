using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class LotViewInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private ContainerViewFactory viewFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<ContainerViewFactory>().FromInstance((object)viewFactory);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<LotView.LotContainerManager>()).FromNewComponentOn(((Component)(object)this).gameObject).AsSingle();
	}
}

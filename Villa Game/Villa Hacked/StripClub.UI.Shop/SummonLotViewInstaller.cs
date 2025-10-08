using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SummonLotViewInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private SummonLotViewFactory viewFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SummonLotViewFactory>().FromInstance((object)viewFactory);
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SummonLotView.Manager>()).FromNewComponentOn(((Component)(object)this).gameObject).AsSingle();
	}
}

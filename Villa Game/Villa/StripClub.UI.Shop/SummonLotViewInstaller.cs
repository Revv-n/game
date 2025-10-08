using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public class SummonLotViewInstaller : MonoInstaller<LotViewInstaller>
{
	[SerializeField]
	private SummonLotViewFactory viewFactory;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SummonLotViewFactory>().FromInstance(viewFactory);
		base.Container.BindInterfacesAndSelfTo<SummonLotView.Manager>().FromNewComponentOn(base.gameObject).AsSingle();
	}
}

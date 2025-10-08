using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class ShopBundlesInstaller : MonoInstaller
{
	[SerializeField]
	private SimpleBundleLotView bundlePrefab;

	public override void InstallBindings()
	{
		((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<SimpleBundleLotView>()).FromComponentInNewPrefab((Object)bundlePrefab);
	}
}

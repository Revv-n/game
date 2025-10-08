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
		base.Container.BindIFactory<SimpleBundleLotView>().FromComponentInNewPrefab(bundlePrefab);
	}
}

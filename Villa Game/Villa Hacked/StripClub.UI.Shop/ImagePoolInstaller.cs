using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class ImagePoolInstaller : MonoInstaller<ImagePoolInstaller>
{
	[SerializeField]
	private ImagePool imagePool;

	[SerializeField]
	private Image imagePrefab;

	public override void InstallBindings()
	{
		((FromBinderGeneric<ImagePool>)(object)((MonoInstallerBase)this).Container.Bind<ImagePool>()).FromInstance(imagePool).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<Image>()).FromComponentInNewPrefab((Object)imagePrefab)).AsSingle();
	}
}

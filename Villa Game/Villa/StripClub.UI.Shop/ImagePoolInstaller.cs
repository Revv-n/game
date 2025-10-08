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
		base.Container.Bind<ImagePool>().FromInstance(imagePool).AsSingle();
		base.Container.BindIFactory<Image>().FromComponentInNewPrefab(imagePrefab).AsSingle();
	}
}

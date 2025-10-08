using StripClub.Gallery.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Gallery.UI;

public class GalleryUIInstaller : MonoInstaller
{
	[SerializeField]
	private FullscreenPhoto fullscreenPhoto;

	[SerializeField]
	private Transform mediaContainer;

	[SerializeField]
	private LockedView lockedViewPrefab;

	[SerializeField]
	private PhotoView photoViewPrefab;

	public override void InstallBindings()
	{
		base.Container.Bind<FullscreenPhoto>().FromInstance(fullscreenPhoto).AsSingle();
		base.Container.BindIFactory<LockedView>().FromComponentInNewPrefab(lockedViewPrefab).UnderTransform(mediaContainer)
			.WhenInjectedInto<LockedView.Manager>();
		base.Container.Bind<LockedView.Manager>().FromNewComponentOn(mediaContainer.gameObject).AsSingle();
		base.Container.Bind<Transform>().FromInstance(mediaContainer).WhenInjectedInto<PhotoView.Factory>();
		base.Container.Bind<PhotoView>().FromInstance(photoViewPrefab).WhenInjectedInto<PhotoView.Factory>();
		base.Container.BindInterfacesTo<PhotoView.Factory>().AsCached();
		base.Container.Bind<PhotoView.Manager>().FromNewComponentOn(mediaContainer.gameObject).AsSingle();
	}
}

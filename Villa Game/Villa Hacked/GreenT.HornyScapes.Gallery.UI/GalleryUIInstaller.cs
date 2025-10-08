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
		((FromBinderGeneric<FullscreenPhoto>)(object)((MonoInstallerBase)this).Container.Bind<FullscreenPhoto>()).FromInstance(fullscreenPhoto).AsSingle();
		((ConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<LockedView>()).FromComponentInNewPrefab((Object)lockedViewPrefab)).UnderTransform(mediaContainer)).WhenInjectedInto<LockedView.Manager>();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<LockedView.Manager>()).FromNewComponentOn(mediaContainer.gameObject).AsSingle();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<Transform>)(object)((MonoInstallerBase)this).Container.Bind<Transform>()).FromInstance(mediaContainer)).WhenInjectedInto<PhotoView.Factory>();
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<PhotoView>)(object)((MonoInstallerBase)this).Container.Bind<PhotoView>()).FromInstance(photoViewPrefab)).WhenInjectedInto<PhotoView.Factory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<PhotoView.Factory>()).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.Bind<PhotoView.Manager>()).FromNewComponentOn(mediaContainer.gameObject).AsSingle();
	}
}

using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPreviewSceneInstaller : MonoInstaller<OfferPreviewSceneInstaller>
{
	[SerializeField]
	private PreviewInstaller offerPreviewPrefab;

	[SerializeField]
	private Transform offerPreviewParent;

	public override void InstallBindings()
	{
		((NonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<PreviewInstaller>()).FromComponentInNewPrefab((Object)(object)offerPreviewPrefab)).UnderTransform(offerPreviewParent).AsSingle()).NonLazy();
	}
}

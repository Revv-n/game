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
		base.Container.Bind<PreviewInstaller>().FromComponentInNewPrefab(offerPreviewPrefab).UnderTransform(offerPreviewParent)
			.AsSingle()
			.NonLazy();
	}
}

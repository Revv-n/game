using System.Collections.Generic;
using GreenT.HornyScapes.Offer;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using ModestTree;
using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class OfferPanelInstaller : MonoInstaller<OfferPanelInstaller>
{
	[SerializeField]
	private OfferPreview previewPrefab;

	[SerializeField]
	private Transform container;

	[SerializeField]
	private OfferPreviewController previewController;

	[SerializeField]
	private PushController offersPushController;

	[SerializeField]
	private PushController eventOffersPushController;

	[SerializeField]
	private PushController bpOffersPushController;

	public override void InstallBindings()
	{
		Assert.IsNotNull(previewPrefab);
		Assert.IsNotNull(container);
		Assert.IsNotNull(previewController);
		Assert.IsNotNull(offersPushController);
		Assert.IsNotNull(eventOffersPushController);
		Assert.IsNotNull(bpOffersPushController);
		base.Container.BindIFactory<OfferPreview>().FromComponentInNewPrefab(previewPrefab).UnderTransform(container)
			.AsCached();
		base.Container.BindInterfacesTo<OfferPreviewManager>().FromNewComponentOn(container.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfferPreviewController>().FromInstance(previewController).AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfferUnlockController>().FromNewComponentOn(previewController.gameObject).AsSingle();
		base.Container.BindSignal<LotBoughtSignal>().ToMethod((OfferPreviewController x) => x.OnLotBoughtRequest).FromResolve();
		BindScreenIndicator<MainContentScreenIndicator>(ContentType.Main);
		BindScreenIndicator<EventMergeScreenIndicator>(ContentType.Event);
		BindScreenIndicator<MainContentScreenIndicator>(ContentType.BattlePass);
		base.Container.Bind<PushController>().WithId(offersPushController.ContentType).FromInstance(offersPushController)
			.AsCached();
		base.Container.Bind<PushController>().WithId(eventOffersPushController.ContentType).FromInstance(eventOffersPushController)
			.AsCached();
		base.Container.Bind<PushController>().WithId(bpOffersPushController.ContentType).FromInstance(bpOffersPushController)
			.AsCached();
		base.Container.Bind<PushControllerCluster>().FromMethod(CreatPushControllerCluster).AsSingle();
		void BindScreenIndicator<T>(ContentType contentType) where T : ScreenIndicator
		{
			base.Container.Bind<ScreenIndicator>().FromResolveGetter((T x) => x).AsCached()
				.When(delegate(InjectContext _context)
				{
					PushController pushController = _context.ObjectInstance as PushController;
					return !(pushController == null) && pushController.ContentType == contentType;
				});
		}
		PushControllerCluster CreatPushControllerCluster()
		{
			return new PushControllerCluster(new Dictionary<ContentType, PushController>
			{
				{
					ContentType.Main,
					offersPushController
				},
				{
					ContentType.Event,
					eventOffersPushController
				},
				{
					ContentType.BattlePass,
					bpOffersPushController
				}
			});
		}
	}
}

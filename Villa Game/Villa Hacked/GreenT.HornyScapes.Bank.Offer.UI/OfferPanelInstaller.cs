using System;
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
		Assert.IsNotNull((object)previewPrefab);
		Assert.IsNotNull((object)container);
		Assert.IsNotNull((object)previewController);
		Assert.IsNotNull((object)offersPushController);
		Assert.IsNotNull((object)eventOffersPushController);
		Assert.IsNotNull((object)bpOffersPushController);
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<OfferPreview>()).FromComponentInNewPrefab((UnityEngine.Object)previewPrefab)).UnderTransform(container).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<OfferPreviewManager>()).FromNewComponentOn(container.gameObject).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferPreviewController>().FromInstance((object)previewController).AsSingle();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferUnlockController>()).FromNewComponentOn(previewController.gameObject).AsSingle();
		((BindSignalToBinder<LotBoughtSignal>)(object)SignalExtensions.BindSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container)).ToMethod<OfferPreviewController>((Func<OfferPreviewController, Action<LotBoughtSignal>>)((OfferPreviewController x) => x.OnLotBoughtRequest)).FromResolve();
		BindScreenIndicator<MainContentScreenIndicator>(ContentType.Main);
		BindScreenIndicator<EventMergeScreenIndicator>(ContentType.Event);
		BindScreenIndicator<MainContentScreenIndicator>(ContentType.BattlePass);
		((FromBinderGeneric<PushController>)(object)((MonoInstallerBase)this).Container.Bind<PushController>().WithId((object)offersPushController.ContentType)).FromInstance(offersPushController).AsCached();
		((FromBinderGeneric<PushController>)(object)((MonoInstallerBase)this).Container.Bind<PushController>().WithId((object)eventOffersPushController.ContentType)).FromInstance(eventOffersPushController).AsCached();
		((FromBinderGeneric<PushController>)(object)((MonoInstallerBase)this).Container.Bind<PushController>().WithId((object)bpOffersPushController.ContentType)).FromInstance(bpOffersPushController).AsCached();
		((FromBinderGeneric<PushControllerCluster>)(object)((MonoInstallerBase)this).Container.Bind<PushControllerCluster>()).FromMethod((Func<PushControllerCluster>)CreatPushControllerCluster).AsSingle();
		void BindScreenIndicator<T>(ContentType contentType) where T : ScreenIndicator
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			((ConditionCopyNonLazyBinder)((FromBinderGeneric<ScreenIndicator>)(object)((MonoInstallerBase)this).Container.Bind<ScreenIndicator>()).FromResolveGetter<T>((Func<T, ScreenIndicator>)((T x) => x)).AsCached()).When((BindingCondition)delegate(InjectContext _context)
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

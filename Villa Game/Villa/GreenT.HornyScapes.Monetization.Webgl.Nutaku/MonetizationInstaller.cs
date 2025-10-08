using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string JS_CONTROLLER_NAME = "NutakuTestController";

	private const string RegionName = "nutaku";

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<Connection>().AsSingle();
		base.Container.Bind<PaymentController>().FromNewComponentOnNewGameObject().AsSingle()
			.OnInstantiated(delegate(InjectContext _context, PaymentController _object)
			{
				_object.gameObject.name = "NutakuTestController";
			})
			.NonLazy();
		base.Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("nutaku");
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<NutakuMonetizationPopupOpener>().AsSingle();
		base.Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		base.Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.Bind<PaymentStatusUpdateRequest>().AsSingle();
	}
}

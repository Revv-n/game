using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeStore;
using GreenT.HornyScapes.Monetization.Android.Harem;
using StripClub.Model.Shop.Data;
using StripClub.UI.Shop;
using Zenject;

namespace StripClub.Model;

public class SignalBusMonoInstaller : MonoInstaller<SignalBusMonoInstaller>
{
	public override void InstallBindings()
	{
		Installer<SignalBusInstaller>.Install(base.Container);
		base.Container.DeclareSignal<ViewUpdateSignal>();
		base.Container.DeclareSignal<LotBoughtSignal>();
		base.Container.DeclareSignal<RouletteLotBoughtSignal>();
		base.Container.DeclareSignal<OpenTabSignal>();
		base.Container.DeclareSignal<IndicatorSignals.PushRequest>();
		base.Container.DeclareSignal<SpendHardMergeStoreSignal>();
		base.Container.DeclareSignal<SpendHardForRechargeSignal>();
		base.Container.DeclareSignal<SpendHardBubbleSignal>();
		base.Container.DeclareSignal<BuyItemInMergeStoreSignal>();
		base.Container.DeclareSignal<CancelPaymentSignal>();
	}
}

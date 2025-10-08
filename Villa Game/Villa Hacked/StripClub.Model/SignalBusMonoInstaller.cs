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
		Installer<SignalBusInstaller>.Install(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<ViewUpdateSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<LotBoughtSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<RouletteLotBoughtSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<OpenTabSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<IndicatorSignals.PushRequest>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<SpendHardMergeStoreSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<SpendHardForRechargeSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<SpendHardBubbleSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<BuyItemInMergeStoreSignal>(((MonoInstallerBase)this).Container);
		SignalExtensions.DeclareSignal<CancelPaymentSignal>(((MonoInstallerBase)this).Container);
	}
}

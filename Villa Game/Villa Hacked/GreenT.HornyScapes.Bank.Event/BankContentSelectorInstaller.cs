using System;
using GreenT.HornyScapes.Events.Content;
using Zenject;

namespace GreenT.HornyScapes.Bank.Event;

public class BankContentSelectorInstaller : MonoInstaller<BankContentSelectorInstaller>
{
	public override void InstallBindings()
	{
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BankContentSelector>()).AsSingle()).OnInstantiated<BankContentSelector>((Action<InjectContext, BankContentSelector>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<OfferContentSelector>()).AsSingle()).OnInstantiated<OfferContentSelector>((Action<InjectContext, OfferContentSelector>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GoldenTicketClusterSelector>()).AsSingle()).OnInstantiated<GoldenTicketClusterSelector>((Action<InjectContext, GoldenTicketClusterSelector>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
	}
}

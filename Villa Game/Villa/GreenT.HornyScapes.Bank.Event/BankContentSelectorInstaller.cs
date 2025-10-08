using GreenT.HornyScapes.Events.Content;
using Zenject;

namespace GreenT.HornyScapes.Bank.Event;

public class BankContentSelectorInstaller : MonoInstaller<BankContentSelectorInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<BankContentSelector>().AsSingle().OnInstantiated<BankContentSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
		base.Container.BindInterfacesAndSelfTo<OfferContentSelector>().AsSingle().OnInstantiated<OfferContentSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketClusterSelector>().AsSingle().OnInstantiated<GoldenTicketClusterSelector>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}

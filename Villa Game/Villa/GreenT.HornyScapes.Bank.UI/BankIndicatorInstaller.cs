using GreenT.HornyScapes.Events.Content;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BankIndicatorInstaller : MonoInstaller<BankIndicatorInstaller>
{
	[SerializeField]
	private BankIndicator indicator;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesTo<BankIndicator>().FromInstance(indicator).AsCached()
			.OnInstantiated<BankIndicator>(ContentSelectorInstaller.AddSelectorToContainer)
			.NonLazy();
	}
}

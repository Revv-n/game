using System;
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
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<BankIndicator>().FromInstance((object)indicator).AsCached()).OnInstantiated<BankIndicator>((Action<InjectContext, BankIndicator>)ContentSelectorInstaller.AddSelectorToContainer)).NonLazy();
	}
}

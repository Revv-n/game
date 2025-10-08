using GreenT.HornyScapes.Events;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventFlyingCurrencyInstaller : MonoInstaller<EventBezierInstaller>
{
	[SerializeField]
	private CurrencyFlyPool _currencyFlyPool;

	[SerializeField]
	private CurrencyFlyFactory _currencyFlyFactory;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyFlyPool>().FromInstance((object)_currencyFlyPool);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<CurrencyFlyFactory>().FromInstance((object)_currencyFlyFactory).AsSingle();
	}
}

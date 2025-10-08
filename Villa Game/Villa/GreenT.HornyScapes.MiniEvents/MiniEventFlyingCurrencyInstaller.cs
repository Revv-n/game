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
		base.Container.BindInterfacesAndSelfTo<CurrencyFlyPool>().FromInstance(_currencyFlyPool);
		base.Container.BindInterfacesAndSelfTo<CurrencyFlyFactory>().FromInstance(_currencyFlyFactory).AsSingle();
	}
}

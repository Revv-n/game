using System;
using GreenT.Model.Collections;
using GreenT.Model.Reactive;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public class VisibleCurrencyGaugesInstaller : MonoInstaller
{
	public override void InstallBindings()
	{
		((FromBinder)((ConcreteBinderNonGeneric)((MonoInstallerBase)this).Container.Bind(new Type[3]
		{
			typeof(ReactiveCollection<CurrencyType>),
			typeof(IReadOnlyReactiveCollection<CurrencyType>),
			typeof(ICollectionManager<CurrencyType>)
		})).To<ReactiveCollection<CurrencyType>>()).FromNew().AsSingle();
	}
}

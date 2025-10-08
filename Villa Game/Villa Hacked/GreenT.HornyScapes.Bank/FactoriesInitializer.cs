using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model.Shop.Data;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class FactoriesInitializer<TInterface, TFactory, TFactoryContainer> : IInitializable where TFactory : IFactory<TInterface> where TFactoryContainer : DependentFactory<ContentSource, TInterface>
{
	private readonly List<IFactory<GreenT.Types.KeyValuePair<ContentSource, TFactory>>> pairFactories;

	private readonly TFactoryContainer container;

	public FactoriesInitializer(List<IFactory<GreenT.Types.KeyValuePair<ContentSource, TFactory>>> factories, TFactoryContainer container)
	{
		pairFactories = factories;
		this.container = container;
	}

	public void Initialize()
	{
		foreach (IFactory<GreenT.Types.KeyValuePair<ContentSource, TFactory>> pairFactory in pairFactories)
		{
			GreenT.Types.KeyValuePair<ContentSource, TFactory> keyValuePair = ((IFactory<GreenT.Types.KeyValuePair<ContentSource, _003F>>)(object)pairFactory).Create();
			container.Set(keyValuePair.Key, keyValuePair.Value);
		}
	}
}

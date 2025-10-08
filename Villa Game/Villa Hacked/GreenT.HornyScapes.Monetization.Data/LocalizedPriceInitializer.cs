using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Data;

public class LocalizedPriceInitializer : StructureInitializer<IEnumerable<LocalizedPrice>>
{
	private readonly IRegionPriceResolver _priceResolver;

	private readonly IManager<LocalizedPrice> manager;

	public LocalizedPriceInitializer(IManager<LocalizedPrice> manager, IRegionPriceResolver priceResolver, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.manager = manager;
		_priceResolver = priceResolver;
	}

	public override IObservable<bool> Initialize(IEnumerable<LocalizedPrice> products)
	{
		try
		{
			manager.AddRange(products);
			_priceResolver.Initialize();
			return Observable.Do<bool>(Observable.Return(true).Debug("Products has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
			{
				isInitialized.Value = _init;
			});
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class GarantChanceStructureInitializer : StructureInitializerViaArray<GarantChanceMapper, GarantChance>
{
	public GarantChanceStructureInitializer(IManager<GarantChance> manager, IFactory<GarantChanceMapper, GarantChance> factory, IEnumerable<IStructureInitializer> others)
		: base(manager, factory, others)
	{
	}

	public override IObservable<bool> Initialize(IEnumerable<GarantChanceMapper> mappers)
	{
		try
		{
			foreach (IGrouping<int, GarantChanceMapper> item in from chance in mappers
				group chance by chance.garant_id)
			{
				GarantChance garantChance = factory.Create(item.First());
				foreach (GarantChanceMapper item2 in item)
				{
					float num = item2.chance_value * 100f;
					garantChance.AddConfiguration(item2.summon_qty, (int)num);
				}
				manager.Add(garantChance);
			}
			return Observable.Return(value: true).Debug(typeof(GarantChance)?.ToString() + " has been Loaded", LogType.Data).Do(delegate(bool _init)
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

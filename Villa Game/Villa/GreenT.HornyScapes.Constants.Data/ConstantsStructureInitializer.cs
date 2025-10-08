using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Data;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Constants.Data;

public class ConstantsStructureInitializer : StructureInitializer<IEnumerable<ConstantMapper>>
{
	private readonly ICollectionSetter<ConstantMapper> constantSetter;

	public ConstantsStructureInitializer(IEnumerable<IStructureInitializer> others = null, ICollectionSetter<ConstantMapper> constantSetter = null)
		: base(others)
	{
		this.constantSetter = constantSetter;
	}

	public override IObservable<bool> Initialize(IEnumerable<ConstantMapper> data)
	{
		try
		{
			constantSetter.Add(data.ToArray());
			return Observable.Return(value: true).Debug("Constants Loaded", LogType.Data).Do(delegate(bool _init)
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

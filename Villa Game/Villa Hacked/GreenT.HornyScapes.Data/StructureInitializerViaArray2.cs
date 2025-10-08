using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class StructureInitializerViaArray2<TMapper, TEntity> : StructureInitializer<IEnumerable<TMapper>>
{
	private readonly ICollectionSetter<TEntity> lockerSetter;

	private readonly IFactory<TMapper, TEntity> factory;

	public StructureInitializerViaArray2(ICollectionSetter<TEntity> lockerSetter, IFactory<TMapper, TEntity> factory, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.lockerSetter = lockerSetter;
		this.factory = factory;
	}

	public override IObservable<bool> Initialize(IEnumerable<TMapper> mappers)
	{
		try
		{
			TEntity[] obj = mappers.Select((Func<TMapper, TEntity>)factory.Create).ToArray();
			lockerSetter.Add(obj);
			return Observable.Do<bool>(Observable.Return(true).Debug(typeof(TEntity)?.ToString() + " has been Loaded", LogType.Data), (Action<bool>)delegate(bool _init)
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

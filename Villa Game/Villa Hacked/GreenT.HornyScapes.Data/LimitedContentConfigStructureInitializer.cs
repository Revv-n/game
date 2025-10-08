using System;
using System.Collections.Generic;
using GreenT.Types;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class LimitedContentConfigStructureInitializer<TMapper, TEntity, TManager> : StructureInitializer<IEnumerable<TMapper>> where TMapper : ILimitedContent where TManager : IManager<TEntity>
{
	protected readonly IDictionary<ContentType, TManager> dictionary;

	protected readonly IFactory<TMapper, TEntity> CreateDataFactory;

	public LimitedContentConfigStructureInitializer(IDictionary<ContentType, TManager> dictionary, IFactory<TMapper, TEntity> createDataFactory, IEnumerable<IStructureInitializer> others = null)
		: base(others)
	{
		this.dictionary = dictionary;
		CreateDataFactory = createDataFactory;
	}

	public override IObservable<bool> Initialize(IEnumerable<TMapper> mappers)
	{
		try
		{
			foreach (TMapper mapper in mappers)
			{
				TEntity entity = CreateDataFactory.Create(mapper);
				DistributeContent(mapper, entity);
			}
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

	protected virtual void DistributeContent(TMapper mapper, TEntity entity)
	{
		switch (mapper.Type)
		{
		case ConfigContentType.Main:
			dictionary[ContentType.Main].Add(entity);
			break;
		case ConfigContentType.Event:
			dictionary[ContentType.Event].Add(entity);
			break;
		case ConfigContentType.BattlePass:
			dictionary[ContentType.BattlePass].Add(entity);
			break;
		case ConfigContentType.MainEvent:
			dictionary[ContentType.Main].Add(entity);
			dictionary[ContentType.Event].Add(entity);
			break;
		default:
			throw new ArgumentOutOfRangeException(mapper.ToString()).LogException();
		}
	}
}

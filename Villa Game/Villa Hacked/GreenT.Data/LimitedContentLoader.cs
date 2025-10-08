using System;
using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model.Data;
using UniRx;
using Zenject;

namespace GreenT.Data;

public abstract class LimitedContentLoader<TMapper, TEntity, TManager> : ILoader<IEnumerable<TEntity>> where TMapper : ILimitedContent where TManager : IManager<TEntity>
{
	protected readonly ILoader<IEnumerable<TMapper>> loader;

	protected readonly IFactory<TMapper, TEntity> factory;

	protected readonly IDictionary<ContentType, TManager> dictionary;

	public LimitedContentLoader(ILoader<IEnumerable<TMapper>> loader, IFactory<TMapper, TEntity> factory, IDictionary<ContentType, TManager> dictionary)
	{
		this.loader = loader;
		this.factory = factory;
		this.dictionary = dictionary;
	}

	public IObservable<IEnumerable<TEntity>> Load()
	{
		return Observable.ToArray<TEntity>(Observable.Select<(TMapper, TEntity), TEntity>(Observable.Catch<(TMapper, TEntity), Exception>(Observable.Do<(TMapper, TEntity)>(Observable.Select<TMapper, (TMapper, TEntity)>(Observable.SelectMany<IEnumerable<TMapper>, TMapper>(loader.Load(), (Func<IEnumerable<TMapper>, IEnumerable<TMapper>>)((IEnumerable<TMapper> x) => x)), (Func<TMapper, (TMapper, TEntity)>)((TMapper _mapper) => (mapper: _mapper, category: factory.Create(_mapper)))), (Action<(TMapper, TEntity)>)DistributeContent), (Func<Exception, IObservable<(TMapper, TEntity)>>)delegate(Exception ex)
		{
			throw ex.SendException(GetType().Name + ": Error when creating " + typeof(TEntity).Name + " by factory: " + ((object)factory).GetType().Name + "\n");
		}), (Func<(TMapper, TEntity), TEntity>)(((TMapper mapper, TEntity category) _tupple) => _tupple.category)));
	}

	protected virtual void DistributeContent((TMapper mapper, TEntity category) tupple)
	{
		switch (tupple.mapper.Type)
		{
		case ConfigContentType.Main:
			dictionary[ContentType.Main].Add(tupple.category);
			break;
		case ConfigContentType.Event:
			dictionary[ContentType.Event].Add(tupple.category);
			break;
		case ConfigContentType.MainEvent:
			dictionary[ContentType.Main].Add(tupple.category);
			dictionary[ContentType.Event].Add(tupple.category);
			break;
		default:
			throw new ArgumentOutOfRangeException(tupple.mapper.ToString()).LogException();
		}
	}
}

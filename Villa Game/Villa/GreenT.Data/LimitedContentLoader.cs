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
		return (from _tupple in (from _mapper in loader.Load().SelectMany((IEnumerable<TMapper> x) => x)
				select (mapper: _mapper, category: factory.Create(_mapper))).Do(DistributeContent).Catch(delegate(Exception ex)
			{
				throw ex.SendException(GetType().Name + ": Error when creating " + typeof(TEntity).Name + " by factory: " + factory.GetType().Name + "\n");
			})
			select _tupple.category).ToArray();
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

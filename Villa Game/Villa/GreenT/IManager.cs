using System;
using System.Collections.Generic;

namespace GreenT;

public interface IManager<TEntity>
{
	IEnumerable<TEntity> Collection { get; }

	IObservable<TEntity> OnNew { get; }

	void Add(TEntity entity);

	void AddRange(IEnumerable<TEntity> entities);
}

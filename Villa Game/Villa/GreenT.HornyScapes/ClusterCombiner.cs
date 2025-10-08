using System.Collections.Generic;
using System.Linq;
using GreenT.Types;

namespace GreenT.HornyScapes;

public abstract class ClusterCombiner<TEntity, TManager> where TManager : IManager<TEntity>
{
	protected readonly IDictionary<ContentType, TManager> _cluster;

	public ClusterCombiner(IDictionary<ContentType, TManager> cluster)
	{
		_cluster = cluster;
	}

	public IEnumerable<TEntity> GetAll()
	{
		IEnumerable<TEntity> enumerable = null;
		foreach (TManager value in _cluster.Values)
		{
			enumerable = ((enumerable != null) ? enumerable.Concat(value.Collection) : value.Collection);
		}
		return enumerable.ToArray();
	}
}

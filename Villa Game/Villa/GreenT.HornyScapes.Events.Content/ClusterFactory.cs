using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.Events.Content;

public class ClusterFactory<T, K> : IFactory<K>, IFactory where K : ContentCluster<T>, new()
{
	private readonly IFactory<T> entityFactory;

	public ClusterFactory(IFactory<T> entityFactory)
	{
		this.entityFactory = entityFactory;
	}

	public virtual K Create()
	{
		return new K
		{
			[ContentType.Main] = entityFactory.Create(),
			[ContentType.Event] = entityFactory.Create(),
			[ContentType.BattlePass] = entityFactory.Create()
		};
	}
}

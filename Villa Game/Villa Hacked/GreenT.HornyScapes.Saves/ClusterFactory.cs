using System.Collections.Generic;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class ClusterFactory : IFactory<SaveEventClusterManager>, IFactory
{
	private readonly List<SaveEvent> eventsMainMode;

	private readonly List<SaveEvent> eventsTutorialMode;

	public ClusterFactory([Inject(Id = SaveMode.Main)] List<SaveEvent> eventsMainMode, [Inject(Id = SaveMode.Tutorial)] List<SaveEvent> eventsTutorialMode)
	{
		this.eventsMainMode = eventsMainMode;
		this.eventsTutorialMode = eventsTutorialMode;
	}

	public virtual SaveEventClusterManager Create()
	{
		return new SaveEventClusterManager
		{
			[SaveMode.None] = new SaveEventManager(),
			[SaveMode.Main] = new SaveEventManager(eventsMainMode),
			[SaveMode.Tutorial] = new SaveEventManager(eventsTutorialMode)
		};
	}
}

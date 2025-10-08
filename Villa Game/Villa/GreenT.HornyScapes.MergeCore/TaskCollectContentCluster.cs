using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class TaskCollectContentCluster : ContentCluster<TaskCollectAnimationManager>
{
	public TaskCollectContentCluster([Inject(Id = ContentType.Main)] TaskCollectAnimationManager mainManager, [Inject(Id = ContentType.Event)] TaskCollectAnimationManager eventManager)
	{
		base[ContentType.Main] = mainManager;
		base[ContentType.Event] = eventManager;
	}
}

using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;

namespace GreenT.HornyScapes.Tasks.Data;

public class MergeAnimationSelector : IContentSelector, ISelector<ContentType>
{
	private readonly TaskCollectContentCluster cluster;

	private readonly TaskCollect taskCollect;

	public MergeAnimationSelector(TaskCollectContentCluster cluster, TaskCollect taskCollect)
	{
		this.cluster = cluster;
		this.taskCollect = taskCollect;
	}

	public void Select(ContentType type)
	{
		TaskCollectAnimationManager manager = cluster[type];
		taskCollect.Set(manager);
	}
}

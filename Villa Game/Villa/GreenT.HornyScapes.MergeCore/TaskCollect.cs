using System;
using System.Collections.Generic;
using Merge;
using UniRx;

namespace GreenT.HornyScapes.MergeCore;

public class TaskCollect : IDisposable
{
	private CompositeDisposable streams = new CompositeDisposable();

	private TaskCollectAnimationManager manager;

	public void Set(TaskCollectAnimationManager manager)
	{
		this.manager = manager;
	}

	public void PlayAnimation(IEnumerable<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			CloudParticle view = manager.GetView();
			view.transform.position = item.transform.position;
			view.Play();
		}
	}

	public void Dispose()
	{
		streams.Dispose();
	}
}

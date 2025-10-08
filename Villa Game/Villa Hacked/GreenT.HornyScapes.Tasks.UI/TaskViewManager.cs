using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskViewManager<T> : OrderedViewManager<T>, IDisposable where T : TaskView
{
	[SerializeField]
	private RectTransform _root;

	protected Subject<T> onUpdate = new Subject<T>();

	public IEnumerable<T> TaskViews => views;

	public IObservable<T> OnUpdate => Observable.AsObservable<T>((IObservable<T>)onUpdate);

	public int Count => views.Count;

	public override T GetView()
	{
		T view = base.GetView();
		onUpdate.OnNext(view);
		return view;
	}

	public override void MoveToFirst(T task)
	{
		views.Remove(task);
		views.Insert(0, task);
		task.transform.SetAsFirstSibling();
	}

	public override void MoveToLast(T task)
	{
		int taskId = views.IndexOf(task);
		T val = views.FirstOrDefault((T view) => !view.IsActive());
		if (val == null)
		{
			Swap(taskId, views.Count - 1);
			return;
		}
		int objectId = views.IndexOf(val);
		Swap(taskId, objectId);
	}

	public void MoveReadyTasksUp()
	{
		List<T> list = (from p in views
			where p.Source.ReadyToComplete
			orderby p.Source.Reward.Count() descending
			select p).ToList();
		list.AddRange(views.Where((T p) => !p.Source.ReadyToComplete));
		for (int i = 0; i < list.Count; i++)
		{
			if (!base.transform.GetSiblingIndex().Equals(i))
			{
				list[i].transform.SetSiblingIndex(i);
			}
		}
		views.Clear();
		views.AddRange(list);
		ForceRebuild();
	}

	private void ForceRebuild()
	{
		if (_root == null)
		{
			_root = base.gameObject.GetComponent<RectTransform>();
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
	}

	private void Swap(int taskId, int objectId)
	{
		List<T> list = views;
		List<T> list2 = views;
		T val = views[objectId];
		T val2 = views[taskId];
		T val4 = (list[taskId] = val);
		val4 = (list2[objectId] = val2);
		views[objectId].transform.SetSiblingIndex(objectId);
	}

	public void Dispose()
	{
		foreach (T view in views)
		{
			view.Dispose();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Tasks.Story;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskGirlPhoto : MonoBehaviour
{
	[SerializeField]
	private Image storyImage;

	private TaskArtManager manager;

	private CompositeDisposable storyUnlockStream = new CompositeDisposable();

	private IDisposable loadArtStream;

	private void OnValidate()
	{
		if (!storyImage)
		{
			storyImage = GetComponent<Image>();
		}
	}

	protected void Awake()
	{
		throw new NotImplementedException();
	}

	private void SetOpenedStory()
	{
		if (TryGetOpenedStories(out var openedStories))
		{
			TaskArt story = openedStories.Max((Func<TaskArt, TaskArt>)Selector);
			SetStory(story);
		}
		TaskArt Selector(TaskArt starStory)
		{
			int lastOpenedID = openedStories.Max((TaskArt _story) => _story.ID);
			return openedStories.Where((TaskArt _story) => _story.ID == lastOpenedID).First();
		}
	}

	private bool TryGetOpenedStories(out IEnumerable<TaskArt> openedStories)
	{
		openedStories = manager.Collection.Where((TaskArt _story) => _story.Locker.IsOpen.Value);
		return openedStories.Any();
	}

	private void SubscribeOnUnlockStory()
	{
		storyUnlockStream.Clear();
		foreach (TaskArt lockedStory in manager.Collection.Where((TaskArt _story) => !_story.Locker.IsOpen.Value))
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)lockedStory.Locker.IsOpen, (Func<bool, bool>)((bool _value) => _value)), (Action<bool>)delegate
			{
				SetStory(lockedStory);
			}), (ICollection<IDisposable>)storyUnlockStream);
		}
	}

	private void SetStory(TaskArt lastOpenedStory)
	{
		loadArtStream?.Dispose();
		throw new NotImplementedException();
	}

	private void OnDestroy()
	{
		storyUnlockStream.Dispose();
		loadArtStream?.Dispose();
	}
}

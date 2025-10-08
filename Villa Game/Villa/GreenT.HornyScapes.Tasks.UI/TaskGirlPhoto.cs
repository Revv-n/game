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
			lockedStory.Locker.IsOpen.Where((bool _value) => _value).Subscribe(delegate
			{
				SetStory(lockedStory);
			}).AddTo(storyUnlockStream);
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

using System;
using System.Linq;
using GreenT.HornyScapes.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryController : IDisposable
{
	private StoryManager storyManager;

	private GameStarter gameStarter;

	private StartFlow startFlow;

	private CompositeDisposable trackStream = new CompositeDisposable();

	private ScreenIndicator current;

	private Subject<Story> onStoryReadyToShow = new Subject<Story>();

	public bool IsStarted { get; private set; }

	[Inject]
	private void Init(GameStarter gameStarter, StartFlow startFlow)
	{
		this.gameStarter = gameStarter;
		this.startFlow = startFlow;
	}

	public void Set(StoryManager source, ScreenIndicator indicator)
	{
		storyManager = source;
		current = indicator;
		Launch();
	}

	public void Launch()
	{
		if (storyManager == null || current == null)
		{
			Debug.LogError("Source weren't set yet");
			return;
		}
		trackStream.Clear();
		IConnectableObservable<Story> connectableObservable = (from _story in gameStarter.IsGameActive.FirstOrDefault((bool x) => x).SelectMany((bool _) => storyManager.Collection)
			where _story.Phrases.Any() && !_story.IsComplete
			select _story).SelectMany((Story _story) => from _value in _story.Phrases.Peek().Lockers.IsOpen
			where _value
			select _value into _
			select _story).Delay(TimeSpan.FromSeconds(0.10000000149011612)).Publish();
		IObservable<Story> observable = connectableObservable.Where((Story _) => !startFlow.IsLaunched);
		connectableObservable.Where((Story _) => startFlow.IsLaunched).SelectMany((Story _story) => from _ in startFlow.OnUpdate.FirstOrDefault((IInputBlocker x) => !x.IsLaunched)
			select _story).Merge(observable)
			.SelectMany((Func<Story, IObservable<Story>>)EmitOnMainScreenIsOpened)
			.Subscribe(onStoryReadyToShow.OnNext, delegate(Exception ex)
			{
				ex.LogException();
			})
			.AddTo(trackStream);
		connectableObservable.Connect().AddTo(trackStream);
		IObservable<Story> EmitOnMainScreenIsOpened(Story story)
		{
			return from _ in current.IsVisible.Where((bool _value) => _value).FirstOrDefault()
				select story;
		}
	}

	public IObservable<Story> OnStoryReadyToShow()
	{
		return onStoryReadyToShow.AsObservable();
	}

	public void Dispose()
	{
		trackStream?.Dispose();
		onStoryReadyToShow?.Dispose();
	}
}

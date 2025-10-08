using System;
using System.Collections.Generic;
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
		IConnectableObservable<Story> val = Observable.Publish<Story>(Observable.Delay<Story>(Observable.SelectMany<Story, Story>(Observable.Where<Story>(Observable.SelectMany<bool, Story>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IEnumerable<Story>>)((bool _) => storyManager.Collection)), (Func<Story, bool>)((Story _story) => _story.Phrases.Any() && !_story.IsComplete)), (Func<Story, IObservable<Story>>)((Story _story) => Observable.Select<bool, Story>(Observable.Where<bool>((IObservable<bool>)_story.Phrases.Peek().Lockers.IsOpen, (Func<bool, bool>)((bool _value) => _value)), (Func<bool, Story>)((bool _) => _story)))), TimeSpan.FromSeconds(0.10000000149011612)));
		IObservable<Story> observable = Observable.Where<Story>((IObservable<Story>)val, (Func<Story, bool>)((Story _) => !startFlow.IsLaunched));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Story>(Observable.SelectMany<Story, Story>(Observable.Merge<Story>(Observable.SelectMany<Story, Story>(Observable.Where<Story>((IObservable<Story>)val, (Func<Story, bool>)((Story _) => startFlow.IsLaunched)), (Func<Story, IObservable<Story>>)((Story _story) => Observable.Select<IInputBlocker, Story>(Observable.FirstOrDefault<IInputBlocker>(startFlow.OnUpdate, (Func<IInputBlocker, bool>)((IInputBlocker x) => !x.IsLaunched)), (Func<IInputBlocker, Story>)((IInputBlocker _) => _story)))), new IObservable<Story>[1] { observable }), (Func<Story, IObservable<Story>>)EmitOnMainScreenIsOpened), (Action<Story>)onStoryReadyToShow.OnNext, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)trackStream);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)trackStream);
		IObservable<Story> EmitOnMainScreenIsOpened(Story story)
		{
			return Observable.Select<bool, Story>(Observable.FirstOrDefault<bool>(Observable.Where<bool>((IObservable<bool>)current.IsVisible, (Func<bool, bool>)((bool _value) => _value))), (Func<bool, Story>)((bool _) => story));
		}
	}

	public IObservable<Story> OnStoryReadyToShow()
	{
		return Observable.AsObservable<Story>((IObservable<Story>)onStoryReadyToShow);
	}

	public void Dispose()
	{
		CompositeDisposable obj = trackStream;
		if (obj != null)
		{
			obj.Dispose();
		}
		onStoryReadyToShow?.Dispose();
	}
}

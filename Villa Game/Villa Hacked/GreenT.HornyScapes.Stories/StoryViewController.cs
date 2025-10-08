using System;
using DG.Tweening;
using GreenT.HornyScapes.Stories.UI;
using Merge;
using StripClub.Stories;
using StripClub.UI.Story;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Stories;

public class StoryViewController : IInitializable, IDisposable
{
	private StoryController storyController;

	private PhraseView phraseView;

	private SpeakersView speakersView;

	private StoryWindow storyWindow;

	private OverView overView;

	private StoryPhrase lastPhrase;

	private IDisposable clickingStream;

	private IDisposable initializeStream;

	private UIClickSuppressor _uIClickSuppressor;

	[Inject]
	private PCUserInputDetector _pcUserInputDetector;

	private IDisposable _spacingStream;

	public bool IsStarted { get; private set; }

	[Inject]
	private void Init(StoryController storyController, PhraseView phraseView, SpeakersView speakersView, StoryWindow storyWindow, OverView overView, UIClickSuppressor uIClickSuppressor)
	{
		this.storyController = storyController;
		this.phraseView = phraseView;
		this.speakersView = speakersView;
		this.storyWindow = storyWindow;
		this.overView = overView;
		_uIClickSuppressor = uIClickSuppressor;
	}

	public void Initialize()
	{
		initializeStream = ObservableExtensions.Subscribe<Story>(storyController.OnStoryReadyToShow(), (Action<Story>)StartDialogue);
	}

	private void StartDialogue(Story story)
	{
		if (!storyWindow.IsOpened)
		{
			storyWindow.Open();
			IsStarted = true;
			lastPhrase = story.Phrases.Dequeue();
			speakersView.InitSpeakers(lastPhrase.CharactersVisible);
			overView.ApplyView(lastPhrase.CharacterID, isFirstApply: true);
			speakersView.ApplySpeaker(lastPhrase.CharacterID, isFirstApply: true);
			overView.StartView();
			phraseView.ApplyPhrase(lastPhrase);
			clickingStream?.Dispose();
			clickingStream = ObservableExtensions.Subscribe<long>(Observable.Where<long>(Observable.DelayFrame<long>(Observable.Where<long>(Observable.EveryLateUpdate(), (Func<long, bool>)((long _) => Input.GetMouseButtonDown(0))), _uIClickSuppressor.Delay, (FrameCountType)0), (Func<long, bool>)((long _) => !_uIClickSuppressor.IsSuppressed.Value)), (Action<long>)delegate
			{
				ApplyNextPhrase(story);
			});
			_spacingStream?.Dispose();
			_spacingStream = ObservableExtensions.Subscribe<UserInputDetector>(_pcUserInputDetector.OnSpace(), (Action<UserInputDetector>)delegate
			{
				ApplyNextPhrase(story);
			});
		}
	}

	private void ApplyNextPhrase(Story story)
	{
		if (lastPhrase.IsComplete)
		{
			if (story.Phrases.Count == 0)
			{
				CloseDialogue(story);
				return;
			}
			lastPhrase = story.Phrases.Dequeue();
			overView.ApplyView(lastPhrase.CharacterID);
			speakersView.ApplySpeaker(lastPhrase.CharacterID);
			phraseView.ApplyPhrase(lastPhrase);
		}
		else
		{
			phraseView.SkipPhraseTyping(lastPhrase);
		}
	}

	private void CloseDialogue(Story story)
	{
		clickingStream?.Dispose();
		Sequence sequence = overView.EndView();
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, (TweenCallback)delegate
		{
			IsStarted = false;
			storyWindow.Close();
			story.SetCompleted();
		});
	}

	public void Dispose()
	{
		clickingStream?.Dispose();
		initializeStream?.Dispose();
	}
}

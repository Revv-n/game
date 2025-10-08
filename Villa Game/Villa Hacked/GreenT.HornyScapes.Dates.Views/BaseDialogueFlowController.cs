using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Stories;
using GreenT.HornyScapes.Stories.UI;
using GreenT.UI;
using StripClub.Stories;
using UniRx;

namespace GreenT.HornyScapes.Dates.Views;

public abstract class BaseDialogueFlowController<TPhrase> : IDisposable where TPhrase : Phrase
{
	private Queue<TPhrase> _phrasesQueue;

	private IDisposable _clickingStream;

	private readonly TapChecker _tapChecker;

	private readonly Window _dialogueWindow;

	private readonly Subject<Unit> _onEnd = new Subject<Unit>();

	protected TPhrase _lastPhrase;

	protected readonly OverView _overView;

	protected readonly SpeakersView _speakersView;

	protected readonly PhraseView _phraseView;

	public IObservable<Unit> OnEnd => Observable.AsObservable<Unit>((IObservable<Unit>)_onEnd);

	public BaseDialogueFlowController(OverView overView, SpeakersView speakersView, PhraseView phraseView, Window dialogueWindow, TapChecker tapChecker)
	{
		_overView = overView;
		_speakersView = speakersView;
		_phraseView = phraseView;
		_dialogueWindow = dialogueWindow;
		_tapChecker = tapChecker;
	}

	public virtual void StartDialogue(Queue<TPhrase> phrasesQueue)
	{
		if (!_dialogueWindow.IsOpened)
		{
			_phrasesQueue = phrasesQueue;
			_lastPhrase = _phrasesQueue.Dequeue();
			_dialogueWindow.Open();
			_speakersView.InitSpeakers(_lastPhrase.CharactersVisible);
			_overView.StartView();
			ApplyPhrase(_lastPhrase, isFirstTime: true);
			_clickingStream?.Dispose();
			_clickingStream = ObservableExtensions.Subscribe<Unit>(_tapChecker.Clicked, (Action<Unit>)delegate
			{
				ApplyNextPhrase();
			});
		}
	}

	public void Dispose()
	{
		_clickingStream?.Dispose();
		_onEnd.Dispose();
	}

	protected virtual void EndDialogue()
	{
		_clickingStream?.Dispose();
		Sequence sequence = _overView.EndView();
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(OnEndView));
		void OnEndView()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			Sequence sequence2 = _overView.EndView();
			sequence2.onComplete = (TweenCallback)Delegate.Remove(sequence2.onComplete, new TweenCallback(OnEndView));
			_dialogueWindow.Close();
			_onEnd.OnNext(Unit.Default);
		}
	}

	protected abstract void ApplyPhrase(TPhrase phrase, bool isFirstTime = false);

	private void ApplyNextPhrase()
	{
		if (_lastPhrase.IsComplete)
		{
			if (_phrasesQueue.Count == 0)
			{
				EndDialogue();
				return;
			}
			_lastPhrase = _phrasesQueue.Dequeue();
			ApplyPhrase(_lastPhrase);
		}
		else
		{
			_phraseView.SkipPhraseTyping(_lastPhrase);
		}
	}
}

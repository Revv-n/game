using System;
using DG.Tweening;
using GreenT.HornyScapes.Dates.Models;
using GreenT.HornyScapes.Dates.Windows;
using GreenT.HornyScapes.Stories.UI;
using GreenT.UI;
using Merge;
using StripClub.Stories;

namespace GreenT.HornyScapes.Dates.Views;

public class DateFlowController : BaseDialogueFlowController<DatePhrase>
{
	private string[] _lastBackgroundSounds;

	private Sequence _backgroundChangeSequence;

	private readonly BackgroundView _backgroundView;

	private readonly DateSoundController _dateSoundController;

	public DateFlowController(OverView overView, SpeakersView speakersView, PhraseView phraseView, DateWindow dialogueWindow, BackgroundView backgroundView, TapChecker tapChecker, DateSoundController dateSoundController)
		: base(overView, speakersView, phraseView, (Window)dialogueWindow, tapChecker)
	{
		_backgroundView = backgroundView;
		_dateSoundController = dateSoundController;
	}

	protected override void EndDialogue()
	{
		base.EndDialogue();
		StopBackgroundSounds();
		_dateSoundController.StopSoundEffects();
	}

	protected override void ApplyPhrase(DatePhrase phrase, bool isFirstTime = false)
	{
		_phraseView.ApplyPhrase(phrase);
		_overView.ApplyView(phrase.CharacterID, isFirstTime);
		_backgroundChangeSequence?.Kill();
		PlayBackgroundSounds(phrase.BackgroundSounds);
		if (isFirstTime)
		{
			_backgroundView.ApplyBackgroundsFirstTime(phrase);
			ApplyCharacters(phrase, isFirstTime: true);
		}
		else if (phrase.IsFade)
		{
			_backgroundChangeSequence = _backgroundView.ApplyBackgrounds(phrase);
			Sequence backgroundChangeSequence = _backgroundChangeSequence;
			backgroundChangeSequence.onKill = (TweenCallback)Delegate.Combine(backgroundChangeSequence.onKill, new TweenCallback(OnChange));
		}
		else
		{
			_backgroundView.ApplyBackgroundsForce(phrase);
			ApplyCharacters(phrase, isFirstTime: false);
		}
		void OnChange()
		{
			Sequence backgroundChangeSequence2 = _backgroundChangeSequence;
			backgroundChangeSequence2.onKill = (TweenCallback)Delegate.Remove(backgroundChangeSequence2.onKill, new TweenCallback(OnChange));
			_backgroundChangeSequence = null;
			ApplyCharacters(phrase, isFirstTime: false);
		}
	}

	private void ApplyCharacters(DatePhrase phrase, bool isFirstTime)
	{
		int[,] charactersVisible = phrase.CharactersVisible;
		if (!_speakersView.HasSpeakers(charactersVisible))
		{
			_speakersView.InitSpeakers(charactersVisible);
		}
		if (!charactersVisible.IsNullOrEmpty())
		{
			_speakersView.ApplySpeaker(phrase.CharacterID, isFirstTime);
		}
	}

	private void PlayBackgroundSounds(string[] backgroundSounds)
	{
		if (backgroundSounds.Length != 0)
		{
			_lastBackgroundSounds = backgroundSounds;
			_dateSoundController.PlayBackgroundSounds(backgroundSounds);
		}
	}

	private void StopBackgroundSounds()
	{
		if (_lastBackgroundSounds != null && _lastBackgroundSounds.Length != 0)
		{
			_dateSoundController.StopBackgroundSound();
		}
	}
}

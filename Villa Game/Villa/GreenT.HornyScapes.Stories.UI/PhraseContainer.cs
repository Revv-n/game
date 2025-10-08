using System;
using System.Collections;
using DG.Tweening;
using GreenT.HornyScapes.Sounds;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Stories.UI;

public class PhraseContainer : MonoBehaviour
{
	[SerializeField]
	private CollectionSoundSO typingPhrase;

	[SerializeField]
	private TextMeshProUGUI _text;

	[SerializeField]
	private TextMeshProUGUI _name;

	[SerializeField]
	private Transform _continueArrow;

	[SerializeField]
	private float _animationDuration = 1f;

	[SerializeField]
	private float _heightArrow = -0.1f;

	[SerializeField]
	private Ease _ease;

	[Inject]
	private IAudioPlayer audioPlayer;

	[Inject]
	private LocalizationService localizationService;

	private string _currentPhraseText;

	private IDisposable textTyping;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	protected void OnValidate()
	{
		if (typingPhrase == null)
		{
			Debug.LogError("Empty typingPhrase sound", this);
		}
	}

	public void ApplyPhrase(Phrase phrase, float waitTime)
	{
		textTyping?.Dispose();
		if (_continueArrow != null)
		{
			DOTween.Restart(_continueArrow);
			DOTween.Kill(_continueArrow, complete: true);
		}
		if (phrase.CharacterID != 0)
		{
			_name.text = localizationService.Text(phrase.Name);
		}
		_localizationDisposables.Clear();
		localizationService.ObservableText(phrase.Name).Subscribe(delegate(string text)
		{
			_name.text = text;
		}).AddTo(_localizationDisposables);
		localizationService.ObservableText(phrase.Text).Subscribe(delegate(string text)
		{
			_currentPhraseText = text;
			_text.text = text;
		}).AddTo(_localizationDisposables);
		textTyping = Observable.FromCoroutine(() => TypeText(_text, _currentPhraseText, waitTime)).Subscribe(delegate
		{
			SetPhraseComplete(phrase);
			if (_continueArrow != null)
			{
				_continueArrow.DOLocalMoveY(_heightArrow, _animationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(_ease);
			}
		});
	}

	public void SkipTyping(Phrase phrase)
	{
		if (_continueArrow != null)
		{
			DOTween.Restart(_continueArrow);
			DOTween.Kill(_continueArrow, complete: true);
		}
		textTyping?.Dispose();
		if (_continueArrow != null)
		{
			_continueArrow.DOLocalMoveY(_heightArrow, _animationDuration).SetLoops(-1, LoopType.Yoyo).SetEase(_ease);
		}
		_text.text = _currentPhraseText;
		SetPhraseComplete(phrase);
	}

	private void SetPhraseComplete(Phrase phrase)
	{
		phrase.Complete();
	}

	private IEnumerator TypeText(TextMeshProUGUI textField, string text, float waitTime)
	{
		textField.text = string.Empty;
		int i = 0;
		char[] array = text.ToCharArray();
		foreach (char c in array)
		{
			textField.text += c;
			audioPlayer.PlayOneShotAudioClip2D(typingPhrase.Sounds[i]);
			i++;
			if (typingPhrase.Sounds.Count == i)
			{
				i = 0;
			}
			yield return new WaitForSeconds(waitTime);
		}
	}

	protected virtual void OnDisable()
	{
		if (_continueArrow != null)
		{
			DOTween.Kill(_continueArrow);
		}
		textTyping?.Dispose();
		_localizationDisposables?.Clear();
	}

	private void OnDestroy()
	{
		_localizationDisposables?.Dispose();
	}
}

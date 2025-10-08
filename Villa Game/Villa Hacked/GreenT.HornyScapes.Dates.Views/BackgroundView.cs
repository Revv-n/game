using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Dates.Models;
using Merge;
using Spine;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Dates.Views;

public class BackgroundView : MonoView<DatePhrase>
{
	private enum AnimationType
	{
		None,
		Add,
		Set
	}

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation _fadeInAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation _fadeOutAnimation;

	[SerializeField]
	private Image _staticBackground;

	[SerializeField]
	private RectTransform _animatedBackgroundContainer;

	private DateSoundController _dateSoundController;

	private AnimationDateView _previousAnimationBackground;

	private AnimationDateView _currentAnimationBackground;

	private readonly Dictionary<string, AnimationDateView> _animatedBackgrounds = new Dictionary<string, AnimationDateView>(4);

	[Inject]
	public void Init(DateSoundController dateSoundController)
	{
		_dateSoundController = dateSoundController;
	}

	public void ApplyBackgroundsFirstTime(DatePhrase phrase)
	{
		ClearAnimationBackgrounds();
		ResetAnimations();
		Set(phrase);
		HideAnimationBackgrounds();
		CreateSoundEffects();
		ChangeBackgrounds();
		PlayCurrentBackgrounds();
	}

	public DG.Tweening.Sequence ApplyBackgrounds(DatePhrase phrase)
	{
		ResetAnimations();
		Set(phrase);
		return _fadeInAnimation.Play().OnKill(SetBackgrounds);
	}

	public void ApplyBackgroundsForce(DatePhrase phrase)
	{
		ResetAnimations();
		Set(phrase);
		HideAnimationBackgrounds();
		CreateSoundEffects();
		ChangeBackgrounds();
		PlayCurrentBackgrounds();
	}

	private void Awake()
	{
		_fadeInAnimation.Init();
		_fadeOutAnimation.Init();
	}

	private void ResetAnimations()
	{
		_fadeInAnimation.ResetToAnimStart();
		_fadeOutAnimation.ResetToAnimStart();
	}

	private void ClearAnimationBackgrounds()
	{
		_previousAnimationBackground = null;
		_currentAnimationBackground = null;
	}

	private void HideAnimationBackgrounds()
	{
		if (base.Source == null || base.Source.BackgroundIds.Length == 0)
		{
			return;
		}
		foreach (AnimationDateView value in _animatedBackgrounds.Values)
		{
			value.gameObject.SetActive(value: false);
		}
	}

	private void SetBackgrounds()
	{
		HideAnimationBackgrounds();
		CreateSoundEffects();
		ChangeBackgrounds();
		PlayCurrentBackgrounds();
		_fadeOutAnimation.Play();
	}

	private void ChangeBackgrounds()
	{
		if (base.Source == null || base.Source.BackgroundIds.Length == 0)
		{
			return;
		}
		_staticBackground.SetActive(active: false);
		for (int i = 0; i < base.Source.BackgroundDatas.Length; i++)
		{
			DateBackgroundData dateBackgroundData = base.Source.BackgroundDatas[i];
			if (dateBackgroundData.AnimatedBackground == null)
			{
				_staticBackground.sprite = dateBackgroundData.StaticBackground;
				_staticBackground.SetActive(active: true);
				if (base.Source.SoundEffects.Length != 0)
				{
					AppendSoundEffect(base.Source.SoundEffects[0], 0f);
				}
				PlaySoundEffects();
				continue;
			}
			string dataId = GetDataId(base.Source.BackgroundIds[i]);
			if (_animatedBackgrounds.ContainsKey(dataId))
			{
				break;
			}
			AnimationDateView animationDateView = UnityEngine.Object.Instantiate(dateBackgroundData.AnimatedBackground, _animatedBackgroundContainer);
			animationDateView.gameObject.SetActive(value: true);
			_animatedBackgrounds.Add(dataId, animationDateView);
		}
	}

	private void PlayCurrentBackgrounds()
	{
		if (_staticBackground.gameObject.activeSelf || base.Source == null)
		{
			return;
		}
		if (base.Source.BackgroundIds.Length == 0)
		{
			PlaySoundEffects();
			return;
		}
		string[] backgroundIds = base.Source.BackgroundIds;
		bool isChangingAfterEnd = base.Source.IsChangingAfterEnd;
		string soundId = string.Empty;
		for (int i = 0; i < backgroundIds.Length - 1; i++)
		{
			bool num = i == 0;
			AnimationType animationType = AnimationType.Set;
			if (num && isChangingAfterEnd)
			{
				animationType = AnimationType.Add;
			}
			soundId = ((base.Source.SoundEffects.Length == 0) ? string.Empty : base.Source.SoundEffects[i]);
			SetupAnimation(i, isLooped: false, animationType, soundId);
		}
		int num2 = backgroundIds.Length - 1;
		AnimationType animationType2 = AnimationType.Add;
		if (backgroundIds.Length == 1 && !isChangingAfterEnd)
		{
			animationType2 = AnimationType.Set;
		}
		if (num2 < base.Source.SoundEffects.Length)
		{
			soundId = base.Source.SoundEffects[num2];
		}
		SetupAnimation(num2, isLooped: true, animationType2, soundId);
		PlaySoundEffects();
	}

	private void SetupAnimation(int index, bool isLooped, AnimationType animationType, string soundId)
	{
		string[] array = base.Source.BackgroundIds[index].Split(':', StringSplitOptions.None);
		string key = array[0];
		string animationId = array[1];
		_previousAnimationBackground = _currentAnimationBackground;
		_currentAnimationBackground = _animatedBackgrounds[key];
		SwitchActiveBackgrounds(_previousAnimationBackground, _currentAnimationBackground);
		float animationDuration = _currentAnimationBackground.GetAnimationDuration(animationId);
		_currentAnimationBackground.GetRemainingAnimationTime();
		if (base.Source.IsChangingAfterEnd)
		{
			if (_previousAnimationBackground != null)
			{
				_currentAnimationBackground.AddAnimation(animationId, isLooped).Start += OnTrackEntryComplete;
			}
			return;
		}
		switch (animationType)
		{
		case AnimationType.Add:
			_currentAnimationBackground.AddAnimation(animationId, isLooped);
			AppendSoundEffect(soundId, animationDuration);
			break;
		case AnimationType.Set:
			_currentAnimationBackground.SetAnimation(animationId, isLooped);
			AppendSoundEffect(soundId, 0f);
			break;
		}
		void OnTrackEntryComplete(TrackEntry entry)
		{
			entry.Start -= OnTrackEntryComplete;
			_dateSoundController.ForcePlay(soundId);
		}
	}

	private void SwitchActiveBackgrounds(AnimationDateView previousBackground, AnimationDateView background)
	{
		if (previousBackground != null && previousBackground.gameObject != null)
		{
			previousBackground.gameObject.SetActive(value: false);
		}
		background.gameObject.SetActive(value: true);
	}

	private string GetDataId(string compositeId)
	{
		return compositeId.Split(':', StringSplitOptions.None)[0];
	}

	private void CreateSoundEffects()
	{
		if (base.Source.BackgroundIds.Length != 0)
		{
			_dateSoundController.CreateSoundEffects();
		}
	}

	private void AppendSoundEffect(string soundId, float duration)
	{
		_dateSoundController.AppendSoundEffect(soundId, duration);
	}

	private void PlaySoundEffects()
	{
		_dateSoundController.PlaySoundEffects();
	}
}

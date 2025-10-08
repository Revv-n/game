using DG.Tweening;
using GreenT.HornyScapes.Dates.Providers;
using GreenT.HornyScapes.Sounds;
using Merge;
using StripClub.Model.Shop.Data;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Dates.Views;

public sealed class DateSoundController
{
	private const string DatesSoundsBundleKey = "DatesSoundsProvider";

	private DatesSoundsProvider _datesSoundsProvider;

	private Sequence _soundEffectsSequence;

	private DateSoundEffectsController _soundEffectsController;

	private string[] _savedClipPaths;

	private int _savedClipId;

	private float _savedTime;

	private readonly SoundController _soundController;

	private readonly BundlesProviderBase _bundlesProvider;

	[Inject]
	public DateSoundController(SoundController soundController, BundlesProviderBase bundlesProvider)
	{
		_soundController = soundController;
		_bundlesProvider = bundlesProvider;
	}

	public void Init()
	{
		_datesSoundsProvider = _bundlesProvider.TryFindInConcreteBundle<DatesSoundsProvider>(ContentSource.Default, "DatesSoundsProvider");
		_soundEffectsController = new DateSoundEffectsController(_soundController, _datesSoundsProvider.SoundEffects);
	}

	public void SetActiveMusic(bool isActive)
	{
		MusicNode instance = MusicNode.Instance;
		if (instance == null)
		{
			return;
		}
		if (isActive)
		{
			if (_savedClipPaths != null)
			{
				instance.SetPlaylist(_savedClipPaths, _savedClipId, _savedTime);
				instance.Resume();
			}
		}
		else
		{
			_savedClipPaths = instance.ClipPaths;
			_savedClipId = instance.CurrentClipId;
			_savedTime = instance.CurrentTime;
			instance.Pause();
		}
	}

	public void PlayBackgroundSounds(string[] backgroundSounds)
	{
		MusicNode instance = MusicNode.Instance;
		if (instance == null || backgroundSounds == null || backgroundSounds.Length == 0)
		{
			return;
		}
		AudioClip[] array = new AudioClip[backgroundSounds.Length];
		for (int i = 0; i < backgroundSounds.Length; i++)
		{
			string soundId = backgroundSounds[i];
			if (TryGetSound(soundId, out var sound) && !(sound == null) && !(sound.Sound == null))
			{
				array[i] = sound.Sound;
			}
		}
		instance.SetPlaylist(array);
		instance.Resume();
	}

	public void StopBackgroundSound()
	{
		MusicNode instance = MusicNode.Instance;
		if (instance != null)
		{
			instance.Pause();
		}
	}

	public void CreateSoundEffects()
	{
		if (_soundEffectsSequence != null)
		{
			_soundEffectsSequence.Kill();
			_soundEffectsSequence = null;
		}
		_soundEffectsController?.Stop(immediate: true);
		_soundEffectsSequence = DOTween.Sequence().Pause();
	}

	public void AppendSoundEffect(string soundId, float animationDuration)
	{
		if (_soundEffectsSequence == null)
		{
			_soundEffectsSequence = DOTween.Sequence().Pause();
		}
		if (string.IsNullOrEmpty(soundId))
		{
			_soundEffectsSequence.AppendInterval(animationDuration);
			return;
		}
		_soundEffectsSequence.AppendInterval(animationDuration).AppendCallback(delegate
		{
			_soundEffectsController.Play(soundId);
		});
	}

	public void ForcePlay(string soundId)
	{
		_soundEffectsController.Play(soundId);
	}

	public void PlaySoundEffects()
	{
		if (_soundEffectsSequence != null)
		{
			_soundEffectsSequence.Play();
		}
	}

	public void StopSoundEffects()
	{
		_soundEffectsController?.Stop(immediate: true);
		if (_soundEffectsSequence != null)
		{
			_soundEffectsSequence.Kill();
			_soundEffectsSequence = null;
		}
	}

	private bool TryGetSound(string soundId, out SoundSO sound)
	{
		if (_datesSoundsProvider == null)
		{
			sound = null;
			return false;
		}
		return _datesSoundsProvider.BackgroundSounds.TryGetValue(soundId, out sound);
	}
}

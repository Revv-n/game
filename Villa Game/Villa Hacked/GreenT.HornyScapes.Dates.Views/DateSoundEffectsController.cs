using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Dates.Providers;
using Merge;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Views;

public sealed class DateSoundEffectsController : IDisposable
{
	private DatesSoundtracksInfo _currentSoundtracksInfo;

	private IDisposable _fadeOutAllStream;

	private readonly DateSoundsPresetDictionary _soundEffects;

	private readonly SoundController _soundController;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	private readonly List<SoundNode> _activeNodes = new List<SoundNode>();

	private readonly List<SoundNode> _previousNodes = new List<SoundNode>();

	private readonly Dictionary<DatesSoundtracks, IDisposable> _activeTrackSubscriptions = new Dictionary<DatesSoundtracks, IDisposable>();

	public DateSoundEffectsController(SoundController soundController, DateSoundsPresetDictionary soundEffects)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_soundController = soundController;
		_soundEffects = soundEffects;
	}

	public void Play(string soundId)
	{
		if (string.IsNullOrEmpty(soundId))
		{
			return;
		}
		string[] array = soundId.Split(':', StringSplitOptions.None);
		if (array.Length >= 2)
		{
			string key = array[0];
			string key2 = array[1];
			if (_soundEffects.TryGetValue(key, out var value) && value.Soundtracks.TryGetValue(key2, out var value2))
			{
				_currentSoundtracksInfo = value2;
				StartSoundtracks(value2);
			}
		}
	}

	private void StartSoundtracks(DatesSoundtracksInfo soundtracksInfo)
	{
		foreach (DatesSoundtracks item in _activeTrackSubscriptions.Keys.Where((DatesSoundtracks existing) => !soundtracksInfo.Soundtracks.Contains(existing)).ToList())
		{
			_activeTrackSubscriptions[item].Dispose();
			_activeTrackSubscriptions.Remove(item);
		}
		DatesSoundtracks[] soundtracks = soundtracksInfo.Soundtracks;
		foreach (DatesSoundtracks datesSoundtracks in soundtracks)
		{
			if (_activeTrackSubscriptions.ContainsKey(datesSoundtracks))
			{
				_activeTrackSubscriptions[datesSoundtracks].Dispose();
				_activeTrackSubscriptions.Remove(datesSoundtracks);
			}
			string subscriptionId = Guid.NewGuid().ToString();
			IDisposable value = ObservableExtensions.Subscribe<Unit>(CreateTrackSequence(datesSoundtracks, soundtracksInfo, subscriptionId), (Action<Unit>)delegate
			{
			}, (Action<Exception>)delegate(Exception exception)
			{
				Debug.LogException(exception);
			});
			_activeTrackSubscriptions[datesSoundtracks] = value;
		}
	}

	private IObservable<Unit> CreateTrackSequence(DatesSoundtracks soundtrack, DatesSoundtracksInfo parentSoundtracksInfo, string subscriptionId)
	{
		return Observable.Defer<Unit>((Func<IObservable<Unit>>)delegate
		{
			float startCooldown = soundtrack.StartCooldown;
			IObservable<Unit> observable = ((0f < startCooldown) ? Observable.FromCoroutine<Unit>((Func<IObserver<Unit>, IEnumerator>)((IObserver<Unit> observer) => StartDelayCoroutine(startCooldown, observer))) : Observable.Empty<Unit>());
			List<DatesSound> list = new List<DatesSound>(soundtrack.DateSounds);
			if (soundtrack.IsRandom)
			{
				Shuffle(list);
			}
			if (list.Count == 0)
			{
				return observable;
			}
			DatesSound firstSound = list[0];
			List<DatesSound> source = list.Skip(1).ToList();
			string callerContextFirst = $"subscription:{subscriptionId} soundtrack:{soundtrack.name} role:first isRepeat:{soundtrack.IsRepeat}";
			string callerContextRest = $"subscription:{subscriptionId} soundtrack:{soundtrack.name} role:rest isRepeat:{soundtrack.IsRepeat}";
			IObservable<Unit> observable2 = Observable.FromCoroutine<Unit>((Func<IObserver<Unit>, IEnumerator>)((IObserver<Unit> observer) => PlaySoundCoroutine(firstSound, withFadeIn: true, parentSoundtracksInfo.FadeIn, observer, parentSoundtracksInfo, callerContextFirst)));
			IObservable<Unit> observable3 = source.Aggregate(Observable.Empty<Unit>(), (IObservable<Unit> chain, DatesSound dateSound) => Observable.Concat<Unit>(chain, new IObservable<Unit>[1] { Observable.FromCoroutine<Unit>((Func<IObserver<Unit>, IEnumerator>)((IObserver<Unit> observer) => PlaySoundCoroutine(dateSound, withFadeIn: false, 0f, observer, parentSoundtracksInfo, callerContextRest))) }));
			IObservable<Unit> observable4 = Observable.Concat<Unit>(observable2, new IObservable<Unit>[1] { observable3 });
			return (!soundtrack.IsRepeat) ? Observable.Concat<Unit>(observable, new IObservable<Unit>[1] { observable4 }) : Observable.Concat<Unit>(observable, new IObservable<Unit>[1] { Observable.Repeat<Unit>(observable4) });
		});
	}

	private IEnumerator StartDelayCoroutine(float delayDuration, IObserver<Unit> observer)
	{
		yield return new WaitForSeconds(delayDuration);
		observer.OnNext(Unit.Default);
		observer.OnCompleted();
	}

	private IEnumerator PlaySoundCoroutine(DatesSound dateSound, bool withFadeIn, float fadeInDuration, IObserver<Unit> observer, DatesSoundtracksInfo expectedParentInfo, string callerContext)
	{
		AudioClip sound = dateSound.Sound;
		SoundNode soundNode = _soundController.CreateSoundNode();
		_activeNodes.Add(soundNode);
		if (sound != null)
		{
			_ = sound.name;
		}
		float num = ((sound != null) ? sound.length : 0f);
		float cooldown = dateSound.Cooldown;
		float initialVolume = _soundController.Data.SoundVolume.Value;
		float num2 = num + cooldown;
		float safeDelay = Mathf.Max(0f, num2 - (withFadeIn ? fadeInDuration : 0f));
		if (_currentSoundtracksInfo != expectedParentInfo)
		{
			if (_activeNodes.Contains(soundNode))
			{
				_activeNodes.Remove(soundNode);
			}
			_soundController.ReleaseSoundNode(soundNode);
			observer.OnCompleted();
			yield break;
		}
		try
		{
			soundNode.Play(sound, initialVolume, _soundController.Debug, isLooped: false, withAutoStop: false);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			if (_activeNodes.Contains(soundNode))
			{
				_activeNodes.Remove(soundNode);
			}
			_soundController.ReleaseSoundNode(soundNode);
			observer.OnCompleted();
			yield break;
		}
		if (withFadeIn)
		{
			float elapsed = 0f;
			while (elapsed < fadeInDuration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.Clamp01(elapsed / fadeInDuration);
				float volume = Mathf.Lerp(0f, initialVolume, t);
				soundNode.SetVolume(volume);
				if (_currentSoundtracksInfo != expectedParentInfo)
				{
					break;
				}
				yield return null;
			}
		}
		if (0f < safeDelay)
		{
			yield return new WaitForSeconds(safeDelay);
		}
		if (!_previousNodes.Contains(soundNode))
		{
			_soundController.ReleaseSoundNode(soundNode);
		}
		if (_activeNodes.Contains(soundNode))
		{
			_activeNodes.Remove(soundNode);
		}
		observer.OnNext(Unit.Default);
		observer.OnCompleted();
	}

	public void Stop(bool immediate = false)
	{
		if (immediate)
		{
			if (_activeNodes.Count == 0 && _activeTrackSubscriptions.Count == 0)
			{
				return;
			}
			_currentSoundtracksInfo = null;
			foreach (SoundNode activeNode in _activeNodes)
			{
				try
				{
					activeNode.Stop();
					activeNode.SetVolume(_soundController.Data.SoundVolume.Value);
					_soundController.ReleaseSoundNode(activeNode);
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
				}
			}
			_activeNodes.Clear();
			{
				foreach (DatesSoundtracks item in _activeTrackSubscriptions.Keys.ToList())
				{
					_activeTrackSubscriptions[item].Dispose();
					_activeTrackSubscriptions.Remove(item);
				}
				return;
			}
		}
		if (_currentSoundtracksInfo != null)
		{
			_currentSoundtracksInfo = null;
			_fadeOutAllStream?.Dispose();
			_fadeOutAllStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(FadeOutAll((_currentSoundtracksInfo != null) ? _currentSoundtracksInfo.FadeOut : 0f), (Action<Unit>)delegate
			{
			}, (Action<Exception>)delegate(Exception exception)
			{
				Debug.LogException(exception);
			}), (ICollection<IDisposable>)_disposables);
		}
	}

	private IObservable<Unit> FadeOutAll(float fadeOutDuration)
	{
		return Observable.FromCoroutine<Unit>((Func<IObserver<Unit>, IEnumerator>)((IObserver<Unit> observer) => FadeOutCoroutine(fadeOutDuration, observer)));
	}

	private IEnumerator FadeOutCoroutine(float fadeOutDuration, IObserver<Unit> observer)
	{
		_previousNodes.Clear();
		foreach (SoundNode activeNode in _activeNodes)
		{
			_previousNodes.Add(activeNode);
		}
		_activeNodes.Clear();
		List<float> startVolumes = _previousNodes.Select((SoundNode node) => node.Volume).ToList();
		float elapsed = 0f;
		while (elapsed < fadeOutDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / fadeOutDuration);
			for (int i = 0; i < _previousNodes.Count; i++)
			{
				SoundNode soundNode = _previousNodes[i];
				if (!(soundNode == null))
				{
					float volume = Mathf.Lerp(startVolumes[i], 0f, t);
					soundNode.SetVolume(volume);
				}
			}
			yield return null;
		}
		foreach (SoundNode previousNode in _previousNodes)
		{
			if (previousNode != null)
			{
				previousNode.Stop();
				_soundController.ReleaseSoundNode(previousNode);
			}
		}
		_previousNodes.Clear();
		observer.OnNext(Unit.Default);
		observer.OnCompleted();
	}

	private void Shuffle(List<DatesSound> clips)
	{
		for (int i = 0; i < clips.Count; i++)
		{
			int num = UnityEngine.Random.Range(i, clips.Count);
			int index = i;
			int index2 = num;
			DatesSound datesSound = clips[num];
			DatesSound datesSound2 = clips[i];
			DatesSound datesSound4 = (clips[index] = datesSound);
			datesSound4 = (clips[index2] = datesSound2);
		}
	}

	public void Dispose()
	{
		Stop(immediate: true);
		_disposables.Dispose();
	}
}

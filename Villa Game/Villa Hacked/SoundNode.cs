using System;
using DG.Tweening;
using UnityEngine;

public class SoundNode : MonoBehaviour, IPoolReturner
{
	[SerializeField]
	private AudioSource source;

	private Tween _tween;

	private bool _returned;

	public float Volume => source.volume;

	public Action ReturnInPool { get; set; }

	public void Play(AudioClip clip, float volume, bool debug = true, bool isLooped = false, bool withAutoStop = true)
	{
		source.clip = clip;
		source.volume = volume;
		source.loop = isLooped;
		source.Play();
		_returned = false;
		if (_tween != null)
		{
			_tween?.Kill();
			_tween = null;
		}
		if (withAutoStop && !isLooped)
		{
			_tween = DOVirtual.DelayedCall(clip.length, Stop);
		}
	}

	public void Stop()
	{
		source.Stop();
		if (!_returned)
		{
			ReturnInPool();
		}
		_returned = true;
		if (_tween != null)
		{
			_tween?.Kill();
			_tween = null;
		}
	}

	public void SetVolume(float volume)
	{
		source.volume = volume;
	}
}

using System.Collections;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
	[SerializeField]
	private AudioSource source;

	[SerializeField]
	private string[] clipPaths;

	private AudioClip[] _audioClips;

	private int _currentClipId;

	private bool _isPlaying;

	private Coroutine _waitCoroutine;

	public static MusicNode Instance { get; set; }

	public AudioClip CurrentClip { get; private set; }

	public float Volume
	{
		get
		{
			return source.volume;
		}
		set
		{
			SetVolume(value);
		}
	}

	public string[] ClipPaths => clipPaths;

	public int CurrentClipId => _currentClipId;

	public float CurrentTime => source.time;

	public void SetPlaylist(string[] newClipPaths, int startIndex = 0, float startTime = 0f)
	{
		Pause();
		clipPaths = newClipPaths;
		_audioClips = null;
		_currentClipId = Mathf.Clamp(startIndex, 0, clipPaths.Length - 1);
		CurrentClip = Resources.Load<AudioClip>(clipPaths[CurrentClipId]);
		source.clip = CurrentClip;
		source.time = Mathf.Clamp(startTime, 0f, CurrentClip?.length ?? 0f);
	}

	public void SetPlaylist(AudioClip[] newClips, int startIndex = 0, float startTime = 0f)
	{
		Pause();
		if (newClips != null && newClips.Length != 0)
		{
			_audioClips = newClips;
			_currentClipId = Mathf.Clamp(startIndex, 0, newClips.Length - 1);
			CurrentClip = _audioClips[_currentClipId];
			source.clip = CurrentClip;
			source.time = Mathf.Clamp(startTime, 0f, CurrentClip?.length ?? 0f);
		}
	}

	public void Resume()
	{
		if (!_isPlaying)
		{
			_isPlaying = true;
			source.Play();
			_waitCoroutine = StartCoroutine(CRT_WaitEndOfClip());
		}
	}

	public void Pause()
	{
		if (_isPlaying)
		{
			_isPlaying = false;
			source.Pause();
			if (_waitCoroutine != null)
			{
				StopCoroutine(_waitCoroutine);
				_waitCoroutine = null;
			}
		}
	}

	private void StartClip(int id)
	{
		if (_audioClips != null)
		{
			CurrentClip = _audioClips[id];
		}
		else
		{
			CurrentClip = Resources.Load<AudioClip>(clipPaths[id]);
		}
		if (!(CurrentClip == null))
		{
			source.clip = CurrentClip;
			source.Play();
			if (_waitCoroutine != null)
			{
				StopCoroutine(_waitCoroutine);
			}
			_waitCoroutine = StartCoroutine(CRT_WaitEndOfClip());
		}
	}

	private void ChangeSound()
	{
		source.Stop();
		AudioClip currentClip = CurrentClip;
		source.clip = null;
		if (_audioClips == null && currentClip != null)
		{
			Resources.UnloadAsset(currentClip);
		}
		_currentClipId++;
		if (_audioClips != null)
		{
			if (_audioClips.Length <= _currentClipId)
			{
				_currentClipId = 0;
			}
		}
		else if (clipPaths.Length <= _currentClipId)
		{
			_currentClipId = 0;
		}
		StartClip(_currentClipId);
	}

	private IEnumerator CRT_WaitEndOfClip()
	{
		while (source.time < source.clip.length)
		{
			yield return null;
		}
		ChangeSound();
		_waitCoroutine = null;
	}

	private void SetVolume(float volume)
	{
		source.volume = volume;
	}
}

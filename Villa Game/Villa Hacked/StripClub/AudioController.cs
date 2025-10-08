using UnityEngine;
using Zenject;

namespace StripClub;

public class AudioController : MonoBehaviour, ISoundPlayer, IMusicPlayer
{
	[Range(0.05f, 1f)]
	[SerializeField]
	private float musicVolume = 0.2f;

	[Range(0.05f, 1f)]
	[SerializeField]
	private float effectVolume = 0.8f;

	private AudioSource musicSource;

	public bool effectsMuted;

	[Inject]
	private GameSettings settings;

	public bool Mute
	{
		get
		{
			return effectsMuted;
		}
		set
		{
			effectsMuted = value;
		}
	}

	public bool IsPlaying
	{
		get
		{
			if ((bool)musicSource)
			{
				return musicSource.isPlaying;
			}
			return false;
		}
	}

	public void Stop(AudioSource source)
	{
		source.Stop();
	}

	public void Play()
	{
		if (musicSource == null)
		{
			musicSource = base.gameObject.AddComponent<AudioSource>();
			musicSource.volume = musicVolume;
		}
		musicSource.clip = settings.SoundTrack;
		musicSource.Play();
		musicSource.loop = true;
		musicSource.volume = 0.2f;
	}

	public void Pause()
	{
		if (musicSource != null)
		{
			musicSource.Pause();
		}
	}
}

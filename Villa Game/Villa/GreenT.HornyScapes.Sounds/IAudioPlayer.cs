using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

public interface IAudioPlayer
{
	void PlayAudioClip2D(AudioClip clip, bool isLooped = false);

	void PlayOneShotAudioClip2D(AudioClip clip);

	void PlayFadingAudioClip2D(AudioClip clip);

	void StopPlayingClip();

	void ChangeSound(float value);

	void ChangeSound(bool mute);

	void ChangeMusic(float value);

	void ChangeMusic(bool mute);
}

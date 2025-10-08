using DG.Tweening;
using UnityEngine;

public class FadingSoundNode : MonoBehaviour
{
	[SerializeField]
	private AudioSource fadingSource;

	[SerializeField]
	private float fadeDuration = 1f;

	private Sequence seq;

	private float volumeMultiplier = 1f;

	public void Play(AudioClip clip, float volume)
	{
		fadingSource.volume = volume;
		if (fadingSource.clip != clip)
		{
			fadingSource.clip = clip;
			fadingSource.Play();
		}
		else if (!fadingSource.isPlaying)
		{
			fadingSource.Play();
		}
		seq?.Kill();
		seq = DOTween.Sequence().Join(FadingTween(fadeDuration));
		Tween FadingTween(float _duration)
		{
			volumeMultiplier = 1f;
			return DOTween.To(() => volumeMultiplier, delegate(float _newVolumeMultiplier)
			{
				volumeMultiplier = _newVolumeMultiplier;
				fadingSource.volume *= volumeMultiplier;
			}, 0f, _duration);
		}
	}
}

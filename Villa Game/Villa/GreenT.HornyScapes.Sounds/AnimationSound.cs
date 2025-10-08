using System;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public class AnimationSound : MonoBehaviour
{
	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation targetAnimation;

	[SerializeField]
	private SoundSO animationSound;

	private IAudioPlayer audioPlayer;

	private IDisposable soundAnimationStream;

	[Inject]
	private void InnerInit(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
		Subscribe();
	}

	private void Subscribe()
	{
		soundAnimationStream = targetAnimation.OnAnimationEnd.Subscribe(delegate
		{
			audioPlayer.PlayAudioClip2D(animationSound.Sound);
		});
	}

	private void OnDestroy()
	{
		soundAnimationStream?.Dispose();
	}
}

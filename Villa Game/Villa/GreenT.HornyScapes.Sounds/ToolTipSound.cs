using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public abstract class ToolTipSound : MonoBehaviour
{
	[SerializeField]
	protected SoundSO openSoundSO;

	[SerializeField]
	protected SoundSO closeSoundSO;

	protected IAudioPlayer audioPlayer;

	protected readonly CompositeDisposable openCloseStream = new CompositeDisposable();

	protected void OnValidate()
	{
		if (openSoundSO == null)
		{
			Debug.LogError("Tooltips: Empty openSoundSO sound", this);
		}
		if (closeSoundSO == null)
		{
			Debug.LogError("Tooltips: Empty closeSoundSO sound", this);
		}
		ValidateToolTip();
	}

	protected abstract void ValidateToolTip();

	[Inject]
	protected void InnerInit(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
		OnOpenTrigger().Subscribe(delegate
		{
			OnOpen();
		}).AddTo(openCloseStream);
		OnCloseTrigger().Subscribe(delegate
		{
			OnClose();
		}).AddTo(openCloseStream);
	}

	protected abstract IObservable<Unit> OnOpenTrigger();

	protected abstract IObservable<Unit> OnCloseTrigger();

	protected virtual void OnClose()
	{
		audioPlayer.PlayAudioClip2D(closeSoundSO.Sound);
	}

	protected virtual void OnOpen()
	{
		audioPlayer.PlayAudioClip2D(openSoundSO.Sound);
	}

	private void OnDestroy()
	{
		openCloseStream.Dispose();
	}
}

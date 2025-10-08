using System;
using System.Collections.Generic;
using GreenT.HornyScapes.ToolTips;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public class ToolTipUIHowToGetSound : MonoBehaviour
{
	[SerializeField]
	protected HowToGetToolTipOpener source;

	[SerializeField]
	protected SoundSO openSoundSO;

	[SerializeField]
	protected SoundSO closeSoundSO;

	protected IAudioPlayer audioPlayer;

	private CompositeDisposable openCloseStream = new CompositeDisposable();

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
		GetComponent<HowToGetToolTipOpener>(ref source);
		void GetComponent<T>(ref T component) where T : MonoBehaviour
		{
			if (component == null && !TryGetComponent<T>(out component))
			{
				Debug.LogError("Empty component: " + component.GetType(), this);
			}
		}
	}

	[Inject]
	protected void InnerInit(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<HowToGetToolTipView>(source.OnOpen, (Action<HowToGetToolTipView>)delegate
		{
			OnOpen();
		}), (ICollection<IDisposable>)openCloseStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<HowToGetToolTipView>(source.OnClose, (Action<HowToGetToolTipView>)delegate
		{
			OnClose();
		}), (ICollection<IDisposable>)openCloseStream);
	}

	protected void OnClose()
	{
		audioPlayer.PlayAudioClip2D(closeSoundSO.Sound);
	}

	protected void OnOpen()
	{
		audioPlayer.PlayAudioClip2D(openSoundSO.Sound);
	}

	private void OnDestroy()
	{
		openCloseStream.Dispose();
	}
}

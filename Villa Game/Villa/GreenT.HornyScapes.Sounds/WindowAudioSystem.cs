using System;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

public class WindowAudioSystem : MonoBehaviour
{
	[SerializeField]
	protected Window root;

	[SerializeField]
	protected WindowSoundSO windowSoundSO;

	protected IAudioPlayer audioPlayer;

	protected void OnValidate()
	{
		if (windowSoundSO == null)
		{
			Debug.LogError("Empty window sound", this);
		}
		GetComponent<Window>(ref root);
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
		root.OnChangeState += OnOpen;
		root.OnChangeState += OnClose;
	}

	protected void OnClose(object sender, EventArgs e)
	{
		if (e is WindowArgs { Active: false })
		{
			audioPlayer.PlayAudioClip2D(windowSoundSO.Close);
		}
	}

	protected void OnOpen(object sender, EventArgs e)
	{
		if (e is WindowArgs { Active: not false })
		{
			audioPlayer.PlayAudioClip2D(windowSoundSO.Open);
		}
	}
}

using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Animations;

public class PopupWindow : AnimatedWindow
{
	[SerializeField]
	protected Button fadeCloser;

	[SerializeField]
	protected WindowSoundSO windowSoundSO;

	[Inject]
	protected IAudioPlayer audioPlayer;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (windowSoundSO == null)
		{
			Debug.LogError(GetType().Name + ": Empty window sound: " + base.gameObject.name, this);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if ((bool)fadeCloser)
		{
			fadeCloser.onClick.AddListener(Close);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)fadeCloser)
		{
			fadeCloser.onClick.RemoveAllListeners();
		}
	}

	public override void Open()
	{
		audioPlayer.PlayAudioClip2D(windowSoundSO.Open);
		base.Open();
	}

	public override void Close()
	{
		audioPlayer.PlayAudioClip2D(windowSoundSO.Close);
		base.Close();
	}
}

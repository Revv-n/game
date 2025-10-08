using GreenT.HornyScapes.Sounds;
using GreenT.HornyScapes.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class PromocodePopupWindow : AnimatedWindow
{
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
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
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

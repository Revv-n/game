using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

[RequireComponent(typeof(Button))]
public class SoundOnClick : MonoBehaviour
{
	[Inject]
	private IAudioPlayer audioPlayer;

	[SerializeField]
	private Button button;

	[SerializeField]
	private ButtonSoundSO click;

	private void OnValidate()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
	}

	private void Awake()
	{
		button.onClick.AddListener(PlaySound);
	}

	private void PlaySound()
	{
		audioPlayer.PlayOneShotAudioClip2D(click.Sound);
	}

	private void OnDestroy()
	{
		button.onClick.RemoveAllListeners();
	}
}

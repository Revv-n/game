using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sounds;

[RequireComponent(typeof(Button))]
public class SoundOnHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	[Inject]
	private IAudioPlayer audioPlayer;

	[SerializeField]
	private Button button;

	[SerializeField]
	private ButtonSoundSO hover;

	public void OnPointerEnter(PointerEventData eventData)
	{
		audioPlayer?.PlayOneShotAudioClip2D(hover.Sound);
	}

	private void OnValidate()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
	}
}

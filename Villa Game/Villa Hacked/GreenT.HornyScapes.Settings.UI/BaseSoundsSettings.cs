using GreenT.HornyScapes.Sounds;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public abstract class BaseSoundsSettings : MonoBehaviour
{
	[SerializeField]
	protected SoundSettingsSO globalSettings;

	public Toggle ToggleSound;

	public Slider SliderSound;

	protected IAudioPlayer audioPlayer;

	[Inject]
	protected virtual void InnerInit(IAudioPlayer audioPlayer)
	{
		this.audioPlayer = audioPlayer;
		SubscribePair();
	}

	protected void SubscribePair()
	{
		ToggleSound.onValueChanged.AddListener(ChangeToggleState);
		SliderSound.onValueChanged.AddListener(ChangeSliderState);
	}

	protected virtual void ChangeSliderState(float sliderValue)
	{
		ToggleSound.SetIsOnWithoutNotify(sliderValue == 0f);
	}

	protected abstract void ChangeToggleState(bool state);

	private void OnDestroy()
	{
		ToggleSound.onValueChanged.RemoveAllListeners();
		SliderSound.onValueChanged.RemoveAllListeners();
	}
}

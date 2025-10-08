namespace GreenT.HornyScapes.Settings.UI;

public class SoundSettings : BaseSoundsSettings
{
	private void OnEnable()
	{
		SetToggle(globalSettings.SoundVolume.Value);
		SetSlider(globalSettings.SoundVolume.Value);
		void SetToggle(float volume)
		{
			ToggleSound.SetIsOnWithoutNotify(volume == 0f);
		}
	}

	private void SetSlider(float volume)
	{
		SliderSound.SetValueWithoutNotify(volume);
	}

	protected override void ChangeSliderState(float sliderValue)
	{
		base.ChangeSliderState(sliderValue);
		audioPlayer.ChangeSound(sliderValue);
	}

	protected override void ChangeToggleState(bool state)
	{
		audioPlayer.ChangeSound(state);
		SetSlider(globalSettings.SoundVolume.Value);
	}
}

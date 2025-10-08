namespace GreenT.HornyScapes.Settings.UI;

public class MusicSettings : BaseSoundsSettings
{
	private void OnEnable()
	{
		SetToggle(globalSettings.MusicVolume.Value);
		SetSlider(globalSettings.MusicVolume.Value);
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
		audioPlayer.ChangeMusic(sliderValue);
	}

	protected override void ChangeToggleState(bool state)
	{
		audioPlayer.ChangeMusic(state);
		SetSlider(globalSettings.MusicVolume.Value);
	}
}

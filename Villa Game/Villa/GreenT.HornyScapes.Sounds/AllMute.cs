namespace GreenT.HornyScapes.Sounds;

public class AllMute : BaseToggleMute
{
	protected override void InnerInit(IAudioPlayer audioPlayer)
	{
		base.InnerInit(audioPlayer);
		Toggle.SetIsOnWithoutNotify(GlobalSettings.MusicVolume.Value == 0f && GlobalSettings.SoundVolume.Value == 0f);
	}

	protected override void ChangeState(bool mute)
	{
		base.ChangeState(mute);
		audioPlayer.ChangeMusic(mute);
		audioPlayer.ChangeSound(mute);
	}
}

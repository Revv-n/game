namespace GreenT.HornyScapes.Sounds;

public class SoundMute : BaseToggleMute
{
	protected override void ChangeState(bool state)
	{
		base.ChangeState(state);
		audioPlayer.ChangeSound(state);
	}
}

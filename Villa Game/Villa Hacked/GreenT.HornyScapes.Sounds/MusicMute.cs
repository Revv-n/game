namespace GreenT.HornyScapes.Sounds;

public class MusicMute : BaseToggleMute
{
	protected override void ChangeState(bool state)
	{
		base.ChangeState(state);
		audioPlayer.ChangeMusic(state);
	}
}

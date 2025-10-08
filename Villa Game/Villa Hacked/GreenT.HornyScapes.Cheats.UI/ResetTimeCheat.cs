using Zenject;

namespace GreenT.HornyScapes.Cheats.UI;

public class ResetTimeCheat : CheatButton
{
	private TimeRewinder rewindTime;

	[Inject]
	public void Init(TimeRewinder rewindTime)
	{
		this.rewindTime = rewindTime;
	}

	public override void Apply()
	{
		rewindTime.Reset();
	}

	public override bool IsValid()
	{
		return rewindTime.TotalRewindedTime.Ticks != 0;
	}
}

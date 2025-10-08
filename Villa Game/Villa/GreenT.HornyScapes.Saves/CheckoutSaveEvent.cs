using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class CheckoutSaveEvent : SaveEvent
{
	private PlayerStats playerStats;

	[Inject]
	public void Init(PlayerStats playerStats)
	{
		this.playerStats = playerStats;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		playerStats.CheckoutAttemptCount.Skip(1).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

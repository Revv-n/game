using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class OnApplicationSaveEvent : SaveEvent
{
	private bool isTracking;

	private IReadOnlyReactiveProperty<bool> isGameActive;

	[Inject]
	private void Init(GameStarter gameStarter)
	{
		isGameActive = gameStarter.IsGameActive;
	}

	private void OnApplicationPause(bool pause)
	{
		if (isTracking && pause && isGameActive.Value)
		{
			Save();
		}
	}

	private void OnApplicationQuit()
	{
		if (isTracking && isGameActive.Value)
		{
			Save();
		}
	}

	public override void Track()
	{
		isTracking = true;
	}

	public override void StopTrack()
	{
		base.StopTrack();
		isTracking = false;
	}
}

using System;

namespace GreenT.HornyScapes;

public sealed class EventRatingView : RatingView
{
	public event Action<int, int> LeaderboardUpdated;

	protected override void Start()
	{
	}

	protected override void OnSuccess()
	{
		base.OnSuccess();
		this.LeaderboardUpdated?.Invoke(RatingController.TryGetGlobalPlace(), RatingController.TryGetGroupPlace());
	}

	protected override void TryGetController()
	{
		if (RatingController != null)
		{
			RatingController.LeaderboardUpdated -= OnLeaderboardUpdated;
		}
		base.TryGetController();
		RatingController.LeaderboardUpdated += OnLeaderboardUpdated;
	}

	private void OnLeaderboardUpdated(int globalPlace, int groupPlace)
	{
		this.LeaderboardUpdated?.Invoke(globalPlace, groupPlace);
	}
}

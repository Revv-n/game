namespace GreenT.HornyScapes.Events;

public interface ICalendarStateStrategy
{
	bool CheckIfRewarded();

	void OnInProgress();

	void OnComplete();

	void OnRewarded();
}

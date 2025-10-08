using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public interface ITutorialModel
{
	ReactiveProperty<bool> IsComplete { get; }

	ReactiveProperty<bool> IsInProgressStep { get; }

	void StartStep();

	void SetComplete(bool state);
}

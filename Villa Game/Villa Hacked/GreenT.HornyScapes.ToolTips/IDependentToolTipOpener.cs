using UniRx;

namespace GreenT.HornyScapes.ToolTips;

public interface IDependentToolTipOpener<in T> where T : ToolTipSettings
{
	ReactiveProperty<bool> ReadyToActivate { get; }

	void Activate();

	void Deactivate();
}

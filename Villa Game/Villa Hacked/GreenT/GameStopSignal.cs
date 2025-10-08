using UniRx;

namespace GreenT;

public class GameStopSignal
{
	private readonly ReactiveProperty<bool> _isStopped = new ReactiveProperty<bool>(false);

	public IReadOnlyReactiveProperty<bool> IsStopped => (IReadOnlyReactiveProperty<bool>)(object)_isStopped;

	public void Stop()
	{
		if (!_isStopped.Value)
		{
			_isStopped.Value = true;
		}
	}
}

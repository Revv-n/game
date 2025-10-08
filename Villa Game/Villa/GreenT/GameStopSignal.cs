using UniRx;

namespace GreenT;

public class GameStopSignal
{
	private readonly ReactiveProperty<bool> _isStopped = new ReactiveProperty<bool>(initialValue: false);

	public IReadOnlyReactiveProperty<bool> IsStopped => _isStopped;

	public void Stop()
	{
		if (!_isStopped.Value)
		{
			_isStopped.Value = true;
		}
	}
}

using UniRx;

namespace StripClub.Model;

public interface ILocker
{
	IReadOnlyReactiveProperty<bool> IsOpen { get; }

	LockerSourceType Source { get; set; }
}

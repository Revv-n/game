using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;

namespace StripClub.Model;

public class CompositeLocker : ILocker
{
	private IReadOnlyReactiveProperty<bool> _isOpenIReadOnly;

	public List<ILocker> Lockers { get; }

	public IReadOnlyReactiveProperty<bool> IsOpen => GetIsOpen();

	public LockerSourceType Source { get; set; }

	public CompositeLocker(IEnumerable<ILocker> lockers)
	{
		Lockers = lockers.ToList();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(base.ToString());
		stringBuilder.Append(" nested lockers: \n");
		foreach (ILocker locker in Lockers)
		{
			stringBuilder.Append(locker.ToString());
			stringBuilder.Append('\n');
		}
		return stringBuilder.ToString();
	}

	private IReadOnlyReactiveProperty<bool> GetIsOpen()
	{
		if (_isOpenIReadOnly != null)
		{
			return _isOpenIReadOnly;
		}
		IReadOnlyReactiveProperty<bool>[] array = Lockers.Select((ILocker locker) => locker.IsOpen).ToArray();
		_isOpenIReadOnly = (from lockerState in ((IEnumerable<IObservable<bool>>)array).CombineLatest()
			select lockerState.All((bool x) => x)).ToReadOnlyReactiveProperty(array.All((IReadOnlyReactiveProperty<bool> x) => x.Value));
		return _isOpenIReadOnly;
	}
}

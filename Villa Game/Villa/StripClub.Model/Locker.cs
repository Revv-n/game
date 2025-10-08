using System;
using UniRx;

namespace StripClub.Model;

public abstract class Locker : ILocker, IDisposable
{
	protected ReactiveProperty<bool> isOpen = new ReactiveProperty<bool>(initialValue: false);

	private IReadOnlyReactiveProperty<bool> _isOpenReadOnly;

	public IReadOnlyReactiveProperty<bool> IsOpen => GetReadOnlyOpen();

	public LockerSourceType Source { get; set; }

	public virtual void Initialize()
	{
		isOpen.Value = false;
	}

	public virtual void Dispose()
	{
		isOpen?.Dispose();
	}

	public override string ToString()
	{
		return base.ToString() + " Status: " + (isOpen.Value ? "Open" : "Locked");
	}

	private IReadOnlyReactiveProperty<bool> GetReadOnlyOpen()
	{
		return _isOpenReadOnly ?? (_isOpenReadOnly = isOpen.ToReadOnlyReactiveProperty());
	}
}

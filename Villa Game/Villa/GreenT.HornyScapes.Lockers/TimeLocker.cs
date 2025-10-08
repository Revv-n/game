using System;
using StripClub.Model;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Lockers;

public class TimeLocker : Locker, IDisposable
{
	private IDisposable timeUpAwaiter;

	private bool openByDefault;

	public GenericTimer Timer { get; private set; }

	public TimeLocker(bool isOpen = false)
	{
		base.isOpen.Value = isOpen;
		openByDefault = isOpen;
		Timer = new GenericTimer();
		timeUpAwaiter = Timer.OnTimeIsUp.Subscribe(OnTimeIsUp);
	}

	public override void Initialize()
	{
		Stop();
	}

	private void OnTimeIsUp(GenericTimer obj)
	{
		isOpen.Value = openByDefault;
	}

	public void Start(TimeSpan forTime)
	{
		isOpen.Value = !openByDefault;
		Timer.Start(forTime);
	}

	public void Stop()
	{
		Timer.Stop();
		isOpen.Value = openByDefault;
	}

	public override void Dispose()
	{
		base.Dispose();
		ReleaseStreams();
	}

	private void ReleaseStreams()
	{
		timeUpAwaiter?.Dispose();
		Timer?.Dispose();
	}
}

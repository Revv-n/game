using System;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public abstract class BaseAnalytic<T> : BaseAnalytic
{
	protected BaseAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	public void SendEventIfIsValid(T enity)
	{
		if (IsValid(enity))
		{
			SendEventByPass(enity);
		}
	}

	protected virtual bool IsValid(T entity)
	{
		return true;
	}

	public virtual void SendEventByPass(T tuple)
	{
	}
}
public abstract class BaseAnalytic : IDisposable
{
	protected IAmplitudeSender<AmplitudeEvent> amplitude;

	protected CompositeDisposable onNewStream = new CompositeDisposable();

	public BaseAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
	{
		this.amplitude = amplitude;
	}

	public virtual void Track()
	{
	}

	public virtual void Dispose()
	{
		onNewStream.Dispose();
	}
}

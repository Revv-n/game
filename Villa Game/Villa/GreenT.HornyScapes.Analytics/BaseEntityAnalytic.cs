namespace GreenT.HornyScapes.Analytics;

public abstract class BaseEntityAnalytic<T> : BaseAnalytic<T>
{
	protected DisposeDictionary itemsStreams = new DisposeDictionary();

	public BaseEntityAnalytic(IAmplitudeSender<AmplitudeEvent> amplitude)
		: base(amplitude)
	{
	}

	protected virtual void ClearStreams()
	{
		onNewStream.Clear();
		itemsStreams.ClearStreams();
	}

	protected void FreeStream(int id)
	{
		itemsStreams.FreeStream(id);
	}

	public override void Dispose()
	{
		ClearStreams();
		base.Dispose();
	}
}

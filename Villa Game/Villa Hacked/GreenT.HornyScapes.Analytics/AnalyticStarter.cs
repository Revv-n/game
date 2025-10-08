using System;

namespace GreenT.HornyScapes.Analytics;

public class AnalyticStarter : IDisposable
{
	private IDisposable stream;

	private AnalyticSystemManager manager;

	public AnalyticStarter(AnalyticSystemManager manager)
	{
		this.manager = manager;
	}

	public void Start()
	{
		try
		{
			foreach (BaseAnalytic item in manager.Collection)
			{
				item.Track();
			}
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	public void Dispose()
	{
		stream?.Dispose();
		foreach (BaseAnalytic item in manager.Collection)
		{
			item?.Dispose();
		}
	}
}

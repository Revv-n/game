using System.Collections.Generic;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Saves;

public class SaveEventManager : SimpleManager<SaveEvent>
{
	public SaveEventManager()
	{
	}

	public SaveEventManager(List<SaveEvent> events)
	{
		AddRange(events);
	}

	public void Track()
	{
		foreach (SaveEvent item in Collection)
		{
			item.Track();
		}
	}

	public void StopTrack()
	{
		foreach (SaveEvent item in Collection)
		{
			item.StopTrack();
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		foreach (SaveEvent item in Collection)
		{
			item.Dispose();
		}
	}
}

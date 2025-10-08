using GreenT.HornyScapes.Stories;
using StripClub;

namespace GreenT.HornyScapes.Events;

public class StoryDataCleaner : CollectionManager<Story>, IDataCleaner
{
	public void ClearData()
	{
		foreach (Story item in collection)
		{
			item.Initialize();
		}
	}
}

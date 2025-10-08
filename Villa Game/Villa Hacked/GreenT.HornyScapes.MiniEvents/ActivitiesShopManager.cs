using System;
using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class ActivitiesShopManager : SimpleManager<ActivityShopMapper>
{
	public ActivityShopMapper GetActivityInfo(int id)
	{
		ActivityShopMapper activityShopMapper = collection.FirstOrDefault((ActivityShopMapper _event) => _event.tab_id == id);
		if (activityShopMapper == null)
		{
			new Exception().SendException($"{GetType().Name}: activityShop with id {id} didn't load");
		}
		return activityShopMapper;
	}
}

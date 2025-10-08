using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.UI;

namespace GreenT.HornyScapes.UI;

public abstract class OrderedViewManager<T> : ViewManager<T> where T : IView
{
	public virtual T TryGetSortedView(Func<T, object> orderBy, Func<T, bool> firstOrDefault)
	{
		return GetSortedObjects(orderBy).FirstOrDefault(firstOrDefault);
	}

	public virtual List<T> GetSortedObjects(Func<T, object> orderBy)
	{
		return views.OrderBy(orderBy).ToList();
	}

	public abstract void MoveToFirst(T view);

	public abstract void MoveToLast(T view);
}

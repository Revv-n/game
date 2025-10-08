using System.Linq;

namespace GreenT.HornyScapes.Tasks.UI;

public class MergeTaskViewManager : TaskViewManager<TaskView>
{
	public bool TryGetItem(Task hasSource, out TaskView view)
	{
		view = base.VisibleViews.FirstOrDefault((TaskView _view) => _view.Source == hasSource);
		return view != null;
	}
}

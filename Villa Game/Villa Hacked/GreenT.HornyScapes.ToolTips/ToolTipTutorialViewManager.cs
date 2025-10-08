using System.Linq;
using StripClub.UI;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipTutorialViewManager : MonoViewManager<ToolTipTutorialSettings, ToolTipTutorialView>
{
	public ToolTipTutorialView GetLast()
	{
		return views.FindLast((ToolTipTutorialView _view) => _view.IsActive());
	}

	public ToolTipTutorialView GetViewWithStep(int id)
	{
		return views.FirstOrDefault((ToolTipTutorialView _view) => _view.Source.StepID == id);
	}
}

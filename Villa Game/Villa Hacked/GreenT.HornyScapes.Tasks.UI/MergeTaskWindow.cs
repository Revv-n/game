using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class MergeTaskWindow : Window
{
	private MergeTaskViewManagerView mergeTaskViewManagerView;

	[Inject]
	private void InnerInit(MergeTaskViewManagerView mergeTaskViewManagerView)
	{
		this.mergeTaskViewManagerView = mergeTaskViewManagerView;
	}

	private void OnEnable()
	{
		mergeTaskViewManagerView.Initialize();
	}
}

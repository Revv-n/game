using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public sealed class StatusViewController : MonoView
{
	private StatusViewManager _manager;

	[Inject]
	private void Init(StatusViewManager manager)
	{
		_manager = manager;
	}

	public void SetStatus(int status, int count)
	{
		_manager.HideAll();
		for (int i = 0; i < count; i++)
		{
			_manager.GetView().Set(status);
		}
	}
}

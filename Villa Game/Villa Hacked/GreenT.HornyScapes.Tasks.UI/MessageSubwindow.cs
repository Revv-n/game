using GreenT.HornyScapes.UI;
using StripClub.Messenger;
using UniRx;

namespace GreenT.HornyScapes.Tasks.UI;

public class MessageSubwindow : AnimatedSubwindow
{
	private CompositeDisposable onUpdateDialogueStream = new CompositeDisposable();

	private IMessengerManager messengerManager;

	private MessageTaskViewManager messageViewManager;

	protected override void OnDestroy()
	{
		base.OnDestroy();
		onUpdateDialogueStream.Dispose();
	}
}

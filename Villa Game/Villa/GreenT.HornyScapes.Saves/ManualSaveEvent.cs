using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class ManualSaveEvent : SaveEvent
{
	[Inject]
	private ManualSave _save;

	public override void Track()
	{
		Init();
	}

	private void Init()
	{
		_save.SaveEvent.Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

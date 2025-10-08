using System;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class ContentStorageSaveEvent : SaveEvent
{
	private const int TIME_SAVE_DELAY = 5;

	private ContentStorageController contentStorageController;

	[Inject]
	public void Init(ContentStorageController contentStorageController)
	{
		this.contentStorageController = contentStorageController;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		contentStorageController.OnAddPlayer.ThrottleFirst(TimeSpan.FromSeconds(5.0)).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

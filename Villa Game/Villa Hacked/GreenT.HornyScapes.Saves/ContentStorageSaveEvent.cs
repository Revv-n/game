using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.ThrottleFirst<Unit>(contentStorageController.OnAddPlayer, TimeSpan.FromSeconds(5.0)), (Action<Unit>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

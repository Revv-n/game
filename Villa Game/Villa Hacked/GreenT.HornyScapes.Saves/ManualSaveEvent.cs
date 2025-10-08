using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(_save.SaveEvent, (Action<Unit>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

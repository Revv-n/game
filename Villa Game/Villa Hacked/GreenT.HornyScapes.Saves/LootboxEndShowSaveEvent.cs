using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Lootboxes;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class LootboxEndShowSaveEvent : SaveEvent
{
	private ILootboxOpener lootboxOpener;

	[Inject]
	public void Init(ILootboxOpener lootboxOpener)
	{
		this.lootboxOpener = lootboxOpener;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lootbox>(lootboxOpener.OnOpen, (Action<Lootbox>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

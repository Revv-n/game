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
		lootboxOpener.OnOpen.Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

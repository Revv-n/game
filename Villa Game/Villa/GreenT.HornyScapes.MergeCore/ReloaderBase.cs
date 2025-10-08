using System;
using GreenT.HornyScapes.Card;
using GreenT.HornyScapes.Card.Bonus;
using Merge;
using StripClub.Model.Cards;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public abstract class ReloaderBase : IInitializable, IDisposable
{
	private readonly CardsCollectionTracker cardsCollectionTracker;

	private readonly MergeFieldProvider mergeFieldProvider;

	private IDisposable reloadStream;

	protected MergeField mergeField;

	public ReloaderBase(CardsCollectionTracker cardsCollectionTracker, MergeFieldProvider mergeFieldProvider)
	{
		this.cardsCollectionTracker = cardsCollectionTracker;
		this.mergeFieldProvider = mergeFieldProvider;
	}

	public void Initialize()
	{
		reloadStream?.Dispose();
		reloadStream = cardsCollectionTracker.GetAnyPromoteStream().Subscribe(Reload);
	}

	public void Dispose()
	{
		reloadStream?.Dispose();
	}

	private void Reload(ICard card)
	{
		int[] array = (card.Bonus as CharacterMultiplierBonus)?.AffectedSpawnerId;
		if (!mergeFieldProvider.TryGetData(card.ContentType, out var field))
		{
			return;
		}
		mergeField = field;
		int[] array2 = array;
		foreach (int num in array2)
		{
			GameItem[] fieldModules = GetFieldModules();
			foreach (GameItem gameItem in fieldModules)
			{
				if (gameItem.Config.UniqId == num)
				{
					RefreshModules(gameItem);
				}
			}
		}
	}

	protected abstract void RefreshModules(GameItem item);

	protected abstract GameItem[] GetFieldModules();
}

using System;
using System.Collections.Generic;
using UniRx;

namespace StripClub.Model.Cards;

public class PromoteCollection
{
	public Dictionary<int, int> TotalCountByGroup;

	private List<IPromote> collection;

	private Subject<IPromote> onUpdate;

	public IEnumerable<IPromote> Collection => collection;

	public IObservable<IPromote> OnUpdate => onUpdate;

	public PromoteCollection()
	{
		onUpdate = new Subject<IPromote>();
		collection = new List<IPromote>();
		TotalCountByGroup = new Dictionary<int, int>();
	}

	public PromoteCollection(IEnumerable<IPromote> cards)
		: this()
	{
		collection.AddRange(cards);
	}

	public void Add(IPromote card)
	{
		if (!collection.Contains(card))
		{
			collection.Add(card);
			onUpdate.OnNext(card);
		}
	}

	public void AddRange(IEnumerable<IPromote> cards)
	{
		foreach (IPromote card in cards)
		{
			Add(card);
		}
	}
}

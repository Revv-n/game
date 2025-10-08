using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using ModestTree;
using UniRx;
using Zenject;

namespace StripClub.Model.Cards;

public sealed class CardsCollection : IDisposable
{
	private readonly IFactory<ICard, IPromote> promoteFactory;

	private List<ICard> collection;

	private Subject<ICard> newCard;

	private Subject<ICard> newPromote;

	private Subject<ICard> unlockedCard;

	private Subject<(ICard, int)> newSoulsCard;

	private CompositeDisposable disposables = new CompositeDisposable();

	public IEnumerable<ICard> Collection => collection;

	public IObservable<ICard> OnNewCard => (IObservable<ICard>)newCard;

	public IObservable<ICard> OnNewPromote => (IObservable<ICard>)newPromote;

	public IEnumerable<ICard> Owned => Collection.Where(Promote.ContainsKey);

	public IEnumerable<ICard> NotOwned => Collection.Where((ICard _card) => !Promote.ContainsKey(_card));

	public IDictionary<ICard, IPromote> Promote { get; }

	public IObservable<ICard> OnCardUnlock => (IObservable<ICard>)unlockedCard;

	public IObservable<(ICard, int)> OnNewSoulsCard => (IObservable<(ICard, int)>)newSoulsCard;

	private IDictionary<int, int> TotalCountByGroup { get; }

	public CardsCollection(IFactory<ICard, IPromote> promoteFactory)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		newCard = new Subject<ICard>();
		newPromote = new Subject<ICard>();
		unlockedCard = new Subject<ICard>();
		collection = new List<ICard>();
		newSoulsCard = new Subject<(ICard, int)>();
		TotalCountByGroup = new Dictionary<int, int>();
		Promote = new Dictionary<ICard, IPromote>();
		Assert.IsNotNull((object)promoteFactory);
		this.promoteFactory = promoteFactory;
	}

	public CardsCollection(IEnumerable<ICard> cards, IFactory<ICard, IPromote> promoteFactory)
		: this(promoteFactory)
	{
		collection.AddRange(cards);
	}

	public void Add(ICard card)
	{
		if (!collection.Contains(card))
		{
			collection.Add(card);
			if (!TotalCountByGroup.ContainsKey(card.GroupID))
			{
				TotalCountByGroup[card.GroupID] = 0;
			}
			IDictionary<int, int> totalCountByGroup = TotalCountByGroup;
			int groupID = card.GroupID;
			int value = totalCountByGroup[groupID] + 1;
			totalCountByGroup[groupID] = value;
			newCard.OnNext(card);
		}
	}

	public void AddRange(IEnumerable<ICard> cards)
	{
		foreach (ICard card in cards)
		{
			Add(card);
		}
	}

	public void Remove(ICard card)
	{
		collection.Remove(card);
	}

	public void Connect(IPromote promote, ICard card, bool callUnlock = false)
	{
		if (!collection.Contains(card))
		{
			Add(card);
		}
		Promote.Add(card, promote);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.DoOnError<int>((IObservable<int>)promote.Level, (Action<Exception>)delegate
		{
		}), (Action<int>)card.Bonus.SetLevel), (ICollection<IDisposable>)disposables);
		card.Bonus.Apply();
		newPromote.OnNext(card);
		if (callUnlock)
		{
			unlockedCard.OnNext(card);
		}
	}

	public int Count(int groupID = -1)
	{
		if (!TotalCountByGroup.ContainsKey(groupID))
		{
			return 0;
		}
		if (groupID == -1)
		{
			return TotalCountByGroup.Values.Sum();
		}
		return collection.Count((ICard card) => card.GroupID.Equals(groupID) && card.ContentType == ContentType.Main);
	}

	public int CountMain()
	{
		return collection.Count((ICard card) => card.ContentType == ContentType.Main);
	}

	public void DropPromotes()
	{
		foreach (IDisposable item in Promote.Values.OfType<IDisposable>())
		{
			item.Dispose();
		}
		Promote.Clear();
		disposables.Clear();
	}

	public int UnlockedCount(int groupID = -1)
	{
		if (groupID == -1)
		{
			return Promote.Values.Count;
		}
		return Promote.Keys.Count((ICard _card) => _card.GroupID.Equals(groupID) && _card.ContentType == ContentType.Main);
	}

	public IPromote GetPromoteOrDefault(ICard card)
	{
		if (Promote.TryGetValue(card, out var value))
		{
			return value;
		}
		return null;
	}

	public bool IsOwned(ICard card)
	{
		return Promote.ContainsKey(card);
	}

	public void AddSouls(ICard card, int count)
	{
		if (!Promote.TryGetValue(card, out var value))
		{
			value = promoteFactory.Create(card);
			Connect(value, card, callUnlock: true);
		}
		value.AddSoul(count);
		newSoulsCard.OnNext((card, count));
	}

	public IReadOnlyReactiveProperty<bool> AnyIsNew()
	{
		IEnumerable<IReactiveProperty<bool>> enumerable = from _card in Collection.Where(Promote.ContainsKey)
			select Promote[_card].IsNew;
		return ReactivePropertyExtensions.ToReactiveProperty<bool>(Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)enumerable), (Func<IList<bool>, bool>)((IList<bool> _list) => _list.Any((bool x) => x))), enumerable.Any((IReactiveProperty<bool> _isNew) => _isNew.Value));
	}

	public void Dispose()
	{
		disposables.Dispose();
		newCard.OnCompleted();
		newCard.Dispose();
		unlockedCard.OnCompleted();
		unlockedCard.Dispose();
	}
}

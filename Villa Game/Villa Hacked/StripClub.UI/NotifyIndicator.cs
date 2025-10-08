using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class NotifyIndicator : MonoBehaviour
{
	private CardsCollection _cards;

	private RelationshipProvider _relationshipProvider;

	[Inject]
	private void Init(CardsCollection cards, RelationshipProvider relationshipProvider)
	{
		_cards = cards;
		_relationshipProvider = relationshipProvider;
	}

	private void OnEnable()
	{
		IObservable<bool> observable = Observable.AsObservable<bool>((IObservable<bool>)_cards.AnyIsNew());
		IReadOnlyReactiveProperty<bool>[] array = _relationshipProvider.Collection.Select((Relationship relationship) => relationship.WasComingSoonDates).ToArray();
		IObservable<bool> observable2;
		if (array.Length == 0)
		{
			observable2 = Observable.Return(false);
		}
		else
		{
			IObservable<bool>[] array2 = (IObservable<bool>[])array;
			observable2 = Observable.Select<IList<bool>, bool>(Observable.CombineLatest<bool>(array2), (Func<IList<bool>, bool>)((IList<bool> list) => list.Any((bool x) => x)));
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.DistinctUntilChanged<bool>(Observable.CombineLatest<bool, bool, bool>(observable2, observable, (Func<bool, bool, bool>)((bool hasDate, bool hasCard) => hasDate || hasCard))), (Action<bool>)delegate(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		}), (Component)this);
	}
}

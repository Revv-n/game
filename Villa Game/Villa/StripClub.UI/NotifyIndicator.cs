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
		IObservable<bool> right = _cards.AnyIsNew().AsObservable();
		IReadOnlyReactiveProperty<bool>[] array = _relationshipProvider.Collection.Select((Relationship relationship) => relationship.WasComingSoonDates).ToArray();
		IObservable<bool> left;
		if (array.Length == 0)
		{
			left = Observable.Return(value: false);
		}
		else
		{
			IObservable<bool>[] sources = array;
			left = from list in Observable.CombineLatest(sources)
				select list.Any((bool x) => x);
		}
		left.CombineLatest(right, (bool hasDate, bool hasCard) => hasDate || hasCard).DistinctUntilChanged().Subscribe(delegate(bool isActive)
		{
			base.gameObject.SetActive(isActive);
		})
			.AddTo(this);
	}
}

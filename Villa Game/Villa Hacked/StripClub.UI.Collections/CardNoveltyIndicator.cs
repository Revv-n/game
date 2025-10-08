using System;
using System.Collections.Generic;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;

namespace StripClub.UI.Collections;

public class CardNoveltyIndicator : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> indicatorsObjects;

	private IDisposable noveltyStream;

	public IPromote Source { get; private set; }

	public void Init(IPromote promote)
	{
		Source = promote;
		TrackNovelty(promote);
	}

	private void OnEnable()
	{
		if (Source != null)
		{
			TrackNovelty(Source);
		}
	}

	private void TrackNovelty(IPromote promote)
	{
		noveltyStream?.Dispose();
		noveltyStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.TakeUntilDisable<bool>(Observable.TakeWhile<bool>((IObservable<bool>)promote.IsNew, (Func<bool, bool>)((bool _value) => _value)), (Component)this), (Action<bool>)Set, (Action)delegate
		{
			Set(isNew: false);
		}), (Component)this);
	}

	private void Set(bool isNew)
	{
		foreach (GameObject indicatorsObject in indicatorsObjects)
		{
			indicatorsObject.SetActive(isNew);
		}
	}
}

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
		noveltyStream = promote.IsNew.TakeWhile((bool _value) => _value).TakeUntilDisable(this).Subscribe((Action<bool>)Set, (Action)delegate
		{
			Set(isNew: false);
		})
			.AddTo(this);
	}

	private void Set(bool isNew)
	{
		foreach (GameObject indicatorsObject in indicatorsObjects)
		{
			indicatorsObject.SetActive(isNew);
		}
	}
}

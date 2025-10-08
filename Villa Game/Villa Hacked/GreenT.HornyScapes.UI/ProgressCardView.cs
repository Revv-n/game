using System;
using System.Collections.Generic;
using StripClub.Model.Cards;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class ProgressCardView : GeneralCardView
{
	public class Factory : PlaceholderFactory<ProgressCardView>
	{
	}

	[Serializable]
	public class Setting
	{
		public Color GlowFXColor;

		public GameObject Rarity;
	}

	[Serializable]
	public class AnimationPromoteDictionary : SerializableDictionary<Rarity, Setting>
	{
	}

	[SerializeField]
	private TextMeshProUGUI level;

	[SerializeField]
	protected AnimatedProgress progressBar;

	[SerializeField]
	private TextFormatDictionary tmproFields;

	[Inject]
	protected CardsCollection cards;

	protected CompositeDisposable promoteLvlStream = new CompositeDisposable();

	protected IPromote promote { get; private set; }

	public override void Set(ICard card)
	{
		base.Set(card);
		progressBar.gameObject.SetActive(value: true);
		promote = cards.GetPromoteOrDefault(card);
		UpdateProgressTMProValue();
		TrackPromote();
		AnimateProgressBar();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		TrackPromote();
		AnimateProgressBar();
	}

	private void AnimateProgressBar()
	{
		if (promote != null)
		{
			progressBar.AnimateFromZero(promote.Progress.Value, promote.Target);
		}
	}

	protected void TrackPromote()
	{
		promoteLvlStream.Clear();
		if (promote != null)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.TakeUntilDisable<int>((IObservable<int>)promote.Level, (Component)this), (Action<int>)OnUpdateLevel), (ICollection<IDisposable>)promoteLvlStream);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.TakeUntilDisable<int>(Observable.Skip<int>((IObservable<int>)promote.Progress, 1), (Component)this), (Action<int>)UpdateProgressBar), (ICollection<IDisposable>)promoteLvlStream);
		}
	}

	protected virtual void OnUpdateLevel(int level)
	{
		try
		{
			this.level.text = level.ToString();
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	protected void UpdateProgressTMProValue()
	{
		if (promote == null)
		{
			return;
		}
		foreach (KeyValuePair<TextMeshProUGUI, string> tmproField in tmproFields)
		{
			tmproField.Key.text = string.Format(tmproField.Value, promote.Progress.Value, promote.Target);
		}
	}

	protected virtual void UpdateProgressBar(int progressValue)
	{
		progressBar.AnimateFromCurrent(progressValue, promote.Target);
		UpdateProgressTMProValue();
	}

	protected virtual void OnDestroy()
	{
		promoteLvlStream.Dispose();
	}
}

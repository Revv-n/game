using System;
using System.Collections.Generic;
using GreenT;
using GreenT.HornyScapes.Characters;
using GreenT.UI;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class GirlPromoWindow : Window
{
	[SerializeField]
	private NewGirlClip newGirlAppearance;

	private readonly CompositeDisposable currentDisposables = new CompositeDisposable();

	private GirlPromoWindowWrapper girlPromoWindowWrapper;

	private CardsCollection cards;

	[Inject]
	public void Init(GirlPromoWindowWrapper girlPromoWindowWrapper, CardsCollection cards)
	{
		this.girlPromoWindowWrapper = girlPromoWindowWrapper;
		this.cards = cards;
	}

	protected override void Awake()
	{
		base.Awake();
		girlPromoWindowWrapper.SetGirlPromoWindow(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		currentDisposables.Dispose();
	}

	private void OnDisable()
	{
		Reset();
	}

	private void Reset()
	{
		newGirlAppearance.gameObject.SetActive(value: false);
	}

	public void OpenGirlPromo(ICharacter character)
	{
		base.Open();
		Reset();
		try
		{
			bool isGirlNew = !cards.IsOwned(character);
			newGirlAppearance.Play();
			newGirlAppearance.Init(character, isGirlNew);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Clip>(Observable.Catch<Clip, Exception>(Observable.DoOnCompleted<Clip>(Observable.DoOnCancel<Clip>(Observable.Take<Clip>(newGirlAppearance.OnEnd, 1), (Action)Close), (Action)Close), (Func<Exception, IObservable<Clip>>)delegate(Exception ex)
			{
				throw ex.LogException();
			})), (ICollection<IDisposable>)currentDisposables);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}

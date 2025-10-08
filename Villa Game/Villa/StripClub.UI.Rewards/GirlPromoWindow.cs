using System;
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
			newGirlAppearance.OnEnd.Take(1).DoOnCancel(Close).DoOnCompleted(Close)
				.Catch(delegate(Exception ex)
				{
					throw ex.LogException();
				})
				.Subscribe()
				.AddTo(currentDisposables);
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}

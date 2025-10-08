using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.Model.Cards;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class CardSetter : MonoBehaviour
{
	[SerializeField]
	private CardView cardView;

	private PromoteWindow promoteWindow;

	private CardsCollection cards;

	private IWindowsManager windowsOpener;

	[Inject]
	private void Init(IWindowsManager windowsOpener, CardsCollection cards)
	{
		this.windowsOpener = windowsOpener;
		this.cards = cards;
	}

	private void Awake()
	{
		promoteWindow = windowsOpener.Get<PromoteWindow>();
	}

	private void OnValidate()
	{
		if (cardView == null)
		{
			cardView = GetComponent<CardView>();
		}
	}

	public void PushCard()
	{
		if (cards.GetPromoteOrDefault(cardView.Source) != null)
		{
			promoteWindow.Set(cardView.Source);
			promoteWindow.Open();
		}
	}
}

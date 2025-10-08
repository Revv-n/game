using GreenT.HornyScapes.Characters;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class SummonRouletteCardDropView : MonoView
{
	public class Manager : ViewManager<SummonRouletteCardDropView>
	{
	}

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private StatableComponent[] _statables;

	public ICharacter Source { get; private set; }

	public void Set(ICharacter character, Sprite cardIcon)
	{
		Source = character;
		_icon.sprite = cardIcon;
		SetStatables(character.Rarity);
	}

	private void SetStatables(Rarity rarity)
	{
		StatableComponent[] statables = _statables;
		for (int i = 0; i < statables.Length; i++)
		{
			statables[i].Set((int)rarity);
		}
	}
}

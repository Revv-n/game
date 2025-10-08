using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.UI;

public class RecipeBookView : MonoView
{
	[SerializeField]
	private Image _recipeBook;

	public void Set(Sprite sprite)
	{
		if (!(sprite == null))
		{
			_recipeBook.sprite = sprite;
		}
	}
}

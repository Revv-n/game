using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events.UI;

public class RecipeBookButtonView : MonoView
{
	[SerializeField]
	private Image _view;

	public void Set(Sprite sprite)
	{
		if (!(sprite == null))
		{
			_view.sprite = sprite;
		}
	}
}

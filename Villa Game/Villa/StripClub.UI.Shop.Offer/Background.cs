using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Shop.Offer;

public class Background : MonoBehaviour
{
	[SerializeField]
	private Image girl;

	[field: SerializeField]
	public Image Backplate { get; private set; }

	public void Setup(Sprite girlSprite)
	{
		if (girl != null)
		{
			girl.color = Color.white;
			girl.sprite = girlSprite;
		}
	}
}

using System.Collections.Generic;
using GreenT.HornyScapes.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Card.UI;

public class CharacterPlaceholderSpriteCollection : SpriteCollection<int>
{
	[SerializeField]
	private List<Sprite> sprites = new List<Sprite>();

	public override Sprite Get(int param)
	{
		int index = (int)((float)param % 100f % (float)sprites.Count);
		return sprites[index];
	}
}

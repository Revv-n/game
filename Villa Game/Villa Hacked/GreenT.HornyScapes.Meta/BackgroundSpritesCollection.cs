using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Meta;

public class BackgroundSpritesCollection : ReactiveDictionary<int, Sprite>
{
	public void AddRange(IEnumerable<Sprite> sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			IEnumerable<char> enumerable = sprite.name.Where(char.IsDigit);
			int num = 0;
			foreach (char item in enumerable)
			{
				int num2 = item - 48;
				num = num * 10 + num2;
			}
			if (!base.ContainsKey(num - 1))
			{
				base.Add(num - 1, sprite);
			}
		}
	}

	public void ReplaceRange(IEnumerable<Sprite> sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			IEnumerable<char> enumerable = sprite.name.Where(char.IsDigit);
			int num = 0;
			foreach (char item in enumerable)
			{
				int num2 = item - 48;
				num = num * 10 + num2;
			}
			if (base.ContainsKey(num - 1))
			{
				base[num - 1] = sprite;
			}
			else
			{
				base.Add(num - 1, sprite);
			}
		}
	}
}

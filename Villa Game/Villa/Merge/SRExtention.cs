using UnityEngine;

namespace Merge;

public static class SRExtention
{
	public static void SetAlpha(this SpriteRenderer img, float a)
	{
		Color color = img.color;
		color.a = a;
		img.color = color;
	}

	public static float GetAlpha(this SpriteRenderer img)
	{
		return img.color.a;
	}

	public static SpriteRenderer SetSprite(this SpriteRenderer img, Sprite sprite)
	{
		img.sprite = sprite;
		return img;
	}

	public static SpriteRenderer SetOrder(this SpriteRenderer img, int order)
	{
		img.sortingOrder = order;
		return img;
	}

	public static SpriteRenderer SetOrder(this SpriteRenderer img, string layer, int order)
	{
		img.sortingOrder = order;
		img.sortingLayerName = layer;
		return img;
	}

	public static SpriteRenderer SetOrder(this SpriteRenderer img, SpriteRenderer source, int offset = 0)
	{
		img.sortingOrder = source.sortingOrder + offset;
		img.sortingLayerName = source.sortingLayerName;
		return img;
	}
}

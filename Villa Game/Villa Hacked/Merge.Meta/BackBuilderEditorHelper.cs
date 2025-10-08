using System.Collections.Generic;
using UnityEngine;

namespace Merge.Meta;

public class BackBuilderEditorHelper : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer tilePrefab;

	[SerializeField]
	private Vector2 offset;

	[SerializeField]
	private int columnsCount;

	[SerializeField]
	private int rowsCount;

	[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private bool isBuilding = true;

	private List<SpriteRenderer> tiles = new List<SpriteRenderer>();

	private void OnValidate()
	{
		if (isBuilding)
		{
			Build();
		}
	}

	[ContextMenu("Rebuild background")]
	private void Build()
	{
		DestroyPreviousTiles();
		int num = columnsCount * rowsCount;
		float x = base.transform.position[0];
		float y = base.transform.position[1];
		int num2 = 0;
		while (num2 != num && num2 != sprites.Length)
		{
			SpriteRenderer spriteRenderer = CreateTile(sprites[num2], new Vector2(x, y));
			tiles.Add(spriteRenderer);
			x = spriteRenderer.bounds.max.x;
			num2++;
			if (num2 % columnsCount == 0)
			{
				x = base.transform.position[0];
				y = spriteRenderer.bounds.min.y;
			}
		}
	}

	private SpriteRenderer CreateTile(Sprite sprite, Vector2 upperLeftCorner)
	{
		Vector2 vector = sprite.bounds.size;
		float x = upperLeftCorner[0] + base.transform.localScale[0] * vector[0] * sprite.pivot[0] / sprite.rect.size[0];
		float y = upperLeftCorner[1] - base.transform.localScale[1] * vector[1] * sprite.pivot[1] / sprite.rect.size[1];
		SpriteRenderer obj = Object.Instantiate(position: new Vector2(x, y), original: tilePrefab, rotation: Quaternion.identity, parent: base.transform);
		obj.sprite = sprite;
		return obj;
	}

	private void DestroyPreviousTiles()
	{
		foreach (SpriteRenderer tile in tiles)
		{
			Object.DestroyImmediate(tile);
		}
		tiles.Clear();
	}
}

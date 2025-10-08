using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class FieldMonoMediator : MonoBehaviour
{
	[Header("GameItemController:")]
	public GameObject Container;

	public Transform FieldParent;

	public SpriteRenderer FieldBackground;

	[Header("TileController:")]
	public Sprite OutframeSp;

	public BackTile LightTile;

	public BackTile DarkTile;

	public Tile[] Tiles;

	public Transform TilesParent;
}

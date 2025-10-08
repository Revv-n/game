using UnityEngine;

namespace Merge;

public class Tile : AbstractTile, IHasCoordinates
{
	[SerializeField]
	private SubTile subTile;

	public SubTile SubTile => subTile;
}

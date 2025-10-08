using MISC.Resolution;
using UnityEngine;

namespace Merge;

[CreateAssetMenu(fileName = "TilesConfig", menuName = "DL/Configs/Controllers/Tiles")]
public class TilesConfig : ScriptableObject
{
	[SerializeField]
	private ResolutionType resolution;

	[SerializeField]
	private float tileSize;

	[SerializeField]
	private float spacing;

	[SerializeField]
	private float outerFrameSize;

	[SerializeField]
	private float innerFrameSize;

	[SerializeField]
	private Point size;

	[SerializeField]
	private Vector2 rootOffset;

	public float TileSize => tileSize;

	public Point FieldSize => size;

	public float OuterFrameSize => outerFrameSize;

	public float InnerFrameSize => innerFrameSize;

	public float Spacing => spacing;

	public ResolutionType Resolution => resolution;

	public Vector2 RootOffset => rootOffset;
}

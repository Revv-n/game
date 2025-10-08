using UnityEngine;

namespace Merge.MotionDesign;

public abstract class RoomObjectEffect : MonoBehaviour
{
	public abstract void Play(params SpriteRenderer[] roView);
}

using UnityEngine;

namespace Merge.MotionDesign;

public class DestructionAnimUnit : MonoBehaviour
{
	public void Destroy()
	{
		Object.Destroy(base.gameObject);
	}
}

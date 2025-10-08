using UnityEngine;

namespace Merge.MotionDesign;

public class ShineCircleCreator : MonoBehaviour
{
	private SmartPool<MergeAllownEffect> effectPool;

	[SerializeField]
	private float circleSize = 1f;

	[SerializeField]
	private float raySize = 1f;

	[SerializeField]
	private MergeAllownEffect prefab;

	public SmartPool<MergeAllownEffect> Pool => effectPool;

	public void Init(Transform root)
	{
		effectPool = new SmartPool<MergeAllownEffect>(prefab, root);
		effectPool.OnItemPop += AtCreated;
	}

	private void AtCreated(MergeAllownEffect obj)
	{
		obj.SetCircleSize(circleSize);
		obj.SetRaySize(raySize);
	}
}

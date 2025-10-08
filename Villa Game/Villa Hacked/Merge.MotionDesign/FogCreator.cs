using UnityEngine;

namespace Merge.MotionDesign;

public class FogCreator : MonoBehaviour
{
	[SerializeField]
	private DropFog prefab;

	[SerializeField]
	private Vector3 offset;

	private SmartPool<DropFog> effectPool;

	public SmartPool<DropFog> Pool => effectPool;

	private void Start()
	{
		effectPool = new SmartPool<DropFog>(prefab, base.transform);
	}

	public void CreateFog(Vector3 position)
	{
		DropFog dropFog = effectPool.Pop();
		dropFog.transform.position = position + offset;
		dropFog.SetActive(active: true);
	}
}

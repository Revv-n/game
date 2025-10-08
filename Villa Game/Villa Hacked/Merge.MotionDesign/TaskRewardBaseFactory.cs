using StripClub.UI;
using UnityEngine;

namespace Merge.MotionDesign;

public abstract class TaskRewardBaseFactory<T> : MonoBehaviourFactory<T> where T : FlyingCurrency
{
	public Transform Container;

	public override T Create()
	{
		T val = base.Create();
		val.transform.SetParent(Container);
		val.transform.localScale = Vector3.one;
		return val;
	}
}

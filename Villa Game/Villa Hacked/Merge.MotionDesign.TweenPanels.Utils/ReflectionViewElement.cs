using UnityEngine;

namespace Merge.MotionDesign.TweenPanels.Utils;

public abstract class ReflectionViewElement : MonoBehaviour
{
	protected object target;

	protected string property;

	public void Init(object target, string property)
	{
		this.target = target;
		this.property = property;
		OnInit();
	}

	protected abstract void OnInit();
}

using System;
using UnityEngine;

namespace Merge;

public abstract class ModuleActionOperator : MonoBehaviour
{
	public abstract GIModuleType Type { get; }

	public bool IsActive { get; protected set; }

	public abstract event Action<ModuleActionOperator> OnAction;

	public abstract GIBox.Base GetBox();

	public abstract void SetBox(GIBox.Base box);

	public virtual void Deactivate()
	{
		base.gameObject.SetActive(value: false);
		IsActive = false;
	}
}

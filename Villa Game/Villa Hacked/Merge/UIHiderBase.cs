using UnityEngine;

namespace Merge;

public abstract class UIHiderBase : MonoBehaviour
{
	public abstract bool IsVisible { get; protected set; }

	public abstract void DoVisible(bool visible);

	public abstract void SetVisible(bool visible);
}

using UnityEngine;

namespace GreenT.Data;

public abstract class ScritpableObjectWrapper<T> : ScriptableObject
{
	public T Object { get; protected set; }
}

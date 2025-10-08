using System;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public abstract class AbstractProgress<T> : MonoBehaviour, IProgress<T> where T : struct, IComparable<T>, IEquatable<T>
{
	public abstract T Progress { get; }

	public abstract void Init(T value, T max, T min = default(T));

	public abstract void Init(T relativeProgress);

	public abstract bool IsComplete();
}

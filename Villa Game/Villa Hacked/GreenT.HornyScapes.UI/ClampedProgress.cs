using System;
using StripClub.Extensions;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public abstract class ClampedProgress<T> : AbstractProgress<T> where T : struct, IComparable<T>, IEquatable<T>
{
	[Range(0f, 1f)]
	[SerializeField]
	protected float MaxFillAmount = 1f;

	[Range(0f, 1f)]
	[SerializeField]
	protected float MinFillAmount;

	protected virtual float Clamp(float relativeProgress)
	{
		return MathExtensions.Clamp(relativeProgress * (MaxFillAmount - MinFillAmount) + MinFillAmount, MinFillAmount, MaxFillAmount);
	}
}

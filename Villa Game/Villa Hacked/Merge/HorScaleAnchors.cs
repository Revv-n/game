using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct HorScaleAnchors
{
	[SerializeField]
	private RectTransform minLeft;

	[SerializeField]
	private RectTransform maxLeft;

	[SerializeField]
	private RectTransform minRight;

	[SerializeField]
	private RectTransform maxRight;

	[SerializeField]
	private float minMul;

	public bool InLeftSimple(Transform tr)
	{
		return tr.position.x < minLeft.position.x;
	}

	public bool InRightSimple(Transform tr)
	{
		return tr.position.x > minRight.position.x;
	}

	public bool InCentre(Transform tr)
	{
		if (tr.position.x > maxLeft.position.x)
		{
			return tr.position.x < maxRight.position.x;
		}
		return false;
	}

	public bool InLeftMulable(Transform tr)
	{
		if (tr.position.x < maxLeft.position.x)
		{
			return tr.position.x > minLeft.position.x;
		}
		return false;
	}

	public bool InRightMulable(Transform tr)
	{
		if (tr.position.x > maxRight.position.x)
		{
			return tr.position.x < minRight.position.x;
		}
		return false;
	}

	public float GetLeftMulableScale(Transform tr)
	{
		return GetScaleByOffset(tr.position.x - minLeft.position.x);
	}

	public float GetRightMulableScale(Transform tr)
	{
		return GetScaleByOffset(minRight.position.x - tr.position.x);
	}

	public float GetMaxScale()
	{
		return 1f;
	}

	public float GetMinScale()
	{
		return minMul;
	}

	public void ValidateItem(Transform tr)
	{
		if (InLeftSimple(tr) || InRightSimple(tr))
		{
			tr.SetScale(GetMinScale());
		}
		else if (InCentre(tr))
		{
			tr.SetScale(GetMaxScale());
		}
		else if (InLeftMulable(tr))
		{
			tr.SetScale(GetLeftMulableScale(tr));
		}
		else if (InRightMulable(tr))
		{
			tr.SetScale(GetRightMulableScale(tr));
		}
	}

	public float GetScaleByOffset(float offset)
	{
		float num = offset / (maxLeft.position.x - minLeft.position.x);
		return (1f - minMul) * num + minMul;
	}
}

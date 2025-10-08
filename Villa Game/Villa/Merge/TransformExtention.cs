using UnityEngine;

namespace Merge;

public static class TransformExtention
{
	public static void SetX(this Transform tr, float value)
	{
		tr.position = new Vector3(value, tr.position.y, tr.position.z);
	}

	public static void SetY(this Transform tr, float value)
	{
		tr.position = new Vector3(tr.position.x, value, tr.position.z);
	}

	public static void SetZ(this Transform tr, float value)
	{
		tr.position = new Vector3(tr.position.x, tr.position.y, value);
	}

	public static void SetLocalX(this Transform tr, float value)
	{
		tr.localPosition = new Vector3(value, tr.localPosition.y, tr.localPosition.z);
	}

	public static void SetLocalY(this Transform tr, float value)
	{
		tr.localPosition = new Vector3(tr.localPosition.x, value, tr.localPosition.z);
	}

	public static void SetLocalZ(this Transform tr, float value)
	{
		tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, value);
	}

	public static void SetScale(this Transform tr, float value)
	{
		tr.localScale = Vector3.one * value;
	}

	public static void SetScaleX(this Transform tr, float value)
	{
		tr.localScale = new Vector3(value, tr.localScale.y, tr.localScale.z);
	}

	public static void SetScaleY(this Transform tr, float value)
	{
		tr.localScale = new Vector3(tr.localScale.x, value, tr.localScale.z);
	}

	public static void SetScaleZ(this Transform tr, float value)
	{
		tr.localScale = new Vector3(tr.localScale.x, tr.localScale.y, value);
	}

	public static void SetLocalRotationX(this Transform tr, float value)
	{
		tr.localEulerAngles = new Vector3(value, tr.localEulerAngles.y, tr.localEulerAngles.z);
	}

	public static void SetLocalRotationY(this Transform tr, float value)
	{
		tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, value, tr.localEulerAngles.z);
	}

	public static void SetLocalRotationZ(this Transform tr, float value)
	{
		tr.localEulerAngles = new Vector3(tr.localEulerAngles.x, tr.localEulerAngles.y, value);
	}

	public static void SetDefault(this Transform tr)
	{
		tr.SetScale(1f);
		tr.localPosition = Vector3.zero;
	}
}

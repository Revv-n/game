using UnityEngine;

namespace GreenT.Utilities;

[ExecuteInEditMode]
public class AdvancedBezier : Bezier
{
	[Header("Отклонение от прямой по пендикуляру (В,С)")]
	[SerializeField]
	private float extremumFactor = -0.4f;

	[Header("Удаленность (В,С) от (А,D)")]
	[SerializeField]
	[Range(0f, 1f)]
	private float extremumDistance = 0.15f;

	[Header("Рандомная дисперсия точек (B,C)")]
	[SerializeField]
	[Range(0f, 10f)]
	private float maxMediateDispersion = 0.7f;

	[SerializeField]
	private bool isRightSide = true;

	[SerializeField]
	private bool isSyncSide;

	public void SetDispersion(float dispersion)
	{
		maxMediateDispersion = dispersion;
	}

	private void AdjustMediatePoint()
	{
		float dist = Vector3.Distance(Point(0), Point(3));
		Vector3 dir = Vector3.Normalize(Point(3) - Point(0));
		Vector3 vector = Vector2.Perpendicular(dir);
		float num = UnityEngine.Random.Range(0f - maxMediateDispersion, maxMediateDispersion);
		Vector3 temp = vector * dist * extremumFactor * (1f + num);
		Vector3 position = CalcPointPosition(0, isHead: true);
		Vector3 position2 = CalcPointPosition(3, isHead: false, isSyncSide);
		controlPoints[1].position = position;
		controlPoints[2].position = position2;
		Vector3 CalcDispersion(bool _isHead)
		{
			if (!_isHead)
			{
				return -(dir * dist * extremumDistance);
			}
			return dir * dist * extremumDistance;
		}
		Vector3 CalcPointPosition(int ind, bool isHead, bool _syncDirection = true)
		{
			return Point(ind) + CalcDispersion(isHead) + ChooseSide(_syncDirection && isRightSide);
		}
		Vector3 ChooseSide(bool _syncDirection)
		{
			if (!_syncDirection)
			{
				return -temp;
			}
			return temp;
		}
	}

	protected override Vector3[] CalculatePoints()
	{
		AdjustMediatePoint();
		return base.CalculatePoints();
	}
}

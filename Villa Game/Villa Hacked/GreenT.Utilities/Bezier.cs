using System.Diagnostics;
using StripClub.Utility;
using UnityEngine;

namespace GreenT.Utilities;

public class Bezier : MonoBehaviour
{
	[SerializeField]
	private bool enablePreview;

	[SerializeField]
	private Color GismoColor = Color.white;

	[SerializeField]
	private float gismoSize = 6f;

	[SerializeField]
	protected Transform[] controlPoints;

	[SerializeField]
	[Range(0.01f, 0.1f)]
	protected float step = 0.0623f;

	[ReadOnly]
	public Transform fromObject;

	[ReadOnly]
	public Transform toObject;

	public Vector3[] Points { get; private set; }

	[Conditional("UNITY_EDITOR")]
	protected void OnDrawGizmos()
	{
		if (enablePreview)
		{
			Points = CalculatePoints();
			DrawGismo();
			DrawLineBetweenDots(0, 1);
			DrawLineBetweenDots(2, 3);
		}
		void DrawGismo()
		{
			Color color = Gizmos.color;
			Gizmos.color = GismoColor;
			Vector3[] points = Points;
			for (int i = 0; i < points.Length; i++)
			{
				Gizmos.DrawSphere(points[i], gismoSize);
			}
			Gizmos.color = color;
		}
	}

	private void DrawLineBetweenDots(int dot1, int dot2)
	{
		Gizmos.DrawLine(Point(dot1), Point(dot2));
	}

	protected Vector3 Point(int dot)
	{
		return new Vector2(GetX(dot), GetY(dot));
		float GetX(int _dot)
		{
			return controlPoints[_dot].position.x;
		}
		float GetY(int _dot)
		{
			return controlPoints[_dot].position.y;
		}
	}

	protected virtual void OnValidate()
	{
		Points = CalculatePoints();
	}

	public void SetDestination(Transform startPoint, Transform target)
	{
		fromObject = startPoint;
		toObject = target;
		SetDestination(startPoint.position, target.position);
	}

	public void SetDestination(Vector3 startPoint, Vector3 target)
	{
		controlPoints[0].position = startPoint;
		controlPoints[controlPoints.Length - 1].position = target;
		Points = CalculatePoints();
	}

	public Vector3[] GetPath(Transform startPoint, Transform target)
	{
		fromObject = startPoint;
		toObject = target;
		return GetPath(fromObject.position, target.position);
	}

	public Vector3[] GetPath(Vector3 startPoint, Vector3 target)
	{
		controlPoints[0].position = startPoint;
		controlPoints[controlPoints.Length - 1].position = target;
		Points = CalculatePoints();
		return Points;
	}

	protected virtual Vector3[] CalculatePoints()
	{
		int num = Mathf.CeilToInt(1f / step);
		Vector3[] array = new Vector3[num + 1];
		float num2 = 0f;
		int num3 = 0;
		while (num3 != num)
		{
			array[num3] = Mathf.Pow(1f - num2, 3f) * controlPoints[0].position + 3f * Mathf.Pow(1f - num2, 2f) * num2 * controlPoints[1].position + 3f * (1f - num2) * Mathf.Pow(num2, 2f) * controlPoints[2].position + Mathf.Pow(num2, 3f) * controlPoints[3].position;
			num3++;
			num2 += step;
		}
		array[array.Length - 1] = controlPoints[controlPoints.Length - 1].position;
		return array;
	}
}

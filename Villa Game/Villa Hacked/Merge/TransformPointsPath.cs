using System;
using DG.Tweening;
using UnityEngine;

namespace Merge;

[Serializable]
public struct TransformPointsPath
{
	public Transform start;

	public Transform end;

	public Transform walker;

	public void ToStart()
	{
		walker.position = start.position;
	}

	public void ToEnd()
	{
		walker.position = end.position;
	}

	public Tween DoWalk(float time, bool direct)
	{
		return walker.DOMove(direct ? end.position : start.position, time);
	}
}

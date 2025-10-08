using DG.Tweening;
using UnityEngine;

namespace Merge.Core.Tasks;

public class TasksWindowShowTweener : MonoBehaviour
{
	[SerializeField]
	private TransformPointsPath titlePP;

	[SerializeField]
	private TransformPointsPath bodyPP;

	[SerializeField]
	private float time;

	private Tween showTween;

	public Tween DoShowTween(float contentOffset = 0f)
	{
		return DoTween(direct: true, contentOffset);
	}

	public Tween DoHideTween()
	{
		return DoTween(direct: false);
	}

	private Tween DoTween(bool direct, float contentOffset = 0f)
	{
		if (direct)
		{
			titlePP.ToStart();
			bodyPP.ToStart();
		}
		else
		{
			titlePP.ToEnd();
			bodyPP.ToEnd();
		}
		showTween?.Kill();
		Sequence sequence = DOTween.Sequence();
		sequence.Join(titlePP.DoWalk(time, direct));
		sequence.Join(bodyPP.DoWalk(time, direct));
		showTween = sequence;
		return sequence;
	}
}

using System;
using UniRx;
using UnityEngine;

namespace StripClub;

public class RelativeProgress : ICompletable<float>
{
	private Subject<float> onUpdate = new Subject<float>();

	public float Progress { get; private set; }

	public float Target { get; } = 1f;


	public IObservable<float> OnProgressUpdate => onUpdate;

	public RelativeProgress(float progress)
	{
		Progress = progress;
	}

	public void Set(float progress)
	{
		float num = Mathf.Clamp01(progress);
		if (Progress != num)
		{
			Progress = num;
			onUpdate.OnNext(num);
		}
	}

	public bool IsComplete()
	{
		return Progress == Target;
	}
}

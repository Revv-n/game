using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ShowHideDefaultTransformAnimation : DefaultTransformAnimation
{
	[SerializeField]
	private GameObject target;

	[Header("Hide/Show")]
	[SerializeField]
	private bool state;

	private bool baseState;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (!target)
		{
			target = base.gameObject;
		}
	}

	public override void Init()
	{
		base.Init();
		baseState = base.gameObject.activeInHierarchy;
	}

	public override Sequence Play()
	{
		sequence = base.Play().OnComplete(Complete);
		return sequence;
	}

	protected override void Complete()
	{
		base.Complete();
		base.gameObject.SetActive(state);
	}

	public override void ResetToAnimStart()
	{
		base.ResetToAnimStart();
		target.SetActive(baseState);
	}
}

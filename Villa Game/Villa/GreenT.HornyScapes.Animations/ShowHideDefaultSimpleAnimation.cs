using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ShowHideDefaultSimpleAnimation : DefaultSimpleAnimation
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
		sequence = base.Play();
		return sequence;
	}

	protected override void Complete()
	{
		if (target.activeSelf != state)
		{
			target.SetActive(state);
		}
		base.Complete();
	}

	public override void ResetToAnimStart()
	{
		target.SetActive(baseState);
		base.ResetToAnimStart();
	}
}

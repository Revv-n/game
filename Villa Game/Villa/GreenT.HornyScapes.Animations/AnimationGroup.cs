using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Extensions;
using StripClub.Extensions;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public abstract class AnimationGroup : Animation
{
	[Tooltip("Количество повторений цепочки. Бесконечное повторение при значение -1")]
	[SerializeField]
	protected int loops;

	[SerializeField]
	protected LoopType loopType;

	[Header("EditorOnly. Назначение.")]
	[SerializeField]
	private string description;

	[SerializeField]
	protected List<Animation> animations = new List<Animation>();

	public List<Animation> Animations => animations;

	protected virtual void OnValidate()
	{
		if (!Application.isPlaying && animations == null)
		{
			animations = GetComponents<Animation>().Except(this.AsEnumerable()).ToList();
		}
	}

	public override void Init()
	{
		base.Init();
		foreach (Animation animation in animations)
		{
			if (animation != null)
			{
				animation.Init();
				continue;
			}
			Debug.LogError(new NullReferenceException("Есть null элементы в списке [animation] в компаненте [AnimationGroup] обьекта [" + base.gameObject.transform.GetFullPath() + "]/[" + base.gameObject.name + "] "));
		}
	}

	public override void ResetToAnimStart()
	{
		for (int num = animations.Count - 1; num != -1; num--)
		{
			animations[num].ResetToAnimStart();
		}
	}
}

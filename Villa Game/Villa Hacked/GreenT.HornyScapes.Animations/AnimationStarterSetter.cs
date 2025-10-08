using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimationStarterSetter : MonoBehaviour
{
	[SerializeField]
	private AnimationStarter starter;

	[SerializeField]
	private List<AnimationController> controllers = new List<AnimationController>();

	public void SetController(int controllerId)
	{
		if (controllers.Count > controllerId)
		{
			starter.SetController(controllers[controllerId]);
			return;
		}
		throw new Exception($"Can't set controller by id: {controllerId}");
	}
}

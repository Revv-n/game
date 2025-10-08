using System;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class TransformStates : StatableComponent
{
	[Serializable]
	public class TransformSettings
	{
		public Vector3 position;

		public Vector3 rotation;
	}

	[SerializeField]
	private Transform target;

	[SerializeField]
	private TransformSettings[] settings = new TransformSettings[0];

	public override void Set(int stateNumber)
	{
		target.transform.position = settings[stateNumber].position;
		target.transform.rotation = Quaternion.Euler(settings[stateNumber].rotation);
	}
}

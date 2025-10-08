using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class OnEnableAnimationStarter : MonoBehaviour
{
	[SerializeField]
	private Animation target;

	private void OnEnable()
	{
		target.Play();
	}

	private void OnDisable()
	{
		target.Stop();
	}
}

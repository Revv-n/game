using System;
using GreenT.UI;
using UnityEngine;

namespace Merge.Core.Collection;

public class ContainerWindowGrid : MonoBehaviour
{
	[Serializable]
	public struct State
	{
		public int padding;

		public float spacing;
	}

	[SerializeField]
	private FlexibleGridLayoutGroup target;

	[SerializeField]
	private State smallState;

	[SerializeField]
	private State bigState;

	[SerializeField]
	private int smallTreshold;

	public void SetSizeByItemsCount(int count)
	{
		State state = ((count > smallTreshold) ? bigState : smallState);
		target.padding.left = state.padding;
		target.padding.right = state.padding;
		target.spacing = new Vector2(state.spacing, target.spacing.y);
	}
}

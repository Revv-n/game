using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonTextPreset", menuName = "DL/Configs/Presets/ButtonText")]
public class ButtonTextPreset : ScriptableObject
{
	[Serializable]
	public struct State
	{
		public Color textColor;

		public Color outlineColor;

		public Vector2 outlineStrange;
	}

	[SerializeField]
	private State lockedState;

	[SerializeField]
	private State normalState;

	public State LockedState => lockedState;

	public State NormalState => normalState;
}

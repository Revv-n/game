using System;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public struct CharacterShowHideConfig
{
	public bool enabled;

	public float extentsCoefWhenNonVisible;

	public float extentsCoefWhenVisible;

	public float loadTimeStep;

	public float releaseDelay;
}

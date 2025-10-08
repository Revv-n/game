using System;
using UnityEngine;

namespace StripClub.Model.Character;

[Serializable]
public class SkinImages
{
	[field: SerializeField]
	public Sprite CardImage { get; private set; }

	[field: SerializeField]
	public Sprite Icon { get; private set; }
}

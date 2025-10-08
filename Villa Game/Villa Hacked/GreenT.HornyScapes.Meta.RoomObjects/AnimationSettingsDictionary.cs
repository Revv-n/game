using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public class AnimationSettingsDictionary : SerializableDictionary<int, CharacterAnimationSettings>
{
	public AnimationSettingsDictionary()
	{
	}

	public AnimationSettingsDictionary(AnimationSettingsDictionary keyValuePairs)
	{
		foreach (KeyValuePair<int, CharacterAnimationSettings> keyValuePair in keyValuePairs)
		{
			Add(keyValuePair.Key, keyValuePair.Value);
		}
	}
}

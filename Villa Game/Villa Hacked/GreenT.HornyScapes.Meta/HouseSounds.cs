using GreenT.HornyScapes.Sounds;
using UnityEngine;

namespace GreenT.HornyScapes.Meta;

[CreateAssetMenu(fileName = "HouseSoundsConfig", menuName = "GreenT/Sound/HouseSounds")]
public class HouseSounds : ScriptableObject
{
	[field: SerializeField]
	public HouseSoundsDictionary HouseSoundsDictionary { get; private set; }

	public SoundSO GetSound(SoundType soundType)
	{
		return HouseSoundsDictionary[soundType];
	}
}

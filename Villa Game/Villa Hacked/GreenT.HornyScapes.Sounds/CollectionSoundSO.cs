using System.Collections.Generic;
using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "CollectionSounds", menuName = "GreenT/Sound/CollectionSounds")]
public class CollectionSoundSO : ScriptableObject
{
	[SerializeField]
	private List<AudioClip> sounds = new List<AudioClip>();

	public List<AudioClip> Sounds => sounds;
}

using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "RoomObjectSound", menuName = "GreenT/Sound/RoomObject/Sound")]
public class RoomObjectSoundSO : SoundSO
{
	public AudioClip OnDestroyTrash => base.Sound;
}

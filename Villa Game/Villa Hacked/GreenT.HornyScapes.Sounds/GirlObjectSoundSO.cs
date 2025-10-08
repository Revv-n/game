using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "GirlSound", menuName = "GreenT/Sound/RoomObject/Girl")]
public class GirlObjectSoundSO : SoundSO
{
	public AudioClip OnClick => base.Sound;
}

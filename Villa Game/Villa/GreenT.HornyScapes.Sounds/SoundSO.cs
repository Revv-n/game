using System;
using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[Serializable]
[CreateAssetMenu(fileName = "Sound", menuName = "GreenT/Sound/Sound")]
public class SoundSO : ScriptableObject
{
	[SerializeField]
	private AudioClip sound;

	[Header("Assets/Recources/[SoundPath]")]
	[SerializeField]
	private string SoundPath;

	public AudioClip Sound => sound;
}

using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "GlobalAudioSound", menuName = "GreenT/Sound/GlobalAudio")]
public class SoundSettingsSO : ScriptableObject
{
	public ReactiveProperty<float> MusicVolume = new ReactiveProperty<float>(0.5f);

	public ReactiveProperty<float> SoundVolume = new ReactiveProperty<float>(0.5f);

	public float DefaultSoundVolume = 0.5f;

	public float DefaultMusicVolume = 0.5f;

	private string saveMusicKey = "MusicVolume";

	private string saveSoundKey = "SoundVolume";

	public void LoadAllData()
	{
		LoadMusicData();
		LoadSoundData();
	}

	public void LoadSoundData()
	{
		SoundVolume.Value = PlayerPrefs.GetFloat(saveSoundKey, 0.5f);
	}

	public void LoadMusicData()
	{
		MusicVolume.Value = PlayerPrefs.GetFloat(saveMusicKey, 0.5f);
	}

	public void SaveAllData()
	{
		PlayerPrefs.SetFloat(saveMusicKey, MusicVolume.Value);
		PlayerPrefs.SetFloat(saveSoundKey, SoundVolume.Value);
		PlayerPrefs.Save();
	}
}

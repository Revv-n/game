using Hedge.Tools;
using UnityEngine;

namespace StripClub.Model.Data;

internal static class UserSettings
{
	internal static void Save(ISoundPlayer soundPlayer, IMusicPlayer musicPlayer)
	{
		PlayerPrefs.SetInt("MuteSounds", soundPlayer.Mute ? 1 : 0);
		PlayerPrefs.SetInt("PlayMusic", musicPlayer.IsPlaying ? 1 : 0);
		PlayerPrefs.SetInt("NumericFormat", (int)FormatGameText.Format);
		PlayerPrefs.SetInt("SFW", 0);
		PlayerPrefs.Save();
	}

	internal static void Load(ISoundPlayer soundPlayer, IMusicPlayer musicPlayer)
	{
		soundPlayer.Mute = ((PlayerPrefs.GetInt("MuteSounds", 0) != 0) ? true : false);
		if (PlayerPrefs.GetInt("PlayMusic", 1) != 0)
		{
			musicPlayer.Play();
		}
		FormatGameText.Format = (FormatGameText.NumberFormat)PlayerPrefs.GetInt("NumericFormat", (int)FormatGameText.Format);
		PlayerPrefs.GetInt("SFW", 0);
	}
}

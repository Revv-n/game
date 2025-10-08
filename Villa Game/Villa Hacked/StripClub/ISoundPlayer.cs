using UnityEngine;

namespace StripClub;

public interface ISoundPlayer
{
	bool Mute { get; set; }

	void Stop(AudioSource source);
}

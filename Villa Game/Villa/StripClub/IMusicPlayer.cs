namespace StripClub;

public interface IMusicPlayer
{
	bool IsPlaying { get; }

	void Play();

	void Pause();
}

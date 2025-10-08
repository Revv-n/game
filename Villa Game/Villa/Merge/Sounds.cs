namespace Merge;

public static class Sounds
{
	public static void Play(string name)
	{
		Controller<SoundController>.Instance.PlaySound(name);
	}
}

public static class Vibration
{
	private static bool initialized;

	public static void Init()
	{
		if (!initialized)
		{
			initialized = true;
		}
	}

	public static void VibratePop()
	{
	}

	public static void VibratePeek()
	{
	}

	public static void VibrateNope()
	{
	}

	public static void Vibrate(long milliseconds)
	{
	}

	public static void Vibrate(long[] pattern, int repeat)
	{
	}

	public static void Cancel()
	{
	}

	public static bool HasVibrator()
	{
		return false;
	}

	public static void Vibrate()
	{
	}
}

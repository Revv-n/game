namespace GreenT;

public static class UserState
{
	public static bool Contains(this User.State state, User.State flags)
	{
		return (state & flags) == flags;
	}
}

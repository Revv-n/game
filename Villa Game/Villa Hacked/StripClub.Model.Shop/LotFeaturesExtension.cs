namespace StripClub.Model.Shop;

public static class LotFeaturesExtension
{
	public static bool Contains(this Stickers state, Stickers flags)
	{
		return (state & flags) == flags;
	}
}

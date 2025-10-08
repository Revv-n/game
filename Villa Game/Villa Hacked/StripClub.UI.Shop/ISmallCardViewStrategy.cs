using GreenT.UI;

namespace StripClub.UI.Shop;

public interface ISmallCardViewStrategy
{
	void UpdateConstraint(FlexibleGridLayoutGroup layoutGroup, int dropsCount);
}

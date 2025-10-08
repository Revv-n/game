using GreenT.Types;

namespace GreenT.HornyScapes.BannerSpace;

public interface IBannerCluster
{
	bool HaveReadyBanner(int id);

	Banner GetForTab(ContentType contentType, int tabID);
}

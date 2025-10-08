namespace StripClub.UI;

public interface ILoadingScreen
{
	void SetLoadingScreenActive(bool isActive);

	void SetProgress(float progress, float animationInSec);

	void SetOnConfigsLoaded();
}

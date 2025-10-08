using GreenT.Localizations.Settings;
using GreenT.Settings.Data;

namespace GreenT.Settings;

public interface IProjectSettings
{
	IRequestUrlResolver RequestUrlResolver { get; }

	IConfigUrlResolver ConfigUrlResolver { get; }

	ILocalizationUrlResolver LocalizationUrlResolver { get; }

	IBundleUrlResolver BundleUrlResolver { get; }

	IGameScenes GameScenes { get; }

	SerializedScene LoginScene { get; }
}

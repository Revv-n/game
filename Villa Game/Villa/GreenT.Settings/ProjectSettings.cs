using GreenT.Localizations.Settings;
using GreenT.Settings.Data;
using StripClub.Utility;
using UnityEngine;

namespace GreenT.Settings;

[CreateAssetMenu(menuName = "GreenT/Project Settings")]
public class ProjectSettings : ScriptableObject, IProjectSettings
{
	[ReadOnly]
	[SerializeField]
	public RequestSettings requestUrlResolver;

	[ReadOnly]
	[SerializeField]
	public ConfigLoadingSettingsBase configUrlResolver;

	[ReadOnly]
	[SerializeField]
	public LocalizationLoadingSettingsBase localizationUrlResolver;

	[ReadOnly]
	[SerializeField]
	public BundlesLoadingSettings bundleUrlResolver;

	[ReadOnly]
	[SerializeField]
	public GameScenesCollection gameScenes;

	public IRequestUrlResolver RequestUrlResolver => requestUrlResolver;

	public IConfigUrlResolver ConfigUrlResolver => configUrlResolver;

	public ILocalizationUrlResolver LocalizationUrlResolver => localizationUrlResolver;

	public IBundleUrlResolver BundleUrlResolver => bundleUrlResolver;

	public IGameScenes GameScenes => gameScenes;

	[field: SerializeField]
	public SerializedScene LoginScene { get; private set; }
}

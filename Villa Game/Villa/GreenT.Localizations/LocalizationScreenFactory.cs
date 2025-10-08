using System.Linq;
using GreenT.HornyScapes.Settings.UI;
using UnityEngine;
using Zenject;

namespace GreenT.Localizations;

public class LocalizationScreenFactory : MonoBehaviour, IFactory<LanguageSettingsLoadingScreen>, IFactory
{
	[SerializeField]
	private LanguageSettingsLoadingScreen _prefab;

	[SerializeField]
	private RectTransform _parentObject;

	[SerializeField]
	private Canvas _canvas;

	private DiContainer _container;

	[Inject]
	public void Init(DiContainer diContainer)
	{
		_container = diContainer;
	}

	public LanguageSettingsLoadingScreen Create()
	{
		DiContainer diContainer = _container.ParentContainers.FirstOrDefault((DiContainer parentContainer) => parentContainer.HasBinding(typeof(LanguageSettingsLoadingScreen)));
		LanguageSettingsLoadingScreen obj = ((diContainer != null) ? diContainer.Resolve<LanguageSettingsLoadingScreen>() : _container.InstantiatePrefabForComponent<LanguageSettingsLoadingScreen>(_prefab, _parentObject));
		obj.SetCanvas(_canvas);
		return obj;
	}
}

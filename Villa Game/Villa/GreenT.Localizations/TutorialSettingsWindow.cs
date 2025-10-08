using GreenT.HornyScapes.Settings.UI;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.Localizations;

public class TutorialSettingsWindow : Window
{
	[SerializeField]
	private Canvas _tutorialCanvas;

	[SerializeField]
	private int _tutorialToolTipOrder;

	[SerializeField]
	private GameObject _openButton;

	[SerializeField]
	private GameObject _root;

	[SerializeField]
	private ScaleButton _openLanguageSettingsLoadingScreenButton;

	[SerializeField]
	private WindowOpener _languageSettingsLoadingScreenWindowOpener;

	private LanguageSettingsLoadingScreen _languageSettingsLoadingScreen;

	private int _tutorialCanvasSortingOrder;

	private int _languageCanvasSortingOrder;

	[Inject]
	public void Construct(LanguageSettingsLoadingScreen languageSettingsLoadingScreen)
	{
		_languageSettingsLoadingScreen = languageSettingsLoadingScreen;
	}

	protected override void Awake()
	{
		base.Awake();
		_openLanguageSettingsLoadingScreenButton.onClick.AddListener(OnLanguageClick);
	}

	private void OnLanguageClick()
	{
		_languageSettingsLoadingScreenWindowOpener.Click();
		_languageSettingsLoadingScreen.Canvas.sortingOrder = _tutorialToolTipOrder + 1;
	}

	public override void Open()
	{
		_openButton.SetActive(value: false);
		_root.SetActive(value: true);
		_tutorialCanvasSortingOrder = _tutorialCanvas.sortingOrder;
		_languageCanvasSortingOrder = _languageSettingsLoadingScreen.Canvas.sortingOrder;
		_tutorialCanvas.sortingOrder = _tutorialToolTipOrder;
		base.Open();
	}

	public override void Close()
	{
		_openButton.SetActive(value: true);
		_root.SetActive(value: false);
		_tutorialCanvas.sortingOrder = _tutorialCanvasSortingOrder;
		_languageSettingsLoadingScreen.Canvas.sortingOrder = _languageCanvasSortingOrder;
		base.Close();
	}
}

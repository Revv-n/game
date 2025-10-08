using GreenT.HornyScapes.Content;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class RewardMergeItemCardView : MonoBehaviour
{
	private const string nameKey = "item.";

	private const string nameSubKey = "ui.hint.item.";

	[SerializeField]
	private Image fx;

	[SerializeField]
	private TextMeshProUGUI itemName;

	[SerializeField]
	private TextMeshProUGUI itemSubName;

	[SerializeField]
	private Image item;

	[SerializeField]
	private GameObject card;

	[SerializeField]
	private TextMeshProUGUI quantityText;

	[SerializeField]
	private GameObject cardBack;

	private LocalizationService _localizationService;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public void Set(MergeItemLinkedContent content)
	{
		fx.gameObject.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		card.SetActive(value: true);
		SetCardBack(state: false);
		SetActiveQuantity(state: true);
		item.sprite = content.GetIcon();
		_localizationDisposables.Clear();
		string key = "item." + content.GameItemConfig.Key.ToString();
		string key2 = "ui.hint.item." + content.GameItemConfig.Key.ToString();
		_localizationService.ObservableText(key).Subscribe(delegate(string text)
		{
			itemName.text = text;
		}).AddTo(_localizationDisposables);
		_localizationService.ObservableText(key2).Subscribe(delegate(string text)
		{
			itemSubName.text = text;
		}).AddTo(_localizationDisposables);
		quantityText.text = "+" + content.Quantity;
	}

	public void SetCardBack(bool state)
	{
		cardBack.SetActive(state);
	}

	public void SetActiveQuantity(bool state)
	{
		quantityText.gameObject.SetActive(state);
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDisable()
	{
		_localizationDisposables.Clear();
	}

	private void OnDestroy()
	{
		_localizationDisposables.Dispose();
	}
}

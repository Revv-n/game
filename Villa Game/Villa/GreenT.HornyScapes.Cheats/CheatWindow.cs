using System;
using System.Linq;
using GreenT.Cheats;
using GreenT.Data;
using GreenT.HornyScapes._HornyScapes._Scripts.Cheats;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Monetization;
using GreenT.UI;
using Merge;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using StripClub.Test;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatWindow : Window
{
	[SerializeField]
	private InputSettingCheats inputSettingCheats;

	public CheatMaintenance Maintenance;

	public CheatNetConnection NetConnection;

	[SerializeField]
	private TMP_InputField inputField;

	[SerializeField]
	private BonusCheatWindow bonusCheatWindow;

	[SerializeField]
	private GameObject elemntsGroup;

	[SerializeField]
	private Button addSoftMoneyBtn;

	[SerializeField]
	private Button addHardMoneyBtn;

	[SerializeField]
	private Button addEnergyBtn;

	[SerializeField]
	private Button addStarsBtn;

	[SerializeField]
	private Button startDialueBtn;

	[SerializeField]
	private Button levelDowngradeBtn;

	[SerializeField]
	private Button levelDownGirlBtn;

	[SerializeField]
	private Button AddExpBtn;

	[SerializeField]
	private Button assetBundleReportBtn;

	[SerializeField]
	private Button deleteSaveBtn;

	[SerializeField]
	private Toggle bubbleCheat;

	[SerializeField]
	private Toggle chestCheat;

	[SerializeField]
	private Toggle bundleRetryTest;

	[SerializeField]
	private TMP_InputField antiInputField;

	[SerializeField]
	private CheatWindowButton windowButton;

	[SerializeField]
	private Button switchRegionBtn;

	[SerializeField]
	private TMP_Text switchRegionText;

	[SerializeField]
	private bool showOnStart;

	private HouseCheat houseCheat;

	private CardsCollection cardsCollection;

	private IRegionPriceResolver _regionPriceResolver;

	private ICurrencyProcessor currencyProcessor;

	private MergeController mergeController;

	private GameStarter gameStarter;

	private CheatConsole cheatConsole;

	private ChestController chestController;

	[EditorButton]
	public void Reload()
	{
		gameStarter.RestartApplication();
	}

	protected override void Awake()
	{
		base.Awake();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, IPlayerExpController playerExpController, CardsCollection cardsCollection, ConsoleCanvas cheatConsole, HouseCheat houseCheat, Saver saver, GameStarter gameStarter, IRegionPriceResolver regionPriceResolver)
	{
		this.currencyProcessor = currencyProcessor;
		this.houseCheat = houseCheat;
		this.cardsCollection = cardsCollection;
		this.gameStarter = gameStarter;
		_regionPriceResolver = regionPriceResolver;
		this.cheatConsole = cheatConsole.CheatConsole;
	}

	public void ChangeVisibility(bool state)
	{
		if (state)
		{
			Open();
		}
		else
		{
			Close();
		}
		elemntsGroup.SetActive(state);
	}

	private void Start()
	{
	}

	private void DeletePrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	private void StartOnlyAndroidPlatformCheats()
	{
		Maintenance.Track();
		NetConnection.Track();
	}

	private void SetupRegionSwitch()
	{
		(from _ in switchRegionBtn.OnClickAsObservable()
			where _regionPriceResolver != null
			select _).Subscribe(delegate
		{
			string text = ReadString();
			_regionPriceResolver.UpdatePrices(text);
			switchRegionText.text = text;
		}).AddTo(this);
	}

	private void DownGradeGirl()
	{
		int girlId = ReadValue();
		cardsCollection.Promote.Keys.FirstOrDefault((ICard _card) => _card.ID == girlId);
	}

	private void DownGradeLevel()
	{
		int.TryParse(antiInputField.text, out var _);
	}

	private void TrackDialogue()
	{
		Check(inputField.text);
		inputField.onValueChanged.AddListener(delegate(string _input)
		{
			Check(_input);
		});
		void Check(string input)
		{
			if (int.TryParse(inputField.text, out var _))
			{
				startDialueBtn.interactable = true;
			}
			else
			{
				startDialueBtn.interactable = false;
			}
		}
	}

	public void SetLotOfBubble(bool state)
	{
		if (mergeController == null)
		{
			mergeController = UnityEngine.Object.FindObjectOfType<MergeController>();
		}
		mergeController.SetTestState(state);
	}

	public void SetFastChest(bool state)
	{
		if (chestController == null)
		{
			chestController = UnityEngine.Object.FindObjectOfType<ChestController>();
		}
		chestController.SetTestState(state);
	}

	public void SetRetryValue(bool state)
	{
	}

	private Action<int> GetSimpleCurrency(CurrencyType type)
	{
		return delegate(int value)
		{
			currencyProcessor.TryChangeAmount(type, value);
		};
	}

	private void RaiseValue(Action<int> action, string additionalInfo = "")
	{
		int obj = ReadValue();
		action(obj);
	}

	private string ReadString()
	{
		string text = inputField.text;
		inputField.text = string.Empty;
		return text;
	}

	private int ReadValue()
	{
		int result;
		int result2 = ((!int.TryParse(inputField.text, out result)) ? 1 : result);
		inputField.text = string.Empty;
		return result2;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		bubbleCheat.onValueChanged.RemoveAllListeners();
		addSoftMoneyBtn.onClick.RemoveAllListeners();
		addHardMoneyBtn.onClick.RemoveAllListeners();
		addEnergyBtn.onClick.RemoveAllListeners();
		levelDowngradeBtn.onClick.RemoveAllListeners();
		assetBundleReportBtn.onClick.RemoveAllListeners();
		deleteSaveBtn.onClick.RemoveAllListeners();
	}
}

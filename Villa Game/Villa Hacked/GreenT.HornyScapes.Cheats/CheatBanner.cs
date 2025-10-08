using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GreenT.HornyScapes.BannerSpace;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatBanner : MonoBehaviour
{
	private DropService _dropService;

	private BannerCluster _bannerCluster;

	private ICurrencyProcessor _currencyProcessor;

	private ContentType _contentType;

	private OnlyState _onlyState;

	private int _id;

	private int _count;

	public TMP_InputField InputField;

	public TMP_InputField CountInputField;

	public TMP_Dropdown ContentTypeDropdown;

	public TMP_Dropdown OnlyStateDropdown;

	public Button AddContractsButton;

	public Button GenerateReportButton;

	[Inject]
	public void Initialization(DropService dropService, BannerCluster bannerCluster, ICurrencyProcessor currencyProcessor)
	{
		_dropService = dropService;
		_bannerCluster = bannerCluster;
		_currencyProcessor = currencyProcessor;
	}

	private void OnEnable()
	{
		InputField.text = string.Empty;
		CountInputField.text = string.Empty;
	}

	private void Start()
	{
		SetupDropdown<ContentType>(ContentTypeDropdown, OnContentTypeChanged);
		SetupDropdown<OnlyState>(OnlyStateDropdown, OnOnlyStateChanged);
		ObservableExtensions.Subscribe<string>(Observable.Select<Unit, string>(UnityEventExtensions.AsObservable((UnityEvent)AddContractsButton.onClick), (Func<Unit, string>)((Unit _) => InputField.text)), (Action<string>)AddContracts);
		ObservableExtensions.Subscribe<Unit>(UnityEventExtensions.AsObservable((UnityEvent)GenerateReportButton.onClick), (Action<Unit>)delegate
		{
			GenerateReport();
		});
	}

	private void AddContracts(string input)
	{
		if (int.TryParse(input, out var result))
		{
			_currencyProcessor.TryAdd(CurrencyType.Contracts, result);
		}
	}

	private void OnContentTypeChanged(int index)
	{
		_contentType = (ContentType)index;
	}

	private void OnOnlyStateChanged(int index)
	{
		_onlyState = (OnlyState)index;
	}

	private void SetupDropdown<TEnum>(TMP_Dropdown dropdown, UnityAction<int> onChanged) where TEnum : Enum
	{
		dropdown.ClearOptions();
		List<string> options = Enum.GetNames(typeof(TEnum)).ToList();
		dropdown.AddOptions(options);
		dropdown.value = 0;
		dropdown.onValueChanged.AddListener(onChanged);
	}

	private void GenerateReport()
	{
		if (!int.TryParse(InputField.text, out _id))
		{
			Debug.LogError("Invalid ID input");
			return;
		}
		if (!int.TryParse(CountInputField.text, out _count))
		{
			Debug.LogError("Invalid ID input");
			return;
		}
		Banner banner = _bannerCluster.Get(_contentType, _id);
		if (banner == null)
		{
			Debug.LogError($"Banner not found: {_contentType} {_id}");
			return;
		}
		Price<int> price = new Price<int>(0, CurrencyType.Contracts, default(CompositeIdentificator));
		_dropService.Drop(banner, _count, price, out var dropAnalytics, addDrop: false);
		string contents = GenerateReport(dropAnalytics.Infos, banner);
		string text = DateTime.Now.ToString("yy.MM.dd_HH.mm_");
		string text2 = Path.Combine(Application.dataPath, "Cheat", "Banner");
		string path = Path.Combine(text2, text + "DropReport.md");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		File.WriteAllText(path, contents);
	}

	private string GenerateReport(List<DropAnalytics.Info> infos, Banner banner)
	{
		if (infos == null || infos.Count == 0)
		{
			return "# Отчёт DropAnalytics\n\n*(нет данных для отображения)*";
		}
		int count = infos.Count;
		decimal num = infos.Sum((DropAnalytics.Info info) => info.Price.Value);
		string value = infos.First().Price.Currency.ToString();
		int num2 = infos.Count((DropAnalytics.Info info) => info.Rarity == Rarity.Common);
		int num3 = infos.Count((DropAnalytics.Info info) => info.Rarity == Rarity.Rare);
		int num4 = infos.Count((DropAnalytics.Info info) => info.Rarity == Rarity.Epic);
		int num5 = infos.Count((DropAnalytics.Info info) => info.Rarity == Rarity.Legendary);
		_ = (double)num2 * 100.0 / (double)count;
		double num6 = (double)num3 * 100.0 / (double)count;
		double num7 = (double)num4 * 100.0 / (double)count;
		double num8 = (double)num5 * 100.0 / (double)count;
		int value2 = infos.Count((DropAnalytics.Info info) => info.Rarity == Rarity.Legendary && info.IsMain);
		int num9 = infos.Count((DropAnalytics.Info info) => info.IsMain);
		double num10 = ((num5 > 0) ? infos.Where((DropAnalytics.Info info) => info.Rarity == Rarity.Legendary).Average((DropAnalytics.Info info) => info.LegendaryStep) : 0.0);
		double num11 = ((num9 > 0) ? infos.Where((DropAnalytics.Info info) => info.IsMain).Average((DropAnalytics.Info info) => info.MainStep) : 0.0);
		int value3 = infos.Min((DropAnalytics.Info info) => info.Step);
		int value4 = infos.Max((DropAnalytics.Info info) => info.Step);
		double num12 = infos.Average((DropAnalytics.Info info) => info.Step);
		var orderedEnumerable = from info in infos
			group info by info.LootboxID into @group
			select new
			{
				LootboxID = @group.Key,
				Count = @group.Count()
			} into g
			orderby g.LootboxID
			select g;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("# Отчёт DropAnalytics");
		stringBuilder.AppendLine("## Общая информация:");
		stringBuilder.AppendLine($"- Banner ID {banner.Id}");
		stringBuilder.AppendLine("- Banner Group " + banner.BannerGroup);
		stringBuilder.Append("- Общее количество дропов: ").Append(count).AppendLine();
		stringBuilder.Append("- Общая сумма потраченной валюты: ").AppendFormat("{0:0.##}", num).AppendLine();
		stringBuilder.Append("- Валюта: ").AppendLine(value);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Распределение по редкости:");
		stringBuilder.Append("- Rare: ").Append(num3).Append(" (")
			.AppendFormat("{0:0.##}", num6)
			.AppendLine("%)");
		stringBuilder.Append("- Epic: ").Append(num4).Append(" (")
			.AppendFormat("{0:0.##}", num7)
			.AppendLine("%)");
		stringBuilder.Append("- Legendary: ").Append(num5).Append(" (")
			.AppendFormat("{0:0.##}", num8)
			.AppendLine("%)");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Легендарные дропы:");
		stringBuilder.Append("- Количество легендарных дропов: ").Append(num5).AppendLine();
		stringBuilder.Append("- Из них основных: ").Append(value2).AppendLine();
		stringBuilder.Append("- Средний LegendaryStep: ").AppendFormat("{0:0.##}", num10).AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Основные дропы:");
		stringBuilder.Append("- Количество основных дропов: ").Append(num9).AppendLine();
		stringBuilder.Append("- Средний MainStep: ").AppendFormat("{0:0.##}", num11).AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Распределение по шагам:");
		stringBuilder.Append("- Минимальный Step: ").Append(value3).AppendLine();
		stringBuilder.Append("- Максимальный Step: ").Append(value4).AppendLine();
		stringBuilder.Append("- Средний Step: ").AppendFormat("{0:0.##}", num12).AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Распределение по LootboxID:");
		foreach (var item in orderedEnumerable)
		{
			stringBuilder.Append("- Lootbox ").Append(item.LootboxID).Append(": ")
				.Append(item.Count)
				.AppendLine();
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("## Как менялся Legendary GarantChance:");
		int num13 = 1;
		foreach (DropAnalytics.Info info in infos)
		{
			stringBuilder.Append($"- {num13} Step ").Append(info.LegendaryStep).Append(": Chance ")
				.Append(banner.LegendaryChance.GetValue(info.LegendaryStep));
			if (info.Rarity == Rarity.Legendary)
			{
				if (info.IsMain)
				{
					stringBuilder.AppendLine($" ==== IsMain {info.LootboxID}  === !");
				}
				else
				{
					stringBuilder.AppendLine($" ==== {info.Rarity} {info.LootboxID} ===");
				}
			}
			else
			{
				stringBuilder.AppendLine($" ==== {info.Rarity} {info.LootboxID}");
			}
			num13++;
		}
		stringBuilder.AppendLine("## Как менялся Main Chance:");
		num13 = 1;
		foreach (DropAnalytics.Info info2 in infos)
		{
			stringBuilder.Append($"- {num13} Step ").Append(info2.MainStep).Append(": Chance ")
				.Append(banner.MainRewardChance.GetValue(info2.MainStep));
			if (info2.Rarity == Rarity.Legendary && info2.IsMain)
			{
				stringBuilder.AppendLine($" ==== IsMain {info2.LootboxID} === !");
			}
			else
			{
				stringBuilder.AppendLine();
			}
			num13++;
		}
		return stringBuilder.ToString();
	}
}

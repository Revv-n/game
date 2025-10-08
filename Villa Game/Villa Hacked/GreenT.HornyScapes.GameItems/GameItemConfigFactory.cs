using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using Merge;
using Merge.Core.Balance;
using ModestTree;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.GameItems;

public class GameItemConfigFactory : IFactory<GameItemMapper, GIConfig>, IFactory
{
	private static RecipeManager _recipeManager;

	public GameItemConfigFactory(RecipeManager recipeManager)
	{
		_recipeManager = recipeManager;
	}

	public GIConfig Create(GameItemMapper mapper)
	{
		return BuildConfig(mapper);
	}

	private GIConfig BuildConfig(GameItemMapper mapper)
	{
		int num = ParsingUtils.ParseInt(mapper.UniqID);
		if (num == 0)
		{
			return null;
		}
		int id = ParsingUtils.ParseInt(mapper.ID);
		string collection = mapper.Collection;
		ContentType contentType = ((!mapper.Bundle.Equals("Main")) ? ContentType.Event : ContentType.Main);
		string bundle = mapper.Bundle;
		GameItemType gameItemType = ParsingUtils.ParseEnum(mapper.Type, GameItemType.Item);
		string name = mapper.Name;
		string description = mapper.Description;
		bool notAffectedAll = ParsingUtils.ParseBool(mapper.NoAffectAll);
		HowToGetType[] howToGetTypeAdditionalWay = new HowToGetType[1] { ParsingUtils.ParseEnum(mapper.HowToGetType, HowToGetType.None) };
		GIKey key = new GIKey(id, collection);
		List<ModuleConfigs.Base> list = new List<ModuleConfigs.Base>(16);
		if (TryBuildMergeModule(mapper, out var result))
		{
			list.Add(result);
		}
		if (TryBuildCollectModule(mapper, out var result2))
		{
			list.Add(result2);
		}
		if (TryBuildSellModule(mapper, out var result3))
		{
			list.Add(result3);
		}
		if (TryBuildAutoSpawnModule(mapper, out var result4))
		{
			list.Add(result4);
		}
		if (TryBuildClickSpawnModule(mapper, out var result5))
		{
			list.Add(result5);
		}
		if (TryBuildChestModule(mapper, out var result6))
		{
			list.Add(result6);
		}
		if (TryBuildTeslaModule(mapper, out var result7))
		{
			list.Add(result7);
		}
		if (TryBuildMixerModule(mapper, out var result8))
		{
			list.Add(result8);
		}
		if (TryBuildStackModule(mapper, out var result9))
		{
			list.Add(result9);
		}
		if (TryBuildMergePointsModule(mapper, out var result10, result))
		{
			list.Add(result10);
		}
		if (TryBuildMergeShopModule(mapper, out var result11))
		{
			list.Add(result11);
		}
		return new GIConfig(num, gameItemType, key, name, description, notAffectedAll, howToGetTypeAdditionalWay, contentType, bundle, list);
	}

	private bool TryBuildMergeModule(GameItemMapper mapper, out ModuleConfigs.Merge result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsMerge))
		{
			return false;
		}
		GIData mergeResult = ParsingUtils.ParseGIData(mapper.MergeResult);
		List<GIData> bonus = ParsingUtils.ParseGIDataList(mapper.MergeBonus);
		float bonusChance = ParsingUtils.ParseFloat(mapper.MergeBubbleChance) / 100f;
		List<WeightNode<GIData>> bonusMergeResult = ParsingUtils.ParseGIDataWeightList(mapper.MergeBubbleList);
		result = new ModuleConfigs.Merge(mergeResult, bonus, bonusChance, bonusMergeResult);
		return true;
	}

	private bool TryBuildCollectModule(GameItemMapper mapper, out ModuleConfigs.Collect result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsCollect))
		{
			return false;
		}
		string[] array = mapper.CollectCurrency.Split(":", StringSplitOptions.None);
		string text = array[0];
		if (!(text == "SpeedUp"))
		{
			if (text == "MiniEvent")
			{
				string s = array[1];
				CurrencyType currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), text);
				int amount = int.Parse(mapper.CollectCount);
				int num = int.Parse(s);
				CompositeIdentificator compositeIdentificator = new CompositeIdentificator(num);
				result = new ModuleConfigs.Collect(currency, amount, compositeIdentificator);
			}
			else
			{
				CurrencyType currency2 = (CurrencyType)Enum.Parse(typeof(CurrencyType), text);
				int amount2 = int.Parse(mapper.CollectCount);
				result = new ModuleConfigs.Collect(currency2, amount2);
			}
		}
		else
		{
			ModuleConfigs.Collect.SpeedUpParams parametres = new ModuleConfigs.Collect.SpeedUpParams(Parser.ParseFloat(mapper.CollectCount, 3600f));
			result = new ModuleConfigs.Collect(CollectableType.SpeedUp, parametres);
		}
		return true;
	}

	private bool TryBuildSellModule(GameItemMapper mapper, out ModuleConfigs.Sell result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsSell))
		{
			return false;
		}
		int price = int.Parse(mapper.SellPrice);
		result = new ModuleConfigs.Sell(price);
		return true;
	}

	private bool TryBuildAutoSpawnModule(GameItemMapper mapper, out ModuleConfigs.AutoSpawn result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsAutoSpawn))
		{
			return false;
		}
		int maxAmount = int.Parse(mapper.AutoSpawnMaxAmount);
		List<WeightNode<GIData>> spawnPool = ParsingUtils.ParseGIDataWeightList(mapper.AutoSpawnSpawnList);
		ParsingUtils.ParseBool(mapper.AutoSpawnCanRestore);
		int restoreAmount = ParsingUtils.ParseInt(mapper.AutoSpawnRestoreAmount);
		float restoreTime = ParsingUtils.ParseFloat(mapper.AutoSpawnRestoreTime);
		float secPrice = ParsingUtils.ParseFloat(mapper.AutoSpawnSecPrice);
		GIDestroyType destroyType = ParsingUtils.ParseEnum(mapper.AutoSpawnDestroyType, GIDestroyType.None);
		GIData destroyResult = ParsingUtils.ParseGIData(mapper.AutoSpawnTransformResult);
		result = new ModuleConfigs.AutoSpawn(restoreTime, restoreAmount, maxAmount, secPrice, spawnPool, destroyType, destroyResult);
		return true;
	}

	private bool TryBuildClickSpawnModule(GameItemMapper mapper, out ModuleConfigs.ClickSpawn result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsClickSpawn))
		{
			return false;
		}
		int maxAmount = int.Parse(mapper.ClickSpawnMaxAmount);
		int energyPrice = ParsingUtils.ParseInt(mapper.ClickSpawnEnergy, 1);
		List<WeightNode<GIData>> spawnPool = ParsingUtils.ParseGIDataWeightList(mapper.ClickSpawnSpawnList);
		ParsingUtils.ParseBool(mapper.ClickSpawnCanRestore);
		int restoreAmount = ParsingUtils.ParseInt(mapper.ClickSpawnRestoreAmount);
		float restoreTime = ParsingUtils.ParseFloat(mapper.ClickSpawnRestoreTime);
		float speedUpMul = ParsingUtils.ParseFloat(mapper.ClickSpawnSecPrice, 0.015f);
		GIDestroyType destroyType = ParsingUtils.ParseEnum(mapper.ClickSpawnDestroyType, GIDestroyType.None);
		GIData destroyResult = ParsingUtils.ParseGIData(mapper.ClickSpawnDestroyResult);
		result = new ModuleConfigs.ClickSpawn(restoreTime, speedUpMul, restoreAmount, maxAmount, energyPrice, destroyType, destroyResult, spawnPool);
		return true;
	}

	private bool TryBuildChestModule(GameItemMapper mapper, out ModuleConfigs.Chest result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsChest))
		{
			return false;
		}
		bool openable = ParsingUtils.ParseBool(mapper.ChestOpenable);
		float timeToOpen = ParsingUtils.ParseFloat(mapper.ChestTime);
		float priceMul = ParsingUtils.ParseFloat(mapper.ChestSecPrice);
		result = new ModuleConfigs.Chest(openable, timeToOpen, priceMul);
		return true;
	}

	private bool TryBuildTeslaModule(GameItemMapper mapper, out ModuleConfigs.Tesla result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsTesla))
		{
			return false;
		}
		float multiplier = ParsingUtils.ParseFloat(mapper.TeslaMultiplier, 1f);
		float lifeTime = ParsingUtils.ParseFloat(mapper.TeslaTime);
		GIDestroyType destroyType = ParsingUtils.ParseEnum(mapper.TeslaDestroyType, GIDestroyType.None);
		GIData destroyResult = ParsingUtils.ParseGIData(mapper.TeslaDestroyResult);
		result = new ModuleConfigs.Tesla(lifeTime, multiplier, destroyType, destroyResult);
		return true;
	}

	private bool TryBuildMixerModule(GameItemMapper mapper, out ModuleConfigs.Mixer result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.IsMixer))
		{
			return false;
		}
		GIDestroyType destroyType = ParsingUtils.ParseEnum(mapper.ClickSpawnDestroyType, GIDestroyType.None);
		GIData destroyResult = ParsingUtils.ParseGIData(mapper.ClickSpawnDestroyResult);
		int energyPrice = ParsingUtils.ParseInt(mapper.MixerEnergy, 1);
		result = new ModuleConfigs.Mixer(destroyType, destroyResult, energyPrice);
		return true;
	}

	private static IEnumerable<RecipeModel> ParseRecipeList(string expression)
	{
		if (LinqExtensions.IsEmpty<char>((IEnumerable<char>)expression))
		{
			return new List<RecipeModel>();
		}
		expression = expression.Replace("\"", "");
		IEnumerable<int> ids = expression.Split(";", StringSplitOptions.None).Select(int.Parse);
		return _recipeManager.Collection.Where((RecipeModel item) => ids.Any((int id) => item.ID == id));
	}

	private bool TryBuildStackModule(GameItemMapper mapper, out ModuleConfigs.Stack result)
	{
		result = null;
		if (!ParsingUtils.ParseBool(mapper.MixerIsStack))
		{
			return false;
		}
		IEnumerable<RecipeModel> source = ParseRecipeList(mapper.MixerRecipes);
		bool isSwapActiveModule = ParsingUtils.ParseBool(mapper.MixerIsSwapModule);
		GIModuleType swapModuleType = ParsingUtils.ParseEnum(mapper.MixerNameModule, GIModuleType.Stack);
		result = new ModuleConfigs.Stack(source.ToList(), isSwapActiveModule, swapModuleType);
		return true;
	}

	private static bool TryBuildMergePointsModule(GameItemMapper mapper, out ModuleConfigs.MergePoints result, ModuleConfigs.Merge merge)
	{
		try
		{
			result = null;
			if (merge == null || !ParsingUtils.ParseBool(mapper.IsPoints))
			{
				return false;
			}
			string currencyTypeStrings = mapper.PointsType.Replace("[", "").Replace("]", "");
			string unlockTypeStrings = mapper.PointsUnlockType.Replace("[", "").Replace("]", "");
			string unlockValueStrings = mapper.PointsUnlockValue.Replace("[", "").Replace("]", "");
			string chanceStrings = mapper.PointsChance.Replace("[", "").Replace("]", "");
			string quantityStrings = mapper.PointsQty.Replace("[", "").Replace("]", "");
			ModuleConfigs.MergePoints.MergePointsCreateData[] createData = ParsingUtils.MergePointsParser.ParseMergePointsCreateData(currencyTypeStrings, unlockTypeStrings, unlockValueStrings, chanceStrings, quantityStrings);
			result = new ModuleConfigs.MergePoints(createData);
			return true;
		}
		catch (Exception ex)
		{
			throw new Exception("Exception occured while trying to build Battle Pass Module" + ex);
		}
	}

	private static bool TryBuildMergeShopModule(GameItemMapper mapper, out ModuleConfigs.MergeShop result)
	{
		try
		{
			result = null;
			if (!ParsingUtils.ParseBool(mapper.IsSelling))
			{
				return false;
			}
			string priceResource = mapper.PriceResource;
			int price = ParsingUtils.ParseInt(mapper.Price);
			int[] sale = ParsingUtils.ParseIntWeightArray(mapper.Sale);
			string shopSection = mapper.ShopSection;
			result = new ModuleConfigs.MergeShop(priceResource, price, sale, shopSection);
			return true;
		}
		catch
		{
			throw new Exception("Exception occured while trying to build Merge Shop Module");
		}
	}
}

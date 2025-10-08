using System;
using System.Text.RegularExpressions;
using GreenT.Data;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Types;
using Merge;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Tasks;

public class MergeObjectiveFactory : ConcreteObjectiveBaseFactory
{
	private const string CONCRETE_ITEM_MERGE_RULE = "concr_item:(\\w+)";

	private const string CONCRETE_ITEM_MERGE_COMPARER = "concr_item:";

	private const string CONCRETE_TYPE_MERGE_RULE = "concr_type:(\\w+)";

	private const string CONCRETE_TYPE_MERGE_COMPARER = "concr_type:";

	private const string ANY_MERGE_RULE = "item_merge";

	private const string SPAWNERS_RELOAD_RULE = "spawners_reload";

	private readonly MergeIconService _iconProvider;

	private readonly MergeFieldProvider _fieldProvider;

	public MergeObjectiveFactory(TaskObjectiveIcons objectiveIcons, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, ISaver saver, MergeIconService iconProvider, MergeFieldProvider fieldProvider)
		: base(objectiveIcons, gameSettings, currencyProcessor, saver, "concr_item:(\\w+)|concr_type:(\\w+)|item_merge|spawners_reload")
	{
		_iconProvider = iconProvider;
		_fieldProvider = fieldProvider;
	}

	public override IObjective Create(TaskMapper mapper, int index, ContentType contentType)
	{
		Match match = _objectiveRegex.Match(mapper.req_items[index]);
		try
		{
			if (match.Success)
			{
				string value = match.Value;
				if (value == "item_merge")
				{
					return TryCreateAnyMergeObjectives(mapper.task_id, mapper.req_items[index], mapper.req_value[index]);
				}
				if (value == "spawners_reload")
				{
					return TryCreateSpawnersReloadObjectives(mapper.task_id, mapper.req_value[index]);
				}
			}
			if (match.Value.Contains("concr_type:"))
			{
				return TryCreateConcreteTypeMergeObjectives(mapper.task_id, mapper.req_items[index], mapper.req_value[index]);
			}
			if (match.Value.Contains("concr_item:"))
			{
				return TryCreateOneWayMergeObjectives(mapper.task_id, mapper.req_items[index], mapper.req_value[index]);
			}
			return TryCreateMergeObjectives(mapper.req_items[index], mapper.req_value[index]);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException($"{GetType().Name}: Can't create Objective {mapper.task_id}" + " from " + mapper.req_items[index]);
		}
	}

	private MergeItemObjective TryCreateMergeObjectives(string reqItems, int reqValue)
	{
		try
		{
			return new MergeItemObjective(GIKey.Parse(reqItems), new ObjectiveData(reqValue), _iconProvider);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("ReqItem lenght doesn't equal ReqValue lenght");
		}
	}

	private OneWayMergeItemObjective TryCreateOneWayMergeObjectives(int taskId, string reqItems, int reqValue)
	{
		Match match = Regex.Match(reqItems, "concr_item:(\\w+)");
		string str = null;
		if (match.Success)
		{
			str = match.Groups[1].Value;
		}
		SavableObjectiveData savableObjectiveData = CreateGainData(taskId, reqValue);
		OneWayMergeItemObjective result;
		try
		{
			result = new OneWayMergeItemObjective(GIKey.Parse(str), savableObjectiveData, _iconProvider);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("ReqItem lenght doesn't equal ReqValue lenght");
		}
		_saver.Add(savableObjectiveData);
		return result;
	}

	private AnyMergeObjective TryCreateAnyMergeObjectives(int taskId, string reqItems, int reqValue)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(taskId, reqValue);
		AnyMergeObjective result;
		try
		{
			result = new AnyMergeObjective(GIKey.Parse(reqItems), savableObjectiveData, _iconProvider);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("ReqItem lenght doesn't equal ReqValue lenght");
		}
		_saver.Add(savableObjectiveData);
		return result;
	}

	private SavableObjective TryCreateSpawnersReloadObjectives(int taskId, int reqValue)
	{
		SavableObjectiveData savableObjectiveData = CreateGainData(taskId, reqValue);
		SpawnerReloadObjective result = new SpawnerReloadObjective(_fieldProvider, savableObjectiveData);
		_saver.Add(savableObjectiveData);
		return result;
	}

	private ConcreteTypeMergeObjective TryCreateConcreteTypeMergeObjectives(int taskId, string reqItems, int reqValue)
	{
		Match match = Regex.Match(reqItems, "concr_type:(\\w+)");
		string collection = null;
		if (match.Success)
		{
			collection = match.Groups[1].Value;
		}
		SavableObjectiveData savableObjectiveData = CreateGainData(taskId, reqValue);
		ConcreteTypeMergeObjective result;
		try
		{
			result = new ConcreteTypeMergeObjective(new GIKey(0, collection), savableObjectiveData, _iconProvider);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("ReqItem lenght doesn't equal ReqValue lenght");
		}
		_saver.Add(savableObjectiveData);
		return result;
	}
}

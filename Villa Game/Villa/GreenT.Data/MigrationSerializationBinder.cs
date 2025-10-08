using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using GreenT.HornyScapes;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Sellouts.Models;
using Merge;

namespace GreenT.Data;

public class MigrationSerializationBinder : SerializationBinder
{
	private static readonly Dictionary<string, Type> TypesRequiringMigration = new Dictionary<string, Type>
	{
		{
			"GreenT.HornyScapes.MergeCore.GameItemController+GeneralData+GeneralDataMemento",
			typeof(GeneralData.GeneralDataMemento)
		},
		{
			"GreenT.HornyScapes.ContentStorage+Memento",
			typeof(ContentStorageProvider.Memento)
		},
		{
			"GreenT.HornyScapes.EnergyRestore+EnergyRestoreMemento",
			typeof(BaseEnergyRestore.EnergyRestoreMemento)
		},
		{
			"Merge.ModuleDatas+BattlePass",
			typeof(ModuleDatas.MergePoints)
		},
		{
			"GreenT.HornyScapes.StepLocker+StepLockerMemento",
			typeof(MigrationDummyMemento)
		},
		{
			"GreenT.HornyScapes.Bank.Sellout+SelloutMemento",
			typeof(MigrationDummyMemento)
		},
		{
			"GreenT.HornyScapes.Sellouts.Models.Sellout+SelloutMemento",
			typeof(Sellout.SelloutMemento)
		}
	};

	public override Type BindToType(string assemblyName, string typeName)
	{
		return TypesRequiringMigration.GetValueOrDefault(typeName);
	}

	public static string GetErrorMessage(string message)
	{
		string pattern = "Unable to load type ([\\w\\.+]+) required for deserialization";
		Match match = Regex.Match(message, pattern);
		if (!match.Success)
		{
			return message;
		}
		string value = match.Groups[1].Value;
		return "Вы переместили/переименовали тип :  " + value + " || Его нужно добавить в скрипт MigrationSerializationBinder || шаблон  { \"" + value + "\", typeof(Новый тип) }\n---------------------------\n" + message;
	}
}

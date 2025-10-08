using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.HornyScapes.Data;
using GreenT.Types;
using Merge;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

[Serializable]
[MementoHolder]
public class GeneralData : Merge.Data, ISavableState, IDisposable
{
	[Serializable]
	public class GeneralDataMemento : Memento
	{
		public List<GIData> Data;

		public GeneralDataMemento(GeneralData savableState)
			: base(savableState)
		{
			Data = savableState.data;
		}
	}

	[SerializeField]
	private List<GIData> data = new List<GIData>();

	private IDisposable saveKeyStream;

	private string uniqueKey;

	public List<GIData> GameItems => data;

	public ContentType Type { get; private set; }

	public RemoteVersion ConfigVersion { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void SetConfigVersion(RemoteVersion remoteVersion)
	{
		ConfigVersion = remoteVersion;
	}

	public GeneralData()
	{
	}

	public GeneralData(List<GIData> field, ContentType type, string saveKey)
	{
		data = field;
		Type = type;
		OnChangedSaveKey(saveKey);
	}

	public GeneralData(List<GIData> field, ContentType type, IReadOnlyReactiveProperty<string> saveKey)
	{
		data = field;
		Type = type;
		saveKeyStream = saveKey.Subscribe(OnChangedSaveKey);
		OnChangedSaveKey(saveKey.Value);
	}

	private void OnChangedSaveKey(string newSaveKey)
	{
		uniqueKey = "GeneralData_" + newSaveKey;
	}

	public void Clear()
	{
		data.Clear();
		GameItems.Clear();
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public Memento SaveState()
	{
		return new GeneralDataMemento(this);
	}

	public void LoadState(Memento memento)
	{
		GeneralDataMemento generalDataMemento = (GeneralDataMemento)memento;
		MigrationToBattlePass.Migrate(generalDataMemento.Data);
		data = generalDataMemento.Data;
	}

	[Obsolete("MigrateSavedDataFrom93To94Version")]
	public void ChangeData(List<GIData> newData)
	{
		data = newData;
	}

	public void Dispose()
	{
		saveKeyStream?.Dispose();
	}
}

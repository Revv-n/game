using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.MergeStore;

public class DataManagerCluster : ContentCluster<CreateDataManager>
{
	private readonly Subject<ContentType> _onUpdate = new Subject<ContentType>();

	public IObservable<ContentType> OnUpdate => (IObservable<ContentType>)_onUpdate;

	public void Initialization()
	{
		HashSet<ContentType> hashSet = new HashSet<ContentType>();
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				System.Collections.Generic.KeyValuePair<ContentType, CreateDataManager> pair = enumerator.Current;
				foreach (CreateData item in pair.Value.Collection)
				{
					if (item.Lock.IsOpen.Value)
					{
						hashSet.Add(pair.Key);
						continue;
					}
					ObservableExtensions.Subscribe<bool>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)item.Lock.IsOpen, (Func<bool, bool>)((bool x) => x)), 1), (Action<bool>)delegate
					{
						_onUpdate?.OnNext(pair.Key);
					});
				}
			}
		}
		foreach (ContentType item2 in hashSet)
		{
			_onUpdate.OnNext(item2);
		}
	}

	public SectionCreateDataPreset GetSectionContainerForTypes(ContentType type, string bundle)
	{
		if (!ValidateBaseConfiguration())
		{
			return null;
		}
		return type switch
		{
			ContentType.Main => GetMainSections(), 
			ContentType.Event => GetEventSections(bundle), 
			ContentType.BattlePass => throw new Exception("BattlePass not supported"), 
			_ => throw new ArgumentOutOfRangeException("type", type, null), 
		};
	}

	private bool ValidateBaseConfiguration()
	{
		CreateData baseCreatData = GetBaseCreatData();
		if (baseCreatData == null)
		{
			return false;
		}
		if (baseCreatData.RegularSectionCreateData == null)
		{
			return false;
		}
		if (baseCreatData.PremiumSectionCreateData == null)
		{
			return false;
		}
		return true;
	}

	private CreateData GetBaseCreatData()
	{
		return base[ContentType.Main].Collection.FirstOrDefault();
	}

	private SectionCreateDataPreset GetMainSections()
	{
		CreateData baseCreatData = GetBaseCreatData();
		if (baseCreatData == null || !baseCreatData.Lock.IsOpen.Value)
		{
			return null;
		}
		return new SectionCreateDataPreset(baseCreatData.RegularSectionCreateData, baseCreatData.PremiumSectionCreateData);
	}

	private SectionCreateDataPreset GetEventSections(string bindle)
	{
		CreateData createData = FindEventByBundle(bindle);
		if (createData == null || !createData.Lock.IsOpen.Value)
		{
			return null;
		}
		SectionCreateData regularSectionCreateData = createData.RegularSectionCreateData;
		SectionCreateData premiumSectionCreateData = createData.PremiumSectionCreateData;
		if (!createData.UsePremium)
		{
			return new SectionCreateDataPreset(regularSectionCreateData, null);
		}
		return new SectionCreateDataPreset(regularSectionCreateData, premiumSectionCreateData);
	}

	private CreateData FindEventByBundle(string id)
	{
		return base[ContentType.Event].Collection.FirstOrDefault((CreateData creatData) => creatData.Bundle.Equals(id));
	}
}

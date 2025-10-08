using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.Bonus;

public class BonusManager
{
	private readonly IDictionary<ISimpleBonus, int> bonusDict = new Dictionary<ISimpleBonus, int>();

	private readonly Subject<ISimpleBonus> onUpdate = new Subject<ISimpleBonus>();

	public IEnumerable<ISimpleBonus> Collection => bonusDict.Keys;

	public virtual IObservable<ISimpleBonus> OnUpdate => Observable.AsObservable<ISimpleBonus>((IObservable<ISimpleBonus>)onUpdate);

	public void Add(int uniqParentID, ISimpleBonus bonus)
	{
		bonusDict[bonus] = uniqParentID;
		onUpdate.OnNext(bonus);
	}

	public void AddRange(List<int> uniqParentID, List<ISimpleBonus> bonus)
	{
		if (uniqParentID.Count() != bonus.Count())
		{
			throw new Exception().SendException($"BonusManager: can't connect parent ID (Count: {uniqParentID.Count()}) with bonus (Count: {bonus.Count()})");
		}
		for (int i = 0; i < uniqParentID.Count && i < bonus.Count; i++)
		{
			Add(uniqParentID[i], bonus[i]);
		}
	}

	public int GetParent(IBonus bonus)
	{
		try
		{
			return bonusDict[bonus];
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("There is no bonus in collection\n");
		}
	}
}

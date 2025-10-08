using GreenT.Bonus;
using GreenT.HornyScapes.Events;
using GreenT.Multiplier;

namespace GreenT.HornyScapes.Card.Bonus;

public class CharacterMultiplierBonus : MultiplierBonus
{
	private readonly BonusType _bonusType;

	private readonly GreenT.Multiplier.Multiplier _multiplier;

	public bool AffectAll { get; }

	public string SpawnerName { get; }

	public int[] AffectedSpawnerId { get; }

	public override IMultiplier Multiplier => _multiplier;

	public CharacterMultiplierBonus(IMultiplierTotalContainer<int> multipliersContainer, double[] values, string name, BonusType bonusType, bool affectAll, int[] affectedSpawnerId, string spawnerName)
		: base(multipliersContainer, values, name, bonusType)
	{
		AffectAll = affectAll;
		AffectedSpawnerId = affectedSpawnerId;
		SpawnerName = spawnerName;
		_bonusType = bonusType;
		_multiplier = (BonusTools.IsPercentBonusType(_bonusType) ? new PercentMultiplier(0.0) : (BonusTools.IsCountBonusType(_bonusType) ? new CountMultiplier(0.0) : new GreenT.Multiplier.Multiplier(1.0)));
	}

	public override void Apply()
	{
		SetMultiplier(isOn: true);
	}

	public override void Undo()
	{
		SetMultiplier(isOn: false);
	}

	private void SetMultiplier(bool isOn)
	{
		if (base.IsApplied != isOn)
		{
			if (AffectAll)
			{
				SetTotal(isOn);
			}
			else
			{
				SetAffected(isOn);
			}
			base.IsApplied = isOn;
		}
	}

	private void SetTotal(bool isOn)
	{
		if (isOn)
		{
			multipliersContainer.AddToTotal(Multiplier);
		}
		else
		{
			multipliersContainer.RemoveFromTotal(Multiplier);
		}
	}

	private void SetAffected(bool isOn)
	{
		if (isOn)
		{
			int[] affectedSpawnerId = AffectedSpawnerId;
			foreach (int key in affectedSpawnerId)
			{
				multipliersContainer.Add(key, Multiplier);
			}
		}
		else
		{
			int[] affectedSpawnerId = AffectedSpawnerId;
			foreach (int key2 in affectedSpawnerId)
			{
				multipliersContainer.Remove(key2, Multiplier);
			}
		}
	}

	public override void SetLevel(int level)
	{
		if (level >= base.Values.Length)
		{
			level = base.Values.Length - 1;
		}
		base.SetLevel(level);
		double value = (BonusTools.IsCountBonusType(_bonusType) ? ((double)base.Level) : base.Values[base.Level]);
		_multiplier.Set(value);
	}
}

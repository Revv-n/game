using System.Runtime.CompilerServices;
using GreenT.HornyScapes.Booster.Effect;
using GreenT.HornyScapes.Card.Bonus;
using Zenject;

namespace GreenT.Bonus;

public class BonusFactory : IFactory<BonusParameters, ISimpleBonus>, IFactory
{
	private readonly BonusManager _bonusManager;

	private readonly IFactory<CharacterBonusParameters, ISimpleBonus> _characterBonusFactory;

	private readonly IFactory<BoosterBonusParameters, ISimpleBonus> _boosterBonusFactory;

	public BonusFactory(BonusManager bonusManager, IFactory<CharacterBonusParameters, ISimpleBonus> characterBonusFactory, IFactory<BoosterBonusParameters, ISimpleBonus> boosterBonusFactory)
	{
		_bonusManager = bonusManager;
		_characterBonusFactory = characterBonusFactory;
		_boosterBonusFactory = boosterBonusFactory;
	}

	public ISimpleBonus Create(BonusParameters parameters)
	{
		ISimpleBonus simpleBonus;
		if (!(parameters is CharacterBonusParameters param))
		{
			if (!(parameters is BoosterBonusParameters param2))
			{
				throw new SwitchExpressionException(parameters);
			}
			simpleBonus = _boosterBonusFactory.Create(param2);
		}
		else
		{
			simpleBonus = _characterBonusFactory.Create(param);
		}
		ISimpleBonus simpleBonus2 = simpleBonus;
		_bonusManager.Add(parameters.UniqParentID, simpleBonus2);
		return simpleBonus2;
	}
}

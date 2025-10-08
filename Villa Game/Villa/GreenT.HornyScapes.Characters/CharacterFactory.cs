using System;
using GreenT.Bonus;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Lockers;
using GreenT.Localizations;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Characters;

public class CharacterFactory : IFactory<CharacterInfoMapper, CharacterInfo>, IFactory
{
	private const string localizationKey = "content.character.{0}";

	private const string characterNameKey = "content.character.{0}.name";

	private const string characterDescriptionKey = "content.character.{0}.description";

	private readonly IFactory<CharacterBonusParameters, ISimpleBonus> bonusFactory;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private readonly LocalizationService _localizationService;

	private readonly SimpleCurrencyFactory _simpleCurrencyFactory;

	public CharacterFactory(IFactory<CharacterBonusParameters, ISimpleBonus> bonusFactory, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory, LocalizationService localizationService, SimpleCurrencyFactory simpleCurrencyFactory)
	{
		this.bonusFactory = bonusFactory;
		this.lockerFactory = lockerFactory;
		_localizationService = localizationService;
		_simpleCurrencyFactory = simpleCurrencyFactory;
	}

	public CharacterInfo Create(CharacterInfoMapper InfoMapper)
	{
		IBonus bonus = CreateBonus(InfoMapper);
		CreatePoints(InfoMapper.id, InfoMapper.id_relationship);
		ILocker displayConditionLocker;
		try
		{
			displayConditionLocker = CreateLocker(InfoMapper.unlock_type, InfoMapper.unlock_value);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Exception on DISPLAY CONDITION LOCKER creation for character: " + InfoMapper.id);
		}
		ILocker preloadLocker;
		try
		{
			preloadLocker = CreateLocker(InfoMapper.preload_type, InfoMapper.preload_value);
		}
		catch (Exception innerException2)
		{
			throw innerException2.SendException("Exception on DATA PRELOAD CONDITION LOCKER creation for character: " + InfoMapper.id);
		}
		string text = $"content.character.{InfoMapper.id}.name";
		string descriptionKey = $"content.character.{InfoMapper.id}.description";
		string name = _localizationService.Text(text);
		return new CharacterInfo(InfoMapper.id, InfoMapper.order_number, InfoMapper.nude_level, InfoMapper.promote_pattern_id, name, text, descriptionKey, InfoMapper.rarity, InfoMapper.type, InfoMapper.load_type, bonus, displayConditionLocker, preloadLocker, InfoMapper.id_relationship);
	}

	private IBonus CreateBonus(CharacterInfoMapper mapper)
	{
		CharacterBonusParameters param = new CharacterBonusParameters(mapper.id, mapper.bonus.ToString(), mapper.bonus, mapper.bonus_value, mapper.affect_all, mapper.spawner_id);
		return bonusFactory.Create(param) as IBonus;
	}

	private ILocker CreateLocker(UnlockType[] unlock_type, string[] unlock_value)
	{
		if (unlock_type == null || unlock_type.Length == 0)
		{
			return new PermanentLocker(isOpen: true);
		}
		ILocker[] array = new ILocker[unlock_type.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = lockerFactory.Create(unlock_type[i], unlock_value[i], LockerSourceType.Character);
		}
		return new CompositeLocker(array);
	}

	private void CreatePoints(int characterId, int relationshipId)
	{
		if (relationshipId != 0)
		{
			string saveKey = $"currency_love_points_{characterId}_{relationshipId}";
			_simpleCurrencyFactory.Create(CurrencyType.LovePoints, 0, saveKey, relationshipId);
		}
	}
}

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GreenT.HornyScapes.Tasks;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTasksDescriptionLocalizationResolver : MiniEventLocalizationKeyResolver<string>
{
	private readonly MiniEventSpendCurrencyTasksLocalizationResolver _miniEventSpendCurrencyTasksLocalizationResolver;

	private readonly MiniEventAddCurrencyTasksLocalizationResolver _miniEventAddCurrencyTasksLocalizationResolver;

	public MiniEventTasksDescriptionLocalizationResolver(MiniEventSpendCurrencyTasksLocalizationResolver miniEventCurrencyTasksDescriptionLocalizationResolver, MiniEventAddCurrencyTasksLocalizationResolver miniEventAddCurrencyTasksLocalizationResolver)
	{
		_miniEventSpendCurrencyTasksLocalizationResolver = miniEventCurrencyTasksDescriptionLocalizationResolver;
		_miniEventAddCurrencyTasksLocalizationResolver = miniEventAddCurrencyTasksLocalizationResolver;
		_localizationKeys = new Dictionary<string, string>
		{
			{ "ConcreteRouletteObjective", "ui.minievent.task.roulette{0}.descr" },
			{ "AnyGirlPromoteObjective", "ui.minievent.task.anyPromoteGirls.descr" },
			{ "AnyMainGirlPromoteObjective", "ui.minievent.task.anyMainPromoteGirls.descr" },
			{ "ConcreteRarityGirlPromoteObjectiveCommon", "ui.minievent.task.common_rarity_PromoteGirls.descr" },
			{ "ConcreteRarityGirlPromoteObjectiveRare", "ui.minievent.task.rare_rarity_PromoteGirls.descr" },
			{ "ConcreteRarityGirlPromoteObjectiveEpic", "ui.minievent.task.epic_rarity_PromoteGirls.descr" },
			{ "ConcreteRarityGirlPromoteObjectiveLegendary", "ui.minievent.task.legendary_rarity_PromoteGirls.descr" },
			{ "ConcreteEventGirlPromoteObjective", "ui.minievent.task.concr_event_PromoteGirls{0}.descr" },
			{ "ConcreteMainGirlPromoteObjective", "ui.minievent.task.concr_main_PromoteGirls_{0}.descr" },
			{ "ConcreteGirlPromoteObjective", "ui.minievent.task.concr_main_PromoteGirls_{0}.descr" },
			{ "AnyGetGirlObjective", "ui.minievent.task.anyGetGirl.descr" },
			{ "ConcreteRarityGetGirlObjectiveCommon", "ui.minievent.task.common_rarity_get_girl.descr" },
			{ "ConcreteRarityGetGirlObjectiveRare", "ui.minievent.task.rare_rarity_get_girl.descr" },
			{ "ConcreteRarityGetGirlObjectiveEpic", "ui.minievent.task.epic_rarity_get_girl.descr" },
			{ "ConcreteRarityGetGirlObjectiveLegendary", "ui.minievent.task.legendary_rarity_get_girl.descr" },
			{ "ConcreteGetGirlObjective", "ui.minievent.task.concr_girl_get_{0}.descr" },
			{ "AnySummonObjective", "ui.minievent.task.anySummon.descr" },
			{ "ConcreteSummonAmountObjective", "ui.minievent.task.concr_summon_amount.descr" },
			{ "GetBPLevelsObjective", "ui.minievent.task.getBPLevel.descr" },
			{ "GetBPRewardsObjective", "ui.minievent.task.getBPRewards.descr" },
			{ "AnyMergeObjective", "ui.minievent.task.anyMergeItem.descr" },
			{ "SpendForPromoteObjective", "ui.minievent.task.spendForPromote.descr" },
			{ "ConcreteTypeMergeObjective", "ui.minievent.task.concr_type_merge_{0}.descr" },
			{ "MergeItemObjective", "ui.minievent.task.mergeItem.descr" },
			{ "OneWayMergeItemObjective", "ui.minievent.task.mergeItem_{0}_{1}.descr" },
			{ "SpawnerReloadObjective", "ui.minievent.task.spawnersReload.descr" },
			{ "GetPhotoObjective", "ui.minievent.task.getPhoto.descr" },
			{ "GetConcreteRarityGirlPhotoObjectiveCommon", "ui.minievent.task.getCommonPhoto.descr" },
			{ "GetConcreteRarityGirlPhotoObjectiveRare", "ui.minievent.task.getRarePhoto.descr" },
			{ "GetConcreteRarityGirlPhotoObjectiveEpic", "ui.minievent.task.getEpicPhoto.descr" },
			{ "GetConcreteRarityGirlPhotoObjectiveLegendary", "ui.minievent.task.getLegendaryPhoto.descr" },
			{ "GetConcreteGirlPhotoObjective", "ui.minievent.task.getConcretePhoto_{0}.descr" },
			{ "GetConcreteCompletedDialogueObjective", "ui.minievent.task.getConcreteCompletedDialogue_{0}.descr" },
			{ "GetConcreteCompletedDialogueAnswersObjective", "ui.minievent.task.getConcreteCompletedDialogueAnswersObjective_{0}.descr" },
			{ "SpendHardInMergeStoreObjective", "ui.minievent.task.spendHardInMergeStoreObjective.descr" },
			{ "SpendHardInForRechargeObjective", "ui.minievent.task.spendHardInMergeStoreForRechargeObjective.descr" },
			{ "SpendHardForOpenBubbleObjective", "ui.minievent.task.spendHardForOpenBubbleObjective.descr" },
			{ "BuyInMergeStoreObjective", "ui.minievent.task.buyInMergeStoreObjective.descr" },
			{ "AnyGirlAnyPresentObjective", "ui.minievent.task.anyPresentAnyGirl.descr" }
		};
	}

	public string GetKeyByObjective(IObjective key)
	{
		string name = key.GetType().Name;
		if (!(key is ConcreteRouletteObjective concreteRouletteObjective))
		{
			if (!(key is ConcreteRarityGirlPromoteObjective concreteRarityGirlPromoteObjective))
			{
				if (!(key is ConcreteEventGirlPromoteObjective concreteEventGirlPromoteObjective))
				{
					if (!(key is ConcreteMainGirlPromoteObjective concreteMainGirlPromoteObjective))
					{
						if (!(key is AnyMainGirlPromoteObjective))
						{
							if (!(key is ConcreteGirlPromoteObjective concreteGirlPromoteObjective))
							{
								if (!(key is AnyGirlPromoteObjective))
								{
									if (!(key is SpendForPromoteObjective))
									{
										if (!(key is ConcreteGetGirlObjective concreteGetGirlObjective))
										{
											if (!(key is ConcreteRarityGetGirlObjective concreteRarityGetGirlObjective))
											{
												if (!(key is AnyGetGirlObjective))
												{
													if (!(key is AnyMergeObjective))
													{
														if (!(key is ConcreteTypeMergeObjective concreteTypeMergeObjective))
														{
															if (!(key is MergeItemObjective))
															{
																if (!(key is OneWayMergeItemObjective oneWayMergeItemObjective))
																{
																	if (!(key is SpawnerReloadObjective))
																	{
																		if (!(key is GetBPLevelsObjective))
																		{
																			if (!(key is GetBPRewardsObjective))
																			{
																				if (!(key is ConcreteSummonAmountObjective))
																				{
																					if (!(key is AnySummonObjective))
																					{
																						if (!(key is GetPhotoObjective))
																						{
																							if (!(key is GetConcreteRarityGirlPhotoObjective getConcreteRarityGirlPhotoObjective))
																							{
																								if (!(key is GetConcreteGirlPhotoObjective getConcreteGirlPhotoObjective))
																								{
																									if (!(key is GetConcreteCompletedDialogueObjective getConcreteCompletedDialogueObjective))
																									{
																										if (!(key is GetConcreteCompletedDialogueAnswersObjective getConcreteCompletedDialogueAnswersObjective))
																										{
																											if (!(key is EventXPAddCurrencyObjective eventXPAddCurrencyObjective))
																											{
																												if (!(key is EventAddCurrencyObjective eventAddCurrencyObjective))
																												{
																													if (!(key is CurrencySpendObjective currencySpendObjective))
																													{
																														if (!(key is CurrencyAddObjective currencyAddObjective))
																														{
																															if (!(key is SpendHardInMergeStoreObjective spendHardInMergeStoreObjective))
																															{
																																if (!(key is SpendHardInForRechargeObjective spendHardInForRechargeObjective))
																																{
																																	if (!(key is SpendHardForOpenBubbleObjective spendHardForOpenBubbleObjective))
																																	{
																																		if (!(key is BuyInMergeStoreObjective buyInMergeStoreObjective))
																																		{
																																			if (key is AnyGirlAnyPresentObjective)
																																			{
																																				return GetKey(name);
																																			}
																																			throw new SwitchExpressionException(key);
																																		}
																																		return string.Format(GetKey(name), buyInMergeStoreObjective.GetTarget());
																																	}
																																	return string.Format(GetKey(name), spendHardForOpenBubbleObjective.GetTarget());
																																}
																																return string.Format(GetKey(name), spendHardInForRechargeObjective.GetTarget());
																															}
																															return string.Format(GetKey(name), spendHardInMergeStoreObjective.GetTarget());
																														}
																														return GetKeyAddCurrency(currencyAddObjective.CurrencyType, currencyAddObjective.CurrencyId);
																													}
																													return GetKeySpendCurrency(currencySpendObjective.CurrencyType, currencySpendObjective.CurrencyId);
																												}
																												return GetKeyAddCurrency(eventAddCurrencyObjective.CurrencyType, eventAddCurrencyObjective.TargetEventID);
																											}
																											return GetKeyAddCurrency(eventXPAddCurrencyObjective.CurrencyType, eventXPAddCurrencyObjective.TargetEventID);
																										}
																										return GetCompositeKey(GetKey(name), getConcreteCompletedDialogueAnswersObjective.CharacterNameKey);
																									}
																									return GetCompositeKey(GetKey(name), getConcreteCompletedDialogueObjective.CharacterNameKey);
																								}
																								return string.Format(GetKey(name), getConcreteGirlPhotoObjective.GirlID);
																							}
																							return GetKey($"{name}{getConcreteRarityGirlPhotoObjective.Rarity}");
																						}
																						return GetKey(name);
																					}
																					return GetKey(name);
																				}
																				return GetKey(name);
																			}
																			return GetKey(name);
																		}
																		return GetKey(name);
																	}
																	return GetKey(name);
																}
																return string.Format(GetKey(name), oneWayMergeItemObjective.ItemKey.Collection, oneWayMergeItemObjective.ItemKey.ID);
															}
															return GetKey(name);
														}
														return string.Format(GetKey(name), concreteTypeMergeObjective.ItemKey.Collection);
													}
													return GetKey(name);
												}
												return GetKey(name);
											}
											return GetKey($"{name}{concreteRarityGetGirlObjective.Rarity}");
										}
										return string.Format(GetKey(name), concreteGetGirlObjective.GirlId);
									}
									return GetKey(name);
								}
								return GetKey(name);
							}
							return string.Format(GetKey(name), concreteGirlPromoteObjective.GirlID);
						}
						return GetKey(name);
					}
					return string.Format(GetKey(name), concreteMainGirlPromoteObjective.GirlID);
				}
				return string.Format(GetKey(name), concreteEventGirlPromoteObjective.TargetEventID);
			}
			return GetKey($"{name}{concreteRarityGirlPromoteObjective.Rarity}");
		}
		return string.Format(GetKey(name), concreteRouletteObjective.RouletteId);
	}

	private string GetCompositeKey(params string[] args)
	{
		return string.Join(":", args);
	}

	private string GetKeySpendCurrency(CurrencyType currencyType, int id)
	{
		return string.Format(_miniEventSpendCurrencyTasksLocalizationResolver.GetKey(currencyType), id);
	}

	private string GetKeyAddCurrency(CurrencyType currencyType, int id)
	{
		return string.Format(_miniEventAddCurrencyTasksLocalizationResolver.GetKey(currencyType), id);
	}
}

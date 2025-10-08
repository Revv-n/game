using System.Linq;
using GreenT.HornyScapes.Tasks.Data;
using GreenT.Localizations;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using Zenject;

namespace GreenT.HornyScapes.Tasks;

public class GoalFactory : IFactory<TaskMapper, ContentType, Goal>, IFactory
{
	private const string CONCRETE_ROULETTE_OBJECTIVE = "ui.merge.taskview.objective.roulette_concr";

	private const string ANY_SUMMON_OBJECTIVE = "ui.merge.taskview.objective.summon";

	private const string CONCRETE_SUMMON_AMOUNT_OBJECTIVE = "ui.merge.taskview.objective.concrete_summon_amount";

	private const string ANY_PROMOTE_OBJECTIVE = "ui.merge.taskview.objective.promote";

	private const string CONCRETE_PROMOTE_OBJECTIVE = "ui.merge.taskview.objective.concrete_promote";

	private const string CONCRETE_RARITY_PROMOTE_OBJECTIVE = "ui.merge.taskview.objective.concrete_rarity_promote";

	private const string ADD_CURRENCY_OBJECTIVE = "ui.merge.taskview.objective.add_currecy";

	private const string SPEND_CURRENCY_OBJECTIVE = "ui.merge.taskview.objective.spend_currecy";

	private const string BUY_IN_MERGE_STORE_OBJECTIVE = "ui.merge.taskview.objective.buy_in_merge_store";

	private const string ANY_GET_GIRL_OBJECTIVE = "ui.merge.taskview.objective.get_card";

	private const string ANY_GET_RARITY_GIRL_OBJECTIVE = "ui.merge.taskview.objective.get_card_rarity";

	private const string CONCRETE_GET_GIRL_OBJECTIVE = "ui.merge.taskview.objective.get_card_concr";

	private const string ONE_WAY_MERGE_OBJECTIVE = "ui.merge.taskview.objective.one_way_merge";

	private const string ANY_MERGE_OBJECTIVE = "ui.merge.taskview.objective.any_merge";

	private const string CONCRETE_TYPE_MERGE_OBJECTIVE = "ui.merge.taskview.objective.concrete_type_merge";

	private const string GET_PHOTO_OBJECTIVE = "ui.merge.taskview.objective.get_photo";

	private const string COMPLETED_DIALOGUE = "ui.merge.taskview.objective.completed_dialogue";

	private const string COMPLETED_DIALOGUE_ANSWER = "ui.merge.taskview.objective.completed.answer_dialogue";

	private const string GET_BP_LEVELS_OBJECTIVE = "ui.merge.taskview.objective.get_bp_levels";

	private const string GET_BP_REWARDS_OBJECTIVE = "ui.merge.taskview.objective.get_bp_rewards";

	private const string SPEND_SOFT_PROMOTE_OBJECTIVE = "ui.merge.taskview.objective.spend_soft_promote";

	private const string GIVE_PRESENTS_OBJECTIVE = "ui.merge.taskview.objective.give_presents";

	private readonly ObjectiveFactory objectiveFactory;

	private readonly CardsCollection cardsCollection;

	private readonly LocalizationService _localizationService;

	public GoalFactory(ObjectiveFactory objectiveFactory, CardsCollection cardsCollection, LocalizationService localizationService)
	{
		this.objectiveFactory = objectiveFactory;
		this.cardsCollection = cardsCollection;
		_localizationService = localizationService;
	}

	public Goal Create(TaskMapper mapper, ContentType contentType)
	{
		IObjective[] array = objectiveFactory.Create(mapper, contentType);
		(string, ActionButtonType) tuple = SelectStrategy(array[0]);
		Goal goal = new Goal(tuple.Item1, tuple.Item2, array);
		goal.Initialize();
		return goal;
	}

	private (string description, ActionButtonType type) SelectStrategy(IObjective objective)
	{
		if (!(objective is SpendHardInMergeStoreObjective))
		{
			if (!(objective is SpendHardInForRechargeObjective))
			{
				if (!(objective is SpendHardForOpenBubbleObjective))
				{
					if (!(objective is BuyInMergeStoreObjective))
					{
						if (!(objective is ConcreteRouletteObjective))
						{
							if (!(objective is CurrencyAddObjective currencyAddObjective))
							{
								if (!(objective is CurrencySpendObjective currencySpendObjective))
								{
									if (!(objective is ConcreteSummonAmountObjective concreteSummonAmountObjective))
									{
										if (!(objective is AnySummonObjective))
										{
											if (!(objective is ConcreteRarityGetGirlObjective))
											{
												ConcreteGetGirlObjective concreteGetGirlObjective = objective as ConcreteGetGirlObjective;
												if (concreteGetGirlObjective == null)
												{
													if (!(objective is AnyGetGirlObjective))
													{
														if (!(objective is ConcreteEventGirlPromoteObjective))
														{
															ConcreteMainGirlPromoteObjective concreteMainGirlPromoteObjective = objective as ConcreteMainGirlPromoteObjective;
															if (concreteMainGirlPromoteObjective == null)
															{
																if (!(objective is ConcreteRarityGirlPromoteObjective))
																{
																	if (!(objective is AnyMainGirlPromoteObjective))
																	{
																		ConcreteGirlPromoteObjective concreteGirlPromoteObjective = objective as ConcreteGirlPromoteObjective;
																		if (concreteGirlPromoteObjective == null)
																		{
																			if (!(objective is AnyGirlPromoteObjective))
																			{
																				if (!(objective is AnyMergeObjective))
																				{
																					if (!(objective is ConcreteTypeMergeObjective concreteTypeMergeObjective))
																					{
																						if (!(objective is OneWayMergeItemObjective))
																						{
																							if (!(objective is MergeItemObjective))
																							{
																								if (!(objective is SpawnerReloadObjective))
																								{
																									if (!(objective is GetPhotoObjective))
																									{
																										if (!(objective is GetConcreteRarityGirlPhotoObjective))
																										{
																											if (!(objective is GetConcreteGirlPhotoObjective))
																											{
																												if (!(objective is GetConcreteCompletedDialogueAnswersObjective))
																												{
																													if (!(objective is GetConcreteCompletedDialogueObjective))
																													{
																														if (!(objective is GetBPLevelsObjective getBPLevelsObjective))
																														{
																															if (!(objective is GetBPRewardsObjective getBPRewardsObjective))
																															{
																																if (!(objective is SpendForPromoteObjective))
																																{
																																	if (objective is PresentObjective)
																																	{
																																		return (description: "ui.merge.taskview.objective.give_presents", type: ActionButtonType.ToCollection);
																																	}
																																	return (description: null, type: ActionButtonType.None);
																																}
																																return (description: "ui.merge.taskview.objective.spend_soft_promote", type: ActionButtonType.ToCollection);
																															}
																															return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.get_bp_rewards"), getBPRewardsObjective.GetTarget()), type: ActionButtonType.ToBattlePass);
																														}
																														return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.get_bp_levels"), getBPLevelsObjective.GetTarget()), type: ActionButtonType.ToBattlePass);
																													}
																													return (description: "ui.merge.taskview.objective.completed_dialogue", type: ActionButtonType.ToChat);
																												}
																												return (description: "ui.merge.taskview.objective.completed.answer_dialogue", type: ActionButtonType.ToChat);
																											}
																											return (description: "ui.merge.taskview.objective.get_photo", type: ActionButtonType.ToChat);
																										}
																										return (description: "ui.merge.taskview.objective.get_photo", type: ActionButtonType.ToChat);
																									}
																									return (description: "ui.merge.taskview.objective.get_photo", type: ActionButtonType.ToChat);
																								}
																								return (description: null, type: ActionButtonType.ToMerge);
																							}
																							return (description: null, type: ActionButtonType.None);
																						}
																						return (description: null, type: ActionButtonType.None);
																					}
																					return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.concrete_type_merge"), concreteTypeMergeObjective.ItemKey.Collection), type: ActionButtonType.ToMerge);
																				}
																				return (description: "ui.merge.taskview.objective.any_merge", type: ActionButtonType.ToMerge);
																			}
																			return (description: "ui.merge.taskview.objective.promote", type: ActionButtonType.ToCollection);
																		}
																		string nameKey = cardsCollection.Collection.FirstOrDefault((ICard card) => card.ID == concreteGirlPromoteObjective.GirlID).NameKey;
																		string arg = _localizationService.Text(nameKey);
																		return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.concrete_promote"), arg, concreteGirlPromoteObjective.TargetLevel), type: ActionButtonType.ToCollection);
																	}
																	return (description: "ui.merge.taskview.objective.promote", type: ActionButtonType.ToCollection);
																}
																return (description: "ui.merge.taskview.objective.concrete_rarity_promote", type: ActionButtonType.ToCollection);
															}
															string nameKey2 = cardsCollection.Collection.FirstOrDefault((ICard card) => card.ID == concreteMainGirlPromoteObjective.GirlID).NameKey;
															string arg2 = _localizationService.Text(nameKey2);
															return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.concrete_promote"), arg2, concreteMainGirlPromoteObjective.GetTarget()), type: ActionButtonType.ToCollection);
														}
														return (description: null, type: ActionButtonType.None);
													}
													return (description: "ui.merge.taskview.objective.get_card", type: ActionButtonType.ToSummonBank);
												}
												string nameKey3 = cardsCollection.Collection.FirstOrDefault((ICard card) => card.ID == concreteGetGirlObjective.GirlId).NameKey;
												string arg3 = _localizationService.Text(nameKey3);
												return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.get_card_concr"), arg3), type: ActionButtonType.ToSummonBank);
											}
											return (description: "ui.merge.taskview.objective.get_card_rarity", type: ActionButtonType.ToSummonBank);
										}
										return (description: "ui.merge.taskview.objective.summon", type: ActionButtonType.ToSummonBank);
									}
									return (description: string.Format(_localizationService.Text("ui.merge.taskview.objective.concrete_summon_amount"), concreteSummonAmountObjective.GetTarget()), type: ActionButtonType.ToSummonBank);
								}
								if (currencySpendObjective.CurrencyType == CurrencyType.Soft)
								{
									return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToCollection);
								}
								if (currencySpendObjective.CurrencyType == CurrencyType.Hard || currencySpendObjective.CurrencyType == CurrencyType.Energy)
								{
									return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToMerge);
								}
								return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.None);
							}
							if (currencyAddObjective.CurrencyType == CurrencyType.Soft)
							{
								return (description: "ui.merge.taskview.objective.add_currecy", type: ActionButtonType.ToBankSoft);
							}
							if (currencyAddObjective.CurrencyType == CurrencyType.Hard)
							{
								return (description: "ui.merge.taskview.objective.add_currecy", type: ActionButtonType.ToBankHard);
							}
							if (currencyAddObjective.CurrencyType == CurrencyType.LovePoints)
							{
								return (description: "ui.merge.taskview.objective.add_currecy", type: ActionButtonType.ToCollection);
							}
							return (description: "ui.merge.taskview.objective.add_currecy", type: ActionButtonType.None);
						}
						return (description: "ui.merge.taskview.objective.roulette_concr", type: ActionButtonType.ToRoulette);
					}
					return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToBankMerge);
				}
				return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToMerge);
			}
			return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToMerge);
		}
		return (description: "ui.merge.taskview.objective.spend_currecy", type: ActionButtonType.ToBankMerge);
	}
}

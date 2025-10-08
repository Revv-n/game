using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using StripClub.UI;

namespace GreenT.HornyScapes.Relationships.Views;

public class DateRewardViewProvider : MonoViewManager<(int Id, IReadOnlyList<RewardWithManyConditions> Rewards), DateRewardView>
{
}

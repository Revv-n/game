using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using StripClub.UI;

namespace GreenT.HornyScapes.Relationships.Views;

public class ComingSoonDateRewardViewProvider : MonoViewManager<(int Id, IReadOnlyList<RewardWithManyConditions> Rewards), ComingSoonDateRewardView>
{
}

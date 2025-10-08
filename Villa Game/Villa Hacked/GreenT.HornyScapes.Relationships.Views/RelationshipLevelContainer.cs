using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships.Views;

public class RelationshipLevelContainer : MonoView
{
	public class ViewManager : ViewManager<RelationshipLevelContainer>
	{
	}

	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private RectTransform _rewardContainer;

	public void Set(BaseRewardView rewardView, int level, bool isNewStatus)
	{
		Transform obj = rewardView.gameObject.transform;
		obj.SetParent(_rewardContainer);
		obj.position = Vector3.zero;
		obj.localScale = Vector3.one;
		rewardView.SetLevel(level, isNewStatus);
		rewardView.ShowLevel(isNewStatus);
	}
}

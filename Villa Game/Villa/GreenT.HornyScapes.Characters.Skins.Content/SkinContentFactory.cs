using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins.Content;

public class SkinContentFactory : IFactory<int, LinkedContentAnalyticData, LinkedContent, SkinLinkedContent>, IFactory, IFactory<int, LinkedContentAnalyticData, SkinLinkedContent>
{
	private const string skinNameKey = "content.character.skins.{0}.name";

	private readonly SkinManager skinManager;

	public SkinContentFactory(SkinManager skinManager)
	{
		this.skinManager = skinManager;
	}

	public SkinLinkedContent Create(int skinID, LinkedContentAnalyticData analyticData, LinkedContent next = null)
	{
		Skin skin = skinManager.Get(skinID);
		string text = $"content.character.skins.{skinID}.name";
		return new SkinLinkedContent(skin, analyticData, text, next);
	}

	public SkinLinkedContent Create(int skinID, LinkedContentAnalyticData analyticData)
	{
		return Create(skinID, analyticData, null);
	}
}

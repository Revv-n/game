using StripClub.Model;

namespace GreenT.HornyScapes.Content;

public interface IContentFactory
{
	LinkedContent Create(LinkedContent.Map map);
}

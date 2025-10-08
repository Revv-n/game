using System.Collections.Generic;
using StripClub.Model;

namespace GreenT.HornyScapes.Extensions;

public static class LinkedContentExtensions
{
	public static void ReplaceNode(LinkedContent previous, LinkedContent current, LinkedContent replacement)
	{
		replacement.Insert(current.Next());
		previous.ReleaseNext();
		previous.Insert(replacement);
	}

	public static List<LinkedContent> TransformToArray(LinkedContent content)
	{
		List<LinkedContent> list = new List<LinkedContent>();
		while (content != null)
		{
			list.Add(content);
			content = content.Next();
		}
		return list;
	}
}

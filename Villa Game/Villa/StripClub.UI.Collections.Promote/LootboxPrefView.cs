using System.Text;

namespace StripClub.UI.Collections.Promote;

public class LootboxPrefView : PrefView
{
	public class LootboxPrefManager : ViewManager<LootboxPrefView>
	{
	}

	public override StringBuilder BuildString(object name, object value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(name).Append("\n").Append(value);
		return stringBuilder;
	}
}

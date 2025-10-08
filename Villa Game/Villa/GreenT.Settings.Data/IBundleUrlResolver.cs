namespace GreenT.Settings.Data;

public interface IBundleUrlResolver
{
	string BundlesRoot { get; }

	string BundleUrl(BundleType type);

	string BundleUrl(string bundle);
}

using System.Diagnostics;
using System.Text;
using GreenT.AssetBundles.Communication;

namespace GreenT.AssetBundles;

public static class AssetBundleReporter
{
	private static StringBuilder report = new StringBuilder("Asset Bundle Report:\n");

	public static void ReportMessage(string nameBundle, AssetBundleRequest request, Stopwatch timer)
	{
		SetReportAssetBundle(new AssetBundleReport(request.Response.info.name, request.BundleUrl, request.Response.isCached, request.Response.info.bundleHash, timer.Elapsed));
	}

	public static void SetReportAssetBundle(AssetBundleReport reportBundle)
	{
		report.Append($"{reportBundle}\n \n");
	}

	public static string GetAssetBundlesReport()
	{
		return report.ToString();
	}
}

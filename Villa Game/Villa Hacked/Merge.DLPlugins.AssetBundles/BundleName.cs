namespace Merge.DLPlugins.AssetBundles;

public struct BundleName
{
	private string m_Name;

	public BundleName(string name)
	{
		m_Name = name;
	}

	public override string ToString()
	{
		return m_Name;
	}

	public static implicit operator string(BundleName bn)
	{
		return bn.m_Name;
	}

	public static implicit operator BundleName(string str)
	{
		return new BundleName(str);
	}
}

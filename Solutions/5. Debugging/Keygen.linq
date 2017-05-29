<Query Kind="Program">
  <Namespace>System.Net.NetworkInformation</Namespace>
</Query>

void Main()
{
	NetworkInterface networkInterface =  NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault<NetworkInterface>();
	var networkBytes = networkInterface.GetPhysicalAddress().GetAddressBytes().Dump();

	Eval_a eval_a = new Eval_a
	{
	   a = BitConverter.GetBytes(DateTime.Now.Date.ToBinary()),
	};
	
	var coded = networkBytes.Select(eval_a.eval_a);
	var correctPass = string.Join("-",coded.Select(eval_a_m)).Dump();
}


int eval_a_m(int a)
{
	if(a>= 999) return a;
	return a*10;
}

public class Eval_a{
  public byte[] a;
	public int[] b;
	public int eval_a(byte a_0, int a_1)
	{
		return a_0 ^ a[a_1];
	}

	public int eval_a(int a_0, int a_1)
	{
		return a_0 - b[a_1];
	}
}
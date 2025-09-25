using System.Text;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  public static string ConvertEucKr(byte[] eucKrBytes) => Encoding.GetEncoding("euc-kr").GetString(eucKrBytes);
}
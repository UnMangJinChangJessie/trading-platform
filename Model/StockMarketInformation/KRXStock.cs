using System.IO.Compression;
using System.Net;
using System.Text;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  private const string KRX_KOSPI_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/kospi_code.mst.zip";
  private const string KRX_KOSDAQ_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/kosdaq_code.mst.zip";
  public class KRXStockInformation {
    public required string Ticker { get; set; }
    public required string Name { get; set; }
    public required string StandardSecuritiesCode { get; set; }
  }
  public static class KRXStock {
    public static readonly List<KRXStockInformation> Data = [];
    public static async Task<bool> Load() {
      Data.Clear();
      // download master zip
      var client = new HttpClient() {
        Timeout = TimeSpan.FromSeconds(5.0)
      };
      var kospiResp = await client.GetStreamAsync(KRX_KOSPI_MASTER_URL);
      var kosdaqResp = await client.GetStreamAsync(KRX_KOSDAQ_MASTER_URL);
      var kospiZip = new ZipArchive(kospiResp);
      var kosdaqZip = new ZipArchive(kosdaqResp);
      var kospiEucKr = kospiZip.GetEntry("kospi_code.mst")?.Open();
      var kosdaqEucKr = kosdaqZip.GetEntry("kosdaq_code.mst")?.Open();
      if (kospiEucKr == null || kosdaqEucKr == null) return false;

      // the last 228 bytes are reserved for further implementations.
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      StreamReader reader = new(kospiEucKr, Encoding.GetEncoding("euc-kr"));
      string? line;
      while ((line = await reader.ReadLineAsync()) != null) {
        Data.Add(new() {
          Ticker = line[0..9].Trim(),
          StandardSecuritiesCode = line[9..21].Trim(),
          Name = line[21..^228].Trim(),
        });
      }
      reader = new(kosdaqEucKr, Encoding.GetEncoding("euc-kr"));
      while ((line = await reader.ReadLineAsync()) != null) {
        Data.Add(new() {
          Ticker = line[0..9].Trim(),
          StandardSecuritiesCode = line[9..21].Trim(),
          Name = line[21..^228].Trim(),
        });
      }
      return true;
    }
    public static KRXStockInformation? SearchByTicker(string ticker) {
      return Data.AsParallel().Where(x => x.Ticker == ticker).FirstOrDefault();
    }
    public static decimal GetTickIncrement(decimal val) => val switch {
      < 0 => 0,
      < 2_000 => Math.Floor(val) + 1,
      < 5_000 => (Math.Floor(val / 5) + 1) * 5,
      < 20_000 => (Math.Floor(val / 10) + 1) * 10,
      < 50_000 => (Math.Floor(val / 50) + 1) * 50,
      < 200_000 => (Math.Floor(val / 100) + 1) * 100,
      < 500_000 => (Math.Floor(val / 500) + 1) * 500,
      _ => (Math.Floor(val / 1_000) + 1) * 1_000
    };
    public static decimal GetTickDecrement(decimal val) => val switch {
      > 500_000 => (Math.Ceiling(val / 1_000) - 1) * 1_000,
      > 200_000 => (Math.Ceiling(val / 500) - 1) * 500,
      > 50_000 => (Math.Ceiling(val / 100) - 1) * 100,
      > 20_000 => (Math.Ceiling(val / 50) - 1) * 50,
      > 5_000 => (Math.Ceiling(val / 10) - 1) * 10,
      > 2_000 => (Math.Ceiling(val / 5) - 1) * 5,
      > 0 => (Math.Ceiling(val) - 1) * 5,
      _ => 0
    };
  }
}
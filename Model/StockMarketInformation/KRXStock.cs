using System.IO.Compression;
using System.Net;

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
      // download master zip
      var client = new WebClient();
      var kospiResp = await client.DownloadFileAsync(KRX_KOSPI_MASTER_URL, "a.");
      var kosdaqResp = await client.GetAsync(KRX_KOSDAQ_MASTER_URL);
      if (!kospiResp.IsSuccessStatusCode || !kosdaqResp.IsSuccessStatusCode) {
        return false;
      }
      var kospiZip = new ZipArchive(await kospiResp.Content.ReadAsStreamAsync());
      var kosdaqZip = new ZipArchive(await kosdaqResp.Content.ReadAsStreamAsync());
      var kospiEucKr = kospiZip.GetEntry("kospi_code.mst")?.Open();
      var kosdaqEucKr = kospiZip.GetEntry("kosdaq_code.mst")?.Open();
      if (kospiEucKr == null || kosdaqEucKr == null) return false;

      // a line of information is 289, including a line feed.
      // The first bytes for a ticker is 9, 12 for a standard code, and 40 for a name.
      // i.e. the last 228 bytes are reserved for further implementations.
      var line = new byte[289];
      while (await kospiEucKr.ReadAsync(line, 0, 289) != 289) {
        Data.Add(new() {
          Ticker = ConvertEucKr(line[0..9]),
          StandardSecuritiesCode = ConvertEucKr(line[9..21]),
          Name = ConvertEucKr(line[21..61]),
        });
      }
      // For KOSDAQ, it's 221 + 61 + 1 = 283 bytes
      while (await kosdaqEucKr.ReadAsync(line, 0, 283) != 283) {
        Data.Add(new() {
          Ticker = ConvertEucKr(line[0..9]),
          StandardSecuritiesCode = ConvertEucKr(line[9..21]),
          Name = ConvertEucKr(line[21..61]),
        });
      }
      return true;
    }
  }
}
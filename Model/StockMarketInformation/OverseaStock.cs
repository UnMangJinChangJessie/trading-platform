using System.IO.Compression;
using System.Text;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  public const string NASDAQ_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/nasmst.cod.zip";
  public const string NYSE_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/nysmst.cod.zip";
  public const string NYSE_AMERICA_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/amsmst.cod.zip";
  public const string SHANGHAI_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/shsmst.cod.zip";
  public const string SHENZHEN_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/szsmst.cod.zip";
  public const string HONGKONG_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/hksmst.cod.zip";
  public const string JAPAN_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/tsemst.cod.zip";
  public const string HANOI_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/hnxmst.cod.zip";
  public const string HOCHIMINH_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/hsxmst.cod.zip";
  public class OverseaStockInformation {
    public required string Ticker { get; set; }
    public required string Name { get; set; }
    public required string Currency { get; set; }
    // public required string StandardSecuritiesCode { get; set; }
    public required Model.KoreaInvestment.Exchange Exchange { get; set; }
    public required int DecimalDigitCount { get; set; }
    public required int BidQuantityUnit { get; set; }
    public required int OfferQuantityUnit { get; set; }
  }
  public static class OverseaStock {
    public static Exchange GetExchange(string input) => input switch {
      "NAS" => Exchange.Nasdaq,
      "NYS" => Exchange.NewYorkStockExchange,
      "AMS" => Exchange.NyseAmerican,
      "USA" => Exchange.UnitedStates,
      "SHS" => Exchange.Shanghai,
      "SZS" => Exchange.Shenzhen,
      "CHN" => Exchange.China,
      "HKS" => Exchange.HongKong,
      "TSE" => Exchange.Tokyo,
      "HNX" => Exchange.Hanoi,
      "HSX" => Exchange.HoChiMinh,
      "VNM" => Exchange.Vietnam,
      _ => throw new ArgumentException($"Invalid exchange code '{input}'")
    };
    public static readonly List<OverseaStockInformation> Data = [];
    public static async Task<bool> Load() {
      Data.Clear();
      // download master zip
      var client = new HttpClient() {
        Timeout = TimeSpan.FromSeconds(5.0)
      };
      if (!await LoadAmerican(client)) return false;
      return true;
    }
    private static void ParseLine(Stream stream) {
      string? line;
      StreamReader reader;
      reader = new(stream, Encoding.GetEncoding("euc-kr"));
      while ((line = reader.ReadLine()) != null) {
        var token = line.Split('\t');
        var item = new OverseaStockInformation() {
          Ticker = token[4].Trim(),
          Name = token[7].Trim(),
          Exchange = GetExchange(token[2].Trim()),
          DecimalDigitCount = int.Parse(token[10].Trim()),
          BidQuantityUnit = int.Parse(token[13].Trim()),
          OfferQuantityUnit = int.Parse(token[14].Trim()),
          Currency = token[9].Trim()
        };
        Data.Add(item);
      }
    }
    private static async Task<bool> LoadAmerican(HttpClient client) {
      var nasdaqResp = await client.GetStreamAsync(NASDAQ_MASTER_URL);
      var nyseResp = await client.GetStreamAsync(NYSE_MASTER_URL);
      var amexResp = await client.GetStreamAsync(NYSE_AMERICA_MASTER_URL);
      if (nasdaqResp == null) return false;
      if (nyseResp == null) return false;
      if (amexResp == null) return false;
      var nasdaqZip = new ZipArchive(nasdaqResp);
      var nyseZip = new ZipArchive(nyseResp);
      var amexZip = new ZipArchive(amexResp);
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      ParseLine(nasdaqZip.GetEntry("NASMST.COD")!.Open());
      ParseLine(nyseZip.GetEntry("NYSMST.COD")!.Open());
      ParseLine(amexZip.GetEntry("AMSMST.COD")!.Open());
      return true;
    }
    public static OverseaStockInformation? SearchByTicker(Exchange exchange, string ticker) =>
      Data.AsParallel().Where(x => (x.Exchange & exchange) != Exchange.None && x.Ticker == ticker).FirstOrDefault();
  }
}
using System.IO.Compression;
using System.Net;
using System.Text;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  private const string KRX_KOSPI_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/kospi_code.mst.zip";
  private const string KRX_KOSDAQ_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/kosdaq_code.mst.zip";
  private const string NEXTRADE_KOSPI_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/nxt_kospi_code.mst.zip";
  private const string NEXTRADE_KOSDAQ_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/nxt_kosdaq_code.mst.zip";
  public class KRXStockInformation {
    public required Exchange Exchange { get; set; }
    public required string Ticker { get; set; }
    public required string Name { get; set; }
    public required string StandardSecuritiesCode { get; set; }
  }
  public static class KRXStock {
    public static readonly List<KRXStockInformation> Data = [];
    public static async Task<bool> Load() {
      Data.Clear();
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      // the last 228 bytes are reserved for further implementations.
      if (await LoadMasterFile("./Resources/MasterFiles/KOSPI.txt", KRX_KOSPI_MASTER_URL) is not Stream kospi) return false;
      using (var reader = new StreamReader(kospi)) {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null) {
          Data.Add(new() {
            Exchange = Exchange.KoreaExchange,
            Ticker = line[0..9].Trim(),
            StandardSecuritiesCode = line[9..21].Trim(),
            Name = line[21..^228].Trim(),
          });
        }
      }
      if (await LoadMasterFile("./Resources/MasterFiles/KOSDAQ.txt", KRX_KOSDAQ_MASTER_URL) is not Stream kosdaq) return false;
      using (var reader = new StreamReader(kosdaq)) {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null) {
          Data.Add(new() {
            Exchange = Exchange.KoreaExchange,
            Ticker = line[0..9].Trim(),
            StandardSecuritiesCode = line[9..21].Trim(),
            Name = line[21..^222].Trim(),
          });
        }
      }
      if (await LoadMasterFile("./Resources/MasterFiles/KOSPI_NEXTRADE.txt", NEXTRADE_KOSPI_MASTER_URL) is not Stream kospiNxt) return false;
      using (var reader = new StreamReader(kospiNxt)) {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null) {
          string ticker = line[0..9].Trim();
          string securitiesCode = line[9..21].Trim();
          string name = line[21..^228];
          var duplicate = Data.Where(x => x.Ticker == ticker).SingleOrDefault();
          if (duplicate != null) duplicate.Exchange |= Exchange.NexTrade;
          else Data.Add(new() {
            Exchange = Exchange.NexTrade,
            Ticker = ticker,
            StandardSecuritiesCode = securitiesCode,
            Name = name
          });
        }
      }
      if (await LoadMasterFile("./Resources/MasterFiles/KOSDAQ.txt", KRX_KOSDAQ_MASTER_URL) is not Stream kosdaqNxt) return false;
      using (var reader = new StreamReader(kosdaqNxt)) {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null) {
          string ticker = line[0..9].Trim();
          string securitiesCode = line[9..21].Trim();
          string name = line[21..^222];
          var duplicate = Data.Where(x => x.Ticker == ticker).SingleOrDefault();
          if (duplicate != null) duplicate.Exchange |= Exchange.NexTrade;
          else Data.Add(new() {
            Exchange = Exchange.NexTrade,
            Ticker = ticker,
            StandardSecuritiesCode = securitiesCode,
            Name = name
          });
        }
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
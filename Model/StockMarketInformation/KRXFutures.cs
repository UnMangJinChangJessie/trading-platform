using System.ComponentModel;
using System.Text;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  private const string KRX_INDEX_FUTURES_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/fo_idx_code_mts.mst.zip";
  private const string KRX_STOCK_FUTURES_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/fo_stk_code_mts.mst.zip";
  private const string KRX_COMMODITY_FUTURES_MASTER_URL = "https://new.real.download.dws.co.kr/common/master/fo_com_code.mst.zip";

  public enum KRXFuturesType {
    Unknown,
    [Description("지수선물")]
    IndexFutures,
    [Description("지수스프레드")]
    IndexSpread,
    [Description("스타선물")]
    StarFutures,
    [Description("스타스프레드")]
    StarSpread,
    [Description("지수옵션(풋)")]
    IndexPut,
    [Description("지수옵션(콜)")]
    IndexCall,
    [Description("변동성선물")]
    VixFutures,
    [Description("변동성스프레드")]
    VixSpread,
    [Description("섹터선물")]
    SectorFutures,
    [Description("섹터스프레드")]
    SectorSpread,
    [Description("미니선물")]
    MiniFutures,
    [Description("미니스프레드")]
    MiniSpread,
    [Description("미니옵션(풋)")]
    MiniPut,
    [Description("미니옵션(콜)")]
    MiniCall,
    [Description("미니코스닥옵션(풋)")]
    KosdaqPut,
    [Description("미니코스닥옵션(콜)")]
    KosdaqCall,
    [Description("위클리옵션(풋)")]
    WeeklyPut,
    [Description("위클리옵션(콜)")]
    WeeklyCall,
    [Description("코스피주식선물")]
    KospiStockFutures,
    [Description("코스피주식스프레드")]
    KospiStockSpread,
    [Description("코스닥주식선물")]
    KosdaqStockFutures,
    [Description("코스닥주식스프레드")]
    KosdaqStockSpread,
    [Description("주식옵션(풋)")]
    StockPut,
    [Description("주식옵션(콜)")]
    StockCall,
    [Description("금리선물")]
    InterestFutures,
    [Description("금리스프레드")]
    InterestSpread,
    [Description("통화선물")]
    CurrencyFutures,
    [Description("통화스프레드")]
    CurrencySpread,
    [Description("상품선물")]
    CommodityFutures,
    [Description("상품스프레드")]
    CommoditySpread,
  }
  public enum OptionsMoney {
    AtTheMoney,
    InTheMoney,
    OutOfTheMoney
  }
  public static KRXFuturesType GetIndexTypeFromCode(char code) => code switch {
    '1' => KRXFuturesType.IndexFutures,
    '2' => KRXFuturesType.IndexSpread,
    '3' => KRXFuturesType.StarFutures,
    '4' => KRXFuturesType.StarSpread,
    '5' => KRXFuturesType.IndexCall,
    '6' => KRXFuturesType.IndexPut,
    '7' => KRXFuturesType.VixFutures,
    '8' => KRXFuturesType.VixSpread,
    '9' => KRXFuturesType.SectorFutures,
    'A' => KRXFuturesType.SectorSpread,
    'B' => KRXFuturesType.MiniFutures,
    'C' => KRXFuturesType.MiniSpread,
    'D' => KRXFuturesType.MiniCall,
    'E' => KRXFuturesType.MiniPut,
    'J' => KRXFuturesType.KosdaqCall,
    'K' => KRXFuturesType.KosdaqPut,
    'L' => KRXFuturesType.WeeklyCall,
    'M' => KRXFuturesType.WeeklyPut,
    _ => KRXFuturesType.Unknown
  };
  public static KRXFuturesType GetStockTypeFromCode(char code) => code switch {
    '1' => KRXFuturesType.KospiStockFutures,
    '2' => KRXFuturesType.KospiStockSpread,
    '3' => KRXFuturesType.KosdaqStockFutures,
    '4' => KRXFuturesType.KosdaqStockSpread,
    '5' => KRXFuturesType.StockCall,
    '6' => KRXFuturesType.StockPut,
    _ => KRXFuturesType.Unknown
  };
  public static KRXFuturesType GetCommodityTypeFromCode(string code) => code switch {
    "11" => KRXFuturesType.InterestFutures,
    "12" => KRXFuturesType.InterestSpread,
    "21" => KRXFuturesType.CurrencyFutures,
    "22" => KRXFuturesType.CurrencySpread,
    "31" => KRXFuturesType.CommodityFutures,
    "32" => KRXFuturesType.CommoditySpread,
    _ => KRXFuturesType.Unknown
  };
  public static OptionsMoney? GetOptionMoneyTypeFromCode(char code) => code switch {
    '1' => OptionsMoney.AtTheMoney,
    '2' => OptionsMoney.InTheMoney,
    '3' => OptionsMoney.OutOfTheMoney,
    _ => null
  };
  public class KRXFuturesInformation {
    public required KRXFuturesType FuturesType { get; set; }
    public required string Ticker { get; set; }
    public required string StandardSecuritiesCode { get; set; }
    public required string Name { get; set; }
    public OptionsMoney? OptionsMoney { get; set; }
    public decimal? OptionsExercise { get; set; }
    public int? MaturityMonths { get; set; }
    public required string UnderlyingAssetTicker { get; set; }
    public required string UnderlyingAssetName { get; set; }
  }
  public static class KRXFutures {
    public static List<KRXFuturesInformation> Data { get; private set; } = [];
    public static async ValueTask<bool> Load() {
      if (await LoadMasterFile("./Resources/MasterFiles/KRXIndexFutures.txt", KRX_INDEX_FUTURES_MASTER_URL) is Stream index) {
        using StreamReader reader = new(index);
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null) {
          string[] tokens = line.Split('|');
          KRXFuturesInformation information = new() {
            FuturesType = GetIndexTypeFromCode(tokens[0][0]),
            Ticker = tokens[1],
            StandardSecuritiesCode = tokens[2],
            Name = tokens[3],
            OptionsMoney = GetOptionMoneyTypeFromCode(tokens[4][0]),
            UnderlyingAssetTicker = tokens[7],
            UnderlyingAssetName = tokens[8]
          };
          information.OptionsExercise = decimal.TryParse(tokens[5], out decimal x) ? x : null;
          information.MaturityMonths = int.TryParse(tokens[6], out int y) ? y : null;
        }
      }
      else return false;
      if (await LoadMasterFile("./Resources/MasterFiles/KRXStockFutures.txt", KRX_STOCK_FUTURES_MASTER_URL) is Stream stock) {
        using StreamReader reader = new(stock);
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null) {
          string[] tokens = line.Split('|');
          KRXFuturesInformation information = new() {
            FuturesType = GetStockTypeFromCode(tokens[0][0]),
            Ticker = tokens[1],
            StandardSecuritiesCode = tokens[2],
            Name = tokens[3],
            OptionsMoney = GetOptionMoneyTypeFromCode(tokens[4][0]),
            UnderlyingAssetTicker = tokens[7],
            UnderlyingAssetName = tokens[8]
          };
          information.OptionsExercise = decimal.TryParse(tokens[5], out decimal x) ? x : null;
          information.MaturityMonths = int.TryParse(tokens[6], out int y) ? y : null;
        }
      }
      else return false;
      if (await LoadMasterFile("./Resources/MasterFiles/KRXCommodityFutures.txt", KRX_COMMODITY_FUTURES_MASTER_URL, true) is Stream commodity) {
        using BinaryReader reader = new(commodity);
        byte[]? line;
        while ((line = reader.ReadBytes(116 + 1)).Length == 117) {
          KRXFuturesInformation information = new() {
            FuturesType = GetCommodityTypeFromCode(ConvertEucKr(line[..2])),
            Ticker = ConvertEucKr(line[2..11]).Trim(),
            StandardSecuritiesCode = ConvertEucKr(line[11..23]),
            // 한투에서 제공한 샘플을 보면 글자 수 계산하기 어려우니 앞 32글자만 읽고 적당히 잘라내는 것 같다.
            // 여기선 EUC-KR 40바이트를 그대로 가져오겠다.
            Name = ConvertEucKr(line[23..63]).Trim(),
            OptionsMoney = GetOptionMoneyTypeFromCode(ConvertEucKr(line[63..64])[0]),
            OptionsExercise = decimal.TryParse(ConvertEucKr(line[64..72]), out decimal x) ? x : null,
            MaturityMonths = int.TryParse(ConvertEucKr(line[72..73]), out int y) ? y : null,
            UnderlyingAssetTicker = ConvertEucKr(line[73..76]),
            UnderlyingAssetName = ConvertEucKr(line[76..116]),
          };
        }
      }
      else return false;
      return true;
    }
  }
}
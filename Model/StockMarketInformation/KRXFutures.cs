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
    MiniKosdaqPut,
    [Description("미니코스닥옵션(콜)")]
    MiniKosdaqCall,
    [Description("위클리옵션(풋)")]
    WeeklyPut,
    [Description("위클리옵션(콜)")]
    WeeklyCall
  }
  public enum OptionsMoney {
    AtTheMoney,
    InTheMoney,
    OutOfTheMoney
  }
  public class KRXFuturesInformation {
    public required KRXFuturesType FuturesType { get; set; }
    public required string Ticker { get; set; }
    public required string StandardSecuritiesCode { get; set; }
    public required string Name { get; set; }
    public required OptionsMoney OptionsMoney { get; set; }
    public required decimal? OptionsExercise { get; set; }
    public required int MaturityMonths { get; set; }
    public required string UnderlyingAssetTicker { get; set; }
    public required string UnderlyingAssetName { get; set; }
  }
  public static class KRXFutures {
    public static List<KRXFuturesInformation> Data { get; private set; } = [];
    public static bool Load() {
      throw new NotImplementedException();
    }
  }
}
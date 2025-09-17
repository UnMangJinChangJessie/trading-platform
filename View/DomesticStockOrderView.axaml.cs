using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class DomesticStockOrderView : UserControl {
  public string ProductName { get; set; } = "";
  private static Dictionary<string, OrderMethod> ORDER_METHOD_NAME_DICT = new() {
    ["지정가"] = OrderMethod.Limit,
    ["시장가"] = OrderMethod.Market,
    ["조건부지정가"] = OrderMethod.ConditionalLimit,
    ["최유리지정가"] = OrderMethod.BestOffer,
    ["최우선지정가"] = OrderMethod.TopPriority,
    ["장전전일종가매매"] = OrderMethod.PreMarket,
    ["장후종가매매"] = OrderMethod.PostMarket,
    ["장후단일가매매"] = OrderMethod.AfterMarket,
    ["중간가"] = OrderMethod.Intermediate
  };
  public DomesticStockOrderView() {
    InitializeComponent();
    var combo = OrderView.FindControl<ComboBox>("OrderMethodComboBox");
    if (combo != null) combo.ItemsSource = ORDER_METHOD_NAME_DICT.Keys;
  }
}
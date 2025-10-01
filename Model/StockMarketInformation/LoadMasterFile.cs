using System.IO.Compression;
using System.Text;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  public static async ValueTask<Stream?> LoadMasterFile(string localPath, string downloadPath) {
    var client = new HttpClient() {
      Timeout = TimeSpan.FromSeconds(5.0)
    };
    if (!File.Exists(localPath) || (DateTime.UtcNow - File.GetLastWriteTimeUtc(localPath)) > TimeSpan.FromHours(8)) {
      var resp = await client.GetStreamAsync(downloadPath);
      var zip = new ZipArchive(resp);
      var eucKr = zip.Entries.SingleOrDefault()?.Open();
      if (eucKr == null) return null;
      using (var writer = new StreamReader(eucKr, Encoding.GetEncoding("euc-kr"))) {
        var localDirectory = Path.GetDirectoryName(localPath);
        if (localDirectory != null && !Directory.Exists(localDirectory)) {
          Directory.CreateDirectory(localDirectory);
        }
        var stream = File.Create(localPath);
        stream.Write(Encoding.UTF8.GetBytes(writer.ReadToEnd()));
        stream.Close();
      }
    }
    return File.OpenRead(localPath);
  }
}
using System.IO.Compression;
using System.Text;

namespace trading_platform.Model;

public static partial class StockMarketInformation {
  private readonly static Encoding EucKrEncoding = Encoding.GetEncoding("euc-kr");
  public static async ValueTask<Stream?> LoadMasterFile(string localPath, string downloadPath, bool asRawStream = false) {
    var client = new HttpClient() {
      Timeout = TimeSpan.FromSeconds(5.0)
    };
    if (!File.Exists(localPath) || (DateTime.UtcNow - File.GetLastWriteTimeUtc(localPath)) > TimeSpan.FromHours(4)) {
      Stream resp;
      try {
        resp = await client.GetStreamAsync(downloadPath);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
        return null;
      }
      var zip = new ZipArchive(resp);
      var eucKr = zip.Entries.SingleOrDefault()?.Open();
      if (eucKr == null) return null;
      using (var writer = new StreamReader(eucKr, EucKrEncoding)) {
        var localDirectory = Path.GetDirectoryName(localPath);
        if (localDirectory != null && !Directory.Exists(localDirectory)) {
          Directory.CreateDirectory(localDirectory);
        }
        var stream = File.Create(localPath);
        stream.Write(Encoding.UTF8.GetBytes(writer.ReadToEnd()));
        stream.Close();
      }
      if (asRawStream) return zip.Entries.SingleOrDefault()?.Open();
    }
    return File.OpenRead(localPath);
  }
  public static string ConvertEucKr(byte[] eucKrBytes) => EucKrEncoding.GetString(eucKrBytes);
}
namespace CinemaBooking.Web.UnitTests.TestHelpers;
internal class TestFile : IDisposable
{
    public string Path { get; }

    private TestFile(string path)
    {
        Path = path;
    }

    public static TestFile CreateFile(string fileExtension) {
        var fileName = GenerateFileName(fileExtension);
        File.Create(fileName).Close();
        return new TestFile(fileName);
    }

    private static string GenerateFileName(string fileExtension) => $"{Guid.NewGuid()}.temp.{fileExtension}";
    public void Dispose()
    {
        File.Delete(Path);
    }
}

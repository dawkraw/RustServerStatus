using System.Text;

namespace RustServerStatus.Extensions;

public static class BinaryReaderExtensions
{
    public static string ReadStringWithoutSize(this BinaryReader reader)
    {
        if (reader.BaseStream.Length == 0) return string.Empty;
        
        var sb = new StringBuilder();

        while (Convert.ToInt32(reader.PeekChar()) != 0) sb.Append(reader.ReadChar());

        reader.ReadChar(); // read null char
        return sb.ToString();
    }
}
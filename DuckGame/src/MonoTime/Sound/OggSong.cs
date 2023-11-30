using System.IO;

namespace DuckGame
{
    public class OggSong
    {
        public static MemoryStream Load(string oggFile, bool localContent = true)
        {
            oggFile = oggFile.TrimStart('/');
            Stream input = !localContent ? File.OpenRead(oggFile) : DuckFile.OpenStream(oggFile);
            MemoryStream output = new MemoryStream();
            CopyStream(input, output);
            input.Close();
            return output;
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16384];
            int count;
            while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, count);
        }
    }
}

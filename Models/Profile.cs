using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LabyrinthGame;

public class Profile
{
    public string Nickname { get; set; }
    public int Highscore { get; set; }
    public string Picture { get; set; }

    public Profile(string pNickname)
    {
        Nickname = pNickname;
        Highscore = 0;
    }

    public BitmapImage LoadImage()
    {
        byte[] imageBytes = Convert.FromBase64String(Picture);

        using var memoryStream = new MemoryStream(imageBytes);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = memoryStream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();

        return bitmap;
    }
}

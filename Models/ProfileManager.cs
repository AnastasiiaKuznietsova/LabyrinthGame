using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LabyrinthGame.Models
{
    internal class ProfileManager
    {
        // Every Profile Manager manages a GlobalProfileList, which stores all the Profiles the Assembly created (It is stored inside a JSON file)
        public List<Profile> GlobalProfileList { get; set; }


        public ProfileManager()
        {
            //this.GlobalProfileList = new();
        }


        //public string CapturePhoto(string nickname)
        //{
        //    VideoCapture capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
        //    if (!capture.IsOpened())
        //    {
        //        throw new Exception("Camera could not be opened.");
        //    }

        //    using var image = new Mat();
        //    capture.Read(image);

        //    int size = Math.Min(image.Width, image.Height);
        //    Rect cropRect = new Rect((image.Width - size) / 2, (image.Height - size) / 2, size, size);
        //    Mat croppedImage = new Mat(image, cropRect);

        //    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
        //    string saveDirectory = Path.Combine(projectDirectory, "Pictures", "Profiles");
        //    if (!Directory.Exists(saveDirectory))
        //    {
        //        Directory.CreateDirectory(saveDirectory);
        //    }

        //    string fileName = Path.Combine(saveDirectory, $"{nickname}.png");
        //    croppedImage.SaveImage(fileName);

        //    byte[] imageBytes = File.ReadAllBytes(fileName);

        //    capture.Release();
        //    capture.Dispose();

        //    return Convert.ToBase64String(imageBytes);
        //}
        public bool CanCreateNewProfile()
        {
            return this.GlobalProfileList.Count >= 3 ? false : true;
        }
    }
}

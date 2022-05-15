using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace GarbageMap.GarbageDetection.ML.DataModels
{
    public struct ImageSettings
    {
        public const int ImageHeight = 416;
        public const int ImageWidth = 416;
    }

    public class ImageInputData
    {
        [ImageType(ImageSettings.ImageHeight, ImageSettings.ImageWidth)]
        public Bitmap Image { get; set; }
    }
}

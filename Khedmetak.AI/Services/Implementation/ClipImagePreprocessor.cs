//using Microsoft.ML.OnnxRuntime.Tensors;
//using SkiaSharp;

//namespace Khedmetak.AI.Services.Implementation
//{
    

//    public static class ClipImagePreprocessor
//    {
//        private static readonly float[] Mean =
//        {
//        0.48145466f,
//        0.4578275f,
//        0.40821073f
//    };

//        private static readonly float[] Std =
//        {
//        0.26862954f,
//        0.26130258f,
//        0.27577711f
//    };

//        public static DenseTensor<float> Preprocess(Stream imageStream)
//        {
//            using var bitmap = SKBitmap.Decode(imageStream);

//            var resized = new SKBitmap(224, 224);

//            bitmap.ScalePixels(
//                resized,
//                new SKSamplingOptions(SKFilterMode.Linear));

//            var tensor = new DenseTensor<float>(new[] { 1, 3, 224, 224 });

//            for (int y = 0; y < 224; y++)
//            {
//                for (int x = 0; x < 224; x++)
//                {
//                    var pixel = resized.GetPixel(x, y);

//                    float r = pixel.Red / 255f;
//                    float g = pixel.Green / 255f;
//                    float b = pixel.Blue / 255f;

//                    tensor[0, 0, y, x] = (r - Mean[0]) / Std[0];
//                    tensor[0, 1, y, x] = (g - Mean[1]) / Std[1];
//                    tensor[0, 2, y, x] = (b - Mean[2]) / Std[2];
//                }
//            }

//            return tensor;
//        }
//    }
//}

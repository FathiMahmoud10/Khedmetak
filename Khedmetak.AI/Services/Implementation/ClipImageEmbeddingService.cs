//using Khedmetak.AI.Services.Abstraction;
//using Microsoft.ML.OnnxRuntime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Khedmetak.AI.Services.Implementation
//{
//    public class ClipImageEmbeddingService: IClipImageEmbeddingService
//    {
//        private readonly InferenceSession _session;

//        public ClipImageEmbeddingService(string modelPath)
//        {
//            _session = new InferenceSession(modelPath);
//        }

//        public Task<float[]> GenerateEmbeddingAsync(Stream imageStream)
//        {
//            var tensor = ClipImagePreprocessor.Preprocess(imageStream);

//            var inputs = new List<NamedOnnxValue>
//        {
//            NamedOnnxValue.CreateFromTensor("pixel_values", tensor)
//        };

//            using var results = _session.Run(inputs);

//            float[] embedding = results
//                .First()
//                .AsEnumerable<float>()
//                .ToArray();

//            return Task.FromResult(embedding);
//        }

//    }
//}

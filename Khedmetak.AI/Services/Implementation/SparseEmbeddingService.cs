//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Qdrant.Client.Grpc;

//namespace Khedmetak.AI.Services.Implementation
//{
    

//    public class SparseEmbeddingService
//    {
//        private readonly Dictionary<string, int> _termToId = new Dictionary<string, int>();
//        private int _nextTermId = 0;
//        private readonly double _k1 = 1.5;
//        private readonly double _b = 0.75;
//        private double _avgDocLength = 0.0;
//        private readonly Dictionary<string, int> _docFreq = new Dictionary<string, int>(); // For better IDF

//        // Call this once when you have all chunks (recommended before upserting)
//        public void FitCorpus(IEnumerable<string> allDocuments)
//        {
//            var docLengths = new List<int>();
//            var termDocFreq = new Dictionary<string, int>();

//            foreach (var doc in allDocuments)
//            {
//                var terms = Tokenize(doc);
//                docLengths.Add(terms.Count);

//                foreach (var term in terms.Distinct())
//                {
//                    termDocFreq[term] = termDocFreq.GetValueOrDefault(term) + 1;
//                }
//            }

//            if (docLengths.Any())
//                _avgDocLength = docLengths.Average();

//            // Build vocabulary + doc frequency
//            foreach (var term in termDocFreq.Keys)
//            {
//                if (!_termToId.ContainsKey(term))
//                    _termToId[term] = _nextTermId++;

//                _docFreq[term] = termDocFreq[term];
//            }
//        }

//        public Task<SparseVector> GetSparseEmbeddingAsync(string text)
//        {
//            var terms = Tokenize(text);
//            if (terms.Count == 0)
//            {
//                return Task.FromResult(new SparseVector()); // empty
//            }

//            var termFreq = terms.GroupBy(t => t)
//                                .ToDictionary(g => g.Key, g => g.Count());

//            var indicesList = new List<uint>();
//            var valuesList = new List<float>();

//            int docLength = terms.Count;

//            foreach (var kvp in termFreq)
//            {
//                string term = kvp.Key;
//                int tf = kvp.Value;

//                if (!_termToId.TryGetValue(term, out int termId))
//                    continue;

//                double idf = Math.Log(1 + (_termToId.Count / (double)(_docFreq.GetValueOrDefault(term) + 1)));
//                double score = ComputeBM25Score(tf, docLength, idf);

//                indicesList.Add((uint)termId);
//                valuesList.Add((float)score);
//            }

//            // Sort by index - VERY IMPORTANT for Qdrant
//            var sorted = indicesList.Zip(valuesList, (idx, val) => (Index: idx, Value: val))
//                                    .OrderBy(x => x.Index)
//                                    .ToList();

//            uint[] indices = sorted.Select(x => x.Index).ToArray();
//            float[] values = sorted.Select(x => x.Value).ToArray();

//            // Correct construction for Qdrant.Client
//            var sparseVector = new SparseVector
//            {
//                Indices = { indices },   // Note the collection initializer syntax
//                Values = { values }
//            };

//            return Task.FromResult(sparseVector);
//        }

//        private double ComputeBM25Score(int tf, int docLength, double idf)
//        {
//            double tfNorm = tf * (_k1 + 1) / (tf + _k1 * (1 - _b + _b * (docLength / _avgDocLength)));
//            return tfNorm * idf;
//        }

//        private List<string> Tokenize(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

//            // Arabic-friendly simple tokenizer
//            return text.ToLowerInvariant()
//                       .Split(new[] { ' ', '\t', '\n', '\r', ',', '.', '،', '؟', '!', '؛', ':' },
//                              StringSplitOptions.RemoveEmptyEntries)
//                       .Where(t => t.Length > 1)
//                       .ToList();
//        }
//    }
//}

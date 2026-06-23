using System;
using System.Threading.Tasks;
using Qdrant.Client;

namespace Khedmetak.AI
{
    public class TempTest
    {
        public async Task Test()
        {
            var client = new QdrantClient(new Uri("http://localhost:6333"));
            var collections = await client.ListCollectionsAsync();
            Console.WriteLine(collections.GetType().FullName);
        }
    }
}

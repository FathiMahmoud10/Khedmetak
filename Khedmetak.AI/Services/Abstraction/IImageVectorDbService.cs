using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Khedmetak.AI.DTOs.ImageRag;
using Qdrant.Client.Grpc;

namespace Khedmetak.AI.Services.Abstraction
{
    /// <summary>
    /// Service interface for Image RAG operations on Qdrant.
    /// Completely hides all Qdrant implementation details from controllers.
    /// </summary>
    public interface IImageVectorDbService
    {
        /// <summary>
        /// Adds a new image document to the collection.
        /// </summary>
        Task AddDocumentAsync(string documentName, Stream imageStream, string fileName);

        /// <summary>
        /// Updates an existing image document's name, vector (via image stream), or both.
        /// </summary>
        Task UpdateDocumentAsync(string documentName, string? newDocumentName, Stream? imageStream, string? fileName);

        /// <summary>
        /// Deletes an existing image document.
        /// </summary>
        Task DeleteDocumentAsync(string documentName);

        /// <summary>
        /// Retrieves document metadata for a given document name.
        /// </summary>
        Task<DocumentResponse?> GetDocumentAsync(string documentName);

        /// <summary>
        /// Retrieves a paginated list of all document file names.
        /// </summary>
        Task<(List<DocumentListResponse> Documents, int TotalCount)> GetAllDocumentsAsync();

        /// <summary>
        /// Performs search on the collection to find the most similar document.
        /// </summary>
        Task<SearchImageResponse> SearchAsync(Stream imageStream);

        /// <summary>
        /// Checks if a document with the given name already exists.
        /// </summary>
        Task<bool> DocumentExistsAsync(string documentName);

        /// <summary>
        /// Finds the Qdrant point matching the given document name.
        /// </summary>
        Task<RetrievedPoint?> FindByDocumentNameAsync(string documentName);
    }
}

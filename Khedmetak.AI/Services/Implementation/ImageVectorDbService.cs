using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Khedmetak.AI.DTOs.ImageRag;
using Khedmetak.AI.Services.Abstraction;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Khedmetak.AI.Services.Implementation
{
    /// <summary>
    /// Production-ready implementation of ImageVectorDbService.
    /// Manages Image CRUD and search operations in the Qdrant KhedmetakImagesCollection.
    /// </summary>
    public class ImageVectorDbService : IImageVectorDbService
    {
        private const string CollectionName = "DocsCollection";
        private readonly QdrantClient _client;
        private readonly IClipImageEmbeddingService _embeddingService;
        private readonly ILogger<ImageVectorDbService> _logger;

        public ImageVectorDbService(
            QdrantClient client,
            IClipImageEmbeddingService embeddingService,
            ILogger<ImageVectorDbService> logger)
        {
            _client = client;
            _embeddingService = embeddingService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> DocumentExistsAsync(string documentName)
        {
            var point = await FindByDocumentNameAsync(documentName);
            return point != null;
        }

        /// <inheritdoc />
        public async Task<RetrievedPoint?> FindByDocumentNameAsync(string documentName)
        {
            try
            {
                var filter = new Filter
                {
                    Must =
                    {
                        new Condition
                        {
                            Field = new FieldCondition
                            {
                                Key = "FileName",
                                Match = new Match { Keyword = documentName }
                            }
                        }
                    }
                };

                var scrollResult = await _client.ScrollAsync(
                    collectionName: CollectionName,
                    filter: filter,
                    limit: 1,
                    payloadSelector: true,
                    vectorsSelector: true
                );

                return scrollResult.Result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for document '{DocumentName}' in Qdrant.", documentName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task AddDocumentAsync(string documentName, Stream imageStream, string fileName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                throw new ArgumentException("DocumentName cannot be empty.", nameof(documentName));
            }

            if (imageStream == null || imageStream.Length == 0)
            {
                throw new ArgumentException("Image stream cannot be empty.", nameof(imageStream));
            }

            // Ensure unique DocumentName
            if (await DocumentExistsAsync(documentName))
            {
                throw new InvalidOperationException($"Duplicate DocumentName error: A document named '{documentName}' already exists.");
            }

            // Generate embedding
            _logger.LogInformation("Embedding Generated: Starting embedding generation for DocumentName: '{DocumentName}'", documentName);
            float[] embedding = await _embeddingService.GenerateEmbeddingAsync(imageStream);
            _logger.LogInformation("Embedding Dimension: Generated embedding with dimension size: {Dimension}", embedding.Length);

            if (embedding.Length != 768)
            {
                throw new InvalidOperationException("Embedding dimension must be 768.");
            }

            // Create Point Struct with unique Guid
            var pointId = Guid.NewGuid();
            var point = new PointStruct
            {
                Id = new PointId { Uuid = pointId.ToString() },
                Vectors = embedding,
                Payload =
                {
                    //["DocumentName"] = documentName,
                    ["FileName"] = documentName,
                    //["UploadedAt"] = DateTime.UtcNow.ToString("o"),
                    //["UpdatedAt"] = DateTime.UtcNow.ToString("o")
                }
            };

            try
            {
                await _client.UpsertAsync(
                    collectionName: CollectionName,
                    points: new[] { point }
                );
                _logger.LogInformation("Document Added: Successfully added document '{DocumentName}' to Qdrant (PointId: {PointId}, FileName: {FileName})", 
                    documentName, pointId, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upsert failed: Error adding document '{DocumentName}' to Qdrant.", documentName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateDocumentAsync(string documentName, string? newDocumentName, Stream? imageStream, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                throw new ArgumentException("DocumentName cannot be empty.", nameof(documentName));
            }

            var existingPoint = await FindByDocumentNameAsync(documentName);
            if (existingPoint == null)
            {
                _logger.LogWarning("Update failed: Document '{DocumentName}' not found.", documentName);
                throw new KeyNotFoundException($"Document '{documentName}' was not found.");
            }

            // If changing DocumentName, check if the new name already exists
            if (!string.IsNullOrWhiteSpace(newDocumentName) && !newDocumentName.Equals(documentName, StringComparison.OrdinalIgnoreCase))
            {
                if (await DocumentExistsAsync(newDocumentName))
                {
                    throw new InvalidOperationException($"Duplicate DocumentName error: A document named '{newDocumentName}' already exists.");
                }
            }

            float[] embedding;
            if (imageStream != null && imageStream.Length > 0)
            {
                _logger.LogInformation("Embedding Generated: Regenerating embedding for update of DocumentName: '{DocumentName}'", documentName);
                embedding = await _embeddingService.GenerateEmbeddingAsync(imageStream);
                _logger.LogInformation("Embedding Dimension: Generated embedding with dimension size: {Dimension}", embedding.Length);

                if (embedding.Length != 768)
                {
                    throw new InvalidOperationException("Embedding dimension must be 768.");
                }
            }
            else
            {
                // Reuse existing vector
                if (existingPoint.Vectors == null || existingPoint.Vectors.Vector == null)
                {
                    throw new InvalidOperationException("Existing vector not found in Qdrant point.");
                }
                embedding = existingPoint.Vectors.Vector.Data.ToArray();
            }

            var updatedPoint = new PointStruct
            {
                Id = existingPoint.Id,
                Vectors = embedding,
                Payload =
                {
                    //["DocumentName"] = newDocumentName ?? documentName,
                    ["FileName"] = fileName ?? existingPoint.Payload.GetValueOrDefault("FileName")?.StringValue ?? string.Empty,
                    //["UploadedAt"] = existingPoint.Payload.GetValueOrDefault("UploadedAt")?.StringValue ?? DateTime.UtcNow.ToString("o"),
                    //["UpdatedAt"] = DateTime.UtcNow.ToString("o")
                }
            };

            try
            {
                await _client.UpsertAsync(
                    collectionName: CollectionName,
                    points: new[] { updatedPoint }
                );
                _logger.LogInformation("Document Updated: Successfully updated document '{DocumentName}' (New Name: '{NewName}')", 
                    documentName, newDocumentName ?? documentName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update failed: Error updating document '{DocumentName}' in Qdrant.", documentName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteDocumentAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                throw new ArgumentException("DocumentName cannot be empty.", nameof(documentName));
            }

            var existingPoint = await FindByDocumentNameAsync(documentName);
            if (existingPoint == null)
            {
                _logger.LogWarning("Delete failed: Document '{DocumentName}' not found.", documentName);
                throw new KeyNotFoundException($"Document '{documentName}' was not found.");
            }

            try
            {
                await _client.DeleteAsync(
                    collectionName: CollectionName,
                    ids: new[] { existingPoint.Id }
                );
                _logger.LogInformation("Document Deleted: Successfully deleted document '{DocumentName}' (PointId: {PointId})", 
                    documentName, existingPoint.Id.Uuid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed: Error deleting document '{DocumentName}' from Qdrant.", documentName);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<DocumentResponse?> GetDocumentAsync(string documentName)
        {
            var point = await FindByDocumentNameAsync(documentName);
            if (point == null)
            {
                return null;
            }

            _logger.LogInformation("Document Retrieved: Metadata for '{DocumentName}' retrieved.", documentName);

            var uploadedAtStr = point.Payload.GetValueOrDefault("UploadedAt")?.StringValue;
            var updatedAtStr = point.Payload.GetValueOrDefault("UpdatedAt")?.StringValue;

            return new DocumentResponse
            {
                DocumentName = point.Payload.GetValueOrDefault("DocumentName")?.StringValue ?? documentName,
                FileName = point.Payload.GetValueOrDefault("FileName")?.StringValue ?? string.Empty,
                UploadedAt = DateTime.TryParse(uploadedAtStr, out var uploaded) ? uploaded : DateTime.MinValue,
                UpdatedAt = DateTime.TryParse(updatedAtStr, out var updated) ? updated : DateTime.MinValue
            };
        }

        /// <inheritdoc />
        public async Task<(List<DocumentListResponse> Documents, int TotalCount)> GetAllDocumentsAsync()
        {
            var documents = new List<DocumentListResponse>();
            PointId? nextOffset = null;

            try
            {
                do
                {
                    var response = await _client.ScrollAsync(
                        collectionName: CollectionName,
                        limit: 100,
                        offset: nextOffset,
                        payloadSelector: true,
                        vectorsSelector: false
                    );

                    documents.AddRange(
                        response.Result.Select(p => new DocumentListResponse
                        {
                            FileName = p.Payload.GetValueOrDefault("FileName")?.StringValue ?? string.Empty
                        }));

                    nextOffset = response.NextPageOffset;

                } while (nextOffset != null);

                int totalCount = documents.Count;

                _logger.LogInformation("Listed {Count} documents.", totalCount);

                return (documents, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scroll operation failed while listing all documents.");
                throw;
            }
        }
        /// <inheritdoc />
        public async Task<SearchImageResponse> SearchAsync(Stream imageStream)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                throw new ArgumentException("Image stream cannot be empty.", nameof(imageStream));
            }

            _logger.LogInformation("User Search Started: Generating embedding for search image.");
            float[] embedding = await _embeddingService.GenerateEmbeddingAsync(imageStream);
            _logger.LogInformation("Embedding Dimension: Generated search embedding with dimension size: {Dimension}", embedding.Length);

            if (embedding.Length != 768)
            {
                throw new InvalidOperationException("Embedding dimension must be 768.");
            }

            try
            {
                var searchResults = await _client.QueryAsync(
                    collectionName: CollectionName,
                    query: embedding,
                    limit: 2
                );

                var bestMatch = searchResults.FirstOrDefault();

                if (bestMatch == null)
                {
                    _logger.LogInformation("Search Completed: No match found.");
                    return new SearchImageResponse
                    {
                        Success = false,
                        Message = "Unknown document."
                    };
                }

                _logger.LogInformation("Search Completed: Best Match: '{FileName}', Similarity Score: {Score}", 
                    bestMatch.Payload.GetValueOrDefault("FileName")?.StringValue, bestMatch.Score);

                if (bestMatch.Score < 0.60f)
                {
                    return new SearchImageResponse
                    {
                        Success = false,
                        Message = "no document match yours."
                    };
                }

                return new SearchImageResponse
                {
                    Success = true,
                    FileName = bestMatch.Payload.GetValueOrDefault("FileName")?.StringValue ?? string.Empty,
                    SimilarityScore = bestMatch.Score
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errors: Image RAG Search failed.");
                throw;
            }
        }
    }
}

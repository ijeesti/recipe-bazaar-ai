using Azure.Search.Documents.Indexes.Models;

namespace RecipeBazaarAi.Infrastructure.Azure.Interfaces;

public interface IIndexService
{
    Task<bool> CreateOrUpdateIndexAsync();
    Task<bool> CreateOrUpdateSqlDataSourceAsync(string dataSourceName, string sqlConnectionString, string tableOrViewName);
    Task<bool> CreateOrUpdateIndexerAsync(string indexerName, string dataSourceName, string targetIndexName, string? scheduleCron = null);
    Task<bool> RunIndexerAsync(string indexerName);
    Task<SearchIndexerStatus> GetIndexerStatusAsync(string indexerName);
    Task<bool> DeleteIndexerAsync(string indexerName);
    Task<bool> DeleteDataSourceAsync(string dataSourceName);
    Task<bool> DeleteIndexAsync(string indexName);
}

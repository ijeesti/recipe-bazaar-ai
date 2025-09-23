using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Options;
using RecipeBazaarAi.Infrastructure.Azure.Indexes;
using RecipeBazaarAi.Infrastructure.Azure.Interfaces;

namespace RecipeBazaarAi.Infrastructure.Azure.Services;


public class IndexService(
    SearchIndexClient indexClient,
    SearchIndexerClient indexerClient,
    IOptions<AzureOptions> options) : IIndexService
{
    private readonly AzureOptions _options = options.Value;

    public async Task<bool> CreateOrUpdateIndexAsync()
    {
        var fieldBuilder = new FieldBuilder();
        var fields = fieldBuilder.Build(typeof(RecipeIndex));
        var index = new SearchIndex(_options.IndexName, fields);

        // Optional: set analyzers, scoring profiles etc on index.Analysis or index.ScoringProfiles
        await indexClient.CreateOrUpdateIndexAsync(index);

        return true; //TODO: proper api status
    }

    // 2) Create or update an Azure SQL data source connection
    // connectionString example: "Server=tcp:<server>.database.windows.net;Database=<db>;User ID=<user>;Password=<pw>;"
    public async Task<bool> CreateOrUpdateSqlDataSourceAsync(string dataSourceName, string sqlConnectionString, string tableOrViewName)
    {
        if (string.IsNullOrWhiteSpace(dataSourceName))
            throw new ArgumentException("dataSourceName required", nameof(dataSourceName));

        var connection = new SearchIndexerDataSourceConnection(
            name: dataSourceName,
            type: SearchIndexerDataSourceType.AzureSql,
            connectionString: sqlConnectionString,
            container: new SearchIndexerDataContainer(tableOrViewName)
        )
        {
            // Optional: description
            Description = $"Data source for table/view {tableOrViewName}"
        };

        // If exists, this call will update it
        await indexerClient.CreateOrUpdateDataSourceConnectionAsync(connection);
        return true; //TODO: proper api status
    }

    // 3) Create or update an indexer that pulls from the datasource into the index
    // schedule: optional cron expression or use default (on-demand)
    public async Task<bool> CreateOrUpdateIndexerAsync(string indexerName, string dataSourceName, string targetIndexName, string? scheduleCron = null)
    {
        var indexer = new SearchIndexer(
            name: indexerName,
            dataSourceName: dataSourceName,
            targetIndexName: targetIndexName
        );

        // Optional: field mappings if your SQL column names differ from index field names
        // indexer.FieldMappings.Add(new FieldMapping(sourceFieldName: "RecipeId", targetFieldName: "Id") { MappingFunction = ... });

        if (!string.IsNullOrWhiteSpace(scheduleCron))
        {
            // Example schedule: "every 5 minutes" -> "0 */5 * * * *"
            indexer.Schedule = new IndexingSchedule(TimeSpan.FromMinutes(5)); // simple timespan schedule
        }

        await indexerClient.CreateOrUpdateIndexerAsync(indexer);
        return true; //TODO: proper api status
    }

    // 4) Run indexer on demand (useful for testing or immediate sync)
    public async Task<bool> RunIndexerAsync(string indexerName)
    {
        await indexerClient.RunIndexerAsync(indexerName);
        return true; //TODO: proper api status
    }

    // 5) Optional helper: get status/result of last run
    public async Task<SearchIndexerStatus> GetIndexerStatusAsync(string indexerName)
    {
        var status = await indexerClient.GetIndexerStatusAsync(indexerName);
        return status.Value;
    }

    // 6) Optional: delete indexer/data source/index
    public async Task<bool> DeleteIndexerAsync(string indexerName)
    {
        await indexerClient.DeleteIndexerAsync(indexerName);
        return true; //TODO: proper api status
    }

    public async Task<bool> DeleteDataSourceAsync(string dataSourceName)
    {
        await indexerClient.DeleteDataSourceConnectionAsync(dataSourceName);
        return true;
    }

    public async Task<bool> DeleteIndexAsync(string indexName)
    {
        await indexClient.DeleteIndexAsync(indexName);
        return true;
    }
}


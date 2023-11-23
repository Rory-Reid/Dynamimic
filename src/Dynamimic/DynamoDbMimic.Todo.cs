using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Endpoint = Amazon.Runtime.Endpoints.Endpoint;

namespace Dynamimic;

/// <summary>
/// This particular "DynamoDbMimic.Todo" part contains all the stuff I haven't even begun to look at.
/// </summary>
public partial class DynamoDbMimic
{
    public IClientConfig Config { get; } = null!;
    public IDynamoDBv2PaginatorFactory Paginators { get; } = null!;

    public Task<BatchExecuteStatementResponse> BatchExecuteStatementAsync(BatchExecuteStatementRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchExecuteStatementResponse());
    }

    public Task<BatchGetItemResponse> BatchGetItemAsync(Dictionary<string, KeysAndAttributes> requestItems,
        ReturnConsumedCapacity returnConsumedCapacity,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchGetItemResponse());
    }

    public Task<BatchGetItemResponse> BatchGetItemAsync(Dictionary<string, KeysAndAttributes> requestItems, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchGetItemResponse());
    }

    public Task<BatchGetItemResponse> BatchGetItemAsync(BatchGetItemRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchGetItemResponse());
    }

    public Task<BatchWriteItemResponse> BatchWriteItemAsync(Dictionary<string, List<WriteRequest>> requestItems, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchWriteItemResponse());
    }

    public Task<BatchWriteItemResponse> BatchWriteItemAsync(BatchWriteItemRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new BatchWriteItemResponse());
    }

    public Task<CreateBackupResponse> CreateBackupAsync(CreateBackupRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CreateBackupResponse());
    }

    public Task<CreateGlobalTableResponse> CreateGlobalTableAsync(CreateGlobalTableRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CreateGlobalTableResponse());
    }

    public Task<DeleteBackupResponse> DeleteBackupAsync(DeleteBackupRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteBackupResponse());
    }

    public Task<DeleteItemResponse> DeleteItemAsync(string tableName, Dictionary<string, AttributeValue> key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteItemResponse());
    }

    public Task<DeleteItemResponse> DeleteItemAsync(string tableName, Dictionary<string, AttributeValue> key, ReturnValue returnValues,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteItemResponse());
    }

    public Task<DeleteItemResponse> DeleteItemAsync(DeleteItemRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteItemResponse());
    }

    public Task<DeleteTableResponse> DeleteTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteTableResponse());
    }

    public Task<DeleteTableResponse> DeleteTableAsync(DeleteTableRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DeleteTableResponse());
    }

    public Task<DescribeBackupResponse> DescribeBackupAsync(DescribeBackupRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeBackupResponse());
    }

    public Task<DescribeContinuousBackupsResponse> DescribeContinuousBackupsAsync(DescribeContinuousBackupsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeContinuousBackupsResponse());
    }

    public Task<DescribeContributorInsightsResponse> DescribeContributorInsightsAsync(DescribeContributorInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeContributorInsightsResponse());
    }

    public Task<DescribeEndpointsResponse> DescribeEndpointsAsync(DescribeEndpointsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeEndpointsResponse());
    }

    public Task<DescribeExportResponse> DescribeExportAsync(DescribeExportRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeExportResponse());
    }

    public Task<DescribeGlobalTableResponse> DescribeGlobalTableAsync(DescribeGlobalTableRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeGlobalTableResponse());
    }

    public Task<DescribeGlobalTableSettingsResponse> DescribeGlobalTableSettingsAsync(DescribeGlobalTableSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeGlobalTableSettingsResponse());
    }

    public Task<DescribeImportResponse> DescribeImportAsync(DescribeImportRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeImportResponse());
    }

    public Task<DescribeKinesisStreamingDestinationResponse> DescribeKinesisStreamingDestinationAsync(DescribeKinesisStreamingDestinationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeKinesisStreamingDestinationResponse());
    }

    public Task<DescribeLimitsResponse> DescribeLimitsAsync(DescribeLimitsRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeLimitsResponse());
    }

    public Task<DescribeTableReplicaAutoScalingResponse> DescribeTableReplicaAutoScalingAsync(DescribeTableReplicaAutoScalingRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeTableReplicaAutoScalingResponse());
    }

    public Task<DescribeTimeToLiveResponse> DescribeTimeToLiveAsync(string tableName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeTimeToLiveResponse());
    }

    public Task<DescribeTimeToLiveResponse> DescribeTimeToLiveAsync(DescribeTimeToLiveRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DescribeTimeToLiveResponse());
    }

    public Task<DisableKinesisStreamingDestinationResponse> DisableKinesisStreamingDestinationAsync(DisableKinesisStreamingDestinationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DisableKinesisStreamingDestinationResponse());
    }

    public Task<EnableKinesisStreamingDestinationResponse> EnableKinesisStreamingDestinationAsync(EnableKinesisStreamingDestinationRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new EnableKinesisStreamingDestinationResponse());
    }

    public Task<ExecuteStatementResponse> ExecuteStatementAsync(ExecuteStatementRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ExecuteStatementResponse());
    }

    public Task<ExecuteTransactionResponse> ExecuteTransactionAsync(ExecuteTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ExecuteTransactionResponse());
    }

    public Task<ExportTableToPointInTimeResponse> ExportTableToPointInTimeAsync(ExportTableToPointInTimeRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ExportTableToPointInTimeResponse());
    }

    public Task<ImportTableResponse> ImportTableAsync(ImportTableRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ImportTableResponse());
    }

    public Task<ListBackupsResponse> ListBackupsAsync(ListBackupsRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListBackupsResponse());
    }

    public Task<ListContributorInsightsResponse> ListContributorInsightsAsync(ListContributorInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListContributorInsightsResponse());
    }

    public Task<ListExportsResponse> ListExportsAsync(ListExportsRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListExportsResponse());
    }

    public Task<ListGlobalTablesResponse> ListGlobalTablesAsync(ListGlobalTablesRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListGlobalTablesResponse());
    }

    public Task<ListImportsResponse> ListImportsAsync(ListImportsRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListImportsResponse());
    }

    public Task<ListTablesResponse> ListTablesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTablesResponse());
    }

    public Task<ListTablesResponse> ListTablesAsync(string exclusiveStartTableName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTablesResponse());
    }

    public Task<ListTablesResponse> ListTablesAsync(string exclusiveStartTableName, int limit,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTablesResponse());
    }

    public Task<ListTablesResponse> ListTablesAsync(int limit, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTablesResponse());
    }

    public Task<ListTablesResponse> ListTablesAsync(ListTablesRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTablesResponse());
    }

    public Task<ListTagsOfResourceResponse> ListTagsOfResourceAsync(ListTagsOfResourceRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ListTagsOfResourceResponse());
    }

    public Task<QueryResponse> QueryAsync(QueryRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new QueryResponse());
    }

    public Task<RestoreTableFromBackupResponse> RestoreTableFromBackupAsync(RestoreTableFromBackupRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new RestoreTableFromBackupResponse());
    }

    public Task<RestoreTableToPointInTimeResponse> RestoreTableToPointInTimeAsync(RestoreTableToPointInTimeRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new RestoreTableToPointInTimeResponse());
    }

    public Task<ScanResponse> ScanAsync(string tableName, List<string> attributesToGet, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ScanResponse());
    }

    public Task<ScanResponse> ScanAsync(string tableName, Dictionary<string, Condition> scanFilter, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ScanResponse());
    }

    public Task<ScanResponse> ScanAsync(string tableName, List<string> attributesToGet, Dictionary<string, Condition> scanFilter,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ScanResponse());
    }

    public Task<ScanResponse> ScanAsync(ScanRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ScanResponse());
    }

    public Task<TagResourceResponse> TagResourceAsync(TagResourceRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TagResourceResponse());
    }

    public Task<TransactGetItemsResponse> TransactGetItemsAsync(TransactGetItemsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TransactGetItemsResponse());
    }

    public Task<TransactWriteItemsResponse> TransactWriteItemsAsync(TransactWriteItemsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new TransactWriteItemsResponse());
    }

    public Task<UntagResourceResponse> UntagResourceAsync(UntagResourceRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UntagResourceResponse());
    }

    public Task<UpdateContinuousBackupsResponse> UpdateContinuousBackupsAsync(UpdateContinuousBackupsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateContinuousBackupsResponse());
    }

    public Task<UpdateContributorInsightsResponse> UpdateContributorInsightsAsync(UpdateContributorInsightsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateContributorInsightsResponse());
    }

    public Task<UpdateGlobalTableResponse> UpdateGlobalTableAsync(UpdateGlobalTableRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateGlobalTableResponse());
    }

    public Task<UpdateGlobalTableSettingsResponse> UpdateGlobalTableSettingsAsync(UpdateGlobalTableSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateGlobalTableSettingsResponse());
    }

    public Task<UpdateItemResponse> UpdateItemAsync(string tableName, Dictionary<string, AttributeValue> key, Dictionary<string, AttributeValueUpdate> attributeUpdates,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateItemResponse());
    }

    public Task<UpdateItemResponse> UpdateItemAsync(string tableName, Dictionary<string, AttributeValue> key, Dictionary<string, AttributeValueUpdate> attributeUpdates, ReturnValue returnValues,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateItemResponse());
    }

    public Task<UpdateItemResponse> UpdateItemAsync(UpdateItemRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateItemResponse());
    }

    public Task<UpdateTableResponse> UpdateTableAsync(string tableName, ProvisionedThroughput provisionedThroughput,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateTableResponse());
    }

    public Task<UpdateTableResponse> UpdateTableAsync(UpdateTableRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateTableResponse());
    }

    public Task<UpdateTableReplicaAutoScalingResponse> UpdateTableReplicaAutoScalingAsync(UpdateTableReplicaAutoScalingRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateTableReplicaAutoScalingResponse());
    }

    public Task<UpdateTimeToLiveResponse> UpdateTimeToLiveAsync(UpdateTimeToLiveRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new UpdateTimeToLiveResponse());
    }

    public Endpoint DetermineServiceOperationEndpoint(AmazonWebServiceRequest request)
    {
        return new Endpoint("dynamomimic://not-a-real-endpoint");
    }
}
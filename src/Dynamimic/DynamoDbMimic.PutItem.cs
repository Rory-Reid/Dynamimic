using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Dynamimic;

public partial class DynamoDbMimic
{
    public Task<PutItemResponse> PutItemAsync(string tableName, Dictionary<string, AttributeValue> item,
        CancellationToken cancellationToken = default) =>
        this.PutItemAsync(new PutItemRequest
        {
            TableName = tableName,
            Item = item
        }, cancellationToken);

    public Task<PutItemResponse> PutItemAsync(string tableName, Dictionary<string, AttributeValue> item,
        ReturnValue returnValues, CancellationToken cancellationToken = default) =>
        this.PutItemAsync(new PutItemRequest
        {
            TableName = tableName,
            Item = item,
            ReturnValues = returnValues
        }, cancellationToken);

    public Task<PutItemResponse> PutItemAsync(PutItemRequest request, CancellationToken cancellationToken = default)
    {
        var table = this.GetTable(request.TableName);
        return Task.FromResult(table.PutItem(request));
    }
}
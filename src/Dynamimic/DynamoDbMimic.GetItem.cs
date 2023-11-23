using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Dynamimic;

public partial class DynamoDbMimic
{
    public Task<GetItemResponse> GetItemAsync(string tableName, Dictionary<string, AttributeValue> key,
        CancellationToken cancellationToken = default) =>
        this.GetItemAsync(new GetItemRequest
        {
            TableName = tableName,
            Key = key
        }, cancellationToken);

    public Task<GetItemResponse> GetItemAsync(string tableName, Dictionary<string, AttributeValue> key,
        bool consistentRead, CancellationToken cancellationToken = default) =>
        this.GetItemAsync(new GetItemRequest
        {
            TableName = tableName,
            Key = key,
            ConsistentRead = consistentRead
        }, cancellationToken);

    public Task<GetItemResponse> GetItemAsync(GetItemRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Key == null || !request.Key.Any())
        {
            throw GetItemException.CannotHaveNullKey;
        }

        // Convert legacy parameters
        if (string.IsNullOrEmpty(request.ProjectionExpression) && request.AttributesToGet.Any())
        {
            request.ProjectionExpression = string.Join(", ", request.AttributesToGet);
        }
        
        var table = this.GetTable(request.TableName);
        return Task.FromResult(table.GetItem(request));
    }
}

public static class GetItemException
{
    public static AmazonDynamoDBException NumberOfKeyConditionsInvalid =>
        Exception("The number of conditions on the keys is invalid");
    
    public static AmazonDynamoDBException CannotHaveNullKey =>
        Exception("Cannot have null key for GetItem, DeleteItem, or UpdateItem");  
    
    private static AmazonDynamoDBException Exception(string message)
    {
        return new AmazonDynamoDBException(message, ErrorType.Unknown, "ValidationException",
            Guid.NewGuid().ToString(), HttpStatusCode.BadRequest)
        {
            Source = nameof(Dynamimic),
            ErrorType = ErrorType.Unknown
        };
    }
}

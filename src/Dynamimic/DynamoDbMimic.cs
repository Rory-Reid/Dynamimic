using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Dynamimic;

public delegate DateTime UtcNow();
public delegate string GenerateRequestId();

public sealed partial class DynamoDbMimic : IAmazonDynamoDB
{
    private readonly UtcNow utcNow;
    private readonly GenerateRequestId generateRequestId;

    public DynamoDbMimic(UtcNow? utcNow = null, GenerateRequestId? generateRequestId = null)
    {
        this.utcNow = utcNow ?? (() => DateTime.UtcNow);
        this.generateRequestId = generateRequestId ?? (() => Guid.NewGuid().ToString());
    }
    
    public void Dispose()
    {
    }

    private Table GetTable(string tableName)
    {
        if (this.tables.TryGetValue(tableName, out var table))
        {
            return table;
        }

        throw DynamoException.CannotDoOperationsOnNonExistentTable;
    }
    
    private ResponseMetadata StandardMetadata(string? requestId = null) =>
        new()
        {
            ChecksumAlgorithm = CoreChecksumAlgorithm.NONE,
            ChecksumValidationStatus = ChecksumValidationStatus.NOT_VALIDATED,
            Metadata = { },
            RequestId = requestId ?? this.generateRequestId()
        };
}

public static class DynamoException
{
    public static AmazonDynamoDBException RequiredKeyNotGivenValue =>
        new("One of the required keys was not given a value", ErrorType.Unknown, "ValidationException",
            Guid.NewGuid().ToString(), HttpStatusCode.BadRequest)
        {
            Source = nameof(Dynamimic),
            ErrorType = ErrorType.Unknown
        };
    
    public static ResourceNotFoundException CannotDoOperationsOnNonExistentTable =>
        new("Cannot do operations on a non-existent table", ErrorType.Unknown, "ResourceNotFoundException",
            Guid.NewGuid().ToString(), HttpStatusCode.BadRequest)
        {
            Source = nameof(Dynamimic),
            ErrorType = ErrorType.Unknown
        };
}
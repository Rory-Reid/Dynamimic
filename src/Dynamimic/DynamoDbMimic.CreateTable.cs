using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Dynamimic.Database;

namespace Dynamimic;

public partial class DynamoDbMimic
{
    private readonly Dictionary<string, Table> tables = new();
    
    public Task<CreateTableResponse> CreateTableAsync(string tableName, List<KeySchemaElement> keySchema,
        List<AttributeDefinition> attributeDefinitions, ProvisionedThroughput provisionedThroughput,
        CancellationToken cancellationToken = default) =>
        this.CreateTableAsync(new CreateTableRequest
        {
            TableName = tableName,
            KeySchema = keySchema,
            AttributeDefinitions = attributeDefinitions,
            ProvisionedThroughput = provisionedThroughput
        }, cancellationToken);

    public Task<CreateTableResponse> CreateTableAsync(CreateTableRequest request,
        CancellationToken cancellationToken = default)
    {
        // TODO error scenarios
        // TODO flesh out response
        var table = new Table(request, this.utcNow());
        this.tables.Add(table.Name, table);
        var description = table.Describe();
        description.TableStatus = TableStatus.CREATING;
        return Task.FromResult(new CreateTableResponse
        {
            TableDescription = description,
            ContentLength = 500,
            HttpStatusCode = HttpStatusCode.OK,
            ResponseMetadata = this.StandardMetadata()
        });
    }
}
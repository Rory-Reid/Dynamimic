using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Shouldly;

namespace Dynamimic.Tests.DocumentModel;

public class SavingAndLoadingTests : IAsyncLifetime
{
    private readonly DynamoDbMimic dynamo = new();
    private readonly string tableName = Guid.NewGuid().ToString();
    
    public async Task InitializeAsync()
    {
        await this.dynamo.CreateTableAsync(new CreateTableRequest
        {
            TableName = this.tableName,
            KeySchema = new List<KeySchemaElement>
            {
                new("pk", KeyType.HASH),
                new("sk", KeyType.RANGE)
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new("pk", ScalarAttributeType.S),
                new("sk", ScalarAttributeType.S)
            },
            BillingMode = BillingMode.PAY_PER_REQUEST
        });
    }

    public Task DisposeAsync() => Task.CompletedTask;
    
    [Fact(Skip = "Document model has problems")]
    public async Task Can_set_and_get_item()
    {
        var table = Amazon.DynamoDBv2.DocumentModel.Table.LoadTable(this.dynamo, this.tableName);

        var testData = new Document
        {
            ["pk"] = "partition_key_value",
            ["sk"] = "sort_key_value",
            ["attribute_1"] = "string attribute"
        };

        await table.UpdateItemAsync(testData, new UpdateItemOperationConfig(), CancellationToken.None);

        var storedItem = await table.GetItemAsync("partition_key_value", "sort_key_value",
            new GetItemOperationConfig { AttributesToGet = new List<string> { "pk", "attribute_1" } });

        storedItem.ShouldNotBeNull().ShouldSatisfyAllConditions(
            doc => doc["pk"].AsString().ShouldBe("partition_key_value"),
            doc => doc["attribute_1"].AsString().ShouldBe("string attribute"),
            doc => doc.ContainsKey("sk").ShouldBeFalse());
    }
}
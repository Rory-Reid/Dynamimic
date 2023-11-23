using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Shouldly;

namespace Dynamimic.Tests;

public class PutItemTests : IAsyncLifetime
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
    
    [Fact]
    public async Task Can_put_item()
    {
        await this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = this.tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });
        
        (await this.dynamo.DescribeTableAsync(this.tableName)).Table.ItemCount.ShouldBe(1);
    }

    [Fact]
    public async Task Cannot_put_item_without_partition_key()
    {
        var putItem = () => this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = this.tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["sk"] = new("sort_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });
        
        var exception = await putItem.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("One of the required keys was not given a value"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Cannot_put_item_without_sort_key()
    {
        var putItem = () => this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = this.tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });
        
        var exception = await putItem.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("One of the required keys was not given a value"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Can_put_item_with_simple_primary_key()
    {
        await this.dynamo.CreateTableAsync(new CreateTableRequest
        {
            TableName = "simple_key_table",
            KeySchema = new List<KeySchemaElement>
            {
                new("pk", KeyType.HASH)
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new("pk", ScalarAttributeType.S)
            },
            BillingMode = BillingMode.PAY_PER_REQUEST
        });
        
        await this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = "simple_key_table",
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });

        (await this.dynamo.DescribeTableAsync("simple_key_table")).Table.ItemCount.ShouldBe(1);
    }

    [Fact]
    public async Task Cannot_put_item_missing_simple_primary_key()
    {
        await this.dynamo.CreateTableAsync(new CreateTableRequest
        {
            TableName = "simple_key_table",
            KeySchema = new List<KeySchemaElement>
            {
                new("pk", KeyType.HASH)
            },
            AttributeDefinitions = new List<AttributeDefinition>
            {
                new("pk", ScalarAttributeType.S)
            },
            BillingMode = BillingMode.PAY_PER_REQUEST
        });
        
        var putItem = () => this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = "simple_key_table",
            Item = new Dictionary<string, AttributeValue>
            {
                ["attribute_1"] = new("string attribute")
            }
        });
        
        var exception = await putItem.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("One of the required keys was not given a value"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
    
    [Fact]
    public async Task Putting_same_item_will_overwrite_and_not_add()
    {
        await this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = this.tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });
        
        await this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = this.tableName,
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value"),
                ["attribute_1"] = new("updated attribute")
            }
        });
        
        (await this.dynamo.DescribeTableAsync(this.tableName)).Table.ItemCount.ShouldBe(1);
    }

    [Fact]
    public async Task Cannot_put_item_on_nonexistent_table()
    {
        var putItem = () => this.dynamo.PutItemAsync(new PutItemRequest
        {
            TableName = "nonexistent_table",
            Item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value"),
                ["attribute_1"] = new("string attribute")
            }
        });

        var exception = await putItem.ShouldThrowAsync<ResourceNotFoundException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot do operations on a non-existent table"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ResourceNotFoundException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
}
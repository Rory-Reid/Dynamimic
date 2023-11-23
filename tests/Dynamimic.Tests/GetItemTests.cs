using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Shouldly;

namespace Dynamimic.Tests;

public class GetItemTests : IAsyncLifetime
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
    public async Task Can_get_put_item()
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

        var getItemResponse = await this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        getItemResponse.ShouldSatisfyAllConditions(
            () => getItemResponse.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => getItemResponse.IsItemSet.ShouldBeTrue(),
            () => getItemResponse.Item.ShouldSatisfyAllConditions(
                item => item["pk"].S.ShouldBe("partition_key_value"),
                item => item["sk"].S.ShouldBe("sort_key_value"),
                item => item["attribute_1"].S.ShouldBe("string attribute")));
    }

    [Fact]
    public async Task Cannot_get_item_when_missing_partition_key()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["sk"] = new("sort_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("The number of conditions on the keys is invalid"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
    
    [Fact]
    public async Task Cannot_get_item_when_missing_sort_key()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("The number of conditions on the keys is invalid"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Cannot_get_item_when_keys_empty()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue> { },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot have null key for GetItem, DeleteItem, or UpdateItem"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
    
    [Fact]
    public async Task Cannot_get_item_when_keys_null()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = null,
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot have null key for GetItem, DeleteItem, or UpdateItem"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Cannot_get_item_when_too_many_keys()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value"),
                ["additional_key"] = new("additional_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("The number of conditions on the keys is invalid"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Theory]
    [InlineData("pk", "wrong_sk")]
    [InlineData("wrong_pk", "sk")]
    [InlineData("wrong_pk", "wrong_sk")]
    public async Task Cannot_get_item_when_keys_are_wrong(string pkName, string skName)
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                [pkName] = new("partition_key_value"),
                [skName] = new("sort_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("One of the required keys was not given a value"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Get_nonexistent_item_returns_itemless_response()
    {
        var getItemResponse = await this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = this.tableName,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });

        getItemResponse.ShouldSatisfyAllConditions(
            () => getItemResponse.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => getItemResponse.IsItemSet.ShouldBeFalse(),
            () => getItemResponse.Item.ShouldBeEmpty());
    }

    [Fact]
    public async Task Can_get_put_item_for_simple_key()
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

        var getItemResponse = await this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value")
            },
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        getItemResponse.ShouldSatisfyAllConditions(
            () => getItemResponse.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => getItemResponse.IsItemSet.ShouldBeTrue(),
            () => getItemResponse.Item.ShouldSatisfyAllConditions(
                item => item["pk"].S.ShouldBe("partition_key_value"),
                item => item["attribute_1"].S.ShouldBe("string attribute")));
    }
    
    [Fact]
    public async Task Cannot_get_put_item_for_simple_key_when_keys_empty()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = new Dictionary<string, AttributeValue> { },
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot have null key for GetItem, DeleteItem, or UpdateItem"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Cannot_get_put_item_for_simple_key_when_keys_null()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = null,
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot have null key for GetItem, DeleteItem, or UpdateItem"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
    
    [Fact]
    public async Task Cannot_get_put_item_for_simple_key_when_too_many_keys()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["additional_key"] = new("additional_key_value")
            },
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("The number of conditions on the keys is invalid"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
    
    [Fact]
    public async Task Cannot_get_put_item_for_simple_key_when_keys_are_wrong()
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

        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = new Dictionary<string, AttributeValue>
            {
                ["wrong_pk"] = new("partition_key_value")
            },
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        var exception = await getItemRequest.ShouldThrowAsync<AmazonDynamoDBException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("One of the required keys was not given a value"),
            () => exception.ErrorCode.ShouldBe("ValidationException"),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Get_nonexistent_item_returns_itemless_response_in_simple_key_table()
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
        
        var getItemResponse = await this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "simple_key_table",
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value")
            },
            AttributesToGet = new List<string> {"pk", "attribute_1"}
        });

        getItemResponse.ShouldSatisfyAllConditions(
            () => getItemResponse.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => getItemResponse.IsItemSet.ShouldBeFalse(),
            () => getItemResponse.Item.ShouldBeEmpty());
    }

    [Fact]
    public async Task Cannot_get_item_for_nonexistent_table()
    {
        var getItemRequest = () => this.dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = "nonexistent_table",
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new("partition_key_value"),
                ["sk"] = new("sort_key_value")
            },
            AttributesToGet = new List<string> {"pk", "sk", "attribute_1"}
        });
        
        var exception = await getItemRequest.ShouldThrowAsync<ResourceNotFoundException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot do operations on a non-existent table"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ResourceNotFoundException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }
}
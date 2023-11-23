using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Dynamimic.Tests.sdk;
using Shouldly;

namespace Dynamimic.Tests;

public class DescribeTableTests
{
    private readonly DynamoDbMimic dynamo = new();
    
    [Fact]
    public async Task Describing_non_existent_table_throws_exception()
    {
        var describeNonExistentTable = () => this.dynamo.DescribeTableAsync("does-not-exist");
        var exception = await describeNonExistentTable.ShouldThrowAsync<ResourceNotFoundException>();
        exception.ShouldSatisfyAllConditions(
            () => exception.Message.ShouldBe("Cannot do operations on a non-existent table"),
            () => exception.ErrorType.ShouldBe(ErrorType.Unknown),
            () => exception.ErrorCode.ShouldBe("ResourceNotFoundException"),
            () => exception.RequestId.ShouldNotBeNullOrEmpty(),
            () => exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest),
            () => exception.Source.ShouldBe(nameof(Dynamimic)));
    }

    [Fact]
    public async Task Created_table_can_be_described()
    {
        await this.dynamo.CreateTableAsync(new CreateTableRequest
        {
            TableName = "HappyPath",
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

        var description = await this.dynamo.DescribeTableAsync("HappyPath");
        description.ShouldSatisfyAllConditions(
            () => description.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => description.Table.ShouldSatisfyAllConditions(
                table => table.TableName.ShouldBe("HappyPath"),
                table => table.KeySchema.ShouldContainEquivalent(new KeySchemaElement("pk", KeyType.HASH)),
                table => table.KeySchema.ShouldContainEquivalent(new KeySchemaElement("sk", KeyType.RANGE)),
                table => table.AttributeDefinitions.ShouldContainEquivalent(new AttributeDefinition("pk", ScalarAttributeType.S)),
                table => table.AttributeDefinitions.ShouldContainEquivalent(new AttributeDefinition("sk", ScalarAttributeType.S)),
                table => table.BillingModeSummary.BillingMode.ShouldBe(BillingMode.PAY_PER_REQUEST),
                table => table.ItemCount.ShouldBe(0)));
    }
}
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamimic.Tests.sdk;
using Shouldly;

namespace Dynamimic.Tests;

public class CreateTableTests
{
    [Fact]
    public async Task Can_create_simple_provisioned_table()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var pkSchema = new KeySchemaElement("pk", KeyType.HASH);
        var skSchema = new KeySchemaElement("sk", KeyType.RANGE);
        var pkAttribute = new AttributeDefinition("pk", ScalarAttributeType.S);
        var skAttribute = new AttributeDefinition("sk", ScalarAttributeType.S);
        var request = new CreateTableRequest
        {
            TableName = "ProvisionedTable",
            KeySchema = new List<KeySchemaElement> {pkSchema, skSchema},
            AttributeDefinitions = new List<AttributeDefinition> {pkAttribute, skAttribute},
            BillingMode = BillingMode.PROVISIONED,
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 10,
                WriteCapacityUnits = 5,
            },
            TableClass = TableClass.STANDARD
        };

        var response = await dynamo.CreateTableAsync(request);

        response.ShouldSatisfyAllConditions(
            () => response.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => response.TableDescription.ShouldSatisfyAllConditions(
                description => description.TableName.ShouldBe(request.TableName),
                description => description.ProvisionedThroughput.ShouldSatisfyAllConditions(
                    throughput =>
                        throughput.ReadCapacityUnits.ShouldBe(request.ProvisionedThroughput.ReadCapacityUnits),
                    throughput =>
                        throughput.WriteCapacityUnits.ShouldBe(request.ProvisionedThroughput.WriteCapacityUnits),
                    throughput => throughput.LastDecreaseDateTime.ShouldBe(DateTime.UnixEpoch),
                    throughput => throughput.LastIncreaseDateTime.ShouldBe(DateTime.UnixEpoch),
                    throughput => throughput.NumberOfDecreasesToday.ShouldBe(0)),
                description => description.TableStatus.ShouldBe(TableStatus.CREATING),
                description => description.KeySchema.ShouldContainEquivalent(pkSchema),
                description => description.KeySchema.ShouldContainEquivalent(skSchema),
                description => description.AttributeDefinitions.ShouldContainEquivalent(pkAttribute),
                description => description.AttributeDefinitions.ShouldContainEquivalent(skAttribute),
                description => description.CreationDateTime.ShouldBe(now),
                description =>
                    description.TableArn.ShouldBe($"arn:aws:dynamodb:dynamimic:000000000000:table/{request.TableName}"),
                description => description.TableClassSummary.ShouldSatisfyAllConditions(
                    tableClass => tableClass.TableClass.ShouldBe(request.TableClass),
                    tableClass => tableClass.LastUpdateDateTime.ShouldBe(DateTime.UnixEpoch)),
                description => description.BillingModeSummary.ShouldSatisfyAllConditions(
                    billingMode => billingMode.BillingMode.ShouldBe(request.BillingMode),
                    billingMode => billingMode.LastUpdateToPayPerRequestDateTime.ShouldBe(DateTime.UnixEpoch))));
    }

    [Fact]
    public async Task Can_create_simple_on_demand_table()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var pkSchema = new KeySchemaElement("pk", KeyType.HASH);
        var skSchema = new KeySchemaElement("sk", KeyType.RANGE);
        var pkAttribute = new AttributeDefinition("pk", ScalarAttributeType.S);
        var skAttribute = new AttributeDefinition("sk", ScalarAttributeType.S);
        var request = new CreateTableRequest
        {
            TableName = "OnDemandTable",
            KeySchema = new List<KeySchemaElement> {pkSchema, skSchema},
            AttributeDefinitions = new List<AttributeDefinition> {pkAttribute, skAttribute},
            BillingMode = BillingMode.PAY_PER_REQUEST,
            TableClass = TableClass.STANDARD
        };

        var response = await dynamo.CreateTableAsync(request);

        response.ShouldSatisfyAllConditions(
            () => response.HttpStatusCode.ShouldBe(HttpStatusCode.OK),
            () => response.TableDescription.ShouldSatisfyAllConditions(
                description => description.TableName.ShouldBe(request.TableName),
                description => description.ProvisionedThroughput.ShouldSatisfyAllConditions(
                    throughput => throughput.ReadCapacityUnits.ShouldBe(0),
                    throughput => throughput.WriteCapacityUnits.ShouldBe(0),
                    throughput => throughput.LastDecreaseDateTime.ShouldBe(DateTime.UnixEpoch),
                    throughput => throughput.LastIncreaseDateTime.ShouldBe(DateTime.UnixEpoch),
                    throughput => throughput.NumberOfDecreasesToday.ShouldBe(0)),
                description => description.TableStatus.ShouldBe(TableStatus.CREATING),
                description => description.KeySchema.ShouldContainEquivalent(pkSchema),
                description => description.KeySchema.ShouldContainEquivalent(skSchema),
                description => description.AttributeDefinitions.ShouldContainEquivalent(pkAttribute),
                description => description.AttributeDefinitions.ShouldContainEquivalent(skAttribute),
                description => description.CreationDateTime.ShouldBe(now),
                description =>
                    description.TableArn.ShouldBe($"arn:aws:dynamodb:dynamimic:000000000000:table/{request.TableName}"),
                description => description.TableClassSummary.ShouldSatisfyAllConditions(
                    tableClass => tableClass.TableClass.ShouldBe(request.TableClass),
                    tableClass => tableClass.LastUpdateDateTime.ShouldBe(DateTime.UnixEpoch)),
                description => description.BillingModeSummary.ShouldSatisfyAllConditions(
                    billingMode => billingMode.BillingMode.ShouldBe(request.BillingMode),
                    billingMode => billingMode.LastUpdateToPayPerRequestDateTime.ShouldBe(DateTime.UnixEpoch))));
    }

    public static IEnumerable<object[]> AllStreamViewTypes() =>
        new List<object[]>
        {
            new object[] {StreamViewType.OLD_IMAGE},
            new object[] {StreamViewType.NEW_IMAGE},
            new object[] {StreamViewType.NEW_AND_OLD_IMAGES},
            new object[] {StreamViewType.KEYS_ONLY}
        };

    [Theory, MemberData(nameof(AllStreamViewTypes))]
    public async Task Can_create_table_with_streaming(StreamViewType type)
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();
        request.StreamSpecification = new StreamSpecification
        {
            StreamEnabled = true,
            StreamViewType = type
        };

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.ShouldSatisfyAllConditions(
            description => description.StreamSpecification.StreamEnabled.ShouldBeTrue(),
            description => description.StreamSpecification.StreamViewType.ShouldBe(type));
    }

    [Fact]
    public async Task Created_table_without_streaming_has_defaults()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.ShouldSatisfyAllConditions(
            description => description.StreamSpecification.StreamEnabled.ShouldBeFalse(),
            description => description.StreamSpecification.StreamViewType.ShouldBeNull()); // TODO is this right?
    }
    
    [Fact]
    public async Task Can_create_table_with_sse()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();
        request.SSESpecification = new SSESpecification
        {
            Enabled = true,
            SSEType = SSEType.KMS
        };

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.ShouldSatisfyAllConditions(
            description => description.SSEDescription.Status.ShouldBe(SSEStatus.ENABLED),
            description => description.SSEDescription.SSEType.ShouldBe(SSEType.KMS));
    }
    
    [Fact]
    public async Task Created_table_without_sse_has_defaults()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.ShouldSatisfyAllConditions(
            description => description.SSEDescription.Status.ShouldBe(SSEStatus.DISABLED),
            description => description.SSEDescription.SSEType.ShouldBeNull()); // TODO is this right?
    }
    
    [Fact]
    public async Task Create_table_with_no_gsi_has_empty_list()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.GlobalSecondaryIndexes.ShouldBeEmpty();
    }
    
    [Fact]
    public async Task Create_table_with_no_lsi_has_empty_list()
    {
        var now = DateTime.UtcNow;
        var dynamo = new DynamoDbMimic(() => now);
        var request = this.CreateSimpleRequest();

        var response = await dynamo.CreateTableAsync(request);
        
        response.TableDescription.LocalSecondaryIndexes.ShouldBeEmpty();
    }

    private CreateTableRequest CreateSimpleRequest() =>
        new()
        {
            TableName = Guid.NewGuid().ToString(),
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
        };
}
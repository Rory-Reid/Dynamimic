using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Dynamimic;

public class Table
{
    private readonly CreateTableRequest creationRequest;
    private readonly TableKey partitionKey;
    private readonly TableKey? sortKey;
    private readonly Dictionary<string, Partition> partitions = new();

    [SuppressMessage("Property Values", "DynamoDB1003:Property value less than minimum value")]
    public Table(CreateTableRequest spec, DateTime creationTime)
    {
        this.creationRequest = spec;

        this.Name = spec.TableName;
        this.KeySchema = spec.KeySchema;
        this.AttributeDefinitions = spec.AttributeDefinitions;
        this.BillingMode = spec.BillingMode;
        this.CreationTime = creationTime;
        this.LocalSecondaryIndexes = spec.LocalSecondaryIndexes ?? new List<LocalSecondaryIndex>();
        this.GlobalSecondaryIndexes = spec.GlobalSecondaryIndexes ?? new List<GlobalSecondaryIndex>();

        this.ProvisionedThroughput = new ProvisionedThroughputDescription
        {
            WriteCapacityUnits = spec.ProvisionedThroughput?.WriteCapacityUnits ?? 0,
            ReadCapacityUnits = spec.ProvisionedThroughput?.ReadCapacityUnits ?? 0,
            LastDecreaseDateTime = DateTime.UnixEpoch,
            LastIncreaseDateTime = DateTime.UnixEpoch,
            NumberOfDecreasesToday = 0
        };
        this.TableClass = new TableClassSummary
        {
            TableClass = spec.TableClass,
            LastUpdateDateTime = DateTime.UnixEpoch
        };
        this.StreamSpecification = new StreamSpecification
        {
            StreamEnabled = spec.StreamSpecification?.StreamEnabled ?? false,
            StreamViewType = spec.StreamSpecification?.StreamViewType
        };
        this.SseDescription = new SSEDescription
        {
            Status = spec.SSESpecification?.Enabled ?? false
                ? SSEStatus.ENABLED
                : SSEStatus.DISABLED,
            SSEType = spec.SSESpecification?.SSEType,
            KMSMasterKeyArn = spec.SSESpecification?.KMSMasterKeyId == null
                ? null
                : $"arn:aws:kms:000000000000:key/{spec.SSESpecification.KMSMasterKeyId}",
            InaccessibleEncryptionDateTime = DateTime.UnixEpoch
        };

        // TODO throw typical errors if spec is invalid
        var pk = this.KeySchema.Single(x => x.KeyType == KeyType.HASH);
        var pkDefinition = this.AttributeDefinitions.Single(x => x.AttributeName == pk.AttributeName);
        this.partitionKey = new TableKey(pk.AttributeName, pkDefinition.AttributeType);

        var sk = this.KeySchema.SingleOrDefault(x => x.KeyType == KeyType.RANGE);
        if (sk != null)
        {
            var skDefinition = this.AttributeDefinitions.Single(x => x.AttributeName == sk.AttributeName);
            this.sortKey = new TableKey(sk.AttributeName, skDefinition.AttributeType);
        }
    }

    public string Name { get; }
    private string Arn => $"arn:aws:dynamodb:dynamimic:000000000000:table/{this.Name}";
    private TableStatus Status { get; } = TableStatus.ACTIVE;
    private List<KeySchemaElement> KeySchema { get; }
    private List<AttributeDefinition> AttributeDefinitions { get; }
    private BillingMode BillingMode { get; }
    private DateTime CreationTime { get; }
    private List<LocalSecondaryIndex> LocalSecondaryIndexes { get; }
    private List<GlobalSecondaryIndex> GlobalSecondaryIndexes { get; }

    private ProvisionedThroughputDescription ProvisionedThroughput { get; }
    private TableClassSummary TableClass { get; }
    private StreamSpecification StreamSpecification { get; }
    private SSEDescription SseDescription { get; }
    private ArchivalSummary ArchivalSummary { get; } = null!;
    private DateTime LastUpdateToPayPerRequestDateTime { get; } = DateTime.UnixEpoch;

    public TableDescription Describe() =>
        new()
        {
            ArchivalSummary = this.ArchivalSummary,
            AttributeDefinitions = this.AttributeDefinitions,
            KeySchema = this.KeySchema,
            ProvisionedThroughput = this.ProvisionedThroughput,
            TableName = this.Name,
            Replicas = new List<ReplicaDescription>(),
            ItemCount = this.partitions.Values.Sum(p => p.ItemCount),
            RestoreSummary = null,
            StreamSpecification = this.StreamSpecification,
            TableArn = this.Arn,
            TableId = this.Name,
            TableStatus = this.Status,
            BillingModeSummary = new BillingModeSummary
            {
                BillingMode = this.BillingMode,
                LastUpdateToPayPerRequestDateTime = this.LastUpdateToPayPerRequestDateTime
            },
            CreationDateTime = this.CreationTime,
            GlobalSecondaryIndexes = this.GlobalSecondaryIndexes
                .Select(gsi =>
                    new GlobalSecondaryIndexDescription
                    {
                        KeySchema = gsi.KeySchema,
                        ProvisionedThroughput = new ProvisionedThroughputDescription
                        {
                            ReadCapacityUnits = gsi.ProvisionedThroughput.ReadCapacityUnits,
                            WriteCapacityUnits = gsi.ProvisionedThroughput.WriteCapacityUnits
                        },
                        ItemCount = 0, // TODO
                        Backfilling = false,
                        Projection = gsi.Projection,
                        IndexArn = $"arn:{this.Name}:gsi:{gsi.IndexName}", // TODO
                        IndexName = gsi.IndexName,
                        IndexStatus = IndexStatus.ACTIVE, // TODO
                        IndexSizeBytes = 1024
                    })
                .ToList(),
            GlobalTableVersion = null,
            LatestStreamArn = null,
            LatestStreamLabel = null,
            LocalSecondaryIndexes = this.LocalSecondaryIndexes
                .Select(lsi =>
                    new LocalSecondaryIndexDescription
                    {
                        KeySchema = lsi.KeySchema,
                        ItemCount = 0, // TODO
                        Projection = lsi.Projection,
                        IndexArn = $"arn:{this.Name}:lsi:{lsi.IndexName}", // TODO
                        IndexName = lsi.IndexName,
                        IndexSizeBytes = 1024
                    })
                .ToList(),
            TableClassSummary = this.TableClass,
            SSEDescription = this.SseDescription,
            TableSizeBytes = 1024,
        };

    public PutItemResponse PutItem(PutItemRequest request)
    {
        var partition = this.GetOrCreatePartition(request.Item);
        partition.PutItem(request.Item);

        return new PutItemResponse
        {
            ConsumedCapacity = new ConsumedCapacity {WriteCapacityUnits = 0.5}, // 🤷
            HttpStatusCode = HttpStatusCode.OK,
            ContentLength = 2,
            Attributes = new Dictionary<string, AttributeValue>(),
            ItemCollectionMetrics = new ItemCollectionMetrics()
        };
    }

    public GetItemResponse GetItem(GetItemRequest request)
    {
        var expectedKeys = this.sortKey == null ? 1 : 2;
        if (request.Key.Count != expectedKeys)
        {
            throw GetItemException.NumberOfKeyConditionsInvalid;
        }
        
        var partition = this.GetOrCreatePartition(request.Key);
        return partition.GetItem(request);
    }

    private Partition GetOrCreatePartition(Dictionary<string, AttributeValue> attributes)
    {
        if (attributes.TryGetValue(this.partitionKey.Name, out var pk) &&
            (this.sortKey == null || attributes.ContainsKey(this.sortKey.Name)))
        {
            var pkValue = this.partitionKey.GetKeyValue(pk);
            if (this.partitions.TryGetValue(pkValue, out var existingPartition))
            {
                return existingPartition;
            }

            var partition = new Partition(pkValue, this.sortKey);
            this.partitions.Add(this.partitionKey.GetKeyValue(pk), partition);
            return partition;
        }

        throw DynamoException.RequiredKeyNotGivenValue;
    }
}
using System.Net;
using Amazon.DynamoDBv2.Model;

namespace Dynamimic;

public class Partition
{
    private readonly string keyValue;
    private readonly TableKey? sortKey;
    private Dictionary<string, TableItem> items = new();
    public Partition(string keyValue, TableKey? sortKey = null)
    {
        this.keyValue = keyValue;
        this.sortKey = sortKey;
    }
    
    public void PutItem(Dictionary<string, AttributeValue> item)
    {
        if (this.sortKey == null)
        {
            this.items = new Dictionary<string, TableItem> {[""] = new(item)};
        }
        else
        {
            this.items[this.sortKey.GetKeyValue(item[this.sortKey.Name])] = new(item);
        }
    }

    public GetItemResponse GetItem(GetItemRequest request)
    {
        TableItem? item;
        if (this.sortKey == null)
        {
            item = this.items.Values.SingleOrDefault();
        }
        else
        {
            this.items.TryGetValue(this.sortKey.GetKeyValue(request.Key[this.sortKey.Name]), out item);
        }
        
        return new GetItemResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            Item = ProjectAttributes(item, request.ProjectionExpression),
            ConsumedCapacity = new ConsumedCapacity {ReadCapacityUnits = 0.5}, // 🤷
            ContentLength = 2,
        };

        Dictionary<string, AttributeValue> ProjectAttributes(TableItem? itemToProject, string projectionExpression)
        {
            // TODO: Arrays, map expressions
            var projection = new Dictionary<string, AttributeValue>();
            var attributes = projectionExpression.Split(",").Select(s => s.Trim());
            foreach (var attribute in attributes)
            {
                if (itemToProject?.Attributes.TryGetValue(attribute, out var value) == true)
                {
                    projection[attribute] = Copy(value);
                }
            }

            return projection;
        }
    }

    public int ItemCount => this.items.Count;
    
    private static AttributeValue Copy(AttributeValue from)
    {
        return from switch
        {
            // TODO proper copy of all things
            {S.Length: > 0} => new AttributeValue {S = from.S}, // Good
            {N.Length: > 0} => new AttributeValue {N = from.N}, // Good?
            {IsLSet: true} => new AttributeValue {L = from.L}, // Bad
            {IsMSet: true} => new AttributeValue {M = from.M}, // Bad
            {IsBOOLSet: true} => new AttributeValue {BOOL = from.BOOL}, // Good?
            {B: {Length: > 0}} => new AttributeValue {B = from.B}, // Bad
            {SS: {Count: > 0}} => new AttributeValue {SS = from.SS}, // Bad
            {NS: {Count: > 0}} => new AttributeValue {NS = from.NS}, // Bad
            {BS: {Count: > 0}} => new AttributeValue {BS = from.BS}, // Bad
            {NULL: true} => new AttributeValue {NULL = true}, // ???
            _ => throw new InvalidOperationException()
        };
    }
}
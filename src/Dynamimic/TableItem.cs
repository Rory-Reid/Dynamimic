using Amazon.DynamoDBv2.Model;

namespace Dynamimic;

public class TableItem
{
    public TableItem(Dictionary<string, AttributeValue> item)
    {
        this.Attributes = item.Select(kvp => new KeyValuePair<string, AttributeValue>(kvp.Key, Copy(kvp.Value)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public Dictionary<string, AttributeValue> Attributes { get; }

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
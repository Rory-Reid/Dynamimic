using Amazon.DynamoDBv2.Model;

namespace Dynamimic;

public static class AttributeValueExtensions
{
    public static AttributeValue Copy(this AttributeValue attribute)
    {
        return attribute switch
        {
            {S.Length: > 0} => new AttributeValue {S = attribute.S},
            {N.Length: > 0} => new AttributeValue {N = attribute.N},
            {IsBOOLSet: true} => new AttributeValue {BOOL = attribute.BOOL},
            {B.Length: > 0} => new AttributeValue {B = attribute.B.Copy()},
            {NULL: true} => new AttributeValue {NULL = true},
            
            {IsLSet: true} => new AttributeValue {L = attribute.L.Copy()},
            {IsMSet: true} => new AttributeValue {M = attribute.M.Copy()},
            
            {SS.Count: > 0} => new AttributeValue {SS = attribute.SS.Select(s => s).ToList()},
            {NS.Count: > 0} => new AttributeValue {NS = attribute.NS.Select(n => n).ToList()},
            {BS.Count: > 0} => new AttributeValue {BS = attribute.BS.Select(Copy).ToList()},
            _ => throw new InvalidOperationException()
        };
    }

    private static List<AttributeValue> Copy(this List<AttributeValue> list) => list.Select(Copy).ToList();

    public static Dictionary<string, AttributeValue> Copy(this Dictionary<string, AttributeValue> map) =>
        map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Copy());

    private static MemoryStream Copy(this MemoryStream binary)
    {
        var copy = new MemoryStream();
        binary.CopyTo(copy);
        copy.Seek(0, SeekOrigin.Begin);
        return copy;
    }
}
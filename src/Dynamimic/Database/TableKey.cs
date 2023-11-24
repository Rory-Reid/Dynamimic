using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Dynamimic.Database;

public record TableKey(string Name, ScalarAttributeType Type)
{
    public string GetKeyValue(AttributeValue attribute)
    {
        if (this.Type.Value == ScalarAttributeType.S.Value)
        {
            return attribute.S;
        }

        if (this.Type.Value == ScalarAttributeType.N.Value)
        {
            return attribute.N;
        }

        if (this.Type.Value == ScalarAttributeType.B.Value)
        {
            using var sr = new StreamReader(attribute.B);
            return sr.ReadToEnd();
        }

        throw new InvalidOperationException("Weirdly, the key wasn't one of the three possible types.");
    }
};
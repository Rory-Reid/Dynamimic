using Amazon.DynamoDBv2.Model;
using Shouldly;

namespace Dynamimic.Tests.sdk;

public static class ShouldContainEquivalentExtension
{
    private static readonly Dictionary<string, object> Comparers = new()
    {
        {nameof(KeySchemaElement), new KeySchemaElementComparer()},
        {nameof(AttributeDefinition), new AttributeDefinitionComparer()}
    };
    
    public static void ShouldContainEquivalent<T>(this IEnumerable<T> actual, T expected)
    {
        actual.ShouldContain(expected, Comparers[typeof(T).Name] as IEqualityComparer<T> ?? EqualityComparer<T>.Default);
    }

    private class KeySchemaElementComparer : IEqualityComparer<KeySchemaElement>
    {
        public bool Equals(KeySchemaElement? x, KeySchemaElement? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.AttributeName == y.AttributeName && Equals(x.KeyType, y.KeyType);
        }

        public int GetHashCode(KeySchemaElement obj)
        {
            return HashCode.Combine(obj.AttributeName, obj.KeyType);
        }
    }
    
    private class AttributeDefinitionComparer : IEqualityComparer<AttributeDefinition>
    {
        public bool Equals(AttributeDefinition? x, AttributeDefinition? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.AttributeName == y.AttributeName && Equals(x.AttributeType, y.AttributeType);
        }

        public int GetHashCode(AttributeDefinition obj)
        {
            return HashCode.Combine(obj.AttributeName, obj.AttributeType);
        }
    }
}
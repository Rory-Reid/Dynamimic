using Amazon.DynamoDBv2.Model;

namespace Dynamimic.Database;

public class TableItem
{
    public TableItem(Dictionary<string, AttributeValue> item) => this.Attributes = item.Copy();

    public Dictionary<string, AttributeValue> Attributes { get; }
}
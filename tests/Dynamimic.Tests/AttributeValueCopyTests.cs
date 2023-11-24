using System.Text;
using Amazon.DynamoDBv2.Model;
using Shouldly;

namespace Dynamimic.Tests;

public class AttributeValueCopyTests
{
    [Fact]
    public void Copies_string()
    {
        var attribute = new AttributeValue {S = "Hello"};
        var copied = attribute.Copy();

        attribute.S = "Modified";
        
        copied.S.ShouldBe("Hello");
    }
    
    [Fact]
    public void Copies_number()
    {
        var attribute = new AttributeValue {N = "123"};
        var copied = attribute.Copy();

        attribute.N = "456";
        
        copied.N.ShouldBe("123");
    }
    
    [Fact]
    public void Copies_bool()
    {
        var attribute = new AttributeValue {BOOL = true};
        var copied = attribute.Copy();

        attribute.BOOL = false;
        
        copied.BOOL.ShouldBe(true);
    }
    
    [Fact]
    public void Copies_binary()
    {
        var attribute = new AttributeValue {B = new MemoryStream(Encoding.UTF8.GetBytes("Hello"))};
        var copied = attribute.Copy();

        attribute.B = new MemoryStream(Encoding.UTF8.GetBytes("Modified"));

        using var reader = new StreamReader(copied.B);
        reader.ReadToEnd().ShouldBe("Hello");
    }
    
    [Fact]
    public void Copies_null()
    {
        var attribute = new AttributeValue {NULL = true};
        var copied = attribute.Copy();

        attribute.NULL = false;
        
        copied.NULL.ShouldBe(true);
    }
    
    [Fact]
    public void Copies_list()
    {
        var attribute = new AttributeValue {L = new List<AttributeValue> {new() {S = "Hello"}}};
        var copied = attribute.Copy();

        attribute.L[0].S = "Modified";
        
        copied.L[0].S.ShouldBe("Hello");
    }
    
    [Fact]
    public void Copies_map()
    {
        var attribute = new AttributeValue {M = new Dictionary<string, AttributeValue> {["key"] = new() {S = "Hello"}}};
        var copied = attribute.Copy();

        attribute.M["key"].S = "Modified";
        
        copied.M["key"].S.ShouldBe("Hello");
    }
    
    [Fact]
    public void Copies_string_set()
    {
        var attribute = new AttributeValue {SS = new List<string> {"Hello"}};
        var copied = attribute.Copy();

        attribute.SS[0] = "Modified";
        
        copied.SS[0].ShouldBe("Hello");
    }
    
    [Fact]
    public void Copies_number_set()
    {
        var attribute = new AttributeValue {NS = new List<string> {"123"}};
        var copied = attribute.Copy();

        attribute.NS[0] = "456";
        
        copied.NS[0].ShouldBe("123");
    }
    
    [Fact]
    public void Copies_binary_set()
    {
        var attribute = new AttributeValue {BS = new List<MemoryStream> {new(Encoding.UTF8.GetBytes("Hello"))}};
        var copied = attribute.Copy();

        attribute.BS[0] = new MemoryStream(Encoding.UTF8.GetBytes("Modified"));

        using var reader = new StreamReader(copied.BS[0]);
        reader.ReadToEnd().ShouldBe("Hello");
    }
}
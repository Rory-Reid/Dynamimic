using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace Dynamimic;

public partial class DynamoDbMimic
{
    public Task<DescribeTableResponse> DescribeTableAsync(string tableName,
        CancellationToken cancellationToken = default) =>
        this.DescribeTableAsync(new DescribeTableRequest
        {
            TableName = tableName
        }, cancellationToken);

    public Task<DescribeTableResponse> DescribeTableAsync(DescribeTableRequest request,
        CancellationToken cancellationToken = default)
    {
        var table = this.GetTable(request.TableName);
        var response = new DescribeTableResponse
        {
            HttpStatusCode = HttpStatusCode.OK,
            ContentLength = 500,
            ResponseMetadata = this.StandardMetadata(),
            Table = table.Describe()
        };
        return Task.FromResult(response);
    }
}
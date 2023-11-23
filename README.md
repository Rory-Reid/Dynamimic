# Dynamimic

Dynamimic is a mimic of DynamoDB for use with local environments. Dynamimic is built with automated testing in mind, but
should perform plenty well enough for manual local testing if that's your bag.

Dynamimic facilitates state and behaviour based testing, rather than interaction testing.

## Usage

The `DynamoDbMimic` class implements the AWS SDK IAmazonDynamoDB interface and is used through that alone. You should be
able to drop it in directly wherever you depend on the real thing.

## Current state

This is not fit for use unless your only use case is to create tables, put, and get single items. Development still has
a long way to go before it's worth using.

"Basic support" means that the core function works but some details around the response might be slightly wrong, or
not fully supported. "DescribeTable" for example will return core things like key schema, but has no support for GSI/LSI
description today.

| API Method    | Basic support |
|---------------|---------------|
| CreateTable   | ✅             |
| DescribeTable | ✅             |
| PutItem       | ✅             |
| GetItem       | ✅             |

All other methods should not crash but won't do anything useful.

## Contributing

Contributions welcome.
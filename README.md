# Dynamimic

Dynamimic is a mimic of DynamoDB for use with local environments. Dynamimic is built with automated testing in mind, but
should perform plenty well enough for manual local testing if that's your bag.

## Usage

The `DynamoDbMimic` class implements the AWS SDK IAmazonDynamoDB interface and is used through that alone. You should be
able to drop it in directly wherever you depend on the real thing.
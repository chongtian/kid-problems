using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using Amazon.DynamoDBv2.Model;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Reflection;

namespace Migration
{
    internal class DynamoDbHelper
    {
        private const string PathToAwsCredential = "D:\\KeyVault\\Aws\\credentials";
        private static AmazonDynamoDBClient? _client;

        private static AmazonDynamoDBClient GetDbClient()
        {
            if (_client == null)
            {
                var chain = new CredentialProfileStoreChain(PathToAwsCredential);
                AWSCredentials awsCredentials;
                if (chain.TryGetAWSCredentials("aws-dynamo-db", out awsCredentials))
                {
                    var dynamoDbClient = new AmazonDynamoDBClient(awsCredentials, Amazon.RegionEndpoint.USEast2);
                    _client = dynamoDbClient;
                }
                else
                {
                    throw new Exception("Cannot find AWS credential.");
                }
            }

            return _client;
        }

        public static async Task<long> BatchWriteAsync<T>(List<T> entities, DynamoDBContextConfig? config = null)
        {
            var client = GetDbClient();
            var context = config == null ? new DynamoDBContext(client) : new DynamoDBContext(client, config);
            var itemBatch = context.CreateBatchWrite<T>();
            itemBatch.AddPutItems(entities);
            Console.WriteLine("Adding entities to the table.");
            await itemBatch.ExecuteAsync();
            return entities.Count;
        }

        private static async Task<List<string>> ListTables()
        {
            var client = GetDbClient();
            List<string> tables = new();

            // Initial value for the first page of table names.
            string? lastEvaluatedTableName = null;
            do
            {
                // Create a request object to specify optional parameters.
                var request = new ListTablesRequest
                {
                    Limit = 10, // Page size.
                    ExclusiveStartTableName = lastEvaluatedTableName
                };

                var response = await client.ListTablesAsync(request);
                foreach (string name in response.TableNames)
                {
                    tables.Add(name);
                }

                lastEvaluatedTableName = response.LastEvaluatedTableName;

            } while (lastEvaluatedTableName != null);

            return tables;
        }

        /// <summary>
        /// This method reads metadata of all kidproblem tables and creates new tables like the source ones.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static async Task<int> CreateTables(string prefix)
        {
            var client = GetDbClient();
            var existingTables = await ListTables();
            var sourceTables = new string[] {
                "kp_codes",
                "kp_exam_assignments",
                "kp_exam_def",
                "kp_exam_runs",
                "kp_exam_run_details",
                "kp_exam_summaries",
                "kp_problems",
                "kp_problem_summaries"
            };

            foreach (var tableName in sourceTables)
            {
                Console.WriteLine($"Getting metadata of table {tableName} ...");
                var table = await client.DescribeTableAsync(tableName);



                if (table.Table != null)
                {
                    string newTableName = $"{prefix}{tableName}";
                    if (existingTables.Contains(newTableName))
                    {
                        Console.WriteLine($"{newTableName} already exists.");
                        continue;
                    }

                    var indexes = new List<GlobalSecondaryIndex>();
                    foreach (var index in table.Table.GlobalSecondaryIndexes)
                    {
                        var gsi = new GlobalSecondaryIndex()
                        {
                            IndexName = index.IndexName,
                            KeySchema = index.KeySchema,
                            Projection = index.Projection
                        };
                        indexes.Add(gsi);
                    }

                    var createTableRequest = new CreateTableRequest
                    {
                        TableName = newTableName,
                        BillingMode = BillingMode.PAY_PER_REQUEST,
                        AttributeDefinitions = table.Table.AttributeDefinitions,
                        KeySchema = table.Table.KeySchema,
                        GlobalSecondaryIndexes = indexes
                    };

                    var res = await client.CreateTableAsync(createTableRequest);
                    Console.WriteLine("Created table " + newTableName);
                }
            }

            return 0;
        }

        public static async Task<int> PrepareTablesForProduction()
        {
            var client = GetDbClient();

            await DeleteTable(client, "kp_exam_runs");
            await DeleteTable(client, "kp_exam_assignments");

            Console.WriteLine($"Creating table kp_exam_runs");
            var createTableRequest = new CreateTableRequest
            {
                TableName = "kp_exam_runs",
                BillingMode = BillingMode.PAY_PER_REQUEST,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "id",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "answer_by",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "family_id",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "create_time",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "id",
                        KeyType = "HASH"
                    }
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex> { 
                    new GlobalSecondaryIndex {
                        IndexName = "answer_by-create_time-index",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "answer_by",
                                KeyType = "HASH"
                            },
                            new KeySchemaElement
                            {
                                AttributeName = "create_time",
                                KeyType = "RANGE"
                            }
                        },
                        Projection = new Projection
                        {
                            ProjectionType = "KEYS_ONLY"
                        }
                    },
                    new GlobalSecondaryIndex {
                        IndexName = "family_id-create_time-index",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "family_id",
                                KeyType = "HASH"
                            },
                            new KeySchemaElement
                            {
                                AttributeName = "create_time",
                                KeyType = "RANGE"
                            }
                        },
                        Projection = new Projection
                        {
                            ProjectionType = "KEYS_ONLY"
                        }
                    }
                }
            };

            var res = await client.CreateTableAsync(createTableRequest);
            await WaitUntilTableReadyOrDeleted(client, "kp_exam_runs", false);
            Console.WriteLine("Created table kp_exam_runs");

            Console.WriteLine($"Creating table kp_exam_assignments");
            createTableRequest = new CreateTableRequest
            {
                TableName = "kp_exam_assignments",
                BillingMode = BillingMode.PAY_PER_REQUEST,
                AttributeDefinitions = new List<AttributeDefinition>()
                {
                    new AttributeDefinition
                    {
                        AttributeName = "id",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "family_id",
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = "create_time",
                        AttributeType = "S"
                    }
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "id",
                        KeyType = "HASH"
                    }
                },
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex> {
                    new GlobalSecondaryIndex {
                        IndexName = "family_id-create_time-index",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement
                            {
                                AttributeName = "family_id",
                                KeyType = "HASH"
                            },
                            new KeySchemaElement
                            {
                                AttributeName = "create_time",
                                KeyType = "RANGE"
                            }
                        },
                        Projection = new Projection
                        {
                            ProjectionType = "KEYS_ONLY"
                        }
                    }
                }
            };

            res = await client.CreateTableAsync(createTableRequest);
            await WaitUntilTableReadyOrDeleted(client, "kp_exam_assignments", false);
            Console.WriteLine("Created table kp_exam_assignments");

            return 0;
        }

        private static async Task DeleteTable(AmazonDynamoDBClient client, string tableName)
        {
            Console.WriteLine($"Deleting table {tableName}");
            var request = new DeleteTableRequest { TableName = tableName };
            await client.DeleteTableAsync(request);
            await WaitUntilTableReadyOrDeleted(client, tableName, true);
            Console.WriteLine($"Table {tableName} is deleted");
        }

        private static async Task WaitUntilTableReadyOrDeleted(AmazonDynamoDBClient client, string tableName, bool delete)
        {
            string? status = null;
            // Let us wait until table is created. Call DescribeTable.
            do
            {
                Thread.Sleep(5000); // Wait 5 seconds.
                try
                {
                    var table = await client.DescribeTableAsync(tableName);
                    status = table.Table.TableStatus;
                }
                catch (ResourceNotFoundException)
                {
                    // DescribeTable is eventually consistent. So you might
                    // get resource not found. So we handle the potential exception.
                    if (delete)
                    {
                        break;
                    }
                }
            } while (status != "ACTIVE");
        }

    }
}

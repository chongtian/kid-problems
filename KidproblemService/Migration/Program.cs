// See https://aka.ms/new-console-template for more information
using Migration;

//var result = await MigrationHelper.MigrateProblemsAsync(true);
//var result = await MigrationHelper.MigrateExamRunsAsync(false);
//Console.WriteLine($"Migrated {result} records.");

// Prepare Aws DynamoDb Tables
// await MigrationHelper.PrepareDynamoDbTables("dev_");

if (args.Length > 1)
{
    string action = args[0].ToLower();
    string arg1 = args[1];
    string arg2 = args.Length > 2 ? args[2] : "dev_";

    switch (action)
    {
        case "problem":
            var r1 = await MigrationHelper.MigrateProblemsAsync(arg1.ToLower()[0] != 'p', arg2);
            Console.WriteLine($"Migrated {r1} records.");
            break;

        case "exam":
            var r2 = await MigrationHelper.MigrateExamRunsAsync(arg1.ToLower()[0] != 'p', arg2);
            Console.WriteLine($"Migrated {r2} records.");
            break;

        case "table":
            if (!string.IsNullOrEmpty(arg1))
            {
                await MigrationHelper.PrepareDynamoDbTables(arg1);
            }
            else
            {
                Console.WriteLine("prefix is required.");
            }
            break;

        default:
            Console.WriteLine($"Action {action} is not supported.");
            break;
    }
}
else
{
    Console.WriteLine("Usages:");
    Console.WriteLine("Migration Problems to DynamoDb. d=development mode, p=production mode");
    Console.WriteLine("Migration problem d|p prefix");
    Console.WriteLine("Migration Exams to DynamoDb. d=development mode, p=production mode");
    Console.WriteLine("Migration exam d|p prefix");
    Console.WriteLine("Clone production tables and create dev tables in DynamoDb. ");
    Console.WriteLine("Migration table prefix");
}

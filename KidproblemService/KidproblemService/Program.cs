using Amazon.CognitoIdentityProvider;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using KidproblemService.Controllers;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using KidproblemService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddEndpointsApiExplorer();

// Get AWS profile and credentials
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
// Get customized AWS configuration
builder.Services.Configure<AwsConfiguration>(builder.Configuration.GetSection("AwsConfiguration"));
var dynamoDbConfig = builder.Configuration.GetSection("AwsConfiguration");

// Add AWS Cognito
builder.Services.AddCognitoIdentity();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["AwsConfiguration:Authority"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false
    };
});
// Add Claim-based policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
                      policy.RequireClaim("cognito:groups", "AdminUserGroup"));
    options.AddPolicy("ParentOnly", policy =>
                      policy.RequireClaim("cognito:groups", "AdminUserGroup", "ParentUserGroup"));
    options.AddPolicy("Child", policy =>
                      policy.RequireClaim("cognito:groups", "AdminUserGroup", "ParentUserGroup", "ChildUserGroup"));
});

builder.Services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
builder.Services.AddScoped<TokenUser>();
builder.Services.AddMemoryCache();

builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>();

// Add AWS DynamoDb
var awsOptions = builder.Configuration.GetAWSOptions();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>(service =>
{
    IAmazonDynamoDB amazonDynamoDBClient = service.GetRequiredService<IAmazonDynamoDB>();
    DynamoDBContextConfig dynamoDBContextConfig = new DynamoDBContextConfig
    {
        TableNamePrefix = dynamoDbConfig.GetValue<string>("DynamoDbTableNamePrefix")
    };
    return new DynamoDBContext(amazonDynamoDBClient, dynamoDBContextConfig);
});

// Add AWS S3
builder.Services.AddAWSService<IAmazonS3>();

// Add KidProblem services
builder.Services.AddScoped<IProblemService, ProblemService>();
builder.Services.AddScoped<IScrapService, ScrapService>();
builder.Services.AddScoped<IExamDefinitionService, ExamDefinitionService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IExamRunService, ExamRunService>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddScoped<ICodeService, CodeService>();
builder.Services.AddScoped<ICacheService, CacheService>();

// Deploy to AWS Lambda
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Define CORS policies
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: "allowCorsDev",
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "allowCorsProduction",
        policy =>
        {
            policy.WithOrigins("http://kidproblemdev.nowsuncorp.com", "http://kidproblem.nowsuncorp.com").AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("allowCorsDev");
}
else
{
    app.UseCors("allowCorsProduction");
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserInfoMiddleware>();

app.MapControllers();

app.Run();

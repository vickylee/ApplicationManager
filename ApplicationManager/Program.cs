using ApplicationManager.Services.Contract;
using ApplicationManager.Services.Implementation;
using Microsoft.OpenApi.Models;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LiveStory Demo",
        Version = "v1"
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
});

builder.Services.AddSingleton<IFamilyService>(InitializeCosmosClientInstanceAsync(builder).GetAwaiter().GetResult());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FBAuthDemoApp v1"));
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();


/// <summary>
/// Creates a Cosmos DB database and a container with the specified partition key.
/// </summary>
/// <returns></returns>
static async Task<FamilyService> InitializeCosmosClientInstanceAsync(WebApplicationBuilder builder)
{
    IConfigurationSection confSection = builder.Configuration.GetSection("CosmosDb");

    string account = confSection.GetSection("Account").Value;
    string databaseName = confSection.GetSection("DatabaseName").Value;
    string containerName = confSection.GetSection("ContainerName").Value;
    string keyVaultUri = confSection.GetSection("KeyVaultUri").Value;
    string keyVaultKey = confSection.GetSection("KeyVaultKey").Value;

    SecretClientOptions options = new SecretClientOptions()
    {
        Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
    };

    SecretClient secretClient = new SecretClient(vaultUri: new Uri(keyVaultUri), credential: new DefaultAzureCredential(), options);
    string secret = secretClient.GetSecret(keyVaultKey).Value.Value;

    Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, secret);
    FamilyService familyService = new FamilyService(client, databaseName, containerName);
    Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
    await database.Database.CreateContainerIfNotExistsAsync(containerName, "/pk");
    return familyService;
}

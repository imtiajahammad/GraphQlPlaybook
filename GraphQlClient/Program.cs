using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using GraphQlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var graphQLURI = builder.Configuration["GraphQLURI"];
if (string.IsNullOrWhiteSpace(graphQLURI))
{
    throw new ArgumentNullException(nameof(graphQLURI), "GraphQL URI is not configured.");
}
builder.Services.AddScoped<IGraphQLClient>(s => new GraphQLHttpClient(graphQLURI, new NewtonsoftJsonSerializer()));

builder.Services.AddScoped<OwnerConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


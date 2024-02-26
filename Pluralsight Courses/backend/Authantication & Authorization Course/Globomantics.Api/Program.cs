using Globomantics.Api;
using Globomantics.Api.Repositories;
using Globomantics.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen( c=> c.AddSwaggerApiKeySecurity());
builder.Services.AddScoped<IConferenceRepository, ConferenceRepository>();
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();

builder.Services.AddSingleton(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://localhost:5002/") };
    client.DefaultRequestHeaders.Add("XApiKey", "secret");
    return client;
});

var app = builder.Build();

app.UseApiKey();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

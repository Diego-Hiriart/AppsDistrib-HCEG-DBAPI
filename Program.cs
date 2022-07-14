using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var hostCORS = "AllowHostOrigins";
string[] hosts = builder.Configuration.GetValue<string>("AllowedHosts").Split(";");

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: hostCORS, policy =>
    {
        policy.WithOrigins(hosts)//hosts allowed to use this API
        .WithHeaders(HeaderNames.ContentType)//Allow content type (to use jsons)
        .WithMethods("POST", "GET", "PUT", "DELETE");//Allow all methods

    });
});

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

app.UseCors(hostCORS);

app.UseAuthorization();

app.MapControllers();

app.Run();

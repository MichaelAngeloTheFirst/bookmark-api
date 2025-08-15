using BookmarkAI_API.Consumers;
using BookmarkAI_API.Contracts;
using BookmarkAI_API.Services;
using BookmarkAI_API.Modules;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();


// MassTransit
builder.Services.AddSingleton<JobScrapperService>();

builder.Services.AddTransient<Scrapper>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookmarkAI_API.Consumers.ScrapperConsumer>();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ReceiveEndpoint("job-queue", e =>
        {
            e.ConfigureConsumer<ScrapperConsumer>(context);
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.MapControllers();
app.Run();

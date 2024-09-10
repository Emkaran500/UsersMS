using UsersMS.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.Configure<RabbitMqOptions>(rabbitMqSection);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.Run();
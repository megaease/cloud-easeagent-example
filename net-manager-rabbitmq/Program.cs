using net.manager.rabbitmq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("UserManager");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Receive receive = new Receive(app.Services.GetService<IHttpClientFactory>());
app.Lifetime.ApplicationStarted.Register(() => new Thread(receive.Start).Start());
app.Lifetime.ApplicationStopped.Register(() => receive.Stop());

app.Run();

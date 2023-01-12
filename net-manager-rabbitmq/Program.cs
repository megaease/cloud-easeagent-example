using net.manager.rabbitmq;
using zipkin4net.Middleware;
using zipkin4net.Transport.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

easeagent.Agent.RegisterFromYaml(Environment.GetEnvironmentVariable("EASEAGENT_CONFIG"));
builder.Services.AddHttpClient("UserManager").AddHttpMessageHandler(provider =>
    TracingHandler.WithoutInnerHandler(easeagent.Agent.GetServiceName()));;

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
app.Lifetime.ApplicationStopped.Register(() => easeagent.Agent.Stop());
app.UseTracing(easeagent.Agent.GetServiceName());
app.Run();

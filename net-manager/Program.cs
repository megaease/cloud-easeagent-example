using net.manager;
using zipkin4net.Middleware;
using zipkin4net.Transport.Http;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("*");
                      });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//init easeagent
easeagent.Agent.RegisterFromYaml(Environment.GetEnvironmentVariable("EASEAGENT_CONFIG"));
builder.Services.AddHttpClient("UserManager").AddHttpMessageHandler(provider =>
    TracingHandler.WithoutInnerHandler(easeagent.Agent.GetServiceName()));

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

app.UseCors(MyAllowSpecificOrigins);


HttpClientProxy.CLIENT = new HttpClientProxy(app.Services.GetService<IHttpClientFactory>());

//register a stop for easeagent
app.Lifetime.ApplicationStopped.Register(() => easeagent.Agent.Stop());

//tracing for http server
app.UseTracing(easeagent.Agent.GetServiceName());
app.Run();

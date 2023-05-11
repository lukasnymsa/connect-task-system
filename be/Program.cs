using MediatR;
using System.Reflection;
using Microsoft.OpenApi.Models;
using TaskSystem.Exceptions.Handlers;
using TaskSystem.Models;
using TaskSystem.Services;
using TaskSystem.Config;
using TaskSystem.Extensions;
using TaskSystem.Enums;
using TaskSystem.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false)
        .AddEnvironmentVariables();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(new Expiration(
    builder.Configuration.GetValue<int>("Config:ExpirationTimeMinutes:RequestCodeTtl"),
    builder.Configuration.GetValue<int>("Config:ExpirationTimeMinutes:TokenTtl")));

builder.Services.AddSingleton<UserCache>(sp =>
{
    var expiration = sp.GetRequiredService<Expiration>();
    var testingEmail = builder.Configuration.GetValue<string>("Config:Testing:Email");
    var testingToken = builder.Configuration.GetValue<string>("Config:Testing:Token");

    return testingEmail is not null && testingToken is not null
        ? new UserCache(expiration, testingEmail, testingToken)
        : new UserCache(expiration);
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Config:MailSettings"));
builder.Services.AddSingleton<MailSettings>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IProjectManagementService, YouTrackService>();

var enumValues = builder.Configuration.GetSection("Config:States").GetChildren();
var dict = enumValues.ToDictionary(x => x.Key, x => x.Value ?? string.Empty);
EnumExtensions.SetEnumValues<TaskStateEnum>(dict);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo() { Title = "Demo API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<UserAuthorizationFilter>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

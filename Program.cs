using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using ZKLT25.API.EntityFrameworkCore;
using ZKLT25.API.Helper;
using ZKLT25.API.Helper.Middlewares;
using ZKLT25.API.Helper.TokenModule;
using ZKLT25.API.IServices;
using ZKLT25.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5266", "http://0.0.0.0:5266");

#region 配置Serilog
// 移除默认日志提供程序
builder.Logging.ClearProviders();

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Filter.ByExcluding(logEvent => logEvent.Exception != null)
    .CreateLogger();

builder.Logging.AddSerilog(logger, dispose: true);
#endregion

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// 异常处理
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddExceptionHandler<BizExceptionHandler>();
builder.Services.AddExceptionHandler<SysExceptionHandler>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo", Version = "v1" });

    // 添加安全定义
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "格式：Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    // 添加安全要求
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }, new string[]{}
        }
    });
});

// 注册AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ZKLT25Profile>();
});

// 注册EFCore DbContext
var configuration = builder.Configuration;
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(configuration.GetConnectionString("Default"));
});

// 注册AutoFac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new AutoFacManager());
});

// 注册NewtonsoftJson，配置大小写不敏感
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
});

// Jwt认证
var token = configuration.GetSection("Jwt").Get<JwtTokenModel>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretByte = Encoding.UTF8.GetBytes(token.Security);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = token.Issuer,

            ValidateAudience = true,
            ValidAudience = token.Audience,

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
        options.MapInboundClaims = false;
    });

// 注册JWT认证中间件
builder.Services.Configure<JwtTokenModel>(configuration.GetSection("Jwt"));
builder.Services.AddTransient<JWTTokenValidationMiddleware>();

// 配置设置
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// 配置跨域
builder.Services.AddCors(option =>
        option
        .AddDefaultPolicy(x => x.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .WithExposedHeaders("IsFile", "TotalCount", "content-disposition"))
    );

var app = builder.Build();

// 启用请求缓冲
app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

// 跨域处理
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Response.Headers.TryGetValue("Access-Control-Allow-Origin", out var values))
        {
            if (values.Count > 1)
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = values[0];
            }
        }
        return Task.CompletedTask;
    });

    await next();
});

app.UseCors();

app.UseExceptionHandler(opt => { });

// Configure the HTTP request pipeline.
// 临时启用所有环境的Swagger，方便测试
app.UseSwagger();
app.UseSwaggerUI();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// 添加鉴权
app.UseAuthentication();

// 使用自定义JWT认证中间件
app.UseMiddleware<JWTTokenValidationMiddleware>();

// 添加授权
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "agv",
    pattern: "agv/*",
    defaults: new { controller = "Agv" })
   .AllowAnonymous();

app.Run();

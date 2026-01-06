using CloudinaryDotNet;
using LibraryManagement.Data;
using LibraryManagement.Helpers;
using LibraryManagement.Helpers.Interface;
using LibraryManagement.Models;
using LibraryManagement.Repository;
using LibraryManagement.Repository.InterFace;
using LibraryManagement.Repository.IRepository;
using LibraryManagement.Service;
using LibraryManagement.Service.Interface;
using LibraryManagement.Service.InterFace;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
builder.Configuration["ConnectionStrings:PostgreSQLConnection"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__PostgreSQLConnection");
builder.Configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT__SecretKey");
builder.Configuration["SendGrid:ApiKey"] = Environment.GetEnvironmentVariable("SENDER_APIKEY");
builder.Configuration["SendGrid:Email"] = Environment.GetEnvironmentVariable("SENDER_EMAIL");
builder.Configuration["SendGrid:Name"] = Environment.GetEnvironmentVariable("SENDER_NAME");
builder.Configuration["CloudinarySettings:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__CLOUDNAME");
builder.Configuration["CloudinarySettings:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APIKEY");
builder.Configuration["CloudinarySettings:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APISECRET");
builder.Configuration["MongoDB:ConnectionString"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__MongoDbConnection");
builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__ID"] = Environment.GetEnvironmentVariable("CLIENT__ID");
builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__SECRET"] = Environment.GetEnvironmentVariable("CLIENT__SECRET");
builder.Configuration["GeminiSettings:ApiKey"] = Environment.GetEnvironmentVariable("APIKEY__GEMINI");
builder.Configuration["GeminiSettings:BaseUrl"] = Environment.GetEnvironmentVariable("GEMINI__APIURL");
builder.Configuration["RedisSettings:Host"] = Environment.GetEnvironmentVariable("REDISSETTINGS__HOST");
builder.Configuration["RedisSettings:Port"] = Environment.GetEnvironmentVariable("REDISSETTINGS__PORT");
builder.Configuration["RedisSettings:Password"] = Environment.GetEnvironmentVariable("REDISSETTINGS__PASSWORD");

System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

builder.Services.AddDbContext<LibraryManagermentContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultSignInScheme = "GoogleCookie";
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is missing!")))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                code = "UNAUTHENTICATION",
                status = 401,
                message = "Vui lòng đăng nhập"
            });
            return context.Response.WriteAsync(result);
        },

        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
})
.AddCookie("GoogleCookie")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__ID"]!;
    options.ClientSecret = builder.Configuration["GOOGLE_SETTINGS:GOOGLE__CLIENT__SECRET"]!;
    options.CallbackPath = "/signin-google";
    options.SignInScheme = "GoogleCookie";
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LibraryManagement API", Version = "v1" });
    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Nhập JWT Bearer token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtScheme.Reference.Id, jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton(option =>
{
    var settings = option.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var acc = new Account(
        Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__CLOUDNAME"),
        Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APIKEY"),
        Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APISECRET")
    );
    return new Cloudinary(acc);
});

// Services DI
builder.Services.AddScoped<IReaderService, ReaderService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ITypeReaderService, TypeReaderService>();
builder.Services.AddScoped<ITypeBookService, TypeBookService>();
builder.Services.AddScoped<IBookReceiptService, BookReceiptService>();
builder.Services.AddScoped<ILoanBookService, LoanBookService>();
builder.Services.AddScoped<ISlipBookService, SlipBookService>();
builder.Services.AddScoped<IParameterService, ParameterService>();
builder.Services.AddScoped<IPenaltyTicketService, PenaltyTicketService>();
builder.Services.AddScoped<ICategoryReportService, CategoryReportService>();
builder.Services.AddScoped<IOverdueReportService, OverdueReportService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddScoped<IChatHistoryService, ChatHistoryService>();
builder.Services.AddScoped<IChatWithAIService, ChatWithAIService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IUpLoadImageFileService, UpLoadImageFileService>();

builder.Services.AddScoped<IMessageHubService, MessageHubService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// Redis Registration
builder.Services.AddStackExchangeRedisCache(options =>
{
    var host = builder.Configuration["RedisSettings:Host"];
    var port = builder.Configuration["RedisSettings:Port"];
    var password = builder.Configuration["RedisSettings:Password"];
    options.Configuration = $"{host}:{port},password={password},ssl=true,abortConnect=false";
});

builder.Services
    .AddFluentEmail(builder.Configuration["SendGrid:Email"], builder.Configuration["SendGrid:Name"])
    .AddRazorRenderer()
    .AddSendGridSender(builder.Configuration["SendGrid:ApiKey"]);

builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseCors("SignalRCors");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{   
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "API Docs";
});

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
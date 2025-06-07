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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

DotNetEnv.Env.Load();

builder.Configuration["ConnectionStrings:PostgreSQLConnection"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__PostgreSQLConnection");
builder.Configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT__SecretKey");

builder.Configuration["EmailSettings:SmtpServer"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SMTPSERVER");
builder.Configuration["EmailSettings:SmtpPort"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SMTPPORT");
builder.Configuration["EmailSettings:SenderEmail"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SENDEREMAIL");
builder.Configuration["EmailSettings:SenderPassword"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__SENDERPASSWORD");
builder.Configuration["EmailSettings:EnableSSL"] = Environment.GetEnvironmentVariable("EMAILSETTINGS__ENABLESSL");

builder.Configuration["CloudinarySettings:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__CLOUDNAME");
builder.Configuration["CloudinarySettings:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APIKEY");
builder.Configuration["CloudinarySettings:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APISECRET");
builder.Configuration["MongoDB:ConnectionString"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__MongoDbConnection");


// Connect to MongoDB
builder.Services.AddSingleton<MessageService>();
System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//NpgsqlConnection.GlobalTypeMapper.MapDateTime(DateTimeKind.Utc);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnglishApp API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Nhập JWT Bearer token (chỉ phần token, không kèm 'Bearer ').",
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


// Cấu hình cho phép tất cả các ứng dụng được gọi đến API
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
 policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));



builder.Services.AddSingleton(option =>
{
    var settings = option.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
    return new Cloudinary(account);

}); 

builder.Services.AddDbContext<LibraryManagermentContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Đăng ký sử dụng Mapper
builder.Services.AddAutoMapper(typeof(Program));

// Tạo service sử dụng JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new
    Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var emailConfig = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

// Life cycle DI
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
builder.Services.AddScoped<IUpLoadImageFileService, UpLoadImageFileService>();



// Cấu hình up ảnh lên Cloudinary
var account = new Account(
    Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__CLOUDNAME"),
    Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APIKEY"),
    Environment.GetEnvironmentVariable("CLOUDINARYSETTINGS__APISECRET")
);
builder.Services.AddSingleton(new Cloudinary(account));
builder.Services.AddScoped<IUpLoadImageFileService, UpLoadImageFileService>();



builder.Services.AddFluentEmail("noreply@gmail.com", "no reply")
                .AddSmtpSender(new System.Net.Mail.SmtpClient(emailConfig!.SmtpServer)
                {
                    Port = emailConfig.SmtpPort,
                    Credentials = new NetworkCredential(emailConfig.SenderEmail, emailConfig.SenderPassword), 
                    EnableSsl =  emailConfig.EnableSSL
                });
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = "swagger"; // Swagger UI sẽ ở /docs
    c.DocumentTitle = "API Docs";
});
app.MapControllers();


app.Run();

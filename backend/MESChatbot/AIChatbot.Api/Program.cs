using AIChatbot.Api.Models;
using AIChatbot.Api.Services;
using AIChatbot.Api.Services.Database;
using AIChatbot.Api.Services.Embedding;
using AIChatbot.Api.Services.Excel;
using AIChatbot.Api.Services.Llm;
using AIChatbot.Api.Services.Vecto;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Models;
using OpenAI.Chat;
using OpenAI.Embeddings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// ==========================================================
// CẤU HÌNH XÁC THỰC JWT CHO CHATBOT
// - Nhận JWT từ header Authorization: Bearer <token>
// - Kiểm tra nơi phát hành token (Issuer)
// - Kiểm tra đối tượng sử dụng token (Audience)
// - Kiểm tra thời hạn token
// - Kiểm tra chữ ký bằng Jwt:Key
// - Giúp API xác định người dùng đang đăng nhập
// - Là cơ sở để tách lịch sử Redis theo từng tài khoản
// ==========================================================
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtSettings["Key"]
                    ?? throw new InvalidOperationException("Jwt:Key chưa được cấu hình.")
                )
            )
        };
    });

builder.Services.AddAuthorization();

//Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://localhost:5173", "http://localhost:5173") // Cho phép domain của React
              .AllowAnyHeader() // Cho phép tất cả các Header (như Content-Type, Accept)
              .AllowAnyMethod(); // Cho phép tất cả các Method (POST, GET, OPTIONS, v.v.)
    });
});

// Add services to the container.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

var configuration = builder.Configuration.Get<AppConfig>() ?? new AppConfig();
builder.Services.AddSingleton(configuration);
builder.Services.AddHttpClient();

//Register SQL Database Service
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddSingleton<ISqlDatabaseService>(sp =>
    new SqlDatabaseService(connectionString, sp.GetRequiredService<IDistributedCache>()));


//Register Open AI
builder.Services.AddSingleton<ILlmChatProvider, OpenAIChatProvider>();
builder.Services.AddSingleton(new ChatClient(configuration.OpenAI.ChatModel, configuration.OpenAI.ApiKey));
builder.Services.AddSingleton(new EmbeddingClient(configuration.OpenAI.EmbedModel, configuration.OpenAI.ApiKey));
builder.Services.AddSingleton<IEmbeddingProvider, OpenAIEmbeddingProvider>();


//Register RAG Pipeline
builder.Services.AddSingleton<IQdrantClient, QdrantHttpClient>();
builder.Services.AddSingleton<RagPipeline>();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 1024L * 1024L * 200L; // 200MB
});
builder.Services.AddScoped<IExcelExtractorService, ExcelExtractorService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "RAG Basic API", Version = "v1" }));

// Đăng ký Redis làm bộ nhớ đệm phân tán (Distributed Cache)
builder.Services.AddStackExchangeRedisCache(options =>
{
    // Trỏ tới Redis đang chạy trên Docker của bạn ở cổng 6379
    options.Configuration = "localhost:6379";
    options.InstanceName = "ChatbotApp_"; // Tiền tố để không đụng hàng với app khác
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Kích hoạt CORS (Phải đặt trước UseAuthorization và MapControllers)
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

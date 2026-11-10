global using AutoMapper;
global using MesMicroservice.Algorithm;
global using MesMicroservice.Application.Policies;
global using MesMicroservice.Domain.AggregateModels.BomAggregate;
global using MesMicroservice.Domain.AggregateModels.BusinessPartnerAggregate;
global using MesMicroservice.Domain.AggregateModels.EquipmentCapabilityAggregate;
global using MesMicroservice.Domain.AggregateModels.OperationAggregate;
global using MesMicroservice.Domain.AggregateModels.ParameterAggregate;
global using MesMicroservice.Domain.AggregateModels.PersonnelClassAggregate;
global using MesMicroservice.Domain.AggregateModels.QualityStandardAggregate;
global using MesMicroservice.Domain.AggregateModels.SegmentAggregate;
global using MesMicroservice.Infrastructure.Repositories.BOMRepositories;
global using MesMicroservice.Infrastructure.Repositories.BusinessPartnerRepositories;
global using MesMicroservice.Infrastructure.Repositories.OperationsDefinitionRepositories;
global using MesMicroservice.Infrastructure.Repositories.ProductionParameterRepositories;
global using MesMicroservice.Infrastructure.Repositories.QualityStandardRepositories;
global using MesMicroservice.Infrastructure.Repositories.SegmentModelRepositories;
global using Microsoft.Extensions.Logging.Abstractions;
using MesMicroservice.Api.Hubs;
using MesMicroservice.Api.Services;
using MesMicroservice.Infrastructure;
using MesMicroservice.MachineLearning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using System.Text;
//using MesMicroservice.Api.Hubs;

namespace MesMicroservice.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(500, 500);
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSettings["Key"]
                ?? throw new InvalidOperationException("Missing required configuration value 'Jwt:Key'.");

            // Authentication & Authorization
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EditPage2Policy", policy =>
                    policy.Requirements.Add(new PagePermissionRequirement(canEdit: true, pageId: 2)));
            });

            builder.Services.AddScoped<IAuthorizationHandler, PagePermissionHandler>();
            builder.Services.AddHttpContextAccessor();

            // Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly("MES.Api");
                    sqlServerOptions.CommandTimeout(600);
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                });
                options.EnableSensitiveDataLogging();
            });

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173",
                                     "https://web-thaiduong-mes.vercel.app", "http://localhost:5174", "https://localhost:5174")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddSignalR();

            // AutoMapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ModelToViewModelProfile>();
            }, new NullLoggerFactory());
            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
            builder.Services.AddSingleton(mapperConfig);

            // MediatR
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<ModelToViewModelProfile>();
                cfg.RegisterServicesFromAssemblyContaining<ApplicationDbContext>();
                cfg.RegisterServicesFromAssemblyContaining<Entity>();
            });

            // Repositories Registration
            builder.Services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
            builder.Services.AddScoped<IMaterialClassRepository, MaterialClassRepository>();
            builder.Services.AddScoped<IMaterialDefinitionRepository, MaterialDefinitionRepository>();
            builder.Services.AddScoped<IMaterialLotRepository, MaterialLotRepository>();
            builder.Services.AddScoped<IEquipmentClassRepository, EquipmentClassRepository>();
            builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            builder.Services.AddScoped<IManufacturingOrderRepository, ManufacturingOrderRepository>();
            builder.Services.AddScoped<IManufacturingRecordRepository, ManufacturingOrderRecordRepository>();
            builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
            builder.Services.AddScoped<IWorkOrderProgressRecord, WorkOrderProgressRecordRepository>();
            builder.Services.AddScoped<IWorkOrderErrorRecord, WorkOrderErrorRecordRepository>();
            builder.Services.AddScoped<IEquipmentWorkOrderRepository, EquipmentWorkOrderRepository>();
            builder.Services.AddScoped<IWorkCenterOutputRepository, WorkCenterOutputRepository>();
            builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
            builder.Services.AddScoped<IResourceRelationshipNetworkRepository, ResourceRelationshipNetworkRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IDowntimeRepository, DowntimeRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPersonRepository, PersonRepository>();
            builder.Services.AddScoped<IPersonnelClassRepository, PersonnelClassRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IQualityStandardRepository, QualityStandardRepository>();
            builder.Services.AddScoped<IBillOfMaterialRepository, BillOfMaterialRepository>();
            builder.Services.AddScoped<IProductionParameterRepository, ProductionParameterRepository>();
            builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
            builder.Services.AddScoped<IProcessSegmentRepository, ProcessSegmentRepository>();
            builder.Services.AddScoped<IOperationsDefinitionRepository, OperationsDefinitionRepository>();
            builder.Services.AddScoped<IMaterialSegmentSpecificationRepository, MaterialSegmentSpecificationRepository>();
            builder.Services.AddScoped<IProcessSegmentDependencyRepository, ProcessSegmentDependencyRepository>();
            builder.Services.AddScoped<IEquipmentCapabilityRepository, EquipmentCapabilityRepository>();
            builder.Services.AddScoped<IPersonnelCapabilityRepository, PersonnelCapabilityRepository>();
            builder.Services.AddScoped<IEquipmentSegmentSpecificationRepository, EquipmentSegmentSpecificationRepository>();
            builder.Services.AddScoped<IPersonnelSegmentSpecificationRepository, PersonnelSegmentSpecificationRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<ISchedulingService, SchedulingService>();
            builder.Services.AddScoped<ISiteRepository, SiteRepository>();
            builder.Services.AddScoped<IAreaRepository, AreaRepository>();
            builder.Services.AddScoped<IErrorService, ErrorServices>();
            builder.Services.AddScoped<TabuAIPredictor>();
            builder.Services.AddSingleton<MLContext>();
            builder.Services.AddScoped<MesMicroservice.Algorithm.BruteForces.BruteForce>();
            builder.Services.AddScoped<MesMicroservice.Algorithm.Metaheuristics.Improvement.GeneticAlgorithm.GAService>();
            // Algorithm & Background Services
            builder.Services.AddAlgorithmServices();
            builder.Services.AddScoped<CompletionTimePredictionsBuffer>();
            builder.Services.AddScoped<CompletionTimePredictor>();
            builder.Services.AddSingleton<DueDateBuffer>();
            builder.Services.AddHostedService<CompletionTimePredictorBackgroundService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập access token JWT. Ví dụ: eyJhbGciOi..."
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            builder.Services.AddHostedService<MesMicroservice.Api.BackgroundServices.AITabuModelRetrainService>();
            builder.Services.AddSingleton<SimulationManager>();
            builder.Services.AddSignalR();
            var app = builder.Build();

            // (ĐÃ XÓA ĐOẠN KHỞI TẠO VÀ TRAIN MODEL Ở ĐÂY ĐỂ SERVER CHẠY NHANH HƠN)

            app.UseCors("AllowAll");

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<NotificationHub>("/notificationHub");
            app.UseCors("ReactClient");
            app.MapHub<ProductionProgressHub>("/hubs/production-progress");
            app.Run();
        }
    }
}
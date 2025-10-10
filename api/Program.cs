using api.Data;
using System.IO;
using api.Model;
using api.Repository;
using api.Interface;
using api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Text.Json;
using api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using api.Interface.Repository;
using api.Interface.Services;
using Microsoft.Extensions.FileProviders;
using api.Helpers;
using CloudinaryDotNet;
using api.Service;
using api.Models;
using api.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        // Quét tất cả các class kế thừa AbstractValidator trong assembly
        fv.RegisterValidatorsFromAssemblyContaining<Program>();
        // Hoặc dùng AppDomain nếu validator nằm ở nhiều project khác nhau:
        // fv.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }); ;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.OperationFilter<FileUploadOperationFilter>();
    option.EnableAnnotations();
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Database
builder.Services.AddDbContext<PrnprojectContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// Configure Identity
//builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequiredLength = 12;
//})
//.AddEntityFrameworkStores<ApplicationDBContext>()
//.AddDefaultTokenProviders();

// Configure Authentication (JWT, Google, GitHub)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = "150017192186-1q7h12g42v1mvuumtubo5nf8smna9l38.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true;
    options.Scope.Add("email");
})
.AddGitHub(GitHubAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
    options.CallbackPath = "/signin-github";
    options.Scope.Add("user:email");
    options.SaveTokens = true;
})
.AddCookie();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAuthorization();
// Add Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

var cloudinarySettings = builder.Configuration.GetSection("Cloudinary");
var cloudinaryAccount = new Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);

builder.Services.AddSingleton(cloudinary);
// Dependency Injection
//builder.Services.AddScoped<ITokenService, TokenService>();
//builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
//builder.Services.AddScoped<IProblemRepository, ProblemRepository>();
//builder.Services.AddScoped<IProblemService, ProblemService>();
//builder.Services.AddScoped<IProblemHomePageRepository, ProblemHomePageRepository>();
//builder.Services.AddScoped<IProblemHomePageServices, ProblemHomePageServices>();
//builder.Services.AddScoped<IContestService, ContestService>();
//builder.Services.AddScoped<IContestRepository, ContestRepository>();
//builder.Services.AddScoped<IContestRegistationRepository, ContestRegistationRepository>();
//builder.Services.AddScoped<IContestRegistationService, ContestRegistationService>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserAdminRepository, UserAdminRepository>();
//builder.Services.AddScoped<IProgramingLanguageService, ProgramingLanguageService>();
//builder.Services.AddScoped<IProgramingLanguageRepository, ProgramingLanguageRepository>();
//builder.Services.AddScoped<IBlogRepository, BlogRepository>();
//builder.Services.AddScoped<IProblemAdminRepository, ProblemAdminRepository>();
//builder.Services.AddScoped<IProblemAdminService, ProblemAdminService>();
//builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
//builder.Services.AddScoped<ISubmissionService, SubmissionService>();
//builder.Services.AddScoped<IProblemManagementRepository, ProblemManagementRepository>();
//builder.Services.AddScoped<IProblemManagementService, ProblemManagementService>();
//builder.Services.AddScoped<ISubmissionsAdminRepository, SubmissionsAdminRepository>();
//builder.Services.AddScoped<ITestCaseRepository, TestCaseRepository>();
//builder.Services.AddScoped<ITestCaseService, TestCaseService>();
//builder.Services.AddScoped<OtpService>();
//builder.Services.AddScoped<IBlogLikeRepository, BlogLikeRepository>();
//builder.Services.AddScoped<IBlogLikeService, BlogLikeService>();
//builder.Services.AddScoped<IBlogShareRepository, BlogShareRepository>();
//builder.Services.AddScoped<IBlogShareService, BlogShareService>();
//builder.Services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
//builder.Services.AddScoped<IBlogCommentService, BlogCommentService>();
//builder.Services.AddScoped<IBlogBookmarkRepository, BlogBookmarkRepository>();
//builder.Services.AddScoped<IBlogBookmarkService, BlogBookmarkService>();
//builder.Services.AddScoped<IBlogForbiddenWordRepository, BlogForbiddenWordRepository>();
//builder.Services.AddScoped<IBlogForbiddenWordService, BlogForbiddenWordService>();
//builder.Services.AddScoped<IRankingRepository, RankingRepository>();
//builder.Services.AddScoped<IRankingService, RankingService>();
//builder.Services.AddScoped<ITestCaseStatusRepository, TestCaseStatusRepository>();
//builder.Services.AddScoped<ITestCaseStatusService, TestCaseStatusService>();
builder.Services.AddScoped<api.Service.IActionService, api.Service.ActionService>();
builder.Services.AddScoped<api.Repositories.IActionRepository, api.Repositories.ActionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Logging
builder.Logging.AddConsole();


var app = builder.Build();

// Tạo thư mục lưu trữ avatar
var avatarDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
if (!Directory.Exists(avatarDir))
{
    Directory.CreateDirectory(avatarDir);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseRouting();
app.MapControllers();
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
app.UseCors("AllowReactApp");
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.Run();

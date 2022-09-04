using System.Globalization;
using System.Net.Mime;
using AutoMapper;
using Azure;
using CourseProject.Common.Interfaces;
using CourseProject.Common.Mapping;
using CourseProject.Dal;
using CourseProject.Dal.Entities;
using CourseProject.Domain.Abstractions;
using CourseProject.Service;
using CourseProject.Web.Hubs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationContextConnection") ??
                       throw new InvalidOperationException(
                           "Connection string 'ApplicationContextConnection' not found.");

builder.Services.AddDbContext<ApplicationContext>(
    options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("CourseProject.Dal")));

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthentication().AddCookie().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["GoogleOauth:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["GoogleOauth:ClientSecret"];
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();
builder.Services.Configure<RequestLocalizationOptions>(
    opt =>
    {
        var supportedCultures = new List<CultureInfo>
        {
            new("en"),
            new("pl")
        };
        opt.DefaultRequestCulture = new RequestCulture("en");
        opt.SupportedCultures = supportedCultures;
        opt.SupportedUICultures = supportedCultures;
    }
);

builder.Services.Configure<SecurityStampValidatorOptions>(options => { options.ValidationInterval = TimeSpan.Zero; });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<ISearchService,SearchService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new UserProfile());
    cfg.AddProfile(new ItemProfile(provider.GetService<ICurrentUserService>()));
    cfg.AddProfile(new CollectionProfile(provider.GetService<ICurrentUserService>()));
    cfg.AddProfile(new TagProfile());
    cfg.AddProfile(new CommentProfile(provider.GetService<ICurrentUserService>()));
    cfg.AddProfile(new TopicProfile());
}).CreateMapper());

builder.Services.AddAzureClients(a =>
{
    a.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]);
    a.AddSearchClient(new Uri(builder.Configuration["AzureSearch:Endpoint"]),builder.Configuration["AzureSearch:IndexName"],new AzureKeyCredential(builder.Configuration["AzureSearch:ApiKey"]));
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(((IApplicationBuilder) app).ApplicationServices
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapHub<CommentsHub>("/ItemComments");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "404-PageNotFound",
    pattern: "{*url}",
    defaults: new {controller = "Error", action = "PageNotFound"});

app.Run();
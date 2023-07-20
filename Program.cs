using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GenericControllerLib.Models;
using GenericControllerLib;
using Microsoft.OpenApi.Models;
using GenericControllerLib.BusinessLogic;
using GenericControllerLib.Config;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Contexts
#if DEV
builder.Services
	.AddDbContext<EntitiesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DEV_EntitiesConnection")));
#endif
#if PRE
builder.Services
	.AddDbContext<EntitiesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PRE_EntitiesConnection")));
#endif
#if PROD
builder.Services
	.AddDbContext<EntitiesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PROD_EntitiesConnection")));
#endif
// Identity
builder.Services
	.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
	.AddRoles<Role>()
	.AddEntityFrameworkStores<EntitiesDbContext>()
	.AddDefaultUI();

// Services
builder.Services
	.AddScoped(typeof(BusinessLogic<>))
	.AddScoped(typeof(UserBL));

builder.Services
	.AddMvc(i => i.Conventions.Add(new GenericControllerRouteConvention()))
	.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider()));

// Adding Authentication with Jwt Bearer
builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.SaveToken = true;
		options.RequireHttpsMetadata = false;
		options.TokenValidationParameters = new TokenValidationParameters()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidAudience = builder.Configuration["JWT:ValidAudience"],
			ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
		};
	});

// Adding Authorization
builder.Services
	.AddAuthorization();

// Logging settings
builder.Services
	.Configure<IdentityOptions>(options =>
	{
		// Password settings.
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequireUppercase = true;
		options.Password.RequiredLength = 6;
		options.Password.RequiredUniqueChars = 1;

		// Lockout settings.
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
		options.Lockout.MaxFailedAccessAttempts = 5;
		options.Lockout.AllowedForNewUsers = true;

		// User settings.
		options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
		options.User.RequireUniqueEmail = false;
	});

// Add services to the container.
builder.Services
	.AddControllersWithViews()
	.AddRazorPagesOptions(options =>
	{
		// options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
	});

builder.Services.AddEndpointsApiExplorer();

// Swagger settings
builder.Services.AddSwaggerGen(swagger =>
{

	//swagger.OrderActionsBy((apiDesc) => { 
	//    return $"{apiDesc.ActionDescriptor.RouteValues["controller"]}"; 
	//});

	swagger.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Generic API",
		Description = "Operaciones CRUD para manejo del sistema.",
		TermsOfService = new Uri("https://example.com/terms"),
		Contact = new OpenApiContact
		{
			Name = "Desarrollador - Marcos Bustamante Mateo",
			Email = "marcosbustamantemateo@gmail.com",
			Url = new Uri("http://www.marcosbustamanteomateo.com")
		},
		License = new OpenApiLicense
		{
			Name = "License",
			Url = new Uri("https://example.com/license")
		},
	});

	swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT Authorization header"
	});

	swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

	// Set the comments path for the Swagger JSON and UI.
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	swagger.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// Para que devuelva los códigos de error
app.UseStatusCodePagesWithRedirects("/Error/Http?statusCode={0}");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

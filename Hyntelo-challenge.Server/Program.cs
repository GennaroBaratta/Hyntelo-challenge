using Hyntelo_challenge.Server.DB;
using Hyntelo_challenge.Server.Models;
using Hyntelo_challenge.Server.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Add Swagger with Bearer Auth
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configure Swagger to use the Bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid token.",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseInMemoryDatabase("BlogDb"));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    if (!context.Posts.Any())
    {
        context.Users.Add(new User { Id = 1, Username = "admin", Password = "password", Name = "Administrator" });

        var posts = new List<Post>();
        for (int i = 1; i <= 50; i++)
        {
            posts.Add(new Post
            {
                Id = i,
                UserId = 1,
                Title = $"Post {i}",
                Body = $"This is the content of post number {i}. Lorem ipsum odor amet, consectetuer adipiscing elit. Condimentum quis commodo dapibus fringilla torquent. Neque class elit nunc justo potenti velit dictum. Dolor justo taciti per porta integer tristique placerat imperdiet. Iaculis vivamus primis taciti urna, morbi laoreet sagittis. Laoreet sapien taciti montes eget vivamus aenean. Accumsan et quam enim ornare; quisque placerat magnis.\r\n\r\nDolor justo himenaeos ut nunc laoreet sem. Sem interdum erat efficitur lectus congue ornare malesuada. Condimentum proin molestie nullam aptent fusce luctus dis tristique. Eu eleifend metus non et laoreet lacus velit luctus. Rutrum pulvinar ad lobortis dictum non primis tempor porta maecenas. Amet natoque facilisis curabitur ex bibendum consectetur ornare a suspendisse. Phasellus per finibus diam metus pulvinar massa lectus mus lorem. Aptent ex porta praesent cubilia justo commodo molestie id.\r\n\r\nAd ornare phasellus congue adipiscing sit morbi massa. Suspendisse scelerisque ultricies senectus nunc dolor ullamcorper morbi. Per sem iaculis venenatis nascetur, dictum blandit himenaeos. Himenaeos justo himenaeos ornare cubilia odio. Nunc interdum posuere dis quisque ornare, imperdiet felis curabitur lorem. Hac ornare aliquet sem ligula ipsum a elementum. Sed platea duis elit tempor, ante vel habitasse senectus venenatis. Finibus curabitur pharetra natoque scelerisque ornare tristique porta dis. Hac potenti quisque urna nibh integer quisque tempor.\r\n\r\nHendrerit hendrerit himenaeos lectus massa porttitor. Nec cubilia pretium commodo vel; mattis mollis montes. Dictumst habitant blandit pretium potenti rutrum class accumsan! Curae bibendum rutrum imperdiet sollicitudin semper per nam diam. Maximus rutrum gravida etiam luctus euismod interdum nullam neque nisl. Sit dui posuere eros, vulputate sodales mauris. Montes placerat pretium primis feugiat himenaeos proin maximus iaculis vehicula. Viverra sapien molestie in gravida aptent placerat sociosqu. Porttitor turpis habitant phasellus torquent leo dapibus."
            });
        }

        context.Posts.AddRange(posts);
        context.SaveChanges();
    }
}


app.Run();

using TrainingApp.Server.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TrainingApp.Server.Interfaces;
using TrainingApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITrainingSession, TrainingSessionService>();
builder.Services.AddScoped<ITrainer, TrainerService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();



builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientApp", policy =>
    {
        policy.WithOrigins("https://localhost:7157", "http://localhost:5004", "https://localhost:5004")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    AppDbSeed.Seed(dbContext);
}

app.UseDeveloperExceptionPage();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowClientApp");
app.UseAuthorization();

app.MapControllers(); // 👈 OVO OBAVEZNO PRE fallback-a
app.MapRazorPages();
app.MapFallbackToFile("index.html"); // 👈 fallback NA KRAJU


app.Run();
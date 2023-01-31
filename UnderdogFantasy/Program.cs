using UnderdogFantasy.Database;
using UnderdogFantasy.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(x => x.LowercaseUrls = true);
builder.Services.AddDbContext<UnderdogFantasyContext>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Ensure database has been created
using (var context = new UnderdogFantasyContext())
{
    context.Database.EnsureCreated();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
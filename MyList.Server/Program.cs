using Microsoft.EntityFrameworkCore;
using MyList.Server.Data;
using MyList.Server.Data.Repositories;
using MyList.Server.Services;
using System.Text.Json.Serialization;

namespace MyList.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            var connectionString = builder.Configuration.GetConnectionString("MyList");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

            builder.Services.AddScoped<IListRepository, ListRepository>();
            builder.Services.AddScoped<IListService, ListService>();
            builder.Services.AddScoped<IItemRepository, ItemRepository>();
            builder.Services.AddScoped<IItemService, ItemService>();

            var app = builder.Build();

            app.UsePathBase("/mylist-api");

            app.UseCors(options =>
                options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

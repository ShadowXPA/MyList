using Microsoft.EntityFrameworkCore;
using MyList.Server.Data;

namespace MyList.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var connectionString = builder.Configuration.GetConnectionString("MyList");

            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

//using BookWeb.Models;
using Microsoft.EntityFrameworkCore;
using static TagAPI.Data.SOAPI;

namespace TagAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {

        }
        public virtual DbSet<TagSQL> Tags { get; set; }
    }
}

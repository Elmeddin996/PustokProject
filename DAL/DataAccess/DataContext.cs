using Microsoft.EntityFrameworkCore;
using PustokProject.Models;

namespace PustokProject.DAL.DataAccess
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }


        public DbSet<Tag>Tags { get; set; } 
        public DbSet<Genre>Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book>Books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Slider> Sliders { get; set; }

    }
}

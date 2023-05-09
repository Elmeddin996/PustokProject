using PustokProject.DAL.DataAccess;
using PustokProject.Models;

namespace PustokProject.Services
{
    public class LayoutServices
    {
        private readonly DataContext _context;

        public LayoutServices(DataContext context)
        {
            _context = context;
        }
        public Dictionary<string,string> GetSettings()
        {
         
            return _context.Settings.ToDictionary(x=>x.Key,x=>x.Value);
        }

        public List<Genre> GetGenres() 
        {
            return _context.Genres.ToList();
        }
    }
}

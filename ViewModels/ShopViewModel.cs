using PustokProject.Models;

namespace PustokProject.ViewModels
{
    public class ShopViewModel
    {
        public List<Genre> Genres { get; set; }
        public List<Author> Authors { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Book> Books { get; set; }
        public PaginatedList<Book> paginatedList { get; set; }

    }
}

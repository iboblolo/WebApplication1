using WebApplication1.DbModels;
namespace WebApplication1.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products = new List<Product>();
    }
}

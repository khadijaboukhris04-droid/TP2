namespace TP2.Models.Help
{
    public class Item
    {
        public int quantite { get; set; }

        public Product _product = null;
        public Product Prod
        {
            get { return _product; }
            set { _product = value; }
        }

        public string Description => _product.Name;
        public float UnitPrice => _product.Price;
        public int categoryId => _product.CategoryId;
        public Category category => _product.Category;
        public float TotalPrice => _product.Price * quantite;

        public Item(Product p)
        {
            this.Prod = p;
        }

        public bool Equals(Item item)
        {
            return item.Prod.ProductId == this.Prod.ProductId;
        }
    }
}
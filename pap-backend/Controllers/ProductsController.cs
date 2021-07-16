using Microsoft.AspNetCore.Mvc;
using pap_backend.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace pap_backend.Controllers
{
    [Produces("application/json")]
    [Route("[Controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        [HttpGet]
        public List<Product> GetAllProducts()
        {
            List<Product> productList = new List<Product>();
            using (SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=pap; Integrated Security=true"))
            {

                String sql = "SELECT idProd, nameProd, price, resourceImg FROM products";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            String ProductName = Reader.GetString("nameProd");
                            int IdProduct = Reader.GetInt32("idProd");
                            decimal Price = Reader.GetDecimal("price");
                            String ResourceImg = Reader.GetString("resourceImg");

                            Product product = new Product();
                            product.nameProd = ProductName;
                            product.idProd = IdProduct;
                            product.price = Price;
                            product.resourceImg = ResourceImg;

                            productList.Add(product);
                        }
                    }
                    connection.Close();
                }
            }
            return productList;

        }
        [HttpGet("bestSellers")]
        public List<Product> GetBestSellers()
        {
            List<Product> productList = new List<Product>();
            using (SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=pap; Integrated Security=true"))
            {

                String sql = "SELECT  idProd, resourceImg FROM products WHERE idprod >=2 and idprod <=5";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            int IdProduct = Reader.GetInt32("idProd");
                            String ResourceImg = Reader.GetString("resourceImg");

                            Product product = new Product();
                            product.idProd = IdProduct;

                            product.resourceImg = ResourceImg;

                            productList.Add(product);
                        }
                    }
                    connection.Close();
                }
            }
            return productList;

        }
       
        [HttpGet("newReleases")]
        public List<Product> GetRandomProducts()
        {
            List<Product> productList = new List<Product>();
            using (SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=pap; Integrated Security=true"))
            {

                String sql = "SELECT idProd,  resourceImg FROM products WHERE idProd>=3 and idprod <=6";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            int IdProduct = Reader.GetInt32("idProd");
                            String ResourceImg = Reader.GetString("resourceImg");

                            Product product = new Product();
                            product.idProd = IdProduct;
                            product.resourceImg = ResourceImg;

                            productList.Add(product);
                        }
                    }
                    connection.Close();
                }
            }
            return productList;

        }

        [HttpGet("{productId}")]
        public Product GetProduct(int productId)
        {
            Product product = new Product();

            using (SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=pap; Integrated Security=true"))
            {

                String productDetails = "SELECT  idProd, nameProd, price, resourceImg FROM products WHERE idProd = @idProd";

                using (SqlCommand command = new SqlCommand(productDetails, connection))
                {
                    SqlParameter Param = new SqlParameter("idProd", productId);
                    command.Parameters.Add(Param);

                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            String ProductName = Reader.GetString("nameProd");
                            int IdProduct = Reader.GetInt32("idProd");
                            decimal Price = Reader.GetDecimal("price");
                            String ResourceImg = Reader.GetString("resourceImg");

                            product = new Product();
                            product.nameProd = ProductName;
                            product.idProd = IdProduct;
                            product.price = Price;
                            product.resourceImg = ResourceImg;


                        }
                    }
                    connection.Close();
                }


                String productSizes =
                    "SELECT size FROM product_size" +
                    " join sizes on product_size.idSize=sizes.idSize" +
                    " WHERE idProd = @idProd";

                using (SqlCommand command = new SqlCommand(productSizes, connection))
                {
                    SqlParameter Param = new SqlParameter("idProd", productId);
                    command.Parameters.Add(Param);
                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            decimal size = Reader.GetDecimal(0);
                            product.availableSizes.Add(size);
                        }
                    }
                    connection.Close();
                }
            }
            return product;
        }

        [HttpPost("sendOrder")]
        public void Post([FromBody] ProductOrder order)
        {
            int idOrder = 0;
            using (SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=pap; Integrated Security=true"))
            {

                String sql = "INSERT INTO orders output inserted.idOrder values (@emailUser, @totalPrice)";

                using (SqlCommand insertOrderCommand = new SqlCommand(sql, connection))
                {
                    insertOrderCommand.Parameters.Add(new SqlParameter("emailUser", order.UserEmail));
                    insertOrderCommand.Parameters.Add(new SqlParameter("totalPrice", order.Products.Sum(p => p.price)));

                    connection.Open();
                    using (SqlDataReader Reader = insertOrderCommand.ExecuteReader())
                    {

                        if (Reader.Read())
                        {
                            idOrder = Reader.GetInt32(0);

                        }
                    }

                    connection.Close();

                }
                sql = "INSERT INTO ordered_product values (@idOrder, @idProd)";
                foreach (Product product in order.Products)
                {
                    using (SqlCommand insertProductsCommand = new SqlCommand(sql, connection))
                    {

                        insertProductsCommand.Parameters.Add(new SqlParameter("idOrder", idOrder));
                        insertProductsCommand.Parameters.Add(new SqlParameter("idProd", product.idProd));
                        connection.Open();
                        insertProductsCommand.ExecuteNonQuery();
                        connection.Close();
                    }

                }
            }
        }
    }
}

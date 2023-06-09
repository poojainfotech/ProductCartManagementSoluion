using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProductCartManagement.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProductCartManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid) return BadRequest("Enter required fields");
            
            product.ProductCreationDate = string.Format("{0:yyyyMMdd}", System.DateTime.Now);
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into products(ProductName,ProductDiscription,ProductCreationDate,ProductPrice) values(@ProductName, @ProductDiscription, @ProductCreationDate, @ProductPrice)", product);
            return Ok(await SelectAllProduct(connection));
        }


        private static async Task<IEnumerable<Product>> SelectAllProduct(SqlConnection connection)
        {
            return await connection.QueryAsync<Product>("select * from products");
        }

        private static async Task<IEnumerable<Product>> SelectSingleProduct(SqlConnection connection, int id)
        {
            return await connection.QueryAsync<Product>("select * from products where id=@Id", id);
        }

        public IActionResult EditProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<List<Product>>> EditProduct(Product product)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update products set ProductName = @ProductName, ProductDiscription=@ProductDiscription, ProductPrice=@ProductPrice where id=@Id", product);
            return Ok(await SelectAllProduct(connection));
        }

        [HttpPost]
        public async Task<ActionResult<Product>> GetProductDetails(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var product = await connection.QueryFirstAsync<Product>("select * from products where id = @Id",
                new { Id = id });
            return Ok(product);
        }

        [HttpPost()]
        public async Task<ActionResult<List<Product>>> DeleteProduct(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from products where id=@Id", new { Id = id });
            return Ok(await SelectAllProduct(connection));
        }
        
        public IActionResult ShowDetails()
        {
            return View();
        }
    }
}

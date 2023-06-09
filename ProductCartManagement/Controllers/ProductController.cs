using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductCartManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductCartManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet()]
        public async Task<ActionResult<Product>> Get()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));            
            var product = await connection.QueryAsync<Product>("select * from products");
            return Ok(product); 
        }
    }
}

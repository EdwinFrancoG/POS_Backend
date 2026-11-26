using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var listProducts = _productService.GetProducts();
            return Ok(listProducts);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _productService.ObtenerporId(id);
            return Ok(product);
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            var response = await _productService.CrearAsync(product);
            return Ok(response);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            var response = await _productService.ActaulizarAsync(product);
            return Ok(response);
        }

        // PATCH: api/Products/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, bool isActive)
        {
            var response = await _productService.CambiarEstadoAsync(id, isActive);
            return Ok(response);
        }
    }
}

using BasicWebApplicationCsharp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApplicationCsharp.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
            => _productService = productService;

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _productService.GetAll();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public IActionResult Create([FromBody] ProductDto dto)
        {
            var product = _productService.Create(dto.Name, dto.Description, dto.Sku, dto.Price, dto.Stock);
            return Ok(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProductDto dto)
        {
            var product = _productService.Update(id, dto.Name, dto.Description, dto.Sku, dto.Price, dto.Stock);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var deleted = _productService.Delete(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        public class ProductDto
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string Sku { get; set; } = "";
            public decimal Price { get; set; }
            public int Stock { get; set; }
        }
    }
}

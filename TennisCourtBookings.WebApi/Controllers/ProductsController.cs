using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisCourtBookings.Application.Repositories;

namespace TennisCourtBookings.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var productDetails = await _service.GetByIdAsync(id);
            return Ok(productDetails);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var productDetails = await _service.GetAllAsync();
            return Ok(productDetails);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateProductRequest request)
        {
            return Ok(await _service.CreateAsync(request.Name, request.Description, request.Rate));
        }
    }
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
    }
}

using API.Dtos;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _genericRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IGenericRepository<Product> genericRepository,
            IMapper mapper,
            ILogger<ProductsController> logger )
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [Cached(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery]ProductSpecParams productParams)
        {
            _logger.LogInformation("Fetching 3D products");
            var spec = new ProductsWithImagesSpecification(productParams);
            var countSpec = new ProductWithFiltersForCountSpecification(productParams);
            var totalItems = await _genericRepository.CountAsync(countSpec);
            var products = await _genericRepository.ListAsync(spec);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex , productParams.PageSize, totalItems, data));
        }

        [Cached(600)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithImagesSpecification(id);
            var product =await _genericRepository.GetEntityWithSpec(spec);
            return _mapper.Map<Product,ProductToReturnDto>(product);
        }

        [Cached(600)]
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetCategories()
        {
            var products = await _genericRepository.ListAllAsync();
            var categories = products.Select(x => x.Category).Distinct().ToList();
            
            return Ok(categories);

        }

        [Cached(600)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetBrands()
        {
            var products = await _genericRepository.ListAllAsync();
            var brands = products.Select(x => x.Brand).Distinct().ToList();

            return Ok(brands);

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductCreateDto productDto)
        {
            // 1. Map DTO to Entity (Manually or via AutoMapper)
            var product = new Product
            {
                Title = productDto.Title,
                Description = productDto.Description,
                Price = productDto.Price,
                Thumbnail = productDto.Thumbnail,
                Stock = productDto.Stock,
                Category = productDto.Category,
                Brand = productDto.Brand,
                IsActive = true,         // Default to Active
                DiscountPercentage = 0,  // Default
                Rating = 0               // Default
            };

            // 2. Add to DB
            _genericRepository.Add(product);
            // 3. Clear Cache (Important!)
            //await _cacheService.RemoveCacheAsync("all_products_list");

            // 4. Return the created product
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            // 1. Find the product
            var spec = new ProductsWithImagesSpecification(id);
            var product = await _genericRepository.GetEntityWithSpec(spec);

            if (product == null)
                return NotFound();

            // 2. SOFT DELETE Logic (Instead of removing it)
            product.IsActive = false;

            // 3. Update DB
            // EF Core tracks the change, so we just save.
            // If using a Repo pattern: _repo.Update(product);
            _genericRepository.Update(product);

            // 4. Clear Cache so the list updates immediately
            //await _cacheService.RemoveCacheAsync("all_products_list");
            //await _cacheService.RemoveCacheAsync($"product_{id}");

            return NoContent(); // Standard 204 response
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, ProductCreateDto productDto)
        {
            // 1. Find the product
            var spec = new ProductsWithImagesSpecification(id);
            var product = await _genericRepository.GetEntityWithSpec(spec);

            if (product == null)
                return NotFound();

            // 2. Update properties on the EXISTING tracked entity
            // DO NOT use 'new Product { ... }'
            product.Title = productDto.Title;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Thumbnail = productDto.Thumbnail;
            product.Stock = productDto.Stock;
            product.Category = productDto.Category;
            product.Brand = productDto.Brand;

            // 3. Update DB
            // EF Core tracks the change, so we just save.
            // If using a Repo pattern: _repo.Update(product);
            _genericRepository.Update(product);

            // 4. Clear Cache so the list updates immediately
            //await _cacheService.RemoveCacheAsync("all_products_list");
            //await _cacheService.RemoveCacheAsync($"product_{id}");

            return NoContent(); // Standard 204 response
        }


    }
}

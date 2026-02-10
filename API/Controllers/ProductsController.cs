using API.Dtos;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
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


    }
}

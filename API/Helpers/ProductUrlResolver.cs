using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        private readonly IConfiguration _config;
        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }

        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Thumbnail))
            {
                // IF it is already a full URL (PostImg, Cloudinary, etc.), just return it
                if (source.Thumbnail.StartsWith("http"))
                {
                    return source.Thumbnail;
                }
                return _config["ApiUrl"] + source.Thumbnail;
            }
            return null;
        }
    }
}

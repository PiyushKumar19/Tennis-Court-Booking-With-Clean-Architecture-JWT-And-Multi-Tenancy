using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TennisCourtBookings.Application.Repositories;
using TennisCourtBookings.Application;
using TennisCourtBookings.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Autofac.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TennisCourtBookings.Persistence.Repositories
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            httpContextAccessor = httpContextAccessor;
        }
        public async Task<Product> CreateAsync(string name, string description, int rate)
        {
            var product = new Product(name, description, rate);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetAllProductDetailsAsync( string tenantId)
        {
            var productDetails = await _context.Products
                            .Where(p => p.TenantId == tenantId)
                            .ToListAsync();

            return productDetails;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> GetProductDetailsAsync(int productId, string tenantId)
        {
            var productDetails = await _context.Products
                .Where(p => p.Id == productId && p.TenantId == tenantId)
                .FirstOrDefaultAsync();

            return productDetails;
        }
    }
}

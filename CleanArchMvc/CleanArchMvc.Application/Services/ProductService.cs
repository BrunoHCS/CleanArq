using AutoMapper;
using CleanArchMvc.Application.DTOs;
using CleanArchMvc.Application.Intefaces;
using CleanArchMvc.Domain.Entities;
using CleanArchMvc.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchMvc.Application.Services
{
    public class ProductService : IProductService
    {
        private IProductRepository _productyRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productyRepository = productRepository ??
                throw new ArgumentNullException(nameof(productRepository));

            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var productEntity = await _productyRepository.GetProductAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(productEntity);
        }

        public async Task<ProductDTO> GetById(int? id)
        {
            var productEntity = await _productyRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task<ProductDTO> GetProductCategory(int? id)
        {
            var productEntity = await _productyRepository.GetProductCategoryAsync(id);
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task Add(ProductDTO productDTO)
        {
            var productEntity = _mapper.Map<Product>(productDTO);
            await _productyRepository.Create(productEntity);
        }

        public async Task Update(ProductDTO productDTO)
        {
            var productEntity = _mapper.Map<Product>(productDTO);
            await _productyRepository.Update(productEntity);
        }

        public async Task Remove(int? id)
        {
            var productEntity = _productyRepository.GetByIdAsync(id).Result;
            await _productyRepository.Remove(productEntity);
        }
    }
}

using AutoMapper;
using CleanArchMvc.Application.DTOs;
using CleanArchMvc.Application.Intefaces;
using CleanArchMvc.Application.Products.Commands;
using CleanArchMvc.Application.Products.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchMvc.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ProductService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var productsQuery = new GetProductsQuery();

            if (productsQuery == null)
            {
                throw new Exception($"Entity could not be loaded.");
            }

            var result = await _mediator.Send(productsQuery);
            return _mapper.Map<IEnumerable<ProductDTO>>(result);
        }

        public async Task<ProductDTO> GetById(int? id)
        {
            var productsByIdQuery = new GetProductByIdQuery(id.Value);

            if (productsByIdQuery == null)
            {
                throw new Exception($"Entity could not be loaded.");
            }

            var result = await _mediator.Send(productsByIdQuery);
            return _mapper.Map<ProductDTO>(result);
        }

        //public async Task<ProductDTO> GetProductCategory(int? id)
        //{
        //    var productsByIdQuery = new GetProductByIdQuery(id.Value);

        //    if (productsByIdQuery == null)
        //    {
        //        throw new Exception($"Entity could not be loaded.");
        //    }

        //    var result = await _mediator.Send(productsByIdQuery);
        //    return _mapper.Map<ProductDTO>(result);
        //}

        public async Task Add(ProductDTO productDTO)
        {
            var productsCreateCommand = _mapper.Map<ProductCreateCommand>(productDTO);
            await _mediator.Send(productsCreateCommand);
        }

        public async Task Update(ProductDTO productDTO)
        {
            var productsUpdateCommand = _mapper.Map<ProductUpdateCommand>(productDTO);
            await _mediator.Send(productsUpdateCommand);
        }

        public async Task Remove(int? id)
        {
            var productsRemoveCommand = new ProductRemoveCommand(id.Value);

            if (productsRemoveCommand == null)
            {
                throw new Exception($"Entity could not be loaded.");
            }

            await _mediator.Send(productsRemoveCommand);
        }
    }
}

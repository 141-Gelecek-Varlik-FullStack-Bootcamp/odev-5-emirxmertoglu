﻿using AutoMapper;
using Broot.DB.Entities.DataContext;
using Broot.Model;
using Broot.Model.ProductModel;
using System.Collections.Generic;
using System.Linq;

namespace Broot.Service.Product
{
    public class ProductService : IProductService
    {
        private readonly IMapper mapper;

        public ProductService(IMapper _mapper)
        {
            mapper = _mapper;
        }

        // Get product by id
        public General<ProductDetail> GetById(int id)
        {
            var result = new General<ProductDetail>() { IsSuccess = false };
            using (var srv = new BrootContext())
            {
                var product = srv.Product.SingleOrDefault(p => p.Id == id);
                // Checking the product is exists?
                if (product is null)
                {
                    result.ExceptionMessage = "Verilen id numarasiyla iliskili bir urun bulunamadi.";
                    return result;
                }

                else
                {
                    result.Entity = mapper.Map<ProductDetail>(product);
                    result.IsSuccess = true;
                }
            }
            return result;
        }

        // Insert product
        public General<ProductDetail> Insert(InsertProductModel newProduct)
        {
            var result = new General<ProductDetail>() { IsSuccess = false };
            try
            {
                var model = mapper.Map<Broot.DB.Entities.Product>(newProduct);
                using (var srv = new BrootContext())
                {
                    model.Idate = System.DateTime.Now;
                    model.IsActive = true;
                    model.Iuser = 2;
                    model.Name = model.DisplayName;
                    srv.Product.Add(model);
                    srv.SaveChanges();
                    result.Entity = mapper.Map<ProductDetail>(model);
                    result.IsSuccess = true;
                }
            }
            catch (System.Exception)
            {
                result.ExceptionMessage = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin!";
            }

            
            return result;
        }

        // List products
        public General<ListProductModel> List()
        {
            var result = new General<ListProductModel>() { IsSuccess = false };
            using (var srv = new BrootContext())
            {
                // Get products (which products the active state is true & order products by id)
                var products = srv.Product.Where(p => p.IsActive && !p.IsDeleted).OrderBy(p => p.Id);

                if (products is null)
                {
                    result.ExceptionMessage = "Urunlerin verileri cekilemedi!";
                    return result;
                }

                // Mapping products
                result.List = mapper.Map<List<ListProductModel>>(products);
                result.TotalCount = products.Count();
                result.IsSuccess = true;
            }

            return result;
        }
    }
}

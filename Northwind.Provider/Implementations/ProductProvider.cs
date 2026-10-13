using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Product;

namespace Northwind.Provider.Implementations
{
    public class ProductProvider : IProductProvider
    {
        private readonly IProductRepository _productRepository;

        public ProductProvider(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<List<ProductResponseDto>>> GetAllProductsAsync(int? categoryId, int? supplierId, bool? discontinued, bool? readyToReorder)
        {
            var products = await _productRepository.GetAllAsync(categoryId, supplierId, discontinued, readyToReorder);
            var response = products.Select(MapToResponseDto).ToList();
            return ApiResponse<List<ProductResponseDto>>.SuccessResult(response);
        }

        public async Task<ApiResponse<PagedResultDto<ProductResponseDto>>> GetPagedProductsAsync(ProductFilterDto filter)
        {
            var (items, totalCount) = await _productRepository.GetPagedAsync(filter);

            var dtos = items.Select(MapToResponseDto).ToList();

            var result = new PagedResultDto<ProductResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<ProductResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<ProductResponseDto>.FailureResult("Produk tidak ditemukan.");

            return ApiResponse<ProductResponseDto>.SuccessResult(MapToResponseDto(product));
        }

        public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto request)
        {
            var product = new Product
            {
                ProductName = request.ProductName,
                SupplierId = request.SupplierId,
                CategoryId = request.CategoryId,
                QuantityPerUnit = request.QuantityPerUnit,
                UnitPrice = request.UnitPrice,
                UnitsInStock = request.UnitsInStock ?? 0,
                UnitsOnOrder = request.UnitsOnOrder ?? 0,
                ReorderLevel = request.ReorderLevel ?? 0,
                Discontinued = request.Discontinued
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            var created = await _productRepository.GetByIdAsync(product.ProductId);
            return ApiResponse<ProductResponseDto>.SuccessResult(MapToResponseDto(created!), "Produk berhasil ditambahkan.");
        }

        public async Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(UpdateProductDto request)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                return ApiResponse<ProductResponseDto>.FailureResult("Produk tidak ditemukan.");

            product.ProductName = request.ProductName;
            product.SupplierId = request.SupplierId;
            product.CategoryId = request.CategoryId;
            product.QuantityPerUnit = request.QuantityPerUnit;
            product.UnitPrice = request.UnitPrice;
            product.UnitsInStock = request.UnitsInStock;
            product.UnitsOnOrder = request.UnitsOnOrder;
            product.ReorderLevel = request.ReorderLevel;
            product.Discontinued = request.Discontinued;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            var updated = await _productRepository.GetByIdAsync(product.ProductId);
            return ApiResponse<ProductResponseDto>.SuccessResult(MapToResponseDto(updated!), "Produk berhasil diperbarui.");
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<bool>.FailureResult("Produk tidak ditemukan.");

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Produk berhasil dihapus.");
        }

        public async Task<ApiResponse<bool>> ToggleDiscontinueAsync(int id, bool discontinued)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<bool>.FailureResult("Produk tidak ditemukan.");

            product.Discontinued = discontinued;
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            string statusText = discontinued ? "discontinue" : "aktif";
            return ApiResponse<bool>.SuccessResult(true, $"Status produk berhasil diubah menjadi {statusText}.");
        }

        // Poin 2: Otomatisasi Reorder
        public async Task<ApiResponse<List<ProductResponseDto>>> ReorderProductAsync(ReorderProductDto request)
        {
            var allProducts = await _productRepository.GetAllForReorderAsync();
            var updatedProducts = new List<Product>();

            foreach (var p in allProducts)
            {
                int effectiveReorderLevel;

                if (request.IgnoreLevel)
                {
                    effectiveReorderLevel = request.StockMinimum;
                }
                else
                {
                    effectiveReorderLevel = (p.ReorderLevel ?? 0) == 0 ? request.StockMinimum : p.ReorderLevel!.Value;
                }

                int currentTotal = (p.UnitsInStock ?? 0) + (p.UnitsOnOrder ?? 0);

                // Pengecekan apakah produk memenuhi kriteria reorder (UnitsInStock + UnitsOnOrder <= ReorderLevel/StockMinimum)
                if (currentTotal <= effectiveReorderLevel)
                {
                    short newUnitsOnOrder = p.UnitsOnOrder ?? 0;

                    if (request.SetStock.HasValue)
                    {
                        newUnitsOnOrder = request.SetStock.Value;
                    }
                    else if (request.AddStock.HasValue)
                    {
                        newUnitsOnOrder = (short)(newUnitsOnOrder + request.AddStock.Value);
                    }

                    p.UnitsOnOrder = newUnitsOnOrder;
                    await _productRepository.UpdateAsync(p);
                    updatedProducts.Add(p);
                }
            }

            if (updatedProducts.Count > 0)
            {
                await _productRepository.SaveChangesAsync();
            }

            var resultDtos = updatedProducts.Select(MapToResponseDto).ToList();
            return ApiResponse<List<ProductResponseDto>>.SuccessResult(resultDtos, $"Proses reorder selesai. {resultDtos.Count} produk diperbarui.");
        }

        // Poin 3: Pindahkan UnitsOnOrder ke UnitsInStock
        public async Task<ApiResponse<List<ProductResponseDto>>> ReceiveStockFromOrderAsync()
        {
            var productsToUpdate = await _productRepository.GetProductsWithUnitsOnOrderAsync();

            foreach (var p in productsToUpdate)
            {
                p.UnitsInStock = (short)((p.UnitsInStock ?? 0) + (p.UnitsOnOrder ?? 0));
                p.UnitsOnOrder = 0;
                await _productRepository.UpdateAsync(p);
            }

            if (productsToUpdate.Count > 0)
            {
                await _productRepository.SaveChangesAsync();
            }

            var resultDtos = productsToUpdate.Select(MapToResponseDto).ToList();
            return ApiResponse<List<ProductResponseDto>>.SuccessResult(resultDtos, $"Berhasil memindahkan UnitsOnOrder ke UnitsInStock untuk {resultDtos.Count} produk.");
        }

        private static ProductResponseDto MapToResponseDto(Product p) => new()
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.CompanyName,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.CategoryName,
            QuantityPerUnit = p.QuantityPerUnit,
            UnitPrice = p.UnitPrice,
            UnitsInStock = p.UnitsInStock,
            UnitsOnOrder = p.UnitsOnOrder,
            ReorderLevel = p.ReorderLevel,
            Discontinued = p.Discontinued
        };
    }
}
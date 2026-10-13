using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.Order;

namespace Northwind.Provider.Implementations
{
    public class OrderProvider : IOrderProvider
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderProvider(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<List<OrderResponseDto>>> GetAllOrdersAsync(
            string? customerId, int? supplierId, int? employeeId, DateTime? startDate, DateTime? endDate,
            string? userRole, string? currentCustomerId, int? currentEmployeeId)
        {
            // Proteksi data berdasarkan Role
            if (string.Equals(userRole, "Customer", StringComparison.OrdinalIgnoreCase))
            {
                // Customer HANYA bisa lihat order miliknya
                customerId = currentCustomerId;
            }
            else if (string.Equals(userRole, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                // Employee HANYA bisa lihat order yang ditanganinya
                employeeId = currentEmployeeId;
            }

            var orders = await _orderRepository.GetAllAsync(customerId, supplierId, employeeId, startDate, endDate);
            var response = orders.Select(MapToResponseDto).ToList();
            return ApiResponse<List<OrderResponseDto>>.SuccessResult(response);
        }

        public async Task<ApiResponse<PagedResultDto<OrderResponseDto>>> GetPagedOrdersAsync(
    OrderFilterDto filter, string? userRole, string? currentCustomerId, int? currentEmployeeId)
        {
            // Proteksi data berdasarkan Role yang pernah kita buat sebelumnya
            if (string.Equals(userRole, "Customer", StringComparison.OrdinalIgnoreCase))
            {
                filter.CustomerId = currentCustomerId;
            }
            else if (string.Equals(userRole, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                filter.EmployeeId = currentEmployeeId;
            }

            var (items, totalCount) = await _orderRepository.GetPagedAsync(filter);
            var dtos = items.Select(MapToResponseDto).ToList();

            var result = new PagedResultDto<OrderResponseDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<OrderResponseDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(
            int id, string? userRole, string? currentCustomerId, int? currentEmployeeId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null)
                return ApiResponse<OrderDetailDto>.FailureResult("Order tidak ditemukan.");

            // Validasi kepemilikan data untuk detail order
            if (string.Equals(userRole, "Customer", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(order.CustomerId, currentCustomerId, StringComparison.OrdinalIgnoreCase))
                {
                    return ApiResponse<OrderDetailDto>.FailureResult("Anda tidak memiliki akses untuk melihat order ini.");
                }
            }
            else if (string.Equals(userRole, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                if (order.EmployeeId != currentEmployeeId)
                {
                    return ApiResponse<OrderDetailDto>.FailureResult("Anda tidak memiliki akses untuk melihat order ini.");
                }
            }

            var detail = new OrderDetailDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CustomerCompanyName = order.Customer?.CompanyName,
                EmployeeId = order.EmployeeId,
                EmployeeFullName = order.Employee != null ? $"{order.Employee.FirstName} {order.Employee.LastName}" : null,
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                ShipVia = order.ShipVia,
                ShipperCompanyName = order.ShipViaNavigation?.CompanyName,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipRegion = order.ShipRegion,
                ShipPostalCode = order.ShipPostalCode,
                ShipCountry = order.ShipCountry,
                TotalAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount)),
                Items = order.OrderDetails.Select(od => new OrderDetailItemDto
                {
                    ProductId = od.ProductId,
                    ProductName = od.Product?.ProductName ?? string.Empty,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    Discount = od.Discount
                }).ToList()
            };

            return ApiResponse<OrderDetailDto>.SuccessResult(detail);
        }

        public async Task<ApiResponse<OrderDetailDto>> CreateOrderAsync(CreateOrderDto request)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId.ToUpper(),
                EmployeeId = request.EmployeeId,
                OrderDate = DateTime.UtcNow,
                RequiredDate = request.RequiredDate,
                ShipVia = request.ShipVia,
                Freight = request.Freight ?? 0,
                ShipName = request.ShipName,
                ShipAddress = request.ShipAddress,
                ShipCity = request.ShipCity,
                ShipRegion = request.ShipRegion,
                ShipPostalCode = request.ShipPostalCode,
                ShipCountry = request.ShipCountry
            };

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    return ApiResponse<OrderDetailDto>.FailureResult($"Produk ID {item.ProductId} tidak ditemukan.");

                if (product.Discontinued)
                    return ApiResponse<OrderDetailDto>.FailureResult($"Produk '{product.ProductName}' sudah discontinued.");

                if ((product.UnitsInStock ?? 0) < item.Quantity)
                    return ApiResponse<OrderDetailDto>.FailureResult($"Stok produk '{product.ProductName}' tidak mencukupi. Sisa stok: {product.UnitsInStock ?? 0}");

                // Potong stok produk
                product.UnitsInStock = (short)((product.UnitsInStock ?? 0) - item.Quantity);
                await _productRepository.UpdateAsync(product);

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    UnitPrice = product.UnitPrice ?? 0,
                    Quantity = item.Quantity,
                    Discount = item.Discount
                });
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            return await GetOrderByIdAsync(order.OrderId, null, null, null);
        }

        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(UpdateOrderStatusDto request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                return ApiResponse<bool>.FailureResult("Order tidak ditemukan.");

            // Update ShippedDate -> Status terupdate secara otomatis (ShippedDate != null -> Shipped, ShippedDate == null -> Pending)
            order.ShippedDate = request.ShippedDate;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            string statusName = request.ShippedDate.HasValue ? "Shipped" : "Pending";
            return ApiResponse<bool>.SuccessResult(true, $"Status order berhasil diperbarui menjadi {statusName}.");
        }

        public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<bool>.FailureResult("Order tidak ditemukan.");

            await _orderRepository.DeleteAsync(order);
            await _orderRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Order berhasil dihapus.");
        }

        private static OrderResponseDto MapToResponseDto(Order o) => new()
        {
            OrderId = o.OrderId,
            CustomerId = o.CustomerId,
            CustomerCompanyName = o.Customer?.CompanyName,
            EmployeeId = o.EmployeeId,
            EmployeeFullName = o.Employee != null ? $"{o.Employee.FirstName} {o.Employee.LastName}" : null,
            OrderDate = o.OrderDate,
            RequiredDate = o.RequiredDate,
            ShippedDate = o.ShippedDate,
            ShipVia = o.ShipVia,
            ShipperCompanyName = o.ShipViaNavigation?.CompanyName,
            Freight = o.Freight,
            ShipName = o.ShipName,
            ShipAddress = o.ShipAddress,
            ShipCity = o.ShipCity,
            ShipRegion = o.ShipRegion,
            ShipPostalCode = o.ShipPostalCode,
            ShipCountry = o.ShipCountry,
            TotalAmount = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))
        };
    }
}
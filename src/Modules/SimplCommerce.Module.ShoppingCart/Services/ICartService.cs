using System.Threading;
using System.Threading.Tasks;

using SimplCommerce.Module.Pricing.Services;
using SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;

namespace SimplCommerce.Module.ShoppingCart.Services
{
    public interface ICartService
    {
        Task<AddToCartResult> AddToCart(long customerId, long productId, int quantity, CancellationToken token = default);

        Task<CartVm> GetCartDetails(long customerId, CancellationToken token = default);

        Task<CouponValidationResult> ApplyCoupon(long customerId, string couponCode, CancellationToken token = default);

        Task MigrateCart(long fromUserId, long toUserId, CancellationToken token = default);
    }
}

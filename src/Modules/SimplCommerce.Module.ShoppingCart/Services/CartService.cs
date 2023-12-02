using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Pricing.Services;
using SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using SimplCommerce.Module.ShoppingCart.Models;

namespace SimplCommerce.Module.ShoppingCart.Services;

public class CartService : ICartService
{
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly IMediaService _mediaService;
    private readonly ICouponService _couponService;
    private readonly ICurrencyService _currencyService;
    private readonly IStringLocalizer _localizer;

    public CartService(IRepository<CartItem> cartItemRepository, ICouponService couponService,
        IMediaService mediaService, ICurrencyService currencyService, IStringLocalizerFactory stringLocalizerFactory)
    {
        _cartItemRepository = cartItemRepository;
        _couponService = couponService;
        _mediaService = mediaService;
        _currencyService = currencyService;
        _localizer = stringLocalizerFactory.Create(null);
    }

    public async Task<AddToCartResult> AddToCart(long customerId, long productId, int quantity, CancellationToken token = default)
    {
        var addToCartResult = new AddToCartResult { Success = false };

        if (quantity <= 0)
        {
            addToCartResult.ErrorMessage = _localizer["The quantity must be larger than zero"].Value;
            addToCartResult.ErrorCode = "wrong-quantity";
            return addToCartResult;
        }

        var cartItem = await _cartItemRepository.Query()
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.CustomerId == customerId, cancellationToken: token);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                CreatedOn = DateTimeOffset.Now
                //TODO add vendor id to cartitem
            };

            _cartItemRepository.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        await _cartItemRepository.SaveChangesAsync();

        addToCartResult.Success = true;
        return addToCartResult;
    }

    // TODO separate getting product thumbnail, variations options from here
    public async Task<CartVm> GetCartDetails(long customerId, CancellationToken token = default)
    {
        var cartItems = await _cartItemRepository.Query()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken: token);

        if (!cartItems.Any())
        {
            return null;
        }

        var cartVm = new CartVm(_currencyService);

        cartVm.Items = _cartItemRepository
            .Query()
            .Include(x => x.Product).ThenInclude(p => p.ThumbnailImage)
            .Include(x => x.Product).ThenInclude(p => p.OptionCombinations).ThenInclude(o => o.Option)
            .Where(x => x.CustomerId == customerId).ToList()
            .Select(x => new CartItemVm(_currencyService)
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                ProductPrice = x.Product.Price,
                ProductStockQuantity = x.Product.StockQuantity,
                ProductStockTrackingIsEnabled = x.Product.StockTrackingIsEnabled,
                IsProductAvailabeToOrder = x.Product.IsAllowToOrder && x.Product.IsPublished && !x.Product.IsDeleted,
                ProductImage = _mediaService.GetThumbnailUrl(x.Product.ThumbnailImage),
                Quantity = x.Quantity,
                VariationOptions = CartItemVm.GetVariationOption(x.Product)
            }).ToList();

        cartVm.SubTotal = cartVm.Items.Sum(x => x.Quantity * x.ProductPrice);

        if (!string.IsNullOrWhiteSpace(cartVm.CouponCode))
        {
            var cartInfoForCoupon = new CartInfoForCoupon
            {
                Items = cartVm.Items.Select(x => new CartItemForCoupon { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()
            };
            var couponValidationResult = await _couponService.Validate(customerId, cartVm.CouponCode, cartInfoForCoupon);
            if (couponValidationResult.Succeeded)
            {
                cartVm.Discount = couponValidationResult.DiscountAmount;
            }
            else
            {
                cartVm.CouponValidationErrorMessage = couponValidationResult.ErrorMessage;
            }
        }

        return cartVm;
    }

    public async Task<CouponValidationResult> ApplyCoupon(long customerId, string couponCode, CancellationToken token = default)
    {
        var cartItems = await _cartItemRepository.Query()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken: token);

        var cartInfoForCoupon = new CartInfoForCoupon
        {
            Items = cartItems.Select(x => new CartItemForCoupon { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()
        };

        var couponValidationResult = await _couponService.Validate(customerId, couponCode, cartInfoForCoupon);

        return couponValidationResult;
    }

    public async Task MigrateCart(long fromUserId, long toUserId, CancellationToken token = default)
    {
        var cartItemsFrom = await _cartItemRepository.Query().Where(x => x.CustomerId == fromUserId).ToListAsync(cancellationToken: token);
        var carItemsTo = await _cartItemRepository.Query().Where(x => x.CustomerId == toUserId).ToListAsync(cancellationToken: token);
        foreach (var cartItem in cartItemsFrom)
        {
            var existingCartItem = carItemsTo.Find(x => x.ProductId == cartItem.ProductId);
            if (existingCartItem == null)
            {
                existingCartItem = new CartItem
                {
                    CustomerId = toUserId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    CreatedOn = DateTimeOffset.Now
                };
                _cartItemRepository.Add(existingCartItem);
            }
            else
            {
                existingCartItem.Quantity += cartItem.Quantity;
            }
        }

        await _cartItemRepository.SaveChangesAsync();
    }
}

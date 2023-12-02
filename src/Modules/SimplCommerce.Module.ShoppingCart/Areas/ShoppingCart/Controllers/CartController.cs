﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.ViewModels;
using SimplCommerce.Module.ShoppingCart.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.ShoppingCart.Areas.ShoppingCart.Controllers;

[Area("ShoppingCart")]
[ApiExplorerSettings(IgnoreApi = true)]
public class CartController : Controller
{
    private readonly IRepository<CartItem> _cartItemRepository;
    private readonly ICartService _cartService;
    private readonly IMediaService _mediaService;
    private readonly IWorkContext _workContext;
    private readonly ICurrencyService _currencyService;
    private readonly IStringLocalizer _localizer;

    public CartController(
        IRepository<CartItem> cartItemRepository,
        ICartService cartService,
        IMediaService mediaService,
        IWorkContext workContext,
        ICurrencyService currencyService,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        _cartItemRepository = cartItemRepository;
        _cartService = cartService;
        _mediaService = mediaService;
        _workContext = workContext;
        _currencyService = currencyService;
        _localizer = stringLocalizerFactory.Create(null);
    }

    [HttpPost("cart/add-item")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model, CancellationToken token = default)
    {
        var currentUser = await _workContext.GetCurrentUser();
        var result = await _cartService.AddToCart(currentUser.Id, model.ProductId, model.Quantity, token);

        if (result.Success)
        {
            return RedirectToAction("AddToCartResult", new { productId = model.ProductId });
        }

        return Ok(result);
    }

    [HttpGet("cart/add-item-result")]
    public async Task<IActionResult> AddToCartResult(long productId, CancellationToken token = default)
    {
        var currentUser = await _workContext.GetCurrentUser();
        var cart = await _cartService.GetCartDetails(currentUser.Id, token);

        var model = new AddToCartResultVm(_currencyService)
        {
            CartItemCount = cart.Items.Count,
            CartAmount = cart.SubTotal
        };

        var addedProduct = cart.Items.First(x => x.ProductId == productId);
        model.ProductName = addedProduct.ProductName;
        model.ProductImage = addedProduct.ProductImage;
        model.ProductPrice = addedProduct.ProductPrice;
        model.Quantity = addedProduct.Quantity;

        return PartialView(model);
    }

    [HttpGet("cart")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("cart/list")]
    public async Task<IActionResult> List(CancellationToken token = default)
    {
        var currentUser = await _workContext.GetCurrentUser();
        var cart = await _cartService.GetCartDetails(currentUser.Id, token);

        if (cart is null)
        {
            cart = new CartVm(_currencyService);
        }

        return Json(cart);
    }

    [HttpPost("cart/update-item-quantity")]
    public async Task<IActionResult> UpdateQuantity([FromBody] CartQuantityUpdate model, CancellationToken token = default)
    {
        if (model.Quantity <= 0)
        {
            return Ok(new { Error = true, Message = _localizer["The quantity must be larger than zero"].Value });
        }
        var currentUser = await _workContext.GetCurrentUser();

        var cartItem = _cartItemRepository.Query().Include(x => x.Product).FirstOrDefault(x => x.Id == model.CartItemId && x.CustomerId == currentUser.Id);
        if (cartItem == null)
        {
            return NotFound();
        }

        if (model.Quantity > cartItem.Quantity) // always allow user to decrease the quantity
        {
            if (cartItem.Product.StockTrackingIsEnabled && cartItem.Product.StockQuantity < model.Quantity)
            {
                return Ok(new { Error = true, Message = _localizer["There are only {0} items available for {1}.", cartItem.Product.StockQuantity, cartItem.Product.Name].Value });
            }
        }

        cartItem.Quantity = model.Quantity;
        await _cartItemRepository.SaveChangesAsync();

        return await List(token);
    }

    [HttpPost("cart/apply-coupon")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponForm model, CancellationToken token = default)
    {
        var currentUser = await _workContext.GetCurrentUser();

        var validationResult = await _cartService.ApplyCoupon(currentUser.Id, model.CouponCode, token);
        if (validationResult.Succeeded)
        {
            var cartVm = await _cartService.GetCartDetails(currentUser.Id, token);
            cartVm.Discount = validationResult.DiscountAmount;

            return Json(cartVm);
        }

        return Json(validationResult);
    }


    [HttpPost("cart/remove-item")]
    public async Task<IActionResult> Remove([FromBody] long itemId, string returnUrl, CancellationToken token = default)
    {
        var currentUser = await _workContext.GetCurrentUser();

        var cartItem = _cartItemRepository.Query().FirstOrDefault(x => x.Id == itemId && x.CustomerId == currentUser.Id);
        if (cartItem == null)
        {
            return NotFound();
        }

        _cartItemRepository.Remove(cartItem);
        await _cartItemRepository.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return await List(token);
    }
}

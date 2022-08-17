using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VShop.CartApi.Context;
using VShop.CartApi.DTOs;
using VShop.CartApi.Models;

namespace VShop.CartApi.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;
    private IMapper _mapper;

    public CartRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> CleanCartAsync(string userId)
    {
        var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeader is not null)
        {
            _context.CartItems.RemoveRange(_context.CartItems.Where(c => c.CartHeaderId == cartHeader.Id));
            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<CartDTO> GetCartByUserIdAsync(string userId)
    {
        Cart cart = new()
        {
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId),
        };

        cart.CartItems = _context.CartItems.Where(c => c.CartHeaderId == cart.CartHeader.Id).Include(c => c.Product);

        return _mapper.Map<CartDTO>(cart);
    }

    

    public async Task<bool> DeleteItemCartAsync(int cartItemId)
    {
        try
        {
            CartItem cartItem = await _context.CartItems
                               .FirstOrDefaultAsync(c => c.Id == cartItemId);

            int total = _context.CartItems.Where(c => c.CartHeaderId == cartItem.CartHeaderId).Count();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            if (total == 1)
            {
                var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(
                                             c => c.Id == cartItem.CartHeaderId);

                _context.CartHeaders.Remove(cartHeader);
                await _context.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<CartDTO> UpdateCartAsync(CartDTO cartDto)
    {
        Cart cart = _mapper.Map<Cart>(cartDto);

        //salva o produto no banco 
        await SaveProductInDataBase(cartDto, cart);

        //Verifica se o CartHeader é null
        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
                               c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader is null)
        {
            //criar o Header e os itens
            await CreateCartHeaderAndItems(cart);
        }
        else
        {
            //atualiza a quantidade e os itens
            await UpdateQuantityAndItems(cartDto, cart, cartHeader);
        }
        return _mapper.Map<CartDTO>(cart);
    }

    private async Task UpdateQuantityAndItems(CartDTO cartDto, Cart cart, CartHeader? cartHeader)
    {
        //Se CartHeader não é null
        //verifica se CartItems possui o mesmo produto
        var cartItem = await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(
                               p => p.ProductId == cartDto.CartItems.FirstOrDefault()
                               .ProductId && p.CartHeaderId == cartHeader.Id);

        if (cartItem is null)
        {
            //Cria o CartItems
            cart.CartItems.FirstOrDefault().CartHeaderId = cartHeader.Id;
            cart.CartItems.FirstOrDefault().Product = null;
            _context.CartItems.Add(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
        else
        {
            //Atualiza a quantidade e o CartItems
            cart.CartItems.FirstOrDefault().Product = null;
            cart.CartItems.FirstOrDefault().Quantity += cartItem.Quantity;
            cart.CartItems.FirstOrDefault().Id = cartItem.Id;
            cart.CartItems.FirstOrDefault().CartHeaderId = cartItem.CartHeaderId;
            _context.CartItems.Update(cart.CartItems.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
    }

    private async Task CreateCartHeaderAndItems(Cart cart)
    {
        //Cria o CartHeader e o CartItems
        _context.CartHeaders.Add(cart.CartHeader);
        await _context.SaveChangesAsync();

        cart.CartItems.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
        cart.CartItems.FirstOrDefault().Product = null;

        _context.CartItems.Add(cart.CartItems.FirstOrDefault());

        await _context.SaveChangesAsync();
    }

    private async Task SaveProductInDataBase(CartDTO cartDto, Cart cart)
    {
        //Verifica se o produto ja foi salvo senão salva
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id ==
                            cartDto.CartItems.FirstOrDefault().ProductId);

        //salva o produto senão existe no bd
        if (product is null)
        {
            _context.Products.Add(cart.CartItems.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ApplyCouponAsync(string userId, string couponCode)
    {
        var cartHeaderApplyCoupon = await _context.CartHeaders
                               .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeaderApplyCoupon is not null)
        {
            cartHeaderApplyCoupon.CouponCode = couponCode;

            _context.CartHeaders.Update(cartHeaderApplyCoupon);

            await _context.SaveChangesAsync();

            return true;
        }
        return false;
    }

    public async Task<bool> DeleteCouponAsync(string userId)
    {
        var cartHeaderDeleteCoupon = await _context.CartHeaders
                              .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cartHeaderDeleteCoupon is not null)
        {
            cartHeaderDeleteCoupon.CouponCode = "";

            _context.CartHeaders.Update(cartHeaderDeleteCoupon);

            await _context.SaveChangesAsync();

            return true;
        }
        return false;
    }
}
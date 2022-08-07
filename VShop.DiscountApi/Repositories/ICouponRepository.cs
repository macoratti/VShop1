using VShop.DiscountApi.DTOs;

namespace VShop.DiscountApi.Repositories;

public interface ICouponRepository
{
    Task<CouponDTO> GetCouponByCode(string couponCode);
}

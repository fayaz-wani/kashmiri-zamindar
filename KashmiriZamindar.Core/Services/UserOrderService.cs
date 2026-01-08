using KashmiriZamindar.Core.Dtos;
using KashmiriZamindar.Core.Interfaces;

namespace KashmiriZamindar.Core.Services
{
    public class UserOrderService:IUserRepository
    {
        private readonly IUserRepository _userRepository;

        public UserOrderService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserOrderDto>> GetUserOrdersAsync(
            Guid userGuid,
            int page,
            int pageSize)

        { 
            return await _userRepository
                .GetUserOrdersAsync(userGuid, page, pageSize);
        }

        public async Task<UserOrderDetailDto?> GetUserOrderDetailsAsync(
            Guid orderGuid,
            Guid userGuid)
        {


            return await _userRepository
                .GetUserOrderDetailsAsync(orderGuid, userGuid);
        }

    }

}

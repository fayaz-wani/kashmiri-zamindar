using KashmiriZamindar.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Interfaces
{
    public interface IUserRepository
    {

   
        // Add to your existing IUserRepository interface

        Task<List<UserOrderDto>> GetUserOrdersAsync(Guid userGuid, int page, int pageSize);
        Task<UserOrderDetailDto?> GetUserOrderDetailsAsync(Guid orderGuid, Guid userGuid);
    }
}

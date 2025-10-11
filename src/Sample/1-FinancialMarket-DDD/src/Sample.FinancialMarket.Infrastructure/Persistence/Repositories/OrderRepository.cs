using Foundry.Domain.Interfaces;
using Foundry.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Sample.FinancialMarket.Domain.Aggregates.Orders;
using Sample.FinancialMarket.Domain.Interfaces.Repositories;

namespace Sample.FinancialMarket.Infrastructure.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(IApplicationDbContext context) : base(context) { }

    public override async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Order>()
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
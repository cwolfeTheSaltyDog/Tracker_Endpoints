using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Followers.GetRecentFollowers;

internal sealed class GetRecentFollowersQueryHandler(IApplicationReadDbContext context)
    : IQueryHandler<GetRecentFollowersQuery, List<FollowerResponse>>
{
    public async Task<Result<List<FollowerResponse>>> Handle(
        GetRecentFollowersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken))
        {
            return Result.Failure<List<FollowerResponse>>(UserErrors.NotFound(request.UserId));
        }

        List<FollowerResponse> followers = await context.Followers
            .Where(f => f.FollowedId == request.UserId)
            .OrderByDescending(f => f.CreatedOnUtc)
            .Take(10)
            .Select(f => new FollowerResponse(f.User.Id, f.User.Name))
            .ToListAsync(cancellationToken);

        return followers;
    }
}

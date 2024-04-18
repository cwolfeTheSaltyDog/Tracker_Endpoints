using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Followers.GetFollowerStats;

internal sealed class GetFollowerStatsQueryHandler(IApplicationReadDbContext context)
    : IQueryHandler<GetFollowerStatsQuery, FollowerStatsResponse>
{
    public async Task<Result<FollowerStatsResponse>> Handle(
        GetFollowerStatsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await context.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken))
        {
            return Result.Failure<FollowerStatsResponse>(UserErrors.NotFound(request.UserId));
        }

        int followerCount = await context.Followers
            .CountAsync(f => f.FollowedId == request.UserId, cancellationToken);

        int followingCount = await context.Followers
            .CountAsync(f => f.UserId == request.UserId, cancellationToken);

        return new FollowerStatsResponse(request.UserId, followerCount, followingCount);
    }
}

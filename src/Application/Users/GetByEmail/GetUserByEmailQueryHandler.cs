using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.GetByEmail;

internal sealed class GetUserByEmailQueryHandler(IApplicationReadDbContext context)
    : IQueryHandler<GetUserByEmailQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(
        GetUserByEmailQuery query,
        CancellationToken cancellationToken)
    {
        UserResponse? user = await context.Users
            .Where(u => u.Email == query.Email)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                Name = u.Name,
                HasPublicProfile = u.HasPublicProfile
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFoundByEmail);
        }

        return user;
    }
}

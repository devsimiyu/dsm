using domain.data.Persistence;
using identity.api.Model;
using Microsoft.EntityFrameworkCore;

namespace identity.api.Repository;

public class UserRepository
{
    private readonly PersistenceContext _persistenceContext;

    public UserRepository(PersistenceContext persistenceContext)
        => _persistenceContext = persistenceContext;

    public async Task<UserDetailsDto?> FindUserByEmail(string email)
    {
        var user = await _persistenceContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
        if (user == null) return null;
        var userDetailsDto = new UserDetailsDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
        };
        return userDetailsDto;
    }
}

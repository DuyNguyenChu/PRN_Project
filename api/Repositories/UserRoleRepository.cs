using api.Interface.Repository;
using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class UserRoleRepository : RepositoryBase<UserRole, int>, IUserRoleRepository
    {
        public UserRoleRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }

    }
}

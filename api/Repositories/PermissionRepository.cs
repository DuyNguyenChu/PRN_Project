using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;

namespace api.Repositories
{
    public class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }

    }
}

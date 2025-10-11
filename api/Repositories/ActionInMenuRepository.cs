using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Interface.Repository;
using api.Models;

namespace api.Repositories
{
    public class ActionInMenuRepository : RepositoryBase<ActionInMenu, int>, IActionInMenuRepository
    {
        public ActionInMenuRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }

    }
}

using api.Interface.Repository;
using api.Models;

namespace api.Repositories
{
    public class TripRepository : RepositoryBase<Trip, int>, ITripRepository
    {
        public TripRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}

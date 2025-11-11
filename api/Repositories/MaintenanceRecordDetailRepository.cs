using api.Models;
using api.Interface.Repository;
using api.Interface;

namespace api.Repositories
{
    public class MaintenanceRecordDetailRepository : RepositoryBase<MaintenanceRecordDetail, int>, IMaintenanceRecordDetailRepository
    {
        public MaintenanceRecordDetailRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}
using api.DTParameters;
using api.Extensions;
using api.Helpers;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class UserRepository : RepositoryBase<User, int>, IUserRepository
    {
        private readonly PrnprojectContext _context;
        //private readonly IStorageService _storageService;
        public UserRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            //_storageService = storageService;
        }

        public async Task<List<MenuAggregate>> GetMenuAsync(int userId)
        {
            var data = await (from a in _context.UserRoles
                              join b in _context.Permissions on a.RoleId equals b.RoleId
                              join c in _context.Menus on b.MenuId equals c.Id
                              where !a.IsDeleted && !b.IsDeleted && !c.IsDeleted &&
                                 a.UserId == userId && b.ActionId == (int)Enums.Action.READ && c.MenuType == CommonConstants.MenuType.ADMIN
                              select new MenuAggregate
                              {
                                  Id = c.Id,
                                  ClassName = c.ClassName,
                                  Icon = c.Icon,
                                  MenuType = c.MenuType,
                                  Name = c.Name,
                                  ParentId = c.ParentId,
                                  SortOrder = c.SortOrder,
                                  Url = c.Url,
                                  TreeIds = c.TreeIds
                              })
                        .GroupBy(x => x.Id)
                        .Select(x => x.First())
                        .ToListAsync();
            data = data
                .OrderBy(x => x.TreeIds)
                .ThenBy(x => x.SortOrder)
                .ToList();

            var treeMenu = BuildMenuTree(data);
            treeMenu = treeMenu
                .OrderBy(x => x.SortOrder)
                .ToList();

            return treeMenu;
        }

        private List<MenuAggregate> BuildMenuTree(List<MenuAggregate> menus, int? parentId = null)
        {
            return menus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuAggregate
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Icon = m.Icon,
                    ClassName = m.ClassName,
                    SortOrder = m.SortOrder,
                    MenuType = m.MenuType,
                    ParentId = m.ParentId,
                    Child = BuildMenuTree(menus, m.Id),
                    TreeIds = m.TreeIds
                })
                .ToList();
        }

        public async Task<DTResult<UserAggregates>> GetPagedAsync(UserDTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;
            if (parameters.Order != null && parameters.Order.Length != 0)
            {
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() != "desc";
            }
            else
            {
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }

            var query = _context.Users
               .Where(x => !x.IsDeleted && !x.UserStatus.IsDeleted && (parameters.IsGetAll || x.Driver == null))
               .Select(x => new UserAggregates
               {
                   Id = x.Id,
                   Username = x.UserName,
                   FirstName = x.FirstName,
                   LastName = x.LastName,
                   Email = x.Email,
                   PhoneNumber = x.PhoneNumber,
                   AvatarId = x.AvatarId,
                   //AvatarUrl = x.Avatar != null ? _storageService.GetOriginalUrl(x.Avatar.FileKey) : null,
                   UserStatusId = x.UserStatusId,
                   UserStatusColor = x.UserStatus.Color,
                   UserStatusName = x.UserStatus.Name,
                   Roles = x.UserRoles.Where(ur => !ur.IsDeleted)
                       .Select(ur => ur.Role)
                       .ToList(),
                   CreatedDate = x.CreatedDate,
               });

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                //keyword = keyword.ToLower();
                query = query
                    .Where(x => EF.Functions.Collate(x.Username, SQLParams.Latin_General).Contains(keyword) ||
                        EF.Functions.Collate(x.Email, SQLParams.Latin_General).Contains(keyword) ||
                        (x.PhoneNumber != null && EF.Functions.Collate(x.PhoneNumber, SQLParams.Latin_General).Contains(keyword)) ||
                        EF.Functions.Collate((x.FirstName + " " + x.LastName), SQLParams.Latin_General).Contains(keyword) ||
                        EF.Functions.Collate(x.LastName, SQLParams.Latin_General).Contains(keyword) ||
                        EF.Functions.Collate(x.FirstName, SQLParams.Latin_General).Contains(keyword) ||
                        EF.Functions.Collate(x.UserStatusName, SQLParams.Latin_General).Contains(keyword) ||
                        x.Roles.Any(r => EF.Functions.Collate(r.Name, SQLParams.Latin_General).Contains(keyword)) ||
                        x.CreatedDate.ToVietnameseDateTimeOffset().Contains(keyword)
                     );
            }
            foreach (var column in parameters.Columns)
            {
                var search = column.Search.Value;
                if (!search.Contains("/"))
                {
                    search = column.Search.Value.ToLower();
                }
                if (string.IsNullOrEmpty(search)) continue;
                switch (column.Data)
                {
                    case "createdDate":
                        if (search.Contains(" - "))
                        {
                            var dates = search.Split(" - ");
                            var startDate = DateTime.ParseExact(dates[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var endDate = DateTime.ParseExact(dates[1], "dd/MM/yyyy", CultureInfo.InvariantCulture).AddDays(1).AddSeconds(-1);
                            query = query.Where(c => c.CreatedDate >= startDate && c.CreatedDate <= endDate);
                        }
                        else
                        {
                            var date = DateTime.ParseExact(search, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            query = query.Where(c => c.CreatedDate.Date == date.Date);
                        }
                        break;
                }
            }
            if (!string.IsNullOrEmpty(parameters.Username))
            {
                query = query.Where(x => EF.Functions.Collate(x.Username, SQLParams.Latin_General).Contains(parameters.Username));
            }
            if (!string.IsNullOrEmpty(parameters.Fullname))
            {
                query = query.Where(x => EF.Functions.Collate(x.FirstName, SQLParams.Latin_General).Contains(parameters.Fullname) ||
                EF.Functions.Collate(x.LastName, SQLParams.Latin_General).Contains(parameters.Fullname) ||
                EF.Functions.Collate((x.FirstName + " " + x.LastName), SQLParams.Latin_General).Contains(parameters.Fullname)
                );
            }
            if (!string.IsNullOrEmpty(parameters.Email))
            {
                query = query.Where(x => EF.Functions.Collate(x.Email, SQLParams.Latin_General).Contains(parameters.Email));
            }
            if (!string.IsNullOrEmpty(parameters.PhoneNumber))
            {
                query = query.Where(x => EF.Functions.Collate(x.PhoneNumber!, SQLParams.Latin_General).Contains(parameters.PhoneNumber));
            }
            if (parameters.UserStatusIds.Any())
            {
                query = query.Where(x => parameters.UserStatusIds.Contains(x.UserStatusId));
            }
            if (parameters.RoleIds.Any())
            {
                query = query.Where(x => x.Roles.Any(r => parameters.RoleIds.Contains(r.Id)));
            }
            //if (parameters.GenderIds.Count > 0)
            //{
            //    query = query.Where(x => parameters.GenderIds.Contains((int)x.Gender));
            //}
            //if (parameters.OfficeIds.Any())
            //{
            //    query = query.Where(x => x.OfficeId != null && parameters.OfficeIds.Contains(x.OfficeId.Value));
            //}
            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);
            var data = new DTResult<UserAggregates>
            {
                draw = parameters.Draw,
                data = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync(),
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;
        }

        public async Task<List<PermissionAggregate>> GetPermissionsAsync(int userId)
        {
            var data = await (from a in _context.Users
                              join b in _context.UserRoles on a.Id equals b.UserId
                              join d in _context.Roles on b.RoleId equals d.Id
                              join c in _context.Permissions on b.RoleId equals c.RoleId
                              where !a.IsDeleted && !b.IsDeleted && !c.IsDeleted && !d.IsDeleted
                                && b.UserId == userId
                              select new
                              {
                                  c.ActionId,
                                  c.MenuId
                              })
                            .GroupBy(x => new { x.MenuId, x.ActionId })
                            .Select(x => x.Key)
                            .GroupBy(x => x.MenuId)
                            .Select(x => new PermissionAggregate
                            {
                                MenuId = x.Key,
                                ActionIds = x.Select(x => x.ActionId).ToList()
                            })
                            .ToListAsync();

            return data;
        }

    }
}

﻿using api.Extensions;
using api.Interface.Repository;
using api.Models;
using api.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace api.Repositories
{
    public class ActionRepository : RepositoryBase<Models.Action, int>, IActionRepository
    {
        private readonly PrnprojectContext _context;
        public ActionRepository(PrnprojectContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<DTResult<ActionAggregate>> GetPagedAsync(api.Extensions.DTParameters parameters)
        {
            var keyword = parameters.Search?.Value;
            var orderCriteria = string.Empty;
            var orderAscendingDirection = true;
            if (parameters.Order != null && parameters.Order.Any())
            {
                orderCriteria = parameters.Columns[parameters.Order[0].Column].Data;
                orderAscendingDirection = parameters.Order[0].Dir.ToString().ToLower() == "asc";
            }
            else
            {
                orderCriteria = "Id";
                orderAscendingDirection = true;
            }

            var query = _context.Actions
                .Where(x => !x.IsDeleted)
                .Select(x => new ActionAggregate
                {
                    CreatedDate = x.CreatedDate,
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                });

            var totalRecord = await query.CountAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query
                    .Where(x => EF.Functions.Collate(x.Name, SQLParams.Latin_General).Contains(keyword) ||
                        (x.Description != null && EF.Functions.Collate(x.Description, SQLParams.Latin_General).Contains(keyword)) ||
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
                    case "name":
                        query = query
                            .Where(r => EF.Functions.Collate(r.Name, SQLParams.Latin_General).Contains(search));
                        break;
                    case "description":
                        query = query.Where(r => r.Description != null && EF.Functions.Collate(r.Description, SQLParams.Latin_General).Contains(search));
                        break;
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
            query = orderAscendingDirection ? query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Asc) : query.OrderByDynamic(orderCriteria, LinqExtensions.Order.Desc);

            var data = new DTResult<ActionAggregate>
            {
                draw = parameters.Draw,
                data = await query.Skip(parameters.Start).Take(parameters.Length).ToListAsync(),
                recordsFiltered = await query.CountAsync(),
                recordsTotal = totalRecord
            };

            return data;

        }
    }
}

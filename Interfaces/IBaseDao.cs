using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Twitchbot.Common.Base.Interfaces
{
    public interface IBaseDao<TEntity, TReadModel, TCreateModel, TUpdateModel>
    {
        Task<TReadModel> ReadModel(int id, CancellationToken cancellationToken = default);
        Task<TReadModel> CreateModel(TCreateModel createModel, CancellationToken cancellationToken = default);
        Task<TReadModel> UpdateModel(int id, TUpdateModel updateModel, CancellationToken cancellationToken = default);
        Task<TReadModel> DeleteModel(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TReadModel>> QueryModel(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
    }
}
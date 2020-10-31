using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Twitchbot.Common.Models.Data;
using Twitchbot.Common.Models.Definitions;

namespace Twitchbot.Common.Base.Dao
{
    /// <summary>
    /// Provides CRUD methods on the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity to update.</typeparam>
    /// <typeparam name="TReadModel">The read model.</typeparam>
    /// <typeparam name="TCreateModel">The create model.</typeparam>
    /// <typeparam name="TUpdateModel">The update model.</typeparam>
    public abstract class BaseDao<TEntity, TReadModel, TCreateModel, TUpdateModel> where TEntity : class, IHaveIdentifier
    {
        /// <summary>
        /// Initializes a nex instance of <see cref="BaseDao{TEntity, TReadModel, TCreateModel, TUpdateModel}"/>.
        /// </summary>
        /// <param name="dataContext">The context.</param>
        /// <param name="mapper">AutoMapper.</param>
        protected BaseDao(TwitchbotContext dataContext, IMapper mapper)
        {
            DataContext = dataContext;
            Mapper = mapper;
        }

        protected TwitchbotContext DataContext { get; }

        protected IMapper Mapper { get; }

        /// <summary>
        /// Read an entity model. 
        /// </summary>
        /// <param name="id">The primary key of <see cref="TEntity"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The model.</returns>
        public virtual async Task<TReadModel> ReadModel(int id, CancellationToken cancellationToken = default)
        {
            var dbSet = DataContext.Set<TEntity>();

            var model = await dbSet
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<TReadModel>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return model;
        }

        /// <summary>
        /// Create an entity model. 
        /// </summary>
        /// <param name="createModel">The model.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created model.</returns>
        public virtual async Task<TReadModel> CreateModel(TCreateModel createModel, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<TEntity>(createModel);

            var dbSet = DataContext.Set<TEntity>();

            await dbSet.AddAsync(entity, cancellationToken);

            await DataContext.SaveChangesAsync(cancellationToken);

            var readModel = await ReadModel(entity.Id, cancellationToken);

            return readModel;
        }

        /// <summary>
        /// Update an entity model. 
        /// </summary>
        /// <param name="id">The primary key of <see cref="TEntity"/>.</param>
        /// <param name="updateModel">The model.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The updated model.</returns>
        public virtual async Task<TReadModel> UpdateModel(int id, TUpdateModel updateModel, CancellationToken cancellationToken = default)
        {
            var keyValue = new object[] { id };

            var dbSet = DataContext.Set<TEntity>();

            var entity = await dbSet.FindAsync(keyValue, cancellationToken);

            if (entity == null)
                return default;

            Mapper.Map(updateModel, entity);

            await DataContext.SaveChangesAsync(cancellationToken);

            var readModel = await ReadModel(id, cancellationToken);

            return readModel;
        }

        /// <summary>
        /// Delete and entity model.
        /// </summary>
        /// <param name="id">The primary key of <see cref="TEntity"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The deleted model.</returns>
        public virtual async Task<TReadModel> DeleteModel(int id, CancellationToken cancellationToken = default)
        {
            var dbSet = DataContext.Set<TEntity>();

            var keyValue = new object[] { id };

            var entity = await dbSet.FindAsync(keyValue, cancellationToken);

            if (entity == null)
                return default;

            var readModel = await ReadModel(id, cancellationToken);

            dbSet.Remove(entity);

            await DataContext.SaveChangesAsync(cancellationToken);

            return readModel;
        }

        /// <summary>
        /// Query an entity model with a predicate
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The list of models.</returns>
        public virtual async Task<IReadOnlyList<TReadModel>> QueryModel(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var dbSet = DataContext.Set<TEntity>();

            var query = dbSet.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            var results = await query
                .ProjectTo<TReadModel>(Mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
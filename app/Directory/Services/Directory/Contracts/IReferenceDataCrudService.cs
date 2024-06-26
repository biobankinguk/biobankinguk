﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IReferenceDataCrudService<T> where T : BaseReferenceData
    {
        /// <summary>
        /// Create a new entity <typeparamref name="T"/>. The Id of the OntologyTerm should be null, as it is
        /// generated by the database
        /// </summary>
        /// <returns>The newly created entity <typeparamref name="T"/>, with assigned Id</returns>
        Task<T> Add(T entity);

        /// <summary>
        /// Counts the number of entities <typeparamref name="T"/>
        /// </summary>
        /// <returns>The integer number of exisiting entities</returns>
        Task<int> Count();

        /// <summary>
        /// Delete a given entity with Id
        /// </summary>
        /// <param name="id">Id of the entity to delete</param>
        Task Delete(int id);

        /// <summary>
        /// Checks if an entity with given Id
        /// </summary>
        /// <returns>true - If an entity exists with given Id</returns>
        Task<bool> Exists(int id);

        /// <summary>
        /// Checks if an entity with given value exists
        /// </summary>
        /// <param name="value">The descriptive value of the Entity <typeparamref name="T"/></param>
        /// <returns>true - If at least one entity exists with given descriptive value</returns>
        Task<bool> Exists(string value);

        /// <summary>
        /// Checks if an entity with given value exists
        /// </summary>
        /// <param name="id">The Id of the Entity <typeparamref name="T"/></param>
        /// <param name="value">The descriptive value of the Entity <typeparamref name="T"/></param>
        /// <returns>true - If at least one entity exists with given descriptive value not equal to the Id</returns>
        Task<bool> ExistsExcludingId(int id, string value);

        /// <summary>
        /// Returns singular entity by its Id
        /// </summary>
        /// <returns>The entity <typeparamref name="T"/> with given Id. Otherwise null</returns>
        Task<T> Get(int id);

        /// <summary>
        /// Returns singular entity by its Value
        /// </summary>
        /// <returns>The first entity <typeparamref name="T"/> with given Value. Otherwise null</returns>
        Task<T> Get(string value);

        /// <summary>
        /// The number of times the entity is used or referenced
        /// </summary>
        /// <param name="id">The Id of the entity <typeparamref name="T"/></param>
        Task<int> GetUsageCount(int id);

        /// <summary>
        /// Whether this entitiy is in use or is being referenced
        /// </summary>
        /// <param name="id">The Id of the entity <typeparamref name="T"/></param>
        Task<bool> IsInUse(int id);

        /// <summary>
        /// Get all untracked entities
        /// </summary>
        /// <returns>An ICollection of all the entities</returns>
        Task<ICollection<T>> List();

        /// <summary>
        /// Get all untracked entities
        /// </summary>
        /// <returns>An ICollection of all the entities</returns>
        Task<ICollection<T>> List(string wildcard);

        /// <summary>
        /// Update an exisiting entity <typeparamref name="T"/> with the provided updated entity
        /// </summary>
        /// <returns>The updated entity <typeparamref name="T"/></returns>
        /// <exception cref="KeyNotFoundException">If no entity of given Id currently exists</exception>
        Task<T> Update(T entity);
    }
}


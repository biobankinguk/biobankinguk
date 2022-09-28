using System;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IContentPageService
    {
        /// <summary>
        /// Create a new content page
        /// </summary>
        /// <param name="title">Title of the page</param>
        /// <param name="body">Page body</param>
        /// <param name="slug">Page </param>
        /// <param name="isEnabled">Whether the page is shown publicly</param>
        /// <returns></returns>
        Task Create(string title, string body, string slug, bool isEnabled);

        /// <summary>
        /// Update an existing content page
        /// </summary>
        /// <param name="id">Id of the page to update</param>
        /// <param name="title">New title of the page</param>
        /// <param name="body">New body of the page</param>
        /// <param name="slug">New slug of the page</param>
        /// <param name="isEnabled">Whether the page is shown publicly</param>
        /// <returns></returns>
        Task Update(int id, string title, string body, string slug, bool isEnabled);

        /// <summary>
        /// Delete an existing content page
        /// </summary>
        /// <param name="id">The Id of the page</param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// List all content pages
        /// </summary>
        /// <returns>An Enumerable of all content pages</returns>
        Task<IEnumerable<ContentPage>> ListContentPages();

        /// <summary>
        /// Get a content page by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The content page with assigned Id</returns>
        Task<ContentPage> GetById(int id);

        /// <summary>
        /// Get a content page by its slug
        /// </summary>
        /// <param name="routeSlug"></param>
        /// <returns>The content page with assigned slug</returns>
        Task<ContentPage> GetBySlug(string routeSlug);
    }
}


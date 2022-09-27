using System;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IContentPageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="slug"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        Task Create(string title, string body, string slug, bool isEnabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="slug"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        Task Update(int id, string title, string body, string slug, bool isEnabled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ContentPage>> ListContentPages();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ContentPage> GetById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeSlug"></param>
        /// <returns></returns>
        Task<ContentPage> GetBySlug(string routeSlug);
    }
}


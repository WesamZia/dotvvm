using System;
using System.Threading.Tasks;

namespace DotVVM.Framework.Controls
{
    /// <summary>
    /// Extends the base GridViewDataSet with paging functionality.
    /// </summary>
    public interface IPageableGridViewDataSet : IBaseGridViewDataSet
    {
        /// <summary>
        /// Gets or sets an object that represents the settings for paging.
        /// </summary>
        IPagingOptions PagingOptions { get; set; }

        /// <summary>
        /// Navigates to the first page.
        /// </summary>
        [Obsolete("Use IPageableGridViewDataSet.GoToFirstPageAsync instead.This property will be removed in future versions.")]
        void GoToFirstPage();

        /// <summary>
        /// Navigates to the first page.
        /// </summary>
        /// <returns></returns>
        Task GoToFirstPageAsync();

        /// <summary>
        /// Navigates to the last page.
        /// </summary>
        [Obsolete("Use IPageableGridViewDataSet.GoToLastPageAsync instead.This property will be removed in future versions.")]
        void GoToLastPage();

        /// <summary>
        /// Navigates to the last page.
        /// </summary>
        Task GoToLastPageAsync();

        /// <summary>
        /// Navigates to the next page (if possible).
        /// </summary>
        [Obsolete("Use IPageableGridViewDataSet.GoToNextPageAsync instead.This property will be removed in future versions.")]
        void GoToNextPage();

        /// <summary>
        /// Navigates to the next page (if possible).
        /// </summary>
        /// <returns></returns>
        Task GoToNextPageAsync();

        /// <summary>
        /// Navigates to the specific page.
        /// </summary>
        [Obsolete("Use IPageableGridViewDataSet.GoToPageAsync instead.This property will be removed in future versions.")]
        void GoToPage(int index);

        /// <summary>
        /// Navigates to the specific page.
        /// </summary>
        Task GoToPageAsync(int index);

        /// <summary>
        /// Navigates to the previous page (if possible).
        /// </summary>
        [Obsolete("Use IPageableGridViewDataSet.GoToPreviousPageAsync instead.This property will be removed in future versions.")]
        void GoToPreviousPage();

        /// <summary>
        /// Navigates to the previous page (if possible).
        /// </summary>
        Task GoToPreviousPageAsync();
    }
}
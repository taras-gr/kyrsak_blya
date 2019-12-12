﻿using System;

namespace Store.ViewModels
{
    public class PageViewModel
    {
        public const int PageSize = 6;
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PageViewModel(int count, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageNumber < TotalPages);
            }
        }
    }
}

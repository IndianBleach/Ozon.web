using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataQueries
{
    public class Page
    {
        public int Number { get; private set; }

        public bool IsCurrent { get; private set; }

        public Page(int num, bool isCur)
        {
            Number = num;
            IsCurrent = isCur;
        }
    }

    public class PagedList<T>
    {
        public IEnumerable<T> Values { get; private set; }

        public int CurrentPage { get; private set; }

        public int PagesCount { get; private set; }

        public List<Page> Pages { get; private set; }

        public PagedList(IEnumerable<T> values, int currentPage, int pagesCount)
        {
            Values = values;
            CurrentPage = currentPage;
            PagesCount = pagesCount;
            Pages = new List<Page>();
        }

        public PagedList(IEnumerable<T> values, List<Page> pages)
        {
            Values = values;
            Pages = pages;
        }

        public static PagedList<T> GeneratePages(int total, int current, IEnumerable<T> values)
        {
            List<Page> pages = new List<Page>();

            if (total <= 3)
            {
                for (int i = 1; i <= total; i++)
                {
                    pages.Add(new Page(i, i == current));
                }
            }
            else
            {
                if (total == current)
                {
                    pages.Add(new Page(current - 2, false));
                    pages.Add(new Page(current - 1, false));
                    pages.Add(new Page(current, true));
                }
                else if (current == 1)
                {
                    pages.Add(new Page(current, true));
                    pages.Add(new Page(current + 1, false));
                    pages.Add(new Page(current + 2, false));
                }
                else
                {
                    pages.Add(new Page(current - 1, false));
                    pages.Add(new Page(current, true));
                    pages.Add(new Page(current + 1, false));
                }
            }

            return new PagedList<T>(values, pages);
        }
    }
}

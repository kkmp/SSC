namespace SSC.Tools
{
    public static class PageExtension
    {
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> elements, int nr, int countPerPage)
        {
            return elements.Skip((nr-1) * countPerPage).Take(countPerPage);
        }
    }
}

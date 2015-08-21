namespace SaleCycle.Svc.Dispatcher.Contract.Extensions
{
    public static class StringExtensions
    {
        public static string Fmt(this string input, params object[] args)
        {
            return string.Format(input, args);
        }

        public static bool IsNotNullOrEmpty(this string input)
        {
            return (input != null && input.Trim().Length > 0);
        }
    }
}

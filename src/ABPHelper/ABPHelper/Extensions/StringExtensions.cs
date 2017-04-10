namespace ABPHelper.Extensions
{
    public static class StringExtensions
    {
        public static string LowerFirstChar(this string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
    }
}
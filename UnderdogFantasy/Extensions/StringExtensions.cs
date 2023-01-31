namespace UnderdogFantasy.Extensions
{
    public static class StringExtensions
    {
        public static char GetFirstCharacter(this string s)
        {
            return string.IsNullOrEmpty(s) ? '\0' : s[0];
        }
    }
}
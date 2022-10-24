namespace TodoApi.Extensions
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string guidAsString)
        {
            Guid guid = Guid.TryParse(guidAsString, out Guid parsedGuid) ? parsedGuid : Guid.Empty;
            return guid;
        }
    }
}

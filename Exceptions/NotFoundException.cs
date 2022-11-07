namespace TodoApi.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
            :base("Not Found Error.")
        {

        }

    }
}

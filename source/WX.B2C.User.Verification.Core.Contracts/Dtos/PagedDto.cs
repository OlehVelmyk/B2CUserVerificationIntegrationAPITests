namespace WX.B2C.User.Verification.Core.Contracts.Dtos
{
    public class PagedDto<T>
    {
        public T[] Items { get; set; }

        public int Total { get; set; }
    }
}
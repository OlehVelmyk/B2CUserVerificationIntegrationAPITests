namespace WX.B2C.User.Verification.Worker.Jobs.Models
{
    public abstract class Batch
    {
        public static Batch<T> Create<T>(T[] items, int totalCount, int batchNumber)
        {
            return new Batch<T>(totalCount, batchNumber, items);
        }
    }

    public class Batch<T>
    {
        public Batch(int totalCount, int batchNumber, T[] items)
        {
            TotalCount = totalCount;
            BatchNumber = batchNumber;
            Items = items;
        }

        internal int TotalCount { get; }

        internal int BatchNumber { get; }

        public T[] Items { get; }
    }
}
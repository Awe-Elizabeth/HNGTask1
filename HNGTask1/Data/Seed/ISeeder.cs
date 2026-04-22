namespace HNGTask1.Data.Seed
{
    public interface ISeeder
    {
        Task SeedAsync(AppDBContext context);
    }
}

namespace HopCore.Server.Database {
    public interface IDbContextPopulator {
        void PopulateTableStores(IDbContext context);
    }
}
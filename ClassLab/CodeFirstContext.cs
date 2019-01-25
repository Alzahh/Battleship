namespace ClassLab
{
    using System.Data.Entity;

    public class CodeFirstContext : DbContext
    {
        public CodeFirstContext()
            : base("name=CodeFirstContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<CodeFirstContext>());
        }

        public DbSet<Save> Saves { get; set; }
    }
}
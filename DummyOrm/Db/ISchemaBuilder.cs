using System;

namespace DummyOrm.Db
{
    public interface ISchemaBuilder
    {
        void Build(SchemaBuildOptions options);
    }

    [Flags]
    public enum SchemaBuildOptions
    {
        AddColumn = 1,
        AlterColumn = 1 << 1,
        DropColumn = 1 << 2,
        DropTable = 1 << 3,
        CreateTable = 1 << 4,
        DropCreateAll = 1 << 31
    }
}

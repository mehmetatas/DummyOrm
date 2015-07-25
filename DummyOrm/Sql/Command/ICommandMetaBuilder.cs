using DummyOrm.Meta;

namespace DummyOrm.Sql.Command
{
    public interface ICommandMetaBuilder
    {
        CommandMeta BuildInsertCommandMeta(TableMeta table);
                    
        CommandMeta BuildUpdateCommandMeta(TableMeta table);
                    
        CommandMeta BuildDeleteCommandMeta(TableMeta table);
                    
        CommandMeta BuildGetByIdCommandMeta(TableMeta table);
    }
}
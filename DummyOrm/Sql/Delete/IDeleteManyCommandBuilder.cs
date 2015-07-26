using System;
using System.Linq.Expressions;

namespace DummyOrm.Sql.Delete
{
    public interface IDeleteManyCommandBuilder
    {
        Command.Command Build<T>(Expression<Func<T, bool>> filter) where T : class, new();
    }
}

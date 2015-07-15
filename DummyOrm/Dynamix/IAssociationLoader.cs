using System.Collections.Generic;
using DummyOrm.Db;

namespace DummyOrm.Dynamix
{
    public interface IAssociationLoader
    {
        void Load<T>(IList<T> entities, ICommandExecutor cmdExec) where T : class, new();
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DummyOrm.Db;

namespace DummyOrm.Dynamix
{
    public interface IAssociationLoader
    {
        void Load<T, TProp>(IList<T> entities, ICommandExecutor cmdExec, Expression<Func<TProp, object>> includeProps = null) 
            where T : class, new()
            where TProp : class, new();
    }
}
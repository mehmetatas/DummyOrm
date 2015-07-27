using DummyOrm.Dynamix;

namespace DummyOrm.Meta
{
    public interface IAssociationMeta
    {
        IAssociationLoader Loader { get; }
    }
}
using System.Collections.Generic;

namespace CatHut
{
    public interface IMasterData
    {
        string id { get; set; }
        object this[string propertyName] { get; set; }
        List<string> PropertyNames { get; }
    }
}
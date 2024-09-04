using System.Reflection;

namespace BergNoten.Interfaces
{
    public interface IExportable
    {
        public abstract List<PropertyInfo> GetProperties();
    }
}
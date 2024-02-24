using System.ComponentModel.DataAnnotations;

namespace MiniORM
{
    public class ChangeTracker<T> where T : class,new()
    {
        private readonly List<T> _allEntities;
        private readonly List<T> _added;
        private readonly List<T> _removed;
        public ChangeTracker(IEnumerable<T> entities)
        {
            _added = new List<T>();
            _removed = new List<T>();
            _allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> AllEntities => _allEntities.AsReadOnly();
        public IReadOnlyCollection<T> Added => _added.AsReadOnly();
        public IReadOnlyCollection<T> Removed => _removed.AsReadOnly();

        private List<T> CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntites = new List<T>();
            var propertiesToClone = typeof(T).GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType)).ToArray();

            foreach (var entity in entities)
            {
                var clonedEntity = Activator.CreateInstance<T>();
                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(clonedEntity, value);

                }
                clonedEntites.Add(clonedEntity);
            }
            return clonedEntites;
        }
        public void Add(T item)
        {
            _added.Add(item);
        }
        public void Remove(T item)
        {
            _removed.Add(item);
        }

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbset)
        {
            var modifiedEntities = new List<T>();
            var primaryKeys = typeof(T).GetProperties()
                .Where(p => p.HasAttribute<KeyAttribute>()).ToArray();
        }
    }
}
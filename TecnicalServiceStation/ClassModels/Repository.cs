using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecnicalServiceStation.ClassModels
{
    class Repository
    {
        public static void Delete<TEntity>(TEntity entity, DbContext context)
        where TEntity : class
        {
            // Настройки контекста
            context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            context.Entry<TEntity>(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }
        public static void Update<TEntity>(TEntity entity, DbContext context)
        where TEntity : class
        {
            // Настройки контекста
            context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            context.Entry<TEntity>(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
        public static void Insert<TEntity>(TEntity entity,DbContext context) 
        where TEntity : class
        {
            // Настройки контекста
            context.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            context.Entry(entity).State = EntityState.Added;
            context.SaveChanges();
        }
        public static void UndoingChangesDbContextLevel(DbContext context)
        {
            foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }  
    }
}

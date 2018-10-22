using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Data
{

    public interface IDistributedPizzaDbContext : IDisposable
    {
        IDbSet<Order> Orders { get; set; }
        IDbSet<Pizza> Pizzas { get; set; }
        IDbSet<Toppings> Toppings { get; set; }
        IDbSet<PrefixSeq> PrefixSeq { get; set; }
        int SaveChanges();
        Database Database { get; }
        void Refresh(RefreshMode refreshMode, Object entity);
    }

    public class DistributedPizzaDbContext : DbContext, IDistributedPizzaDbContext
    {
        public DistributedPizzaDbContext() : base("DistributedPizza")
        {
        }

        public IDbSet<Order> Orders { get; set; }
        public IDbSet<Pizza> Pizzas { get; set; }
        public IDbSet<Toppings> Toppings { get; set; }
        public IDbSet<PrefixSeq> PrefixSeq { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Pizza>()
                        .HasMany(p => p.Toppings)
                        .WithMany()
                        .Map(mc =>
                        {
                            mc.ToTable("PizzaToppings");
                            mc.MapLeftKey("Pizza_Id");
                            mc.MapRightKey("Toppings_Id");
                        });

            base.OnModelCreating(modelBuilder);
        }

        public void Refresh(RefreshMode refreshMode, object entity)
        {
            var context = ((IObjectContextAdapter)this).ObjectContext;
            context.Refresh(refreshMode, entity);
        }
    }
}

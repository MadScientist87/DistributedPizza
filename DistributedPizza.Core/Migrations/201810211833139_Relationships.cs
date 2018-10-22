namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Relationships : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pizza", "Toppings_Id", c => c.Int());
            CreateIndex("dbo.Pizza", "Toppings_Id");
            AddForeignKey("dbo.Pizza", "Toppings_Id", "dbo.Toppings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pizza", "Toppings_Id", "dbo.Toppings");
            DropIndex("dbo.Pizza", new[] { "Toppings_Id" });
            DropColumn("dbo.Pizza", "Toppings_Id");
        }
    }
}

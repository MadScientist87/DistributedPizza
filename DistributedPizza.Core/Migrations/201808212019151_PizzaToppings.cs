namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PizzaToppings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Toppings", "Pizza_Id", "dbo.Pizza");
            DropIndex("dbo.Toppings", new[] { "Pizza_Id" });
            CreateTable(
                "dbo.PizzaToppings",
                c => new
                    {
                        Pizza_Id = c.Int(nullable: false),
                        Toppings_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Pizza_Id, t.Toppings_Id })
                .ForeignKey("dbo.Pizza", t => t.Pizza_Id, cascadeDelete: true)
                .ForeignKey("dbo.Toppings", t => t.Toppings_Id, cascadeDelete: true)
                .Index(t => t.Pizza_Id)
                .Index(t => t.Toppings_Id);
            
            DropColumn("dbo.Toppings", "Pizza_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Toppings", "Pizza_Id", c => c.Int());
            DropForeignKey("dbo.PizzaToppings", "Toppings_Id", "dbo.Toppings");
            DropForeignKey("dbo.PizzaToppings", "Pizza_Id", "dbo.Pizza");
            DropIndex("dbo.PizzaToppings", new[] { "Toppings_Id" });
            DropIndex("dbo.PizzaToppings", new[] { "Pizza_Id" });
            DropTable("dbo.PizzaToppings");
            CreateIndex("dbo.Toppings", "Pizza_Id");
            AddForeignKey("dbo.Toppings", "Pizza_Id", "dbo.Pizza", "Id");
        }
    }
}

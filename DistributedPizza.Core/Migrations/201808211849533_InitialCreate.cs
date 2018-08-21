namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderReferenceId = c.String(),
                        CustomerName = c.String(),
                        CustomerPhone = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pizza",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SauceType = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.Toppings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Pizza_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pizza", t => t.Pizza_Id)
                .Index(t => t.Pizza_Id);
            
            CreateTable(
                "dbo.PrefixSeq",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdType = c.Int(nullable: false),
                        Prefix = c.String(nullable: false, maxLength: 32),
                        Seq = c.Int(nullable: false),
                        AlphaPrefix = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pizza", "Order_Id", "dbo.Order");
            DropForeignKey("dbo.Toppings", "Pizza_Id", "dbo.Pizza");
            DropIndex("dbo.Toppings", new[] { "Pizza_Id" });
            DropIndex("dbo.Pizza", new[] { "Order_Id" });
            DropTable("dbo.PrefixSeq");
            DropTable("dbo.Toppings");
            DropTable("dbo.Pizza");
            DropTable("dbo.Order");
        }
    }
}

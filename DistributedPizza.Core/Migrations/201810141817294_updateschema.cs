namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateschema : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Pizza", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pizza", "Status");
            DropColumn("dbo.Order", "Status");
        }
    }
}

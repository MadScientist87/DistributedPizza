namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "CreateDate");
        }
    }
}

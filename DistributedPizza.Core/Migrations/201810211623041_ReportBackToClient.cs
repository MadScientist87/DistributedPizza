namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportBackToClient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "ReportBackToClient", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "ReportBackToClient");
        }
    }
}

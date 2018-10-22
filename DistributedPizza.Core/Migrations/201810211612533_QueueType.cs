namespace DistributedPizza.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QueueType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "QueueType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "QueueType");
        }
    }
}

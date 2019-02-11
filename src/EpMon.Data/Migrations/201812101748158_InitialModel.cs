namespace EpMon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Endpoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckInterval = c.Int(nullable: false),
                        CheckType = c.Int(nullable: false),
                        Url = c.String(),
                        Tags = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EndpointStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResponseTime = c.Long(nullable: false),
                        Status = c.Int(nullable: false),
                        IsHealthy = c.Boolean(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        Message = c.String(),
                        EndpointId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Endpoints", t => t.EndpointId, cascadeDelete: true)
                .Index(t => t.EndpointId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EndpointStats", "EndpointId", "dbo.Endpoints");
            DropIndex("dbo.EndpointStats", new[] { "EndpointId" });
            DropTable("dbo.EndpointStats");
            DropTable("dbo.Endpoints");
        }
    }
}

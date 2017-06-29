namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenericAttributeTblAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GenericAttributes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntityKey = c.String(),
                        EntityValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GenericAttributes");
        }
    }
}

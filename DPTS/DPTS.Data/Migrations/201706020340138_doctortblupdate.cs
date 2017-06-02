namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doctortblupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "IsEMailConsult", c => c.Boolean(nullable: false));
            AddColumn("dbo.Doctors", "EmailConsultFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Doctors", "EmailConsultFee");
            DropColumn("dbo.Doctors", "IsEMailConsult");
        }
    }
}

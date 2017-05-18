namespace DPTS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSession1Session2Appoinment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "SessionOneStartTime", c => c.String());
            AddColumn("dbo.Schedules", "SessionOneEndTime", c => c.String());
            AddColumn("dbo.Schedules", "SessionTwoStartTime", c => c.String());
            AddColumn("dbo.Schedules", "SessionTwoEndTime", c => c.String());
            DropColumn("dbo.Schedules", "StartTime");
            DropColumn("dbo.Schedules", "EndTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "EndTime", c => c.String());
            AddColumn("dbo.Schedules", "StartTime", c => c.String());
            DropColumn("dbo.Schedules", "SessionTwoEndTime");
            DropColumn("dbo.Schedules", "SessionTwoStartTime");
            DropColumn("dbo.Schedules", "SessionOneEndTime");
            DropColumn("dbo.Schedules", "SessionOneStartTime");
        }
    }
}

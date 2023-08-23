namespace PetStore.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mgtPetEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 120),
                        DateOfBirth = c.DateTime(nullable: false, storeType: "date"),
                        Weight = c.Decimal(nullable: false, precision: 4, scale: 2),
                        Type = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Name, t.DateOfBirth }, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Pets", new[] { "Name", "DateOfBirth" });
            DropTable("dbo.Pets");
        }
    }
}

namespace Diseño.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cuenta", "PasivoCirculante", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Metodologia", "FechaInicio", c => c.DateTime(nullable: true));
            AddColumn("dbo.Metodologia", "FechaFin", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cuenta", "PasivoCirculante");
            DropColumn("dbo.Metodologia", "FechaInicio");
            DropColumn("dbo.Metodologia", "FechaFin");
        }
    }
}

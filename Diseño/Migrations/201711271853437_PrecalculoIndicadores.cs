namespace Diseño.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrecalculoIndicadores : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IndicadorCuentaValor",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cuenta_ID = c.Int(),
                        Indicador_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cuenta", t => t.Cuenta_ID)
                .ForeignKey("dbo.Indicador", t => t.Indicador_ID)
                .Index(t => t.Cuenta_ID)
                .Index(t => t.Indicador_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IndicadorCuentaValor", "Indicador_ID", "dbo.Indicador");
            DropForeignKey("dbo.IndicadorCuentaValor", "Cuenta_ID", "dbo.Cuenta");
            DropIndex("dbo.IndicadorCuentaValor", new[] { "Indicador_ID" });
            DropIndex("dbo.IndicadorCuentaValor", new[] { "Cuenta_ID" });
            DropTable("dbo.IndicadorCuentaValor");
        }
    }
}

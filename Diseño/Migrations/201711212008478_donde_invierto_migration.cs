namespace Diseño.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class donde_invierto_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cuenta",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Fecha = c.DateTime(nullable: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Empresa_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Empresa", t => t.Empresa_ID)
                .Index(t => t.Empresa_ID);
            
            CreateTable(
                "dbo.Empresa",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Indicador",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Formula = c.String(nullable: false),
                        Descripcion = c.String(),
                        Tipo = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Metodologia",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 50),
                        Formula = c.String(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cuenta", "Empresa_ID", "dbo.Empresa");
            DropIndex("dbo.Cuenta", new[] { "Empresa_ID" });
            DropTable("dbo.Usuario");
            DropTable("dbo.Metodologia");
            DropTable("dbo.Indicador");
            DropTable("dbo.Empresa");
            DropTable("dbo.Cuenta");
        }
    }
}

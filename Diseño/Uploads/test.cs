Cuenta OtraCuenta = new Cuenta();
db.Entry(OtraCuenta).State = EntityState.Modified;
string NombreEmpresa = row.Split(',')[0];
string NombreCuenta = row.Split(',')[1];
OtraCuenta = db.Cuentas.Where(e => e.Nombre == NombreCuenta).FirstOrDefault();
if (OtraCuenta.Nombre == null)
{
    //La cuenta no existe y se crea una nueva
    Cuenta NewCuenta = db.Cuentas.Find(OtraCuenta.Nombre);
    db.Entry(NewCuenta).State = EntityState.Modified;
    NewCuenta.Empresa = db.Empresas.Where(e => e.Nombre == NombreEmpresa).FirstOrDefault();
    if (NewCuenta.Empresa == null)
    {
        TempData["msgExpresionNoValida"] = "<script>alert('Empresa inexistente');</script>";
    }
    NewCuenta.Fecha = Convert.ToDateTime(row.Split(',')[2]);
    NewCuenta.Valor = Convert.ToDecimal(row.Split(',')[3]);
    db.SaveChanges();
    return RedirectToAction("Index");
}
else
{
    //La cuenta existe y se actualiza
    Cuenta cuenta = new Cuenta();
    db.Entry(cuenta).State = EntityState.Modified;
    cuenta.Empresa = db.Empresas.Where(e => e.Nombre == NombreEmpresa).FirstOrDefault();
    if (cuenta.Empresa == null)
    {
        TempData["msgExpresionNoValida"] = "<script>alert('Empresa inexistente');</script>";
    }
    cuenta.Nombre = row.Split(',')[1];
    cuenta.Fecha = Convert.ToDateTime(row.Split(',')[2]);
    cuenta.Valor = Convert.ToDecimal(row.Split(',')[3]);
    db.Cuentas.Add(cuenta);
    db.SaveChanges();
    return RedirectToAction("Index");
}
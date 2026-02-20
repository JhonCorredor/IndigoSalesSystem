# IndiGO Sales System - Configuración de Base de Datos

## 🗄️ Configuración de SQL Server

El proyecto está configurado para trabajar con **SQL Server** en tres ambientes:

### 1️⃣ Desarrollo Local (Recomendado)

Usa **SQL Server LocalDB** (incluido con Visual Studio, sin instalación adicional):

```json
// appsettings.Development.json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IndigoSalesDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Aplicar migraciones:**
```powershell
dotnet ef database update -p Infrastructure -s Presentation.Api
```

---

### 2️⃣ Azure SQL Database (Producción)

Para Azure SQL Database Serverless (low-cost con créditos gratis):

```json
// appsettings.json (o variables de entorno)
"ConnectionStrings": {
  "DefaultConnection": "Server=indigo-sales-server.database.windows.net;Database=IndigoSalesDb;User Id=sqladmin;Password=TU_PASSWORD;TrustServerCertificate=True;"
}
```

**Crear Azure SQL Database:**
```powershell
# 1. Crear grupo de recursos
az group create --name rg-indigo-sales --location "East US"

# 2. Crear servidor SQL
az sql server create `
  --name indigo-sales-server `
  --resource-group rg-indigo-sales `
  --location "East US" `
  --admin-user sqladmin `
  --admin-password "TuPassword123!"

# 3. Crear DB Serverless (low-cost)
az sql db create `
  -g rg-indigo-sales `
  -s indigo-sales-server `
  -n IndigoSalesDb `
  -e GeneralPurpose `
  --compute-model Serverless `
  -f Gen5 `
  --min-capacity 0.5 `
  -c 2 `
  --auto-pause-delay 60

# 4. Configurar firewall
az sql server firewall-rule create `
  --resource-group rg-indigo-sales `
  --server indigo-sales-server `
  -n AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0

# 5. Permitir tu IP
$myIp = (Invoke-RestMethod -Uri "https://api.ipify.org")
az sql server firewall-rule create `
  --resource-group rg-indigo-sales `
  --server indigo-sales-server `
  -n AllowMyIp `
  --start-ip-address $myIp `
  --end-ip-address $myIp
```

**Aplicar migraciones a Azure:**
```powershell
dotnet ef database update -p Infrastructure -s Presentation.Api --connection "Server=indigo-sales-server.database.windows.net;Database=IndigoSalesDb;User Id=sqladmin;Password=TuPassword123!;TrustServerCertificate=True;"
```

---

### 3️⃣ SQL Server Local (Instalación completa)

Si tienes SQL Server instalado localmente:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=IndigoSalesDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

O con autenticación SQL:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=IndigoSalesDb;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
}
```

---

## 🔧 Comandos Útiles de EF Core

### Crear nueva migración
```powershell
dotnet ef migrations add NombreMigracion -p Infrastructure -s Presentation.Api
```

### Aplicar migraciones
```powershell
dotnet ef database update -p Infrastructure -s Presentation.Api
```

### Revertir última migración
```powershell
dotnet ef migrations remove -p Infrastructure -s Presentation.Api
```

### Ver migraciones aplicadas
```powershell
dotnet ef migrations list -p Infrastructure -s Presentation.Api
```

### Generar script SQL
```powershell
dotnet ef migrations script -p Infrastructure -s Presentation.Api -o migration.sql
```

---

## 👥 Usuarios de Prueba (Seed Data)

La base de datos incluye usuarios de prueba (password: **Password123!** para todos):

| Usuario | Email | Rol | Permisos |
|---------|-------|-----|----------|
| `admin` | admin@indigosales.com | Administrator | Full access |
| `manager` | manager@indigosales.com | Manager | Gestión completa |
| `seller1` | seller1@indigosales.com | Seller | Ventas y productos |
| `seller2` | seller2@indigosales.com | Seller | Ventas y productos |
| `viewer` | viewer@indigosales.com | Viewer | Solo lectura |
| `guest` | guest@indigosales.com | Guest | Limitado |

---

## 💰 Costos Estimados Azure SQL Serverless

| Característica | Valor |
|---|---|
| **Tier** | General Purpose Serverless |
| **vCores** | 0.5 - 2 (auto-scaling) |
| **Auto-pause** | 60 min inactividad |
| **Almacenamiento** | 32 GB max (incluido) |
| **Costo estimado** | ~$5-15 USD/mes |
| **Sin actividad** | $0 (cuando pausada) |

---

## ⚠️ Solución de Problemas

### Error: "Cannot open database"
1. Verifica que la cadena de conexión sea correcta
2. Asegúrate de haber ejecutado `dotnet ef database update`
3. Si usas Azure SQL, verifica las reglas de firewall

### Error: "Login failed"
1. Revisa el usuario y contraseña en la cadena de conexión
2. En Azure, verifica que el usuario SQL esté habilitado
3. Para LocalDB, usa `Trusted_Connection=True`

### Error 500 al llamar API
1. Revisa los logs en `Output > ASP.NET Core Web Server`
2. Verifica que las migraciones estén aplicadas
3. Comprueba la conectividad a la base de datos

### Regenerar base de datos desde cero
```powershell
# Eliminar base de datos
dotnet ef database drop -p Infrastructure -s Presentation.Api --force

# Recrear
dotnet ef database update -p Infrastructure -s Presentation.Api
```

---

## 📚 Referencias

- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Azure SQL Database](https://azure.microsoft.com/en-us/products/azure-sql/database/)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)

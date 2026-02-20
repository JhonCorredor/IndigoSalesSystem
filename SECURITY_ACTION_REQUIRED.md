# 🔐 ACCIÓN URGENTE DE SEGURIDAD

## ⚠️ CREDENCIALES EXPUESTAS EN GITHUB

He detectado que tu repositorio **público** contiene credenciales sensibles en `appsettings.json`:

### 🔴 Credenciales comprometidas:
1. **SQL Server Password**: `p835AB_6&I"K`
2. **Azure Storage Key**: `RWUsQRanaj9rKzX4qLPHsg/bx9rVNFi2Ga+mDpJbjjWaDlAFw92rfv/3ITZg8BnzicIL4ESux4VK+AStuLTqwQ==`
3. **JWT Secret**: `EstaEsUnaClaveSecretaSuperSeguraParaIndiGO2026+`

---

## ✅ LO QUE YA HICE:

1. ✅ Agregué `appsettings.json` al `.gitignore`
2. ✅ Removí `appsettings.json` del tracking de Git
3. ✅ Creé un template `appsettings.json.template` sin credenciales

---

## 🚨 LO QUE DEBES HACER AHORA (URGENTE):

### 1. Regenerar las credenciales comprometidas

#### A. Cambiar password de SQL Server:
```powershell
az sql server update `
  --name ss-indigo `
  --resource-group rg-indigo-sales `
  --admin-password "NUEVA_PASSWORD_SEGURA_123!"
```

#### B. Regenerar Azure Storage Key:
```powershell
az storage account keys renew `
  --account-name indigosalesstorage `
  --resource-group rg-indigo-sales `
  --key primary
```

Luego obtener la nueva key:
```powershell
az storage account show-connection-string `
  --name indigosalesstorage `
  --resource-group rg-indigo-sales `
  --output tsv
```

#### C. Cambiar JWT Secret:
Genera una nueva clave de al menos 32 caracteres aleatorios.

### 2. Actualizar tu appsettings.json local

Actualiza tu `appsettings.json` local (que ahora está ignorado por Git) con las **nuevas credenciales**.

### 3. Hacer commit de los cambios de seguridad

```powershell
git add .gitignore
git add Presentation.Api\appsettings.json.template
git commit -m "security: Remove appsettings.json from tracking and add template"
git push origin develop
```

### 4. OPCIONAL: Limpiar el historial de Git (Avanzado)

Si quieres eliminar completamente las credenciales del historial:

```powershell
# ⚠️ CUIDADO: Esto reescribe el historial
git filter-branch --force --index-filter `
  "git rm --cached --ignore-unmatch Presentation.Api/appsettings.json" `
  --prune-empty --tag-name-filter cat -- --all

# Force push (reescribe la rama remota)
git push origin --force --all
```

---

## 📋 Checklist de Seguridad

- [ ] Regenerar password de SQL Server en Azure
- [ ] Regenerar Azure Storage Access Key
- [ ] Generar nuevo JWT Secret
- [ ] Actualizar `appsettings.json` local con nuevas credenciales
- [ ] Commit y push de `.gitignore` actualizado
- [ ] Verificar que `appsettings.json` NO aparezca en el próximo commit
- [ ] (Opcional) Limpiar historial de Git

---

## 🔒 Mejores Prácticas para el Futuro

### 1. Usar User Secrets para desarrollo local

```powershell
cd Presentation.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "TU_CONNECTION_STRING"
dotnet user-secrets set "ConnectionStrings:BlobStorage" "TU_BLOB_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "TU_JWT_SECRET"
```

### 2. Usar Azure Key Vault en producción

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
}
```

### 3. Usar variables de entorno

En Azure App Service, configura las connection strings como "Application Settings" en lugar de incluirlas en el código.

---

## 📚 Referencias

- [ASP.NET Core User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/)
- [GitHub Secret Scanning](https://docs.github.com/en/code-security/secret-scanning)

---

## ⏰ Tiempo estimado: 10-15 minutos

**Prioridad:** 🔴 CRÍTICA - Hazlo lo antes posible para evitar accesos no autorizados a tus recursos de Azure.

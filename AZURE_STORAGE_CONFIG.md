# Guía: Configurar Azure Blob Storage

## ✅ Storage Account Creado

**Nombre:** `indigosalesstorage`  
**Resource Group:** `rg-indigo-sales`  
**Región:** East US  
**Tier:** Standard LRS (Hot)

---

## 🔑 Obtener Connection String

### Desde Azure Portal:

1. Ve a [Azure Portal](https://portal.azure.com)
2. Busca **"indigosalesstorage"**
3. Click en **Access keys** (menú izquierdo)
4. Click en **"Show"** junto a key1
5. Copia el **Connection string** completo

Ejemplo:
```
DefaultEndpointsProtocol=https;AccountName=indigosalesstorage;AccountKey=TU_KEY_AQUI;EndpointSuffix=core.windows.net
```

### Desde Azure CLI:

```powershell
az storage account show-connection-string `
  --name indigosalesstorage `
  --resource-group rg-indigo-sales `
  --output tsv
```

---

## 🔓 IMPORTANTE: Habilitar Acceso Público a Blobs

Por defecto, tu Storage Account tiene `allowBlobPublicAccess: false`. Para que las imágenes de productos sean accesibles públicamente:

### Opción 1: Azure Portal
1. Ve a tu Storage Account → **Configuration**
2. Busca **"Allow Blob public access"**
3. Cambia a **Enabled**
4. Click **Save**

### Opción 2: Azure CLI
```powershell
az storage account update `
  --name indigosalesstorage `
  --resource-group rg-indigo-sales `
  --allow-blob-public-access true
```

---

## 🔑 Configurar Connection String

La cadena de conexión tiene este formato:
```
DefaultEndpointsProtocol=https;AccountName=indigosalestorage;AccountKey=TU_KEY_AQUI;EndpointSuffix=core.windows.net
```

### Agregar a appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=indigosalestorage;AccountKey=TU_KEY;EndpointSuffix=core.windows.net"
  }
}
```

### Para Desarrollo Local (Opcional)

Si solo quieres probar localmente sin Azure, deja la cadena vacía en `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "BlobStorage": ""
  }
}
```

La app usará automáticamente `MockBlobStorageService` que guarda archivos localmente.

---

## 💰 Costos de Azure Storage

| Característica | Costo Estimado |
|---|---|
| **Almacenamiento (Hot tier)** | ~$0.018 por GB/mes |
| **Operaciones de escritura** | ~$0.05 por 10,000 |
| **Operaciones de lectura** | ~$0.004 por 10,000 |
| **Estimado mensual (uso ligero)** | ~$1-3 USD/mes |

Con 1 GB de imágenes y uso moderado, gastará menos de $2/mes.

---

## 🧪 Probar que funciona

### 1. Subir una imagen vía API

```http
POST https://localhost:44377/api/Products
Content-Type: multipart/form-data

{
  "name": "Producto Test",
  "price": 100,
  "stock": 10,
  "image": [archivo.jpg]
}
```

### 2. Verificar en Azure Portal

1. Ve a tu Storage Account
2. Click en "Containers"
3. Deberías ver el container `product-images`
4. Dentro estarán tus imágenes subidas

---

## 🔧 Comandos útiles

### Ver contenedores
```powershell
az storage container list `
  --account-name indigosalestorage `
  --query "[].name" `
  --output table
```

### Ver archivos en un contenedor
```powershell
az storage blob list `
  --account-name indigosalestorage `
  --container-name product-images `
  --query "[].{Name:name, Size:properties.contentLength}" `
  --output table
```

### Eliminar un blob
```powershell
az storage blob delete `
  --account-name indigosalestorage `
  --container-name product-images `
  --name nombre-archivo.jpg
```

---

## ⚠️ Solución de Problemas

### Error: "Settings must be of the form 'name=value'"
✅ **Solución:** La cadena de conexión está mal formateada o vacía.
- Verifica que copiaste la cadena completa desde Azure
- No debe tener espacios extras ni saltos de línea
- Debe empezar con `DefaultEndpointsProtocol=https`

### Error: "The specified container does not exist"
✅ **Solución:** El container se crea automáticamente en el primer upload.
- Intenta subir una imagen vía POST /api/Products
- O créalo manualmente:
```powershell
az storage container create `
  --name product-images `
  --account-name indigosalestorage `
  --public-access blob
```

### Error: "Server failed to authenticate the request"
✅ **Solución:** La key de acceso es incorrecta
- Ve a Azure Portal → Storage Account → Access keys
- Regenera una key si es necesario
- Copia la nueva connection string

### Para desarrollo sin Azure (Mock)
Si no quieres usar Azure durante desarrollo, simplemente:
1. Deja vacía `"BlobStorage": ""` en `appsettings.Development.json`
2. La app usará automáticamente `MockBlobStorageService`
3. Los archivos se guardarán en `wwwroot/uploads/` localmente

---

## 🔐 Seguridad

### ❌ NO hagas esto:
- Subir `appsettings.json` con connection strings reales a Git
- Compartir tu connection string públicamente
- Usar el mismo connection string en producción y desarrollo

### ✅ Haz esto:
- Usa **User Secrets** para desarrollo local:
```powershell
dotnet user-secrets set "ConnectionStrings:BlobStorage" "TU_CONNECTION_STRING"
```

- Usa **Azure Key Vault** en producción
- Agrega `appsettings.*.json` a `.gitignore` si contienen secrets

---

## 📚 Referencias
- [Azure Blob Storage Docs](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- [Azure Storage Pricing](https://azure.microsoft.com/en-us/pricing/details/storage/blobs/)

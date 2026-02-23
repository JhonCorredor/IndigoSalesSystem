# 🛒 IndiGO Sales System - Backend API

Sistema de gestión de ventas desarrollado con **.NET 8** siguiendo principios de **Clean Architecture** y **Domain-Driven Design (DDD)**.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Cloud-0078D4?logo=microsoft-azure)](https://azure.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Prerrequisitos](#-prerrequisitos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Testing](#-testing)
- [Despliegue](#-despliegue)
- [API Endpoints](#-api-endpoints)
- [Patrones y Prácticas](#-patrones-y-prácticas)

---

## ✨ Características

### 🔐 Seguridad
- Autenticación JWT 
- Encriptación de contraseñas con **BCrypt**
- Manejo global de excepciones con middleware personalizado

### 📦 Gestión de Productos
- CRUD completo de productos
- Subida de imágenes a **Azure Blob Storage**
- Control de stock con validación de negocio
- Soft delete (desactivación lógica)
- Paginación y filtrado 

### 💰 Gestión de Ventas
- Registro de ventas con múltiples items
- Validación de stock en tiempo real
- Cálculo automático de totales
- Patrón de resiliencia con **Polly** (retry con exponential backoff)


### 🏗️ Arquitectura
- **Clean Architecture** (Cebolla)
- **Domain-Driven Design (DDD)**
- **Abstract Factory Pattern** para repositorios genéricos
- **Repository Pattern** con Unit of Work
- **Dependency Injection** nativo de .NET

---

## 🏛️ Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation.Api                         │
│  (Controllers, Middleware, Program.cs)                      │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│               Core.Application                              │
│  (Interfaces, Services, DTOs, Common)                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                 Core.Domain                                 │
│  (Entities, Value Objects, Domain Logic)                    │
└─────────────────────────────────────────────────────────────┘
                       ▲
┌──────────────────────┴──────────────────────────────────────┐
│                Infrastructure                               │
│  (Repositories, DbContext, External Services)               │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│              Shared.Utilities                               │
│  (Exceptions, Mappers, Middleware, Extensions)              │
└─────────────────────────────────────────────────────────────┘
```

### Capas del Proyecto

| Capa | Responsabilidad | Dependencias |
|------|----------------|--------------|
| **Presentation.Api** | Controllers, configuración, middleware | Core.Application, Infrastructure, Shared.Utilities |
| **Core.Application** | Casos de uso, interfaces, DTOs | Core.Domain |
| **Core.Domain** | Entidades, lógica de dominio | Ninguna |
| **Infrastructure** | Acceso a datos, servicios externos | Core.Application, Shared.Utilities |
| **Shared.Utilities** | Helpers, excepciones, mappers | Core.Application, Core.Domain |
| **Tests.Unit** | Pruebas unitarias | Todos los proyectos |

---

## 🛠️ Tecnologías

### Backend
- **.NET 8** - Framework principal
- **C# 12** - Lenguaje
- **ASP.NET Core** - Web API
- **Entity Framework Core 8** - ORM
- **SQL Server** - Base de datos relacional

### Azure Services
- **Azure SQL Database** - Base de datos en la nube
- **Azure Blob Storage** - Almacenamiento de imágenes

### Bibliotecas y Paquetes
- **BCrypt.Net** - Encriptación de contraseñas
- **Polly** - Resiliencia y retry policies
- **Swashbuckle** - Documentación OpenAPI/Swagger
- **xUnit** - Framework de testing
- **Moq** - Mocking para tests
---

## 📁 Estructura del Proyecto

```
IndigoSalesSystem/
├── Core.Application/
│   ├── Common/             # DTOs base, filtros, paginación
│   ├── DTOs/               # Data Transfer Objects
│   ├── Interfaces/         # Contratos de repositorios y servicios
│   └── Services/           # Lógica de aplicación
│
├── Core.Domain/
│   ├── Common/             # BaseEntity abstracto
│   └── Entities/           # Entidades de dominio
│       ├── Product.cs
│       ├── Sale.cs
│       ├── SaleItem.cs
│       ├── User.cs
│       └── Role.cs
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/  # Seed data
│   │   └── Migrations/
│   ├── Repositories/
│   │   ├── ABaseRepository.cs  # Repositorio genérico
│   │   ├── ProductRepository.cs
│   │   ├── SaleRepository.cs
│   │   ├── UserRepository.cs
│   │   └── RoleRepository.cs
│   ├── Service/
│   │   ├── AzureBlobStorageService.cs
│   │   └── MockBlobStorageService.cs
│   └── modules/            # Bicep IaC
│
├── Presentation.Api/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   └── SalesController.cs
│   ├── Program.cs
│   └── appsettings.json
│
├── Shared.Utilities/
│   ├── Exceptions/         # Excepciones personalizadas
│   ├── Middleware/         # ExceptionMiddleware
│   ├── Mappers/            # IMapper<T, D>
│   ├── Responses/          # ApiResponse<T>
│   └── Extensions/         # Extension methods
│
└── Tests.Unit/
    ├── Application/Services/
    └── Infrastructure/Services/
```

---

## 📦 Prerrequisitos

### Obligatorios
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) o [Azure SQL Database](https://azure.microsoft.com/services/sql-database/)
- [Git](https://git-scm.com/)

### Opcionales
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- [Azure CLI](https://docs.microsoft.com/cli/azure/) para despliegue
- [Azurite](https://github.com/Azure/Azurite) para desarrollo local con Blob Storage
- [Postman](https://www.postman.com/) o similar para testing de API

---

## 🚀 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/JhonCorredor/IndigoSalesSystem.git
cd IndigoSalesSystem
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar base de datos

Actualiza `Presentation.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=IndigoSalesDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "BlobStorage": ""
  }
}
```

### 4. Aplicar migraciones

```bash
cd Presentation.Api
dotnet ef database update --project ../Infrastructure
```

---

## ⚙️ Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tu-servidor;Database=IndigoSalesDb;User Id=tu-usuario;Password=tu-password;",
    "BlobStorage": "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;"
  },
  "Jwt": {
    "Key": "TuClaveSecretaSuperSegura256BitsMinimo!!!",
    "Issuer": "IndigoSalesApi",
    "Audience": "IndigoSalesClient"
  },
  "BlobStorageSettings": {
    "ContainerName": "product-images",
    "AccountName": "tuStorageAccount"
  }
}
```

### Variables de Entorno (Producción)

```bash
# Azure App Service Configuration
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<azure-sql-connection>
ConnectionStrings__BlobStorage=<azure-storage-connection>
Jwt__Key=<secret-key-from-keyvault>
```

---

## ▶️ Ejecución

### Desarrollo Local

```bash
# Ejecutar la API
cd Presentation.Api
dotnet run

# O con hot reload
dotnet watch run
```

La API estará disponible en:
- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`
- **Swagger:** `https://localhost:5001/swagger`

### Compilación

```bash
# Debug
dotnet build

# Release
dotnet build -c Release

# Publicar
dotnet publish -c Release -o ./publish
```

---

## 🧪 Testing

### Ejecutar todas las pruebas

```bash
dotnet test
```

### Con cobertura de código

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Pruebas específicas

```bash
# Solo pruebas de servicios
dotnet test --filter FullyQualifiedName~Services

# Solo MockBlobStorageService
dotnet test --filter FullyQualifiedName~MockBlobStorageServiceTests
```

### Verbosidad detallada

```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 🌐 Despliegue

### Azure (usando Bicep)

```bash
# Login a Azure
az login

# Crear resource group
az group create --name rg-indigo-sales --location eastus

# Desplegar infraestructura
az deployment group create \
  --resource-group rg-indigo-sales \
  --template-file Infrastructure/main.bicep \
  --parameters Infrastructure/parameters/dev.parameters.json
---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| POST | `/api/Auth/login` | Iniciar sesión | ❌ |
| POST | `/api/Auth/register` | Registrar usuario | ❌ |

**Login Request:**
```json
{
  "username": "super",
  "password": "super123"
}
```

### 📦 Products

| Method | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/Products` | Listar productos | ✅ |
| GET | `/api/Products/{id}` | Obtener producto | ✅ |
| POST | `/api/Products` | Crear producto | ✅ |
| PUT | `/api/Products/{id}` | Actualizar producto | ✅ |
| DELETE | `/api/Products/{id}` | Eliminar producto | ✅ |

**Create Product (multipart/form-data):**
```
Name: Laptop HP Pavilion
Price: 1299.99
Stock: 10
Image: [archivo]
```

### 💰 Sales

| Method | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/Sales` | Listar ventas | ✅ |
| GET | `/api/Sales/{id}` | Obtener venta | ✅ |
| POST | `/api/Sales` | Registrar venta | ✅ |

**Create Sale:**
```json
{
  "items": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "quantity": 2
    }
  ]
}
```

---

## 🎯 Patrones y Prácticas

### ✅ Implementados

| Patrón | Descripción | Ubicación |
|--------|-------------|-----------|
| **Clean Architecture** | Separación en capas independientes | Toda la solución |
| **Domain-Driven Design** | Lógica de negocio en entidades | `Core.Domain/Entities` |
| **Repository Pattern** | Abstracción de acceso a datos | `Infrastructure/Repositories` |
| **Abstract Factory** | Repositorio genérico base | `ABaseRepository<T, D>` |
| **Dependency Injection** | IoC nativo de .NET | `Program.cs` |
| **Unit of Work** | Transacciones coordinadas | `SaveChangesAsync()` |
| **Middleware Pattern** | Manejo global de excepciones | `ExceptionMiddleware` |
| **Strategy Pattern** | Azure/Mock storage | `IFileStorageService` |
| **Mapper Pattern** | Mapeo Entity → DTO | `Shared.Utilities/Mappers` |
| **Retry Pattern** | Resiliencia con Polly | `SaleService` |

### 🔒 Seguridad

- ✅ Validación de entrada en todos los endpoints
- ✅ Parametrized queries (EF Core)
- ✅ HTTPS obligatorio en producción
- ✅ JWT
- ✅ BCrypt para hash de contraseñas
- ✅ Soft delete para auditoría

### 📊 Buenas Prácticas

- ✅ Código autodocumentado con XML comments
- ✅ Validaciones de dominio en entidades
- ✅ DTOs separados de entidades
- ✅ Excepciones personalizadas por tipo
- ✅ Respuestas API estandarizadas
- ✅ Logging estructurado
- ✅ Tests unitarios 
- ✅ Migraciones versionadas
- ✅ Seed data para desarrollo

---

## 👥 Seed Data (Desarrollo)

### Usuarios por defecto

| Username | Password | Rol | Email |
|----------|----------|-----|-------|
| `super` | `super123` | Super | super@indigosales.com |
| `manager` | `admin123` | Manager | manager@indigosales.com |
| `seller1` | `admin123` | Seller | seller1@indigosales.com |
| `seller2` | `admin123` | Seller | seller2@indigosales.com |
| `viewer` | `admin123` | Viewer | viewer@indigosales.com |

### Productos de ejemplo

- 7 productos precargados (Laptop, Mouse, Teclado, Monitor, Audífonos, Webcam, SSD)
- 5 ventas de ejemplo con items relacionados

---


## 📝 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

---

## 👨‍💻 Autor

**Jhon Corredor**

- GitHub: [@JhonCorredor](https://github.com/JhonCorredor)
- LinkedIn: [Jhon Corredor](https://linkedin.com/in/jhoncorredor)

---

## 🙏 Agradecimientos

- [.NET Foundation](https://dotnetfoundation.org/)
- [Microsoft Azure](https://azure.microsoft.com/)
- [Clean Architecture - Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture)
- [Domain-Driven Design - Eric Evans](https://www.domainlanguage.com/ddd/)

---

⭐ Si este proyecto te fue útil, ¡dale una estrella en GitHub!

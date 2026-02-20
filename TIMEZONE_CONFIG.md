# ⏰ Configuración de Zona Horaria - Colombia

## 📍 Zona Horaria Configurada

**Zona horaria:** America/Bogota (Colombia Standard Time)  
**Offset UTC:** -5 horas  
**Horario de verano:** No aplica (Colombia no usa DST)

---

## 🔧 Implementación

### 1. Almacenamiento en Base de Datos

- **Todas las fechas se almacenan en UTC** en SQL Server
- Esto garantiza consistencia y facilita operaciones globales
- Las columnas DateTime usan tipo `datetime2` para mayor precisión

### 2. Conversión Automática en la API

- **Entrada:** Las fechas del frontend se asumen en hora de Colombia
- **Procesamiento:** Se convierten a UTC para consultar la BD
- **Salida:** Se convierten de UTC a hora de Colombia para la respuesta

### 3. Servicio de Zona Horaria

El servicio `ColombiaTimeZone` proporciona:

```csharp
// Obtener hora actual de Colombia
var now = ColombiaTimeZone.Now;

// Convertir de UTC a Colombia
var colombiaTime = ColombiaTimeZone.ConvertFromUtc(utcDateTime);

// Convertir de Colombia a UTC
var utcTime = ColombiaTimeZone.ConvertToUtc(colombiaDateTime);
```

---

## 📋 Ejemplos de Uso

### Filtrar ventas por rango de fechas

**Request (hora de Colombia):**
```
GET /api/Sales/report?startDate=2026-02-01&endDate=2026-02-28&page=1&pageSize=10
```

**Proceso interno:**
1. Recibe `2026-02-01 00:00:00` (hora de Colombia)
2. Convierte a `2026-02-01 05:00:00 UTC` para consultar BD
3. Busca ventas entre `2026-02-01 05:00:00 UTC` y `2026-03-01 04:59:59 UTC`
4. Convierte fechas de resultados a hora de Colombia

**Response:**
```json
{
  "totalRecords": 11,
  "filterApplied": {
    "startDate": "2026-02-01T00:00:00",
    "endDate": "2026-02-28T23:59:59",
    "timeZone": "America/Bogota (UTC-5)"
  },
  "data": [
    {
      "id": "...",
      "date": "2026-02-19T21:24:04",  // ← Hora de Colombia
      "total": 229.98
    }
  ]
}
```

---

## 🗄️ Base de Datos

### Ejemplo de datos almacenados

| Campo | Valor en BD (UTC) | Valor en API (Colombia) |
|---|---|---|
| Sale.Date | 2026-02-20 02:24:04.000 | 2026-02-19 21:24:04 |
| CreatedAt | 2026-02-20 03:00:00.000 | 2026-02-19 22:00:00 |

**Nota:** La diferencia de 5 horas es automática y transparente.

---

## ⚠️ Consideraciones Importantes

### 1. Frontend debe enviar fechas en hora local
El backend asume que todas las fechas entrantes están en **hora de Colombia**.

**Correcto:**
```javascript
// JavaScript/Angular
const startDate = new Date(2026, 1, 1); // 1 de febrero, medianoche hora local
```

**Incorrecto:**
```javascript
const startDate = new Date('2026-02-01T00:00:00Z'); // La 'Z' indica UTC
```

### 2. Migración de datos existentes

Si tienes datos con fechas incorrectas:

```sql
-- Ver todas las fechas actuales
SELECT Id, Date, DATEADD(HOUR, -5, Date) as ColombiaTime
FROM Sales;

-- Si las fechas ya están en hora de Colombia y necesitas convertir a UTC
UPDATE Sales 
SET Date = DATEADD(HOUR, 5, Date);
```

### 3. Endpoints afectados

Todos los endpoints que manejan fechas:
- ✅ `GET /api/Sales/report` - Con paginación
- ✅ `GET /api/Sales/report/all` - Sin paginación  
- ✅ `GET /api/Sales/{id}` - Detalle de venta
- ✅ `POST /api/Sales` - Registro de venta (automático con DateTime.Now)

---

## 🧪 Pruebas

### Verificar conversión de zona horaria

```csharp
// Test unitario
[Fact]
public void Should_Convert_UTC_To_Colombia_Time()
{
    var utc = new DateTime(2026, 2, 20, 2, 24, 4, DateTimeKind.Utc);
    var colombia = ColombiaTimeZone.ConvertFromUtc(utc);
    
    Assert.Equal(new DateTime(2026, 2, 19, 21, 24, 4), colombia);
}
```

### Verificar en Swagger

1. Llamar a `/api/Sales/report?startDate=2026-02-01&endDate=2026-02-28`
2. Revisar el campo `filterApplied.timeZone` en la respuesta
3. Verificar que las fechas en `data[].date` correspondan a hora de Colombia

---

## 📚 Referencias

- [TimeZoneInfo Class (Microsoft Docs)](https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo)
- [IANA Time Zone Database](https://www.iana.org/time-zones)
- [Colombia Time Zone (timeanddate.com)](https://www.timeanddate.com/time/zone/colombia)

---

## 🔄 Cambiar Zona Horaria

Si necesitas cambiar a otra zona horaria en el futuro:

1. Modifica `Shared.Utilities/TimeZone/ColombiaTimeZone.cs`
2. Cambia el ID de zona horaria:

```csharp
// Para México (UTC-6)
private static readonly TimeZoneInfo _timeZone = 
    TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

// Para Argentina (UTC-3)
private static readonly TimeZoneInfo _timeZone = 
    TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
```

3. Actualiza `appsettings.json`:
```json
{
  "TimeZone": {
    "Default": "America/Mexico_City",
    "DisplayName": "Central Standard Time (UTC-6)",
    "UtcOffset": "-06:00"
  }
}
```

4. Ejecuta migraciones si es necesario para actualizar datos existentes

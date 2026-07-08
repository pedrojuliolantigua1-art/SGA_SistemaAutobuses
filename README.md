# SGA - Sistema de Gestión de Autorizaciones de Transporte

Proyecto para gestionar el transporte institucional: usuarios, rutas, autobuses, viajes, autorizaciones (tickets, tarjetas, permisos), pagos, accesos, notificaciones y auditoría.

## ¿Cómo está organizado el proyecto?

El proyecto sigue una arquitectura por capas (Clean Architecture simplificada). Cada capa es un proyecto de .NET distinto, y cada uno solo puede depender de la capa de abajo:

### 1. Domain (`SGA.Domain`)

Este el centro del siste sistema. No depende de ninguna otra capa (ni de Entity Framework, ni de la Api).

- **Entities/** → las clases que representan las tablas de la base de datos (`Ruta`, `Autobus`, `Viaje`, `UsuarioTransporte`, etc.). Todas heredan de `BaseEntity`/`Auditable`, que traen los campos comunes: `Id`, `FechaCreacion`, `CreadoPor`, `Eliminado`, `FechaEliminacion`, `EliminadoPor`.
- **Models/** → versiones "planas" de las entidades, usadas para leer datos desde los repositorios sin traer toda la entidad (evita mapeos innecesarios).
- **Rules/** → aquí viven las reglas de negocio (ej. "una ruta necesita mínimo 2 paradas", "un autobús no puede tener capacidad negativa"). Cada regla devuelve un `Result` indicando si pasó o no.
- **Validation/** → validaciones genéricas reutilizables (`IdValido`, `Requerido`, `RangoFechasValido`, etc.) que usan las Rules.
- **Error/** → hice un catálogo de errores del dominio, organizados por módulo.
- **Repository/Interfaces/** → contratos de los repositorios (`IRutaRepository`, `IUsuarioRepository`, etc.). Domain define **qué** se puede hacer, pero no **cómo**.

### 2. Application (`SGA.Application`)

Es la capa de orquestación. Aquí vive la lógica de "qué hacer" con cada petición.

- **DTOs/** → los objetos que entran y salen por la API (`CrearRutaDto`, `RutaDto`, `EliminarDto`, etc.), organizados por módulo. Nunca se expone la entidad directo al cliente.
- **Services/** → cada Service (`RutaService`, `UsuarioService`, `ViajeService`...) recibe un DTO, llama a las Rules del Domain para validar, usa el repositorio correspondiente para leer/guardar, y devuelve el resultado envuelto en `Result<T>`.
- **Interfaces/Services/** → contratos de los Services (`IRutaService`, etc.), para que la Api dependa de la interfaz y no de la implementación.
- **Common/** → utilidades compartidas por los Services, como `ApplicationErrors` (errores de "no encontrado", "operación inválida").

**Regla importante del proyecto:** los Services nunca validan con `if` sueltos — siempre delegan la validación de negocio a las Rules del Domain. Solo deciden cosas que dependen de la base de datos, como "¿existe este registro?".

### 3. Infrastructure (`SGA.Infrastructure` / `SGA.Infrastructure.Persistence`)

Es donde se implementa todo lo que Domain solo declaró como interfaz.

- **Persistence/Data/SgaDbContext.cs** → el `DbContext` de Entity Framework Core. Aquí se configuran las relaciones entre tablas (`HasOne`, `HasMany`, `HasForeignKey`) y los filtros de soft delete (`HasQueryFilter`).
- **Persistence/Repository/** → la implementación real de cada repositorio, usando LINQ + Entity Framework Core contra SQL Server. Se usa `AsNoTracking()` en toda consulta de solo lectura.
- **Email/**, **Almacenamiento/** → servicios externos (correo, subida de archivos/fotos).

**No se usa ADO.NET en ningún punto del proyecto** — toda la persistencia pasa por Entity Framework Core.

### 4. Api (`SGA.Api`)

Es la puerta de entrada HTTP. Aquí viven los Controllers.

- **Controllers/** → un controller por módulo (`RutasController`, `AutobusesController`, `UsuariosController`, etc.). Cada acción del controller solo hace: recibir el DTO, llamar al Service correspondiente, y traducir el `Result` a una respuesta HTTP con `AResultado`/`AResultadoCreado`.
- **Common/ResultadosHttp.cs** → convierte los `Result`/`Result<T>` del dominio en códigos HTTP (200, 201, 204, 404, 400, 409) de forma automática.

**Los controllers no validan nada de negocio** — solo reciben, delegan y traducen la respuesta.

## Patrones usados en el proyecto

| Patrón | Para qué sirve |
|---|---|
| **Result / Result\<T\>** | En vez de lanzar excepciones para errores esperados (ej. "no encontrado"), los métodos devuelven un objeto `Result` que indica éxito o el error específico. |
| **Repository** | Cada entidad tiene su repositorio (`IRutaRepository`, `IViajeRepository`...) que abstrae el acceso a datos. Los Services nunca usan EF Core directamente. |
| **Soft Delete** | Ningún registro se borra físicamente. El `DELETE` marca `Eliminado = true`, `FechaEliminacion` y `EliminadoPor`, y un filtro global (`HasQueryFilter`) lo excluye de las consultas normales. Siempre queda evidencia. |


## Convención de endpoints

Casi todos los módulos siguen la misma forma:

```
GET    /api/{modulo}                -> listar
GET    /api/{modulo}/{id}           -> obtener uno
POST   /api/{modulo}                -> crear
PUT    /api/{modulo}/{id}           -> actualizar
DELETE /api/{modulo}/{id}           -> eliminar (soft delete, recibe Motivo y EliminadoPor)
POST   /api/{modulo}/{id}/restaurar -> revertir la eliminación
```

## Cómo correr el proyecto la Api que ya funciona

1. Configurar la cadena de conexión en `Api/SGA.Api/appsettings.json` (`ConnectionStrings:DefaultConnection`).

3. Ejecutar la Api:
   ```
   dotnet run --project Api/SGA.Api
   ```
4. Probar los endpoints desde Swagger en `https://localhost:{puerto}/swagger`.

## Módulos del sistema

- **Usuarios** (Estudiantes, Empleados Docentes/Administrativos, Conductores)
- **Transporte** (Rutas, Paradas, Horarios, Autobuses)
- **Viajes** (programación, ejecución, incidencias)
- **Autorizaciones** (Ticket Diario, Tarjeta Recargable, Permiso de Transporte)
- **Pagos**
- **Accesos** (registro de abordajes)
- **Notificaciones**
- **Auditoría** (registro de acciones del sistema, de solo lectura)

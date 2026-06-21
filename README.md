# SGA — Sistema de Gestión de Acceso al Transporte del ITLA

Proyecto desarrollado para la asignatura **Programación II** — ITLA  
**Estudiante:** Pedro Julio Lantigua Martínez | **Matrícula:** 2025-2022  

---

## ¿Qué hace este sistema?

El SGA gestiona el acceso de estudiantes, empleados y conductores al transporte institucional del ITLA. Controla autorizaciones, viajes, pagos, notificaciones y deja un registro de auditoría de todo lo que ocurre.

---

## La capa de Dominio

Es el núcleo del sistema. No depende de ningún otro proyecto — ni de la base de datos, ni de frameworks externos. 
Aquí vive todo lo relacionado con las reglas del negocio.

### Entidades principales

**Usuarios** — todos heredan de `UsuarioTransporte`:
- `Estudiante` — tiene matrícula y carrera
- `EmpleadoDocente` — tiene código de empleado, departamento y especialidad
- `EmpleadoAdministrativo` — tiene área administrativa
- `Conductor` — tiene número de licencia y disponibilidad

**Autorizaciones** — todas heredan de `AutorizacionTransporte`:
- `TicketMensual` — válido por un mes, tiene fecha de inicio y fin
- `TarjetaRecargable` — tiene número de tarjeta y saldo disponible
- `PermisoTransporte` — para empleados, tiene condición institucional

**Transporte** — el catálogo del servicio:
- `Ruta`, `Parada`, `HorarioRuta`, `Autobus`

**Operaciones:**
- `Viaje` — un viaje planificado con ruta, horario, autobús y conductor asignados
- `Incidencia` — evento reportado durante un viaje
- `RegistroUsoTransporte` — cada intento de abordaje al autobús
- `PagoTransporte` — pago registrado por un administrador
- `Notificacion` — mensaje enviado a un usuario
- `RegistroAuditoria` — log de acciones del sistema

**Fotos:**
- `FotoAutobus` y `FotoIncidencia` — guardan la URL de la foto en Cloudinary

### Clases base

Todas las entidades heredan de `BaseEntity` que a su vez hereda de `Auditable`:


Auditable
  FechaCreacion, CreadoPor
  FechaModificacion
  Eliminado, FechaEliminacion, EliminadoPor  ← soft delete

BaseEntity : Auditable
  Id (int, autogenerado)


### Manejo de errores — Result Pattern

En lugar de lanzar excepciones para errores de negocio, los métodos retornan un objeto `Result`:

Los errores están organizados en `DomainErrors` separado por contexto:
- `DomainErrors.General` — errores comunes (campo requerido, id inválido)
- `DomainErrors.Accesos` — errores al validar abordaje
- `DomainErrors.Autorizaciones` — errores al emitir o usar autorizaciones
- `DomainErrors.Viajes` — errores al planificar o ejecutar viajes
- `DomainErrors.Notificaciones` — errores de notificaciones
- `DomainErrors.Auditoria` — errores de registros de auditoría

### Reglas de negocio — Rules

La lógica del negocio vive en clases estáticas dentro de `Rules/`.

| Clase | Qué hace |
|---|---|
| `AccesoRules` | Valida si un usuario puede abordar el autobús |
| `AutorizacionRules` | Emite y consume tickets, tarjetas y permisos |
| `ViajeRules` | Planifica viajes y controla sus estados |
| `NotificacionRules` | Genera notificaciones según eventos del sistema |
| `AuditoriaRules` | Construye registros de auditoría |
| `CatalogoTransporteRules` | Valida rutas, autobuses y conductores |
| `CommonValidationRules` | Validaciones reutilizables: campos, montos, fechas |

### Interfaces de repositorio

El dominio define los contratos de acceso a datos. No sabe cómo se implementan.

Todas heredan de `IBaseRepository<T>`:

Cada entidad tiene su propia interfaz con métodos adicionales:

| Interfaz | Métodos extra |
|---|---|
| `IUsuarioRepository` | `GetByCorreo`, `GetByRol`, `ValidarPassword` |
| `IViajeRepository` | `GetByFecha`, `GetByConductor`, `GetByPeriodo`, `AddIncidencia` |
| `IAutorizacionRepository` | `GetByUsuario`, `GetVigentes`, `GetByPeriodo` |
| `IAccesoRepository` | `GetByViaje`, `GetByUsuario`, `GetByPeriodo` |
| `IPagoRepository` | `GetByUsuario`, `GetPagoSinAutorizacion` |
| `IAuditoriaRepository` | `GetByPeriodo`, `GetByActor`, `GetByAccion` |
| `INotificacionRepository` | `GetByUsuario`, `GetByTipo`, `MarcarComoLeida` |
| `IRutaRepository` | `GetActivas`, `GetParadas`, `GetHorarios` |
| `IAutobusRepository` | `GetDisponibles` |
| `IParadaRepository` | `GetByRuta` |
| `IHorarioRutaRepository` | `GetByRuta` |
| `IFotoAutobusRepository` | `GetByAutobusId`, `GetAllByAutobusId` |
| `IFotoIncidenciaRepository` | `GetByIncidenciaId`, `GetByViajeId` |

### Interfaces de servicios externos

En `Services/`:

| Interfaz | Para qué |
|---|---|
| `IJwt` | Generar y validar tokens JWT |
| `IEmailSender` | Enviar correos |
| `IAlmacenamientoArchivos` | Subir y eliminar fotos en Cloudinary |

---

## La capa de Persistencia

Implementa todas las interfaces del dominio usando ADO.NET puro con Stored Procedures.

### Patrón Template Method

Todos los repositorios heredan de `SqlRepositoryBase` que encapsula la mecánica de ADO.NET:

- `QueryAsync` — SP que retorna muchas filas
- `QuerySingleOrDefaultAsync` — SP que retorna una fila o null
- `ExecuteAsync` — SP que no retorna filas (UPDATE/DELETE)
- `ExecuteScalarAsync` — SP de INSERT que retorna el Id generado

### Separación de responsabilidades

Cada archivo de repositorio tiene tres clases:

```
UsuarioRepository      ← solo llama stored procedures
UsuarioMapper          ← solo convierte SqlDataReader → entidad
UsuarioParameters      ← solo construye SqlParameter[]
```
---

## Fotos — Cloudinary

```
1. El archivo físico sube a Cloudinary → retorna URL pública
2. La URL y el PublicId se guardan en SQL Server
3. El frontend usa la URL directamente para mostrar la imagen
```

---

## Tecnologías usadas

- .NET 8 — C#
- SQL Server — base de datos principal
- ADO.NET puro — acceso a datos sin ORM
- Stored Procedures — toda operación SQL pasa por procedimientos almacenados
- Cloudinary — almacenamiento de fotos de autobuses e incidencias
- JWT — autenticación por tokens
- SMTP — envío de correos

---

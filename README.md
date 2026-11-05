# POS Backend

API para un sistema de punto de venta (POS), desarrollada con ASP.NET Core y .NET 10.

> **Estado:** en desarrollo inicial. La estructura de capas ya está creada, pero aún no hay funciones del punto de venta ni base de datos configurada.

## Estructura

La solución está organizada en tres capas:

```text
POS_Backend (interfaz) → POS_Backend.Logica → POS_Backend.Datos
```

| Capa | Proyecto | Responsabilidad |
|---|---|---|
| Interfaz | `POS_Backend` | Controladores y configuración de la API |
| Lógica | `POS_Backend.Logica` | Servicios y reglas de negocio |
| Datos | `POS_Backend.Datos` | Entidades, acceso a la base de datos y repositorios |

La capa de interfaz no referencia directamente a la de datos: todo acceso pasa por la capa de lógica.

## Requisitos

- [SDK de .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0)

## Cómo ejecutarlo

Clona el repositorio:

```bash
git clone https://github.com/EdwinFrancoG/POS_Backend.git
```

Entra en la carpeta:

```bash
cd POS_Backend
```

Ejecuta la API:

```bash
dotnet run --project POS_Backend
```

La API queda disponible en `http://localhost:5186`.

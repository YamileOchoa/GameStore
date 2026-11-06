# 🎮 Tienda de Videojuegos – Web API REST

Web API REST desarrollada con **ASP.NET Core (.NET 10)** y **Entity Framework Core** para gestionar el catálogo de una tienda en línea de videojuegos: desarrolladoras, géneros y juegos, con control de stock de claves (keys).

Proyecto del curso **Desarrollo de Aplicaciones Empresariales Avanzadas** (Semana 12).

---

## 📌 Tabla de contenido

1. [Descripción](#-descripción)
2. [Tecnologías](#-tecnologías)
3. [Modelo de datos](#-modelo-de-datos)
4. [Endpoints](#-endpoints)
5. [Reglas de negocio](#-reglas-de-negocio)
6. [Códigos de respuesta](#-códigos-de-respuesta)
7. [Estructura del proyecto](#-estructura-del-proyecto)
8. [Ejecución en local](#-ejecución-en-local)
9. [Despliegue en la nube](#-despliegue-en-la-nube-monsteraspnet--sql-server)
10. [Pruebas](#-pruebas)
11. [Autor](#-autor)

---

## 📖 Descripción

La API centraliza la información de una tienda de videojuegos en una única fuente de datos:

- Catálogo de juegos con precio, fecha de lanzamiento y clasificación por edad.
- Control del stock de claves digitales de cada título.
- Administración de desarrolladoras y géneros.
- Consultas con filtros por query string y rutas anidadas.

El acceso a datos se resuelve con **LINQ** y los controladores usan directamente el `DbContext` (sin DTOs ni capas adicionales). El modelo se generó con **Scaffold** (enfoque *base de datos primero*).

---

## 🛠️ Tecnologías

| Componente | Tecnología |
|---|---|
| Framework | ASP.NET Core Web API (.NET 10) con controladores |
| ORM | Entity Framework Core |
| Proveedor de BD | `Microsoft.EntityFrameworkCore.SqlServer` |
| Base de datos | Microsoft SQL Server |
| Documentación | OpenAPI + Swagger UI |
| Hosting y BD en la nube | MonsterASP.NET |
| Patrón | MVC (Modelos + Controladores; la "vista" es la respuesta JSON) |

---

## 🗂️ Modelo de datos

Los nombres de tablas, entidades y atributos están en **inglés**; los datos de ejemplo y los mensajes de la API están en **español**.

```
Developer 1 ────< N Game N >──── 1 Genre
```

### Developer (desarrolladora)

| Campo | Tipo | Reglas |
|---|---|---|
| `Id` | int | Clave primaria |
| `Name` | string | Obligatorio, máx. 100, único |
| `Country` | string | Obligatorio, máx. 60 |
| `FoundedYear` | int | Entre 1970 y 2026 |

### Genre (género)

| Campo | Tipo | Reglas |
|---|---|---|
| `Id` | int | Clave primaria |
| `Name` | string | Obligatorio, máx. 50, único |
| `Description` | string? | Opcional, máx. 200 |

### Game (juego)

| Campo | Tipo | Reglas |
|---|---|---|
| `Id` | int | Clave primaria |
| `Title` | string | Obligatorio, máx. 120. Único por desarrolladora |
| `Price` | decimal | Entre 0 y 500 |
| `ReleaseDate` | DateOnly | Fecha de lanzamiento |
| `AgeRating` | string | Solo `E`, `T` o `M` (`^[ETM]$`) |
| `KeyStock` | int | Entre 0 y 100 000 |
| `DeveloperId` | int | Clave foránea → `Developer` |
| `GenreId` | int | Clave foránea → `Genre` |

> Las propiedades de navegación de `Game` (`Developer?`, `Genre?`) admiten nulo, por lo que el POST solo necesita enviar las claves foráneas. El ciclo JSON se evita con `[JsonIgnore]` en las colecciones `Games`.

---

## 🔗 Endpoints

**Total: 17 endpoints.**

### Developers — `/api/developers`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/developers` | Listar desarrolladoras |
| GET | `/api/developers/{id}` | Obtener por id |
| GET | `/api/developers/{id}/games` | Juegos de una desarrolladora (ruta anidada) |
| POST | `/api/developers` | Crear |
| PUT | `/api/developers/{id}` | Actualizar |
| DELETE | `/api/developers/{id}` | Eliminar |

### Genres — `/api/genres`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/genres` | Listar géneros |
| GET | `/api/genres/{id}` | Obtener por id |
| GET | `/api/genres/{id}/games` | Juegos de un género (ruta anidada) |
| POST | `/api/genres` | Crear |
| PUT | `/api/genres/{id}` | Actualizar |
| DELETE | `/api/genres/{id}` | Eliminar |

### Games — `/api/games`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/games` | Listar con filtros opcionales |
| GET | `/api/games/{id}` | Obtener por id |
| POST | `/api/games` | Crear |
| PUT | `/api/games/{id}` | Actualizar |
| DELETE | `/api/games/{id}` | Eliminar |

**Filtros de `GET /api/games` (query string):**

| Parámetro | Ejemplo | Descripción |
|---|---|---|
| `genre` | `?genre=RPG` | Nombre del género |
| `ageRating` | `?ageRating=M` | Clasificación (E, T, M) |
| `minPrice` | `?minPrice=10` | Precio mínimo |
| `maxPrice` | `?maxPrice=50` | Precio máximo |
| `title` | `?title=witcher` | Texto contenido en el título |

Se pueden combinar: `/api/games?genre=RPG&ageRating=M&maxPrice=50`

---

## 📏 Reglas de negocio

### Validaciones (Data Annotations → 400)

`[Required]`, `[StringLength]`, `[Range]` y `[RegularExpression]`. Con `[ApiController]`, los datos inválidos responden **400 Bad Request** automáticamente.

### Control de duplicados (409 Conflict)

- No se repite el nombre de una desarrolladora.
- No se repite el nombre de un género.
- No se repite el título de un juego dentro de la misma desarrolladora.

La comparación no distingue mayúsculas de minúsculas, y la base de datos refuerza estas reglas con restricciones `UNIQUE`.

### Integridad referencial

- No se puede eliminar una desarrolladora o género con juegos vinculados → **409 Conflict**.
- No se puede asignar un `DeveloperId` o `GenreId` inexistente → **400 Bad Request**.

---

## 🚦 Códigos de respuesta

| Código | Resultado | Cuándo |
|---|---|---|
| 200 | `Ok(datos)` | GET exitoso |
| 201 | `CreatedAtAction(...)` | POST exitoso |
| 204 | `NoContent()` | PUT y DELETE exitosos |
| 400 | `BadRequest()` | Datos inválidos, id de ruta ≠ id del cuerpo, clave foránea inexistente |
| 404 | `NotFound()` | El recurso no existe |
| 409 | `Conflict(...)` | Duplicados o eliminación con dependientes |

---

## 📁 Estructura del proyecto

```
TiendaVideojuegosApi/
├── Controllers/
│   ├── DevelopersController.cs
│   ├── GenresController.cs
│   └── GamesController.cs
├── Models/
│   ├── Developer.cs
│   ├── Genre.cs
│   ├── Game.cs
│   └── TiendaContext.cs
├── Database/
│   └── script.sql          # Script de la BD con datos de ejemplo
├── Program.cs
├── appsettings.json        # Sin cadena de conexión
└── README.md
```

---

## 💻 Ejecución en local

**Requisitos:** SDK de .NET 10, SQL Server (o SQL Server Express) y la herramienta `dotnet-ef`.

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/YamileOchoa/GameStore.git
   cd GameStore
   ```
2. Crear la base de datos y ejecutar `Database/script.sql`.
3. Guardar la cadena de conexión en *user secrets* (no se sube al repositorio):
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=tienda_videojuegos;Trusted_Connection=True;TrustServerCertificate=True;"
   ```
4. (Opcional) Regenerar el modelo con Scaffold:
   ```bash
   dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context TiendaContext --data-annotations --force
   ```
5. Ejecutar:
   ```bash
   dotnet run
   ```
6. Abrir `http://localhost:<puerto>/swagger`.

---

## 🚀 Despliegue en la nube (MonsterASP.NET + SQL Server)

La API y la base de datos están publicadas en **MonsterASP.NET**, ambas con plan gratuito.

### 1. Configuración del entorno (.NET + SQL Server)

**DbContext:** el proyecto usa `Microsoft.EntityFrameworkCore.SqlServer` como proveedor en `Program.cs`:

```csharp
builder.Services.AddDbContext<TiendaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**CORS:** se habilitó una política de CORS para permitir peticiones desde Swagger UI y clientes externos (Postman, aplicaciones frontend, etc.).

### 2. Compilación y empaquetado (Publish)

Se generaron los archivos de producción con la CLI de .NET:

```bash
dotnet publish -c Release -o ./publish
```

Se seleccionó todo el contenido de la carpeta `./publish` (archivos `.dll`, `web.config`, `appsettings.json` y recursos) y se comprimió en un archivo `.zip`.

### 3. Carga de archivos al hosting

1. Ingresar al panel de MonsterASP.NET → **File Manager** → carpeta `wwwroot`.
2. Subir el `.zip` y descomprimirlo directamente dentro de `wwwroot`.

### 4. Seguridad y variables de entorno

Siguiendo buenas prácticas, la cadena de conexión **no** está en el repositorio. En el panel de MonsterASP.NET (sección *Environment Variables / Scripting*) se configuró con la convención de doble guion bajo (`__`):

| Nombre | Valor |
|---|---|
| `ConnectionStrings__DefaultConnection` | `Server=sqlXXXX.monsterasp.net;Database=NOMBRE_BD;User Id=USUARIO;Password=CONTRASEÑA;TrustServerCertificate=True;` |

> ⚠️ Los valores de arriba son ilustrativos. Las credenciales reales nunca se publican.

### 5. Reinicio y verificación

1. Desde el menú **Process** del panel se reinició la aplicación (*Restart Site*) para que IIS tome la variable de entorno.
2. Se verificó el despliegue entrando a Swagger y probando todos los endpoints:

   🔗 **http://gamestore-api.runasp.net/swagger/index.html**

> ⏳ En planes gratuitos el primer request puede tardar unos segundos mientras la aplicación se reactiva.

### Proveedores usados

| Componente | Proveedor |
|---|---|
| Hosting de la API | MonsterASP.NET |
| Base de datos | MonsterASP.NET (SQL Server) |

---

## 🧪 Pruebas

Todas las pruebas se realizaron sobre la **API publicada** (no en localhost), usando Swagger / Postman / archivo `.http`.

| Prueba | Código esperado |
|---|---|
| GET lista y por id de las 3 entidades | 200 |
| GET `/api/games?genre=RPG&maxPrice=50` | 200 |
| GET `/api/developers/1/games` y `/api/genres/1/games` | 200 |
| POST válido en las 3 entidades | 201 |
| PUT y DELETE válidos | 204 |
| POST con precio 600 o título vacío | 400 |
| PUT con id de ruta distinto al del cuerpo | 400 |
| POST de juego con `DeveloperId` o `GenreId` inexistente | 400 |
| GET / PUT / DELETE con id 9999 | 404 |
| POST de desarrolladora o género con nombre repetido | 409 |
| POST de juego con título repetido en la misma desarrolladora | 409 |
| DELETE de desarrolladora o género con juegos | 409 |

Las capturas de cada prueba se incluyen en el informe de entrega.

---

## 👤 Autor

**Ochoa Marin, Yamile**
Desarrollo de Aplicaciones Empresariales Avanzadas — 6 - C24 - Sección C - D

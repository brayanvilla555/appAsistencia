# Sistema de Registro de Asistencia

Este proyecto es una aplicación de registro y gestión de asistencia desarrollada para la Semana Sistémica de la Escuela Académico Profesional de Ingeniería de Sistemas (EAPIS) en la Universidad Nacional de Cajamarca. El sistema permite a los administradores gestionar eventos, estudiantes, profesores y reportes de asistencia, mientras que los alumnos y profesores pueden acceder a sus respectivas funcionalidades mediante un inicio de sesión seguro.

## **Características Principales**

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 1. Registro de Asistencia
- **Entrada de datos:** Los administradores registran la asistencia de los estudiantes usando su código de estudiante o DNI.
- **Validación:** Verificación de que el código o DNI del estudiante sea válido y esté registrado en la base de datos antes de permitir el registro de la asistencia.
- **Registro:** Almacena la asistencia en la base de datos con la fecha y hora actual, asociándola con el alumno y el evento correspondiente.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 2. Gestión de Estudiantes
- **Registro de alumnos:** Los alumnos pueden ser registrados directamente en la base de datos, con opción de registro manual.
- **Actualización de información:** Permite la actualización de la información de los alumnos.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 3. Gestión de Eventos
- **Creación de eventos:** Los administradores pueden crear nuevos eventos, especificando nombre, fecha, hora de inicio, fecha y hora de fin, y el ponente.
- **Edición de eventos:** Permite editar la información de eventos existentes.
- **Eliminación de eventos:** Los administradores pueden eliminar eventos.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 4. Generación de Reportes
- **Filtrado:** Los reportes pueden filtrarse por:
  - Fecha del evento
  - Nombre del evento
  - Ciclo
  - Nombre o Apellido del estudiante.
- **Contenido del reporte:** Incluye:
  - Lista de alumnos que asistieron a cada evento.
  - Nombre del evento.
  - Fecha y hora del inicio del evento.
  - Fecha y hora de cada asistencia registrada.
- **Exportación:** Los reportes pueden exportarse en formato Excel.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 5. Gestión de Usuarios
- **Administradores:**
  - Los administradores pueden iniciar sesión para gestionar eventos y generar reportes.
  - Los administradores pueden crear, editar y eliminar estudiantes, docentes, cursos y ponentes.
- **Alumnos:**
  - Los alumnos pueden iniciar sesión y ver sus asistencias a eventos.
  - La contraseña se genera de la siguiente manera: `CódigoEstudiante_unc_AñoActual` (ejemplo: `2022001190_unc_2024`).
- **Docentes:**
  - Los docentes pueden buscar a sus alumnos por ciclo, grupo, nombre del evento o fecha.
  - La contraseña es generada por el administrador.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 6. Seguridad y Autenticación
- **Autenticación:** Administradores, alumnos y docentes deben autenticarse para acceder a sus funciones respectivas.
- **Validación de datos:** El sistema garantiza la inserción de datos válidos en la base de datos.

### ![#00ff00](https://placehold.co/15x15/00ff00/00ff00.png) 7. Interfaz de Usuario
#### Para Administradores:
- Panel para gestionar eventos, ponentes, estudiantes, docentes y cursos.
- Panel para el registro de asistencias.
- Sección para generar y filtrar reportes.

#### Para Alumnos:
- Interfaz para visualizar sus asistencias a eventos.

#### Para Docentes:
- Interfaz para ver las asistencias de sus alumnos.

## **Requisitos del Sistema**

- **Backend:** Implementación con C#, utilizando el patrón MVC.
- **Base de datos:** Base de datos SQL manejada con Entity Framework.
- **Frontend:** Razor Pages para el desarrollo del frontend.

## **Instalación**

## Guía para Descargar e Instalar el Proyecto

### Requisitos Previos
Antes de comenzar, asegúrate de tener instalado en tu PC:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- [Visual Studio](https://visualstudio.microsoft.com/) (con soporte para desarrollo web y C#).
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

### Pasos para Descargar e Instalar el Proyecto
1. **Clonar el Repositorio:**
   Abre tu terminal o símbolo del sistema y ejecuta:
   ```bash
<!--
# Clonar el repositorio
git clone https://github.com/brayanvilla555/appAsistencia.git

# Entrar en la carpeta del proyecto
cd appAsistencia

# Restaurar dependencias
Update-Package -Reinstall

# Configurar la base de datos
# Asegúrate de que la cadena de conexión esté correctamente configurada en appsettings.json
# Ejecutar migraciones
Update-Database
-->

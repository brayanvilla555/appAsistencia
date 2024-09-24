using Microsoft.AspNetCore.Identity;
using proyectoAsistencia.Models;

namespace proyectoAsistencia.Data
{
    public class BDInicializar
    {
        public static void Inicializar(AppDbContext context)
        {
            //var hasher = new PasswordHasher();
            var hasher = new PasswordHasher<Administrador>();
            var hasher2 = new PasswordHasher<Estudiante>();
            var hasher3 = new PasswordHasher<Docente>();
            if (context.Estudiante.Any())
            {
                return;
            }

            var administradores = new Administrador[]
            {

                    new Administrador
                    {
                        Nombre = "Ruth",
                        Apellido = "Ramirez Lopez",
                        Telefono = "923456789",
                        Email = "ruth@gmail.com",
                        Contrasena = hasher.HashPassword(null, "12121212"),
                        //Contrasena = hasher.HashPassword(administradores, administradores.Contrasena),
                        FechaAcceso = DateTime.Now,
                    },
                    new Administrador
                    {
                        Nombre = "Carlos",
                        Apellido = "Perez Jimenez",
                        Telefono = "999888777",
                        Email = "carlos@gmail.com",
                        Contrasena = hasher.HashPassword(null, "12121212"),
                        FechaAcceso = DateTime.Now,
                    }
            };
            context.AddRange(administradores);
            context.SaveChanges();

            var estudiantes = new Estudiante[]
            {
                   new Estudiante {
                        CodigoEstudiante = "2939292",
                        Nombre = "Alber Yoel",
                        Apellido = "Fernandez Saldaña",
                        Dni = "79730320",
                        Promocion = "2022-1",
                        Telefono = "939493939",
                        Ciclo = Utils.Ciclo.QUINTO,
                        Email = "alberyoel@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "alber1111")
                    },
                    new Estudiante {
                        CodigoEstudiante = "3435435",
                        Nombre = "Sayra Silvana",
                        Apellido = "Reyes Rodrigues",
                        Dni = "79730390",
                        Promocion = "2022-1",
                        Telefono = "945678234",
                        Ciclo = Utils.Ciclo.QUINTO,
                        Email = "sayrasilvana@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "sayra2222")

                    },
                    new Estudiante {
                        CodigoEstudiante = "5656565",
                        Nombre = "Brayan Alexander",
                        Apellido = "Villanueva Quispe",
                        Dni = "79703920",
                        Promocion = "2022-1",
                        Telefono = "952346789",
                        Ciclo = Utils.Ciclo.QUINTO,
                        Email = "brayanalexander@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "brayan3333")
                    },
                    new Estudiante {
                        CodigoEstudiante = "6767676",
                        Nombre = "Josue Etwin",
                        Apellido = "Saldaña Fustamante",
                        Dni = "79703920",
                        Promocion = "2022-1",
                        Telefono = "938567234",
                        Ciclo = Utils.Ciclo.QUINTO,
                        Email = "josueetwin@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "josue4444")

                    },
                    new Estudiante {
                        CodigoEstudiante = "7878787",
                        Nombre = "Luciana",
                        Apellido = "Perez Montes",
                        Dni = "79733920",
                        Promocion = "2021-2",
                        Telefono = "932123456",
                        Ciclo = Utils.Ciclo.CUARTO,
                        Email = "luciana@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "luciana5555")

                    },
                    new Estudiante {
                        CodigoEstudiante = "8989898",
                        Nombre = "Diego Armando",
                        Apellido = "Vega Torres",
                        Dni = "79733920",
                        Promocion = "2020-1",
                        Telefono = "940987654",
                        Ciclo = Utils.Ciclo.SEGUNDO,
                        Email = "diegoarmando@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "diego6666")
                    },
                    new Estudiante {
                        CodigoEstudiante = "9090909",
                        Nombre = "Mariana",
                        Apellido = "Santos Lopez",
                        Dni = "79703920",
                        Promocion = "2022-2",
                        Telefono = "934567890",
                        Ciclo = Utils.Ciclo.TERCERO,
                        Email = "mariana@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "mariana7777")
                    },
                    new Estudiante {
                        CodigoEstudiante = "1010101",
                        Nombre = "Andres",
                        Apellido = "Ruiz Benitez",
                        Dni = "79733920",
                        Promocion = "2021-1",
                        Telefono = "937654321",
                        Ciclo = Utils.Ciclo.PRIMERO,
                        Email = "andresruiz@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "andres8888")
                    },
                    new Estudiante {
                        CodigoEstudiante = "1111111",
                        Nombre = "Camila",
                        Apellido = "Gutierrez Herrera",
                        Dni = "79730320",
                        Promocion = "2020-2",
                        Telefono = "931234567",
                        Ciclo = Utils.Ciclo.CUARTO,
                        Email = "camila@gmail.com",
                        Contrasena = hasher2.HashPassword(null, "camila9999")
                    }
            };
            context.AddRange(estudiantes);
            context.SaveChanges();

            var ponentes = new Ponente[]
            {
                    new Ponente
                    {
                        Nombre = "Cecilia",
                        Apellido = "Valdes",
                        Especializacion = "Administracion",
                        Email = "cecilia@gmail.com",
                        Telefono = "99493039",
                    },
                    new Ponente
                    {
                        Nombre = "Pedro",
                        Apellido = "Suarez",
                        Especializacion = "Ingenieria de Sistemas",
                        Email = "pedro.suarez@gmail.com",
                        Telefono = "988765432",
                    },
                    new Ponente
                    {
                        Nombre = "Maria",
                        Apellido = "Gonzalez",
                        Especializacion = "Marketing",
                        Email = "maria.gonzalez@gmail.com",
                        Telefono = "912345678",
                    },
                    new Ponente
                    {
                        Nombre = "Luis",
                        Apellido = "Torres",
                        Especializacion = "Economia",
                        Email = "luis.torres@gmail.com",
                        Telefono = "987654321",
                    }
            };
            context.AddRange(ponentes);
            context.SaveChanges();

            var docentes = new Docente[] {
                    new Docente {
                        Nombre = "ROSEL", //INTELIGENCIA DE NEGOCIOS
                        Apellido = "BURGA CABRERA",
                        Telefono = "931234567",
                        Email = "rosel@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "rosel1111"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "MANUEL ENRIQUE", //BASE DE DATOS I
                        Apellido = "MALPICA RODRIGUEZ",
                        Telefono = "932345678",
                        Email = "manuel_enrique@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "manuel2222"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "MARISOL", //BASE DE DATOS I
                        Apellido = "TAPIA ROMERO",
                        Telefono = "933456789",
                        Email = "marisol@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "marisol1111"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "JAIME AMADOR", //BASE DE DATOS I
                        Apellido = "MEZA HUAMAN",
                        Telefono = "934567890",
                        Email = "jaime_amador@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "jaimeamador2222"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "LAURA SOFIA", //INVESTIGACIÓN DE OPERACIONES EN INGENIERÍA I
                        Apellido = "BAZAN DIAZ",
                        Telefono = "935678901",
                        Email = "laura_sofia@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "laurasofia1111"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "VÍCTOR RAMÓN", //METODOLOGÍA DEL TRABAJO UNIVERSITARIO
                        Apellido = "ORTIZ CHÁVEZ",
                        Telefono = "936789012",
                        Email = "victor_ramon@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "victorramon2222"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "LISI JANET", //PROGRAMACIÓN APLICADA II Y PROGRAMACIÓN APLICADA I
                        Apellido = "VÁSQUEZ FERNÁNDEZ",
                        Telefono = "937890123",
                        Email = "lisi_janet@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "lisijanet1111"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "SANDRA CECILIA", //TEORÍA DE AUTÓMATAS Y LENGUAJES FORMALES
                        Apellido = "RODRIGUEZ AVILA",
                        Telefono = "938901234",
                        Email = "sandra_cecilia@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "sandracecilia2222"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "MIGUEL ANGEL", //ESTADÍSTICA APLICADA
                        Apellido = "MACETAS HERNANDEZ",
                        Telefono = "939012345",
                        Email = "miguel_angel@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "miguelangel1111"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "SEGUNDO ENRIQUE", //FÍSICA APLICADA
                        Apellido = "DOBBERTIN SANCHEZ",
                        Telefono = "930123456",
                        Email = "segundo_enrique@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "segundoenrique2222"),
                        FechaAcceso = DateTime.Now
                    },
                    new Docente {
                        Nombre = "AMALIA DELICIA DEL SAGRARIO", //FUNDAMENTOS DE LOS SISTEMAS DE INFORMACIÓN
                        Apellido = "FERNANDEZ VARGAS",
                        Telefono = "931234567",
                        Email = "amalia_del_sagrario@gmail.com",
                        Contrasena = hasher3.HashPassword(null, "amaliadel1111"),
                        FechaAcceso = DateTime.Now
                    }
            };

            context.AddRange(docentes);
            context.SaveChanges();

            var cursos = new Curso[] {
                    new Curso
                    {
                        DocenteId = 1,
                        ciclo = Utils.Ciclo.QUINTO,
                        Nombre = "Inteligencia de Negocios"
                    },
                    new Curso
                    {
                        DocenteId = 2,
                        ciclo = Utils.Ciclo.TERCERO,
                        Nombre = "Base de Datos I"
                    },
                    new Curso
                    {
                        DocenteId = 3,
                        ciclo = Utils.Ciclo.TERCERO,
                        Nombre = "Base de Datos I"
                    },
                    new Curso
                    {
                        DocenteId = 4,
                        ciclo = Utils.Ciclo.CUARTO,
                        Nombre = "Programación Aplicada I"
                    },
                    new Curso
                    {
                        DocenteId = 5,
                        ciclo = Utils.Ciclo.TERCERO,
                        Nombre = "Investigación de Operaciones en Ingeniería I"
                    },
                    new Curso
                    {
                        DocenteId = 6,
                        ciclo = Utils.Ciclo.PRIMERO,
                        Nombre = "Metodología del Trabajo Universitario"
                    },
                    new Curso
                    {
                        DocenteId = 7,
                        ciclo = Utils.Ciclo.SEGUNDO,
                        Nombre = "Programación Aplicada II"
                    },
                     new Curso
                    {
                        DocenteId = 4,
                        ciclo = Utils.Ciclo.QUINTO,
                        Nombre = "Programación Aplicada II"
                    },
                    new Curso
                    {
                        DocenteId = 7,
                        ciclo = Utils.Ciclo.CUARTO,
                        Nombre = "Programación Aplicada I"
                    },
                    new Curso
                    {
                        DocenteId = 8,
                        ciclo = Utils.Ciclo.CUARTO,
                        Nombre = "Teoría de Autómatas y Lenguajes Formales"
                    },
                    new Curso
                    {
                        DocenteId = 9,
                        ciclo = Utils.Ciclo.TERCERO,
                        Nombre = "Estadística Aplicada"
                    },
                    new Curso
                    {
                        DocenteId = 10,
                        ciclo = Utils.Ciclo.TERCERO,
                        Nombre = "Física Aplicada"
                    },
                    new Curso
                    {
                        DocenteId = 11,
                        ciclo = Utils.Ciclo.QUINTO,
                        Nombre = "Fundamentos de los Sistemas de Información"
                    }
                };
            context.AddRange(cursos);
            context.SaveChanges();
            var eventos = new Evento[]
            {
                    new Evento
                    {
                        Nombre = "Power Bi",
                        FechaHoraFin = DateTime.Now.AddHours(2), // Evento de 2 horas
                        FechaHoraInicio = DateTime.Now,
                        Descripcion = "Creación de dashboard",
                        AdministradorId = context.Administrador.First(u=>u.Nombre=="Ruth").Id,
                        Estado = proyectoAsistencia.Utils.Estado.ACTIVO,
                        Ubicacion = "Auditorio Ingeniería de Sistemas"
                    },
                    new Evento
                    {
                        Nombre = "Curso de Python",
                        FechaHoraFin = DateTime.Now.AddHours(3),
                        FechaHoraInicio = DateTime.Now,
                        Descripcion = "Curso avanzado de Python para análisis de datos",
                        AdministradorId = context.Administrador.First(u=>u.Nombre=="Ruth").Id,
                        Estado = proyectoAsistencia.Utils.Estado.ACTIVO,
                        Ubicacion = "Laboratorio de Computación 1"
                    },
                    new Evento
                    {
                        Nombre = "Taller de Inteligencia Artificial",
                        FechaHoraFin = DateTime.Now.AddHours(4),
                        FechaHoraInicio = DateTime.Now,
                        Descripcion = "Introducción a algoritmos de IA",
                        AdministradorId = context.Administrador.First(u=>u.Nombre=="Ruth").Id,
                        Estado = proyectoAsistencia.Utils.Estado.INACTIVO,
                        Ubicacion = "Aula Magna"
                    },
                    new Evento
                    {
                        Nombre = "Conferencia de Big Data",
                        FechaHoraFin = DateTime.Now.AddHours(1),
                        FechaHoraInicio = DateTime.Now.AddHours(1), // Evento empezando en 1 hora
                        Descripcion = "Tendencias actuales en Big Data",
                        AdministradorId = context.Administrador.First(u=>u.Nombre=="Ruth").Id,
                        Estado = proyectoAsistencia.Utils.Estado.ACTIVO,
                        Ubicacion = "Auditorio Central"
                    },
                    new Evento
                    {
                        Nombre = "Hackathon de Desarrollo Web",
                        FechaHoraFin = DateTime.Now.AddHours(6),
                        FechaHoraInicio = DateTime.Now,
                        Descripcion = "Competencia de desarrollo de aplicaciones web",
                        AdministradorId = context.Administrador.First(u=>u.Nombre=="Ruth").Id,
                        Estado = proyectoAsistencia.Utils.Estado.ACTIVO,
                        Ubicacion = "Sala de Conferencias 3"
                    }
           };

            context.AddRange(eventos);
            context.SaveChanges();


            var asistencias = new Asistencia[]
              {
                    new Asistencia
                    {
                        EventoId = 1, // Evento de "Power Bi"
                        EstudianteId = 1, // Alber Yoel
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                        EventoId = 1, // Evento de "Power Bi"
                        EstudianteId = 2, // Sayra Silvana
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                        EventoId = 2, // Evento de "Curso de Python"
                        EstudianteId = 3, // Brayan Alexander
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                        EventoId = 2, // Evento de "Curso de Python"
                        EstudianteId = 4, // Josue Etwin
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                        EventoId = 1, // Evento de "Power Bi"
                        EstudianteId = 5, // Luciana
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                        EventoId = 2, // Evento de "Curso de Python"
                        EstudianteId = 6, // Diego Armando
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,

                    },
                    new Asistencia
                    {
                    EstudianteId = 1, // Alber Yoel
                    EventoId = 1, // Power Bi
                    FechaHoraRegistro = DateTime.Now,
                     AdministradorId = 1,
                    },
                    new Asistencia
                    {
                        EstudianteId = 7, // Mariana
                        EventoId = 2, // Curso de Python
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,
                    },
                    new Asistencia
                    {
                        EstudianteId = 8, // Andres
                        EventoId = 2, // Curso de Python
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,
                    },
                    new Asistencia
                    {
                        EstudianteId = 9, // Camila
                        EventoId = 2, // Curso de Python
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,
                    },
                    new Asistencia
                    {
                        EstudianteId = 2, // Sayra Silvana
                        EventoId = 2, // Curso de Python
                        FechaHoraRegistro = DateTime.Now,
                        AdministradorId = 1,
                    }
              };
            context.AddRange(asistencias);
            context.SaveChanges();

        }
    }
}

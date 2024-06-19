using OrientaTEC_MVC.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class Actividad
    {
        private int idActividad;
        private string nombre;
        private string descripcion;
        private int semana;
        private DateTime fechaExacta;
        private int diasPreviosParaAnunciar;
        private int diasParaRecordar;
        private bool esVirtual;
        private string reunionURL;
        private string aficheURL;
        private EstadoRegistrado estado;
        private TipoActividad tipo;
        private List<Comentario> comentarios = new List<Comentario>();
        private List<Profesor> responsables = new List<Profesor>();
        private List<FechaRecordatorio> recordatorios = new List<FechaRecordatorio>();
        private List<Observador> observadores = new List<Observador>() { };


        [Required]
        public int IdActividad
        {
            get => idActividad;
            set => idActividad = value;
        }

        [Required]
        public string Nombre
        {
            get => nombre;
            set => nombre = value;
        }

        public string Descripcion
        {
            get => descripcion;
            set => descripcion = value;
        }

        public int Semana
        {
            get => semana;
            set => semana = value;
        }

        public DateTime FechaExacta
        {
            get => fechaExacta;
            set => fechaExacta = value;
        }

        public int DiasPreviosParaAnunciar
        {
            get => diasPreviosParaAnunciar;
            set => diasPreviosParaAnunciar = value;
        }

        public int DiasParaRecordar
        {
            get => diasParaRecordar;
            set => diasParaRecordar = value;
        }

        public bool EsVirtual
        {
            get => esVirtual;
            set => esVirtual = value;
        }

        public string ReunionURL
        {
            get => reunionURL;
            set => reunionURL = value;
        }

        public string AficheURL
        {
            get => aficheURL;
            set => aficheURL = value;
        }
        public void CambiarEstado(EstadoActividad estado)
        {
            Estado.Estado = estado;
            ActividadDAO actividadDAO = new ActividadDAO();
            actividadDAO.ActualizarEstado(this.idActividad, estado);
            AgregarObservador(new BuzonDeEstudiante());
            if (estado == EstadoActividad.Cancelada)
            {
                NotificarObservadores(this, $"Cancelación: {this.Nombre}", $"La actividad '{this.Nombre}' ha sido cancelada.", new DateTime());
            }
        }
        public EstadoRegistrado Estado
        {
            get => estado;
            set => estado = value;
        }

        public TipoActividad Tipo
        {
            get => tipo;
            set => tipo = value;
        }

        public List<Comentario> Comentarios
        {
            get => comentarios;
            set => comentarios = value ?? new List<Comentario>();
        }

        public List<Profesor> Responsables
        {
            get => responsables;
            set => responsables = value ?? new List<Profesor>();
        }

        public List<FechaRecordatorio> Recordatorios
        {
            get => recordatorios;
            set => recordatorios = value ?? new List<FechaRecordatorio>();
        }

        public void Aceptar(Visitor visitor)
        {
            visitor.VisitarPublicacion(this);
            visitor.VisitarRecordatorio(this);
        }

        public void AgregarObservador(Observador observador)
        {
            observadores.Add(observador);
        }

        public void EliminarObservador(Observador observador)
        {
            observadores.Remove(observador);
        }

        public void NotificarObservadores(Actividad actividad, string titulo, string mensaje, DateTime fecha)
        {
            foreach (var observador in observadores)
            {
                observador.Actualizar(actividad, titulo, mensaje, fecha);
            }
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        private List<Comentario> comentarios;
        private List<Profesor> responsables;
        private List<FechaRecordatorio> recordatorios;

        [Required]
        public int IdActividad
        {
            get { return idActividad; }
            set { idActividad = value; }
        }

        [Required]
        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        [Required]
        public int Semana
        {
            get { return semana; }
            set { semana = value; }
        }

        [Required]
        public DateTime FechaExacta
        {
            get { return fechaExacta; }
            set { fechaExacta = value; }
        }

        public int DiasPreviosParaAnunciar
        {
            get { return diasPreviosParaAnunciar; }
            set { diasPreviosParaAnunciar = value; }
        }

        public int DiasParaRecordar
        {
            get { return diasParaRecordar; }
            set { diasParaRecordar = value; }
        }

        public bool EsVirtual
        {
            get { return esVirtual; }
            set { esVirtual = value; }
        }

        public string ReunionURL
        {
            get { return reunionURL; }
            set { reunionURL = value; }
        }

        public string AficheURL
        {
            get { return aficheURL; }
            set { aficheURL = value; }
        }

        [Required]
        public EstadoRegistrado Estado
        {
            get { return estado; }
            set { estado = value; }
        }

        [Required]
        public TipoActividad Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public List<Comentario> Comentarios
        {
            get { return comentarios; }
            set { comentarios = value ?? new List<Comentario>(); }
        }

        public List<Profesor> Responsables
        {
            get { return responsables; }
            set { responsables = value ?? new List<Profesor>(); }
        }

        public List<FechaRecordatorio> Recordatorios
        {
            get { return recordatorios; }
            set { recordatorios = value ?? new List<FechaRecordatorio>(); }
        }
    }
}



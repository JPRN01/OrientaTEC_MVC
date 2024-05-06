using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class PlanTrabajo
    {
        private int id;
        private List<Actividad> actividades;

        [Required]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public List<Actividad> Actividades
        {
            get { return actividades; }
            set { actividades = value ?? new List<Actividad>(); }
        }
    }
}

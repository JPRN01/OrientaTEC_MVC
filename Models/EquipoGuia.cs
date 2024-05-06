using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace OrientaTEC_MVC.Models
{
    public class EquipoGuia
    {
        private int generacion;
        private Profesor coordinador;
        private List<Profesor> integrantes;
        private List<PlanTrabajo> planes;

        [Required]
        public int Generacion
        {
            get { return generacion; }
            set { generacion = value; }
        }

        public Profesor Coordinador
        {
            get { return coordinador; }
            set { coordinador = value; }
        }

        public List<Profesor> Integrantes
        {
            get { return integrantes; }
            set { integrantes = value ?? new List<Profesor>(); }
        }

        public List<PlanTrabajo> Planes
        {
            get { return planes; }
            set { planes = value ?? new List<PlanTrabajo>(); }
        }
    }
}

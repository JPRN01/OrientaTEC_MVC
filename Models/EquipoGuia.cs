using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrientaTEC_MVC.Models
{
    public class EquipoGuia
    {
        [Required]
        private int generacion;
        public int Generacion
        {
            get => generacion;
            set => generacion = value;
        }

        private Profesor coordinador;
        public Profesor Coordinador
        {
            get => coordinador;
            set => coordinador = value;
        }

        private List<Profesor> integrantes;
        public List<Profesor> Integrantes
        {
            get => integrantes;
            set => integrantes = value;
        }

        public EquipoGuia(){}

        public void AsignarCoordinador(Profesor profesor)
        {
            Coordinador = profesor;
        }

        public void AgregarProfesor(Profesor profesor)
        {
            Integrantes.Add(profesor);
        }
    }
}

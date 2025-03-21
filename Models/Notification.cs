﻿using System;
using System.ComponentModel.DataAnnotations;


namespace OrientaTEC_MVC.Models
{
    public class Notification
    {
        private int id;
        private Actividad actividad;
        private bool visto; 


        [Required]
        public int Id
        {
            get => id;
            set => id = value;
        }

        private string title;


        // Añade la propiedad Visto al modelo
        public bool Visto
        {
            get => visto;
            set => visto = value;
        }




        [Required]
        [StringLength(100)]
        public string Title
        {
            get => title;
            set => title = value;
        }

        private string message;

        [Required]
        [StringLength(100)]
        public string Message
        {
            get => message;
            set => message = value;
        }

        private DateTime dateTime;

        [Required]

        public DateTime DateTime

        {
            get => dateTime;
            set => dateTime = value;
        }



        //PONGO ACTIVIDAD DAVID

        public Actividad Actividad
        {
            get => actividad;
            set => actividad = value;
        }


        public Notification() { }

        public Notification(int id, string title, string message, DateTime dateTime, bool visto=false)

        {
            Id = id;
            Title = title;
            Message = message;
            DateTime = dateTime;
            Visto = visto; 
        }


    }

}
using System;
using System.ComponentModel.DataAnnotations;


namespace OrientaTEC_MVC.Models
{
    public class Notification
    {
        private int id;

        [Required]
        public int Id
        {
            get => id;
            set => id = value;

        }

        private string title;


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


        public Notification() { }

        public Notification(int id , string title , string message , string time)
        {

            Id = id;
            Title = title;
            Message = message;
            DateTime = dateTime;
           
         


        }


    }

}
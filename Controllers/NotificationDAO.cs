using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OrientaTEC_MVC.Models;

namespace OrientaTEC_MVC.Controllers
{
    public class NotificationDAO : Controller
    {
        private readonly string connectionString;

        public NotificationDAO()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            connectionString = config.GetConnectionString("DefaultConnection");
        }

        public List<Notification> GetAllNotifications()
        {
            List<Notification> notifications = new List<Notification>();
            string query = "SELECT Id, Title, Message, DateTime, Visto, Actividad_Id FROM Notification";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                notifications.Add(new Notification
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Message = reader["Message"].ToString(),
                                    DateTime = Convert.ToDateTime(reader["DateTime"]),
                                    Visto = Convert.ToBoolean(reader["Visto"]),
                                    Actividad = reader["Actividad_Id"] != DBNull.Value ? new Actividad { IdActividad = Convert.ToInt32(reader["Actividad_Id"]) } : null
                                });
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return notifications;
        }

        public Notification GetNotificationById(int id)
        {
            Notification notification = null;
            string query = "SELECT Id, Title, Message, DateTime, Visto, Actividad_Id FROM Notification WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                notification = new Notification
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Message = reader["Message"].ToString(),
                                    DateTime = Convert.ToDateTime(reader["DateTime"]),
                                    Visto = Convert.ToBoolean(reader["Visto"]),
                                    Actividad = reader["Actividad_Id"] != DBNull.Value ? new Actividad { IdActividad = Convert.ToInt32(reader["Actividad_Id"]) } : null
                                };
                            }
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return notification;
        }

        public void MarkAsViewed(int id)
        {
            string query = "UPDATE Notification SET Visto = 1 WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error marking notification as viewed: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool AddNotification(Notification notification)
        {
            string queryCheck = "SELECT COUNT(*) FROM Notification WHERE Title = @Title AND DateTime = @DateTime AND Actividad_Id = @Actividad_Id";
            string queryInsert = "INSERT INTO Notification (Title, Message, DateTime, Visto, Actividad_Id) VALUES (@Title, @Message, @DateTime, @Visto, @Actividad_Id)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand checkCommand = new SqlCommand(queryCheck, conn, transaction))
                        {
                            checkCommand.Parameters.AddWithValue("@Title", notification.Title);
                            checkCommand.Parameters.AddWithValue("@DateTime", notification.DateTime);
                            checkCommand.Parameters.AddWithValue("@Actividad_Id", notification.Actividad?.IdActividad ?? (object)DBNull.Value);

                            int count = (int)checkCommand.ExecuteScalar();
                            if (count > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                        }

                        using (SqlCommand insertCommand = new SqlCommand(queryInsert, conn, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@Title", notification.Title);
                            insertCommand.Parameters.AddWithValue("@Message", notification.Message);
                            insertCommand.Parameters.AddWithValue("@DateTime", notification.DateTime);
                            insertCommand.Parameters.AddWithValue("@Visto", notification.Visto);
                            insertCommand.Parameters.AddWithValue("@Actividad_Id", notification.Actividad?.IdActividad ?? (object)DBNull.Value);

                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error adding notification: " + ex.Message);
                        return false;
                    }
                }
            }
        }


        public bool UpdateNotification(Notification notification)
        {
            string query = "UPDATE Notification SET Title = @Title, Message = @Message, DateTime = @DateTime, Visto = @Visto, Actividad_Id = @Actividad_Id WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Id", notification.Id);
                        command.Parameters.AddWithValue("@Title", notification.Title);
                        command.Parameters.AddWithValue("@Message", notification.Message);
                        command.Parameters.AddWithValue("@DateTime", notification.DateTime);
                        command.Parameters.AddWithValue("@Visto", notification.Visto);
                        command.Parameters.AddWithValue("@Actividad_Id", notification.Actividad?.IdActividad ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error updating notification: " + ex.Message);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public bool DeleteNotification(int id)
        {
            string query = "DELETE FROM Notification WHERE Id = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Error deleting notification: " + ex.Message);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}

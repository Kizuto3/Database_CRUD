using System;
using System.Collections.Generic;
using System.Data;
using machineCRUD.Models;
using MySql.Data.MySqlClient;

namespace machineCRUD.DataOperations
{
    public class InventoryDAL
    {
        private readonly string _connectionString;
        private MySqlConnection _mySqlConnection = null;
        
        public InventoryDAL() : this(@"Datasource=localhost;Database=master;uid=root;pwd=Assassin_Ezio123;"){}

        public InventoryDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        private void OpenConnection()
        {
            _mySqlConnection = new MySqlConnection
            {
                ConnectionString = _connectionString
            };
            _mySqlConnection.Open();
        }

        private void CloseConnection()
        {
            if (_mySqlConnection?.State != ConnectionState.Closed)
            {
                _mySqlConnection?.Close();
            }
        }
        
        public List<Machine> GetAllMachines()
        {
            OpenConnection();
            var machines = new List<Machine>();
            const string sql = "Select * from machine";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                command.CommandType = CommandType.Text;
                MySqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (dataReader.Read())
                {
                    machines.Add(new Machine
                    {
                        MachineId = (int) dataReader["id"],
                        Producer = (string) dataReader["producer"],
                        Type = (string) dataReader["type"],
                        Price = (float) dataReader["price"],
                        Flops = (int) dataReader["flops"]
                    });
                }
                dataReader.Close();
            }

            return machines;
        }

        public Machine GetMachine(int id)
        {
            OpenConnection();
            Machine machine = null;
            var sql = $"Select * from machine where id = {id}";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                command.CommandType = CommandType.Text;
                var dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (dataReader.Read())
                {
                    machine = new Machine
                    {
                        MachineId = (int) dataReader["id"],
                        Producer = (string) dataReader["producer"],
                        Type = (string) dataReader["type"],
                        Price = (float) dataReader["price"],
                        Flops = (int) dataReader["flops"]
                    };
                }
                dataReader.Close();
            }

            return machine;
        }

        public void InsertMachine(string producer, string type, int flops, float price)
        {
            OpenConnection();
            var sql = "Insert into machine (producer, type, flops, price) values(" +
                      "@Producer, @Type, @Flops, @Price);";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                var parameter = new MySqlParameter
                {
                    ParameterName = "@Producer",
                    Value = producer,
                    MySqlDbType = MySqlDbType.String,
                    Size = 20
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Type",
                    Value = type,
                    MySqlDbType = MySqlDbType.String,
                    Size = 6
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Flops",
                    Value = flops,
                    MySqlDbType = MySqlDbType.Int32
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Price",
                    Value = price,
                    MySqlDbType = MySqlDbType.Float
                };
                command.Parameters.Add(parameter);
                
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        /// <summary>
        /// Non parametrized query
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="something"></param> просто шо бы не мешало параметризированному запросу
        public void InsertMachine(Machine machine, string something)
        {
            OpenConnection();
            var sql = $"Insert into machine (producer, type, flops, price) values(" +
                      $"'{machine.Producer}', '{machine.Type}', {machine.Flops}, {machine.Price});";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }
        
        /// <summary>
        /// Parametrized query
        /// </summary>
        /// <param name="machine"></param>
        public void InsertMachine(Machine machine)
        {
            OpenConnection();
            var sql = $"Insert into machine (producer, type, flops, price) values(" +
                      "@Producer, @Type, @Flops, @Price);";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                var parameter = new MySqlParameter
                {
                    ParameterName = "@Producer",
                    Value = machine.Producer,
                    MySqlDbType = MySqlDbType.String,
                    Size = 20
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Type",
                    Value = machine.Type,
                    MySqlDbType = MySqlDbType.String,
                    Size = 6
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Flops",
                    Value = machine.Flops,
                    MySqlDbType = MySqlDbType.Int32
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@Price",
                    Value = machine.Price,
                    MySqlDbType = MySqlDbType.Float
                };
                command.Parameters.Add(parameter);
                
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public void DeleteMachine(int id)
        {
            OpenConnection();
            var sql = $"delete from machine where id = {id};";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                try
                {
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    var error = new Exception("Sorry, there is no such machine", e);
                    throw error;
                }
            }
            CloseConnection();
        }

        public void UpdateMachine(int id, string producer)
        {
            OpenConnection();
            var sql = $"Update machine Set producer = '{producer}' where id = {id}";
            using (var command = new MySqlCommand(sql, _mySqlConnection))
            {
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public string LookUpProducer(int machineId)
        {
            OpenConnection();
            string machineProducer;
            using (var command = new MySqlCommand("GetMachineProducer", _mySqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;
                var parameter = new MySqlParameter
                {
                    ParameterName = "@machineId",
                    MySqlDbType = MySqlDbType.Int32,
                    Value = machineId,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(parameter);
                
                parameter = new MySqlParameter
                {
                    ParameterName = "@machineProducer",
                    MySqlDbType = MySqlDbType.String,
                    Size = 20,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();

                machineProducer = (string) command.Parameters["@machineProducer"].Value;
                CloseConnection();
            }

            return machineProducer;
        }

        public void ProcessOnRepair(bool throwEx, int machineId)
        {
            OpenConnection();
            Machine machine;
            var sqlSelect = new MySqlCommand("Select * from machine where id = @machineId", _mySqlConnection);
            var parameter = new MySqlParameter
            {
                ParameterName = "@machineId",
                MySqlDbType = MySqlDbType.Int32,
                Value = machineId
            };
            sqlSelect.Parameters.Add(parameter);
            using (var dataReader = sqlSelect.ExecuteReader())
            {
                if (dataReader.HasRows)
                {
                    dataReader.Read();
                    machine = new Machine
                    {
                        Producer = (string)dataReader["producer"],
                        Price = (float) dataReader["price"],
                        Type = (string) dataReader["type"],
                        Flops = (int) dataReader["flops"]
                    };
                }
                else
                {
                    CloseConnection();
                    return;
                }
            }
            var sqlRemove = new MySqlCommand("Delete from machine where id = @machineId", _mySqlConnection);
            sqlRemove.Parameters.Add(parameter);
            var sqlInsert = new MySqlCommand("Insert into onrapairs(id, producer, type, price, flops) values (" +
            "@machineId, @Producer, @Type, @Price, @Flops)", _mySqlConnection);
            sqlInsert.Parameters.Add(parameter);
            
            parameter = new MySqlParameter
            {
                ParameterName = "@Producer",
                Value = machine.Producer,
                MySqlDbType = MySqlDbType.String,
                Size = 20
            };
            sqlInsert.Parameters.Add(parameter);
                
            parameter = new MySqlParameter
            {
                ParameterName = "@Type",
                Value = machine.Type,
                MySqlDbType = MySqlDbType.String,
                Size = 6
            };
            sqlInsert.Parameters.Add(parameter);
                
            parameter = new MySqlParameter
            {
                ParameterName = "@Flops",
                Value = machine.Flops,
                MySqlDbType = MySqlDbType.Int32
            };
            sqlInsert.Parameters.Add(parameter);
                
            parameter = new MySqlParameter
            {
                ParameterName = "@Price",
                Value = machine.Price,
                MySqlDbType = MySqlDbType.Float
            };
            sqlInsert.Parameters.Add(parameter);
            MySqlTransaction tx = null;
            try
            {
                tx = _mySqlConnection.BeginTransaction();
                sqlInsert.Transaction = tx;
                sqlRemove.Transaction = tx;

                sqlInsert.ExecuteNonQuery();
                sqlRemove.ExecuteNonQuery();

                if (throwEx)
                {
                    throw new Exception("Sorry! Database failed transaction");
                }

                tx.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                tx?.Rollback();
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
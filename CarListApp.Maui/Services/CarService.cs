using CarListApp.Maui.Models;
using SQLite;

namespace CarListApp.Maui.Services
{
    public class CarService
    {
        private SQLiteConnection _connection;
        private readonly string _dbPath;
        private int result = 0;

        public string StatusMessage { get; private set; }

        public CarService(string dbPath)
        {
            _dbPath = dbPath;
        }

        private void Init()
        {
            if (_connection != null) return;

            _connection = new SQLiteConnection(_dbPath);
            _connection.CreateTable<Car>();
        }

        public List<Car> GetCars()
        {
            try
            {
                Init();
                return _connection.Table<Car>().ToList();
            }
            catch (Exception)
            {
                StatusMessage = "Failed to retrieve data.";
            }

            return new List<Car>();
        }

        public Car GetCar(int id)
        {
            try
            {
                Init();
                return _connection.Table<Car>().FirstOrDefault(q => q.Id == id);
            }
            catch (Exception)
            {
                StatusMessage = "Failed to retrieve data.";
            }

            return null;
        }

        public void AddCar(Car car)
        {
            try
            {
                Init();

                if (car == null)
                    throw new Exception("Invalid car record.");

                result = _connection.Insert(car);
                StatusMessage = result == 0 ? "Insert Failed." : "Insert Successful.";
            }
            catch (Exception)
            {
                StatusMessage = "Failed to insert data.";
            }
        }

        public int DeleteCar(int id)
        {
            try
            {
                Init();
                return _connection.Table<Car>().Delete(q => q.Id == id);
            }
            catch (Exception)
            {
                StatusMessage = "Failed to delete data.";
            }

            return 0;
        }
    }
}

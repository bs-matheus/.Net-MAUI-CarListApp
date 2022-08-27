using CarListApp.Maui.Models;
using CarListApp.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CarListApp.Maui.ViewModels
{
    public partial class CarListViewModel : BaseViewModel
    {
        private const string ADD_CAR_TEXT = "Add Car";
        private const string UPDATE_CAR_TEXT = "Update Car";

        private const string CANCEL_TEXT = "Cancel";
        private const string CLEAR_FORM_TEXT = "Clear Form";

        private int updatingCarId = 0;

        public ObservableCollection<Car> Cars { get; private set; } = new ObservableCollection<Car>();

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        string addOrEditText;
        
        [ObservableProperty]
        string clearOrCancelText;

        [ObservableProperty]
        string make;

        [ObservableProperty]
        string model;

        [ObservableProperty]
        string vin;

        public CarListViewModel()
        {
            Title = "Car List";

            ResetButtonsText();
            GetCarListAsync().Wait();
        }

        private void ResetButtonsText()
        {
            AddOrEditText = ADD_CAR_TEXT;
            ClearOrCancelText = CLEAR_FORM_TEXT;
        }

        [RelayCommand]
        void ClearForm()
        {
            updatingCarId = 0;
            Make = Model = Vin = string.Empty;

            ResetButtonsText();
        }

        [RelayCommand]
        async Task GetCarListAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;

                if (Cars.Any())
                    Cars.Clear();

                var cars = App.CarService.GetCars();

                foreach (var car in cars)
                {
                    Cars.Add(car);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get cars: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to retrieve list of cars.", "Ok");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        async Task GoToCarDetailsAsync(int id)
        {
            if (id == 0) return;

            await Shell.Current.GoToAsync($"{nameof(CarDetailsPage)}?Id={id}", true);
        }

        [RelayCommand]
        async Task SaveCarAsync()
        {
            if (string.IsNullOrEmpty(Model) || string.IsNullOrEmpty(Make) || string.IsNullOrEmpty(Vin))
            {
                await Shell.Current.DisplayAlert("Invalid Data", "Please insert valid data.", "Ok");
                return;
            }

            var car = new Car
            {
                Make = Make,
                Model = Model,
                Vin = Vin
            };

            if (updatingCarId != 0)
            {
                car.Id = updatingCarId;
                App.CarService.UpdateCar(car);
            }
            else
            {
                App.CarService.AddCar(car);
            }

            await Shell.Current.DisplayAlert("Info", App.CarService.StatusMessage, "Ok");
            await GetCarListAsync();
            ClearForm();
        }

        [RelayCommand]
        void SetEditMode(int id)
        {
            var car = App.CarService.GetCar(id);

            Make = car.Make;
            Model = car.Model;
            Vin = car.Vin;

            updatingCarId = id;
            AddOrEditText = UPDATE_CAR_TEXT;
            ClearOrCancelText = CANCEL_TEXT;
        }

        [RelayCommand]
        async Task DeleteCarAsync(int id)
        {
            if (id == 0)
            {
                await Shell.Current.DisplayAlert("Invalid Record", "Please try again.", "Ok");
                return;
            }

            int result = App.CarService.DeleteCar(id);

            if (result == 0)
            {
                await Shell.Current.DisplayAlert("Failed", "Please try again.", "Ok");
            }
            else
            {
                await Shell.Current.DisplayAlert("Deletion Successful", "Record removed successfully.", "Ok");
                await GetCarListAsync();
            }
        }
    }
}

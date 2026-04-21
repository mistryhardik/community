using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyWeatherApp
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowMessage(string title, string message, string buttonConfirmText, string buttonCancelText)
        {
            try
            {
                var result = await Application.Current.MainPage.DisplayAlert(title, message, buttonConfirmText, buttonCancelText);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return false;
            }
        }

        public async Task ShowMessage(string title, string message, string buttonCloseText)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(title, message, buttonCloseText);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        public async Task<string> ShowMultipleSelection(string title, string[] options)
        {
            try
            {
                var result = await Application.Current.MainPage.DisplayActionSheet(title, null, null, options);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return null;
            }
        }

        public async Task ShowMessage(string message)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(App.AppName, message, "Close");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task ShowServerUnreachableMessage()
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert("Cannot connect to our servers", "Check the device network settings", "Close");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public interface IDialogService
    {
        Task<bool> ShowMessage(string title, string message, string buttonConfirmText, string buttonCancelText);

        Task<string> ShowMultipleSelection(string title, string[] options);

        Task ShowMessage(string title, string message, string buttonCloseText);

        Task ShowMessage(string message);

        Task ShowServerUnreachableMessage();
    }
}

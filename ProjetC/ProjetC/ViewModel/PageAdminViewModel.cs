using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using ProjetC.DAL;
using ProjetC.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace ProjetC.ViewModel
{
    public class PageAdminViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private DataAccess dataAccess;
        private ObservableCollection<RendezVous> _problem;
        private INavigationService _navigationService;
        private Permanence _perma;

        [PreferredConstructor]
        public PageAdminViewModel(INavigationService navigationService = null)
        {
            _navigationService = navigationService;
            dataAccess = new DataAccess();
            _problem = new ObservableCollection<RendezVous>();
            _perma = new Permanence();
            getAllPerma();
            getAllProblem();
        }


        public void OnNavigatedTo(NavigationEventArgs e)
        { }

        public ObservableCollection<RendezVous> RendezVous
        {
            get
            {
                return _problem;
            }
            set
            {
                _problem = value;
                RaisePropertyChanged("PageAdmin");
            }
        }
        public async void getAllProblem()
        {
            if (CheckNet())
            {
                List<RendezVous> lstProblem = await dataAccess.getAllProblem();
                foreach (var item in lstProblem)
                    if (item.idPerma == _perma.idPerma)
                        RendezVous.Add(item);
            }
        }

        public ICommand _ButtonAccueil;
        public ICommand ButtonAccueil
        {
            get
            {
                if (_ButtonAccueil == null)
                    _ButtonAccueil = new RelayCommand(() => GoToMainPage());
                return _ButtonAccueil;
            }
        }

        private void GoToMainPage()
        {
            _navigationService.NavigateTo("MainPage");
        }


        public Permanence Permanence
        {
            get
            {
                return _perma;
            }
            set
            {
                _perma = value;
                RaisePropertyChanged("FAQ");
            }
        }

        public async void getAllPerma()
        {
            if (CheckNet())
            {
                List<Permanence> lstPerma = await dataAccess.getAllPerma();
                DateTime dateAjd = DateTime.Today;
                foreach (var item in lstPerma)
                {
                    DateTime td = Convert.ToDateTime(item.datePerma);
                    if (td > dateAjd && _perma.heureDebutPerma == 0)
                    {
                        _perma = item;
                    }
                }
            }
            else
                exit();
        }

        public async void exit()
        {
            var messageDialog = new MessageDialog("No internet connection has been found.");
            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand(
                "Close program",
                new UICommandInvokedHandler(this.CommandInvokedHandler)));
            await messageDialog.ShowAsync();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label.Equals("Close program"))
            {
                Application.Current.Exit();
            }
        }

        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckNet()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }


    }
}


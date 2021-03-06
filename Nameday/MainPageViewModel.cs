﻿using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;

namespace Nameday
{
    public class MainPageViewModel : ObservableObject
    {
        private List<NamedayModel> _allNamedays =
            new List<NamedayModel>();
        
        public ObservableCollection<NamedayModel> Namedays { get; set; }

        public ObservableCollection<ContactEx> Contacts { get; } =
            new ObservableCollection<ContactEx>();

        public Settings Settings { get; } = new Settings();

        public MainPageViewModel()
        {
            AddReminderCommand = new AddReminderCommand(this);

            Namedays = new ObservableCollection<NamedayModel>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Contacts = new ObservableCollection<ContactEx>
                {
                    new ContactEx("Contact", "1"),
                    new ContactEx("Contact", "2"),
                    new ContactEx("Contact", "3")
                };

                for (int month = 1; month <= 12; month++)
                {
                    _allNamedays.Add(new NamedayModel(
                        month, 1, new string[] { "Nathan" }));
                    _allNamedays.Add(new NamedayModel(
                        month, 24, new string[] { "Arthur", "Allan" }));
                }

                PerformFiltering();
            }
            else
            {
                LoadData();
            }
        }

        private LoadingStates _loadingStates = LoadingStates.Loading;

        public LoadingStates LoadingState
        {
            get { return _loadingStates; }
            set { Set(ref _loadingStates, value); }
        }
        
        private async void LoadData()
        {
            try
            {
                _allNamedays = await NamedayRepository.GetAllNamedaysAsync();
                PerformFiltering();
                LoadingState = LoadingStates.Loaded;
            }
            catch
            {
                LoadingState = LoadingStates.Error;
            }
            
            // get current nameday
            var now = DateTime.Now;
            SelectedNameday = _allNamedays.FirstOrDefault(n => n.Day == now.Day && n.Month == now.Month);
        }

        private string _greeting = "Hello UWP";
        public string Greeting
        {
            get { return _greeting; }
            set { Set(ref _greeting, value); }
        }

        private NamedayModel _selectedNameday;       
        public NamedayModel SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                Set(ref _selectedNameday, value);

                if (value == null)
                    Greeting = "Hello UWP!";
                else
                    Greeting = "Hello " + value.NamesAsString;

                UpdateContacts();
                AddReminderCommand.FireCanExecuteChanged();
            }
        }
        
        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (Set(ref _filter, value))
                    PerformFiltering();
            }
        }

        private async void UpdateContacts()
        {
            Contacts.Clear();

            if (SelectedNameday != null)
            {
                var contactStore =
                    await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

                foreach (var name in SelectedNameday.Names)
                    foreach (var contact in await contactStore.FindContactsAsync(name))
                        Contacts.Add(new ContactEx(contact));
            }
        }

        private void PerformFiltering()
        {
            if (_filter == null)
                _filter = "";

            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();

            var result =
                _allNamedays.Where(d => d.NamesAsString.ToLowerInvariant()
                .Contains(lowerCaseFilter))
                .ToList();

            var toRemove = Namedays.Except(result).ToList();

            foreach (var x in toRemove)
                Namedays.Remove(x);

            var resultCount = result.Count;
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > Namedays.Count || !Namedays[i].Equals(resultItem))
                    Namedays.Insert(i, resultItem);
            }
        }

        public async Task SendEmailAsync(Contact contact)
        {
            if (contact == null || contact.Emails.Count == 0)
                return;

            var msg = new EmailMessage();
            msg.To.Add(new EmailRecipient(contact.Emails[0].Address));
            msg.Subject = "Joyeuse fête!";

            await EmailManager.ShowComposeNewEmailAsync(msg);
        }

        public enum LoadingStates
        {
            Loading,
            Loaded,
            Error
        }

        public AddReminderCommand AddReminderCommand { get; }

        public async void AddReminderToCalendarAsync()
        {
            var appointment = new Appointment();
            appointment.Subject = "Rappel de fête pour " + SelectedNameday.NamesAsString;
            appointment.AllDay = true;
            appointment.BusyStatus = AppointmentBusyStatus.Free;
            var dateThisYear = new DateTime(
                DateTime.Now.Year, SelectedNameday.Month, SelectedNameday.Day);
            appointment.StartTime =
                dateThisYear < DateTime.Now ? dateThisYear.AddYears(1) : dateThisYear;

            await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
        }
    }
}

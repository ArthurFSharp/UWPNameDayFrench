using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nameday
{
    public class AddReminderCommand : System.Windows.Input.ICommand
    {
        private MainPageViewModel _mpd;

        public AddReminderCommand(MainPageViewModel mpd)
        {
            _mpd = mpd;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _mpd.SelectedNameday != null;

        public void Execute(object parameter) => _mpd.AddReminderToCalendarAsync();

        public void FireCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

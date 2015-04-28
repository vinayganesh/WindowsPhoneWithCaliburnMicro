using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CaliburnMicroWIndowsPhone.IServices;

namespace CaliburnMicroWIndowsPhone.ViewModels
{
    public class EmulateContactsViewModel : PropertyChangedBase
    {
        private IEmulateContactsService _emulateContacts;
        private string _contactsCount;
        private bool _isContactsCountEmpty;
        private bool _isInProgress;

        public EmulateContactsViewModel(IEmulateContactsService emulateContacts)
        {
            _emulateContacts = emulateContacts;
        }

        public string ContactsCount
        {
            get { return _contactsCount; }
            set
            {
                if (value == _contactsCount) return;
                _contactsCount = value;
                NotifyOfPropertyChange(() => ContactsCount);
            }
        }

        public bool IsContactsCountEmpty
        {
            get { return _isContactsCountEmpty; }
            set
            {
                if (value == _isContactsCountEmpty) return;
                _isContactsCountEmpty = value;
                NotifyOfPropertyChange(() => IsContactsCountEmpty);
            }
        }

        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                if (value == _isInProgress) return;
                _isInProgress = value;
                NotifyOfPropertyChange(() => IsInProgress);
            }
        }

        public void CheckButtonEnabled()
        {
            if (ContactsCount != null)
                IsContactsCountEmpty = false;
            else
            {
                IsContactsCountEmpty = true;
            }
        }

        public void Emulate()
        {
            try
            {
                IsInProgress = true;
                _emulateContacts.Emulate(ContactsCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsInProgress = false;
            }

        }
    }
}

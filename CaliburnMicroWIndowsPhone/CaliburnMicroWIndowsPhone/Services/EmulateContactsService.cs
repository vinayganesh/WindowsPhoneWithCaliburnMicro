using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.PersonalInformation;
using Windows.UI.Popups;
using CaliburnMicroWIndowsPhone.IServices;

namespace CaliburnMicroWIndowsPhone.Services
{
    public class EmulateContactsService : IEmulateContactsService
    {
        public async void Emulate(string numberOfContacts)
        {
            try
            {
                for (var i = 0; i < int.Parse(numberOfContacts); i++)
                {
                    // Use this for the remote Id 
                    // for each contact
                    var r = new Random();
                    var remoteId = r.Next();

                    // Generate Random Names
                    var givenName = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
                    var mail = Path.GetRandomFileName().Replace(".", "").Substring(0, 10);
                    var familyName = Path.GetRandomFileName().Replace(".", "").Substring(0, 4);
                    var displayName = Path.GetRandomFileName().Replace(".", "").Substring(0, 6);

                    var remoteContact = new RemoteContact
                    {
                        RemoteId = remoteId.ToString(),
                        GivenName = givenName,
                        FamilyName = familyName,
                        DisplayName = displayName,
                        Email = mail + "@gmail.com",
                        CodeName = "R"
                    };

                    await AddContact(remoteContact);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                var messageDialog = new MessageDialog("Contacts Generation Complete");
                
                messageDialog.ShowAsync();
            }
        }

        private static async Task AddContact(RemoteContact remoteContact)
        {
            try
            {
                ContactStore store = await ContactStore.CreateOrOpenAsync(ContactStoreSystemAccessMode.ReadWrite, ContactStoreApplicationAccessMode.ReadOnly);

                var contact = new StoredContact(store);

                if (remoteContact.RemoteId == null) return;

                var remoteIdHelper = new RemoteIdHelper();
                contact.RemoteId = await remoteIdHelper.GetTaggedRemoteId(store, remoteContact.RemoteId);

                IDictionary<string, object> props = await contact.GetPropertiesAsync();
                if (remoteContact.Email != null) props.Add(KnownContactProperties.Email, remoteContact.Email);

                IDictionary<string, object> extprops = await contact.GetExtendedPropertiesAsync();
                if (remoteContact.CodeName != null) extprops.Add("Codename", remoteContact.CodeName);

                if (remoteContact.DisplayName != null) contact.DisplayName = remoteContact.DisplayName + " " + remoteContact.GivenName;

                var prop = await contact.GetPropertiesAsync();

                prop.Add(KnownContactProperties.MobileTelephone, "+1" + MdnGenerator(10));
                prop.Add(KnownContactProperties.JobTitle, JobTitleGenerator());
                prop.Add(KnownContactProperties.OfficeLocation, LocationGenerator());
                prop.Add(KnownContactProperties.Notes, JobTitleGenerator());

                Debug.WriteLine(contact.Store);

                await contact.SaveAsync();
                Debug.WriteLine(String.Format("Adding:\n{0}", remoteContact.ToString()));
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static string MdnGenerator(int size)
        {
            var rng = new Random();
            const string chars = "0123456789";

            var buffer = new char[size];

            for (var i = 0; i < size; i++)
            {
                buffer[i] = chars[rng.Next(chars.Length)];
            }
            return new string(buffer);
        }

        private static string LocationGenerator()
        {
            var random = new Random();
            var cities = new[]
            {
                "Bridgewater", "Brooklyn", "San Francisco", "Salinas", "Washington", "Los Angeles", "Seattle", "Dallas",
                "Austin", "Memphis", "Sacramento", "Oakland", "Hayward", "San Jose", "Milipitas", "Sunnyvale",
                "Mountain View"
            };

            return cities[random.Next(cities.Length)];
        }

        private static string JobTitleGenerator()
        {
            var random = new Random();
            var job = new[]
            {
                "Developer", "Technical Lead", "Manager", "Vice President", "Director", "Senior Director",
                "Technical Director", "Software Engineer", "Senior Software Engineer"
            };

            return job[random.Next(job.Length)];
        }
    }

    internal class RemoteIdHelper
    {
        private const string ContactStoreLocalInstanceIdKey = "LocalInstanceId";

        public async Task SetRemoteIdGuid(ContactStore store)
        {
            IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask();
            if (!properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                var guid = Guid.NewGuid();
                properties.Add(ContactStoreLocalInstanceIdKey, guid.ToString());
                var readonlyProperties = new ReadOnlyDictionary<string, object>(properties);
                await store.SaveExtendedPropertiesAsync(readonlyProperties).AsTask();
            }
        }

        public async Task<string> GetTaggedRemoteId(ContactStore store, string remoteId)
        {
            var taggedRemoteId = string.Empty;

            IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                taggedRemoteId = string.Format("{0}_{1}", properties[ContactStoreLocalInstanceIdKey], remoteId);
            }

            return taggedRemoteId;
        }

        public async Task<string> GetUntaggedRemoteId(ContactStore store, string taggedRemoteId)
        {
            var remoteId = string.Empty;

            IDictionary<string, object> properties;
            properties = await store.LoadExtendedPropertiesAsync().AsTask();
            if (properties.ContainsKey(ContactStoreLocalInstanceIdKey))
            {
                var localInstanceId = properties[ContactStoreLocalInstanceIdKey] as string;
                if (taggedRemoteId.Length > localInstanceId.Length + 1)
                {
                    remoteId = taggedRemoteId.Substring(localInstanceId.Length + 1);
                }
            }

            return remoteId;
        }
    }

    public class RemoteContact
    {
        public string RemoteId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CodeName { get; set; }
        public Stream photo { get; set; }

        public override string ToString()
        {
            return String.Format(" {0}\n {1}\n {2}\n {3}\n {4}\n {5}", RemoteId, GivenName, FamilyName, DisplayName,
                Email, CodeName);
        }
    }
}

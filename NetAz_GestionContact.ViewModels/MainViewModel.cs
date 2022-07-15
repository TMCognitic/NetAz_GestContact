using NetAz_GestionContact.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Input;
using Tools.Database;
using Tools.Mvvm.Commands;
using Tools.Mvvm.Mediator;
using Tools.Mvvm.ViewModels;

namespace NetAz_GestionContact.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ContactViewModel> _items;
        private readonly string _ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NetAz_GestContact;Integrated Security=True;";
        private readonly DbProviderFactory _providerFactory;

        private string? _nom, _prenom, _email, _tel;
        private DateTime? _naissance;

        private ICommand? _insertCommand;

        public ObservableCollection<ContactViewModel> Items
        {
            get
            {
                return _items;
            }
        }

        public string? Nom
        {
            get
            {
                return _nom;
            }

            set
            {
                if (_nom != value)
                {
                    _nom = value;
                    RaisePropertyChanged(nameof(Nom));
                }
            }
        }

        public string? Prenom
        {
            get
            {
                return _prenom;
            }

            set
            {
                if (_prenom != value)
                {
                    _prenom = value;
                    RaisePropertyChanged(nameof(Prenom));
                }
            }
        }

        public string? Email
        {
            get
            {
                return _email;
            }

            set
            {
                if (_email != value)
                {
                    _email = value;
                    RaisePropertyChanged(nameof(Email));
                }
            }
        }

        public string? Tel
        {
            get
            {
                return _tel;
            }

            set
            {
                if (_tel != value)
                {
                    _tel = value;
                    RaisePropertyChanged(nameof(Tel));
                }
            }
        }

        public DateTime? Naissance
        {
            get
            {
                return _naissance;
            }

            set
            {
                if (_naissance != value)
                {
                    _naissance = value;
                    RaisePropertyChanged(nameof(Naissance));
                }
            }
        }

        public ICommand InsertCommand
        {
            get
            {
                return _insertCommand ??= new DelegateCommand(Insert, CanInsert);
            }
        }

        

        public MainViewModel()
        {
            _providerFactory = SqlClientFactory.Instance;
            _items = new ObservableCollection<ContactViewModel>();
            LoadItems();
            Messenger<ContactViewModel>.Instance.Register(OnDeleteContact);
        }

        private void OnDeleteContact(ContactViewModel viewModel)
        {
            Items.Remove(viewModel);
        }

        private void LoadItems()
        {
            using(DbConnection? dbConnection = _providerFactory.CreateConnection())
            {
                if (dbConnection is null)
                    throw new InvalidOperationException("dbConnection is null");

                dbConnection.ConnectionString = _ConnectionString;

                IEnumerable<Contact> contacts = dbConnection.ExecuteReader("SELECT Id, Nom, Prenom, Email, Naissance, Tel FROM Contact;", (dr) => new Contact() { Id = (int)dr["Id"], Nom = (string)dr["Nom"], Prenom = (string)dr["Prenom"], Email = (string)dr["Email"], Naissance = (DateTime)dr["Naissance"], Tel = (string)dr["Tel"] });

                foreach (Contact contact in contacts)
                    Items.Add(new ContactViewModel(contact));
            }
        }

        private void Insert()
        {
            Contact contact = new Contact() { Nom = Nom, Prenom = Prenom, Email = Email, Naissance = Naissance!.Value, Tel = Tel };

            Nom = Prenom = Email = Tel = null;
            Naissance = null;

            using (DbConnection? dbConnection = _providerFactory.CreateConnection())
            {
                if (dbConnection is null)
                    throw new InvalidOperationException("dbConnection is null");

                dbConnection.ConnectionString = _ConnectionString;

                object? result = dbConnection.ExecuteScalar("INSERT INTO Contact (Nom, Prenom, Email, Naissance, Tel) OUTPUT Inserted.Id Values (@Nom, @Prenom, @Email, @Naissance, @Tel);", parameters: contact);

                if (result is null)
                    return;

                contact.Id = (int)result;
                Items.Add(new ContactViewModel(contact));
            }
        }

        private bool CanInsert()
        {
            return !string.IsNullOrEmpty(Nom) &&
                !string.IsNullOrEmpty(Prenom) &&
                !string.IsNullOrEmpty(Email) &&
                Email.Contains("@") &&
                !string.IsNullOrEmpty(Tel) &&
                Naissance.HasValue;
        }
    }
}

using NetAz_GestionContact.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.Database;
using Tools.Mvvm.Commands;
using Tools.Mvvm.Mediator;
using Tools.Mvvm.ViewModels;

namespace NetAz_GestionContact.ViewModels
{
    public class ContactViewModel : ViewModelBase
    {
        private readonly string _ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=NetAz_GestContact;Integrated Security=True;";
        private readonly DbProviderFactory _providerFactory;

        private readonly Contact _entity;
        private string? _email;

        private ICommand? _saveCommand;
        private ICommand? _deleteCommand;

        public ContactViewModel(Contact entity)
        {
            _providerFactory = SqlClientFactory.Instance;
            _entity = entity;
            Email = entity.Email;
        }

        public string? Nom
        {
            get { return _entity.Nom; }
        }

        public string? Prenom
        {
            get { return _entity.Prenom; }
        }

        public string? Email
        {
            get
            {
                return _email;
            }

            set
            {
                if (Email != value)
                {
                    _email = value;
                    RaisePropertyChanged(nameof(Email));
                }
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(Save, CanSave);
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new DelegateCommand(Delete);
            }
        }

        private void Delete()
        {
            using (DbConnection? dbConnection = _providerFactory.CreateConnection())
            {
                if (dbConnection is null)
                    throw new InvalidOperationException("dbConnection is null");

                dbConnection.ConnectionString = _ConnectionString;

                int rows = dbConnection.ExecuteNonQuery("DELETE FROM Contact WHERE Id = @Id;", parameters: new { _entity.Id });

                if (rows == 1)
                    Messenger<ContactViewModel>.Instance.Send(this);
            }            
        }

        private void Save()
        {
            //Sauvegarde dans la DB
            using (DbConnection? dbConnection = _providerFactory.CreateConnection())
            {
                if (dbConnection is null)
                    throw new InvalidOperationException("dbConnection is null");

                dbConnection.ConnectionString = _ConnectionString;

                int rows = dbConnection.ExecuteNonQuery("UPDATE Contact SET Email = @Email WHERE Id = @Id;", parameters: new { Email, _entity.Id });

                if (rows == 1)
                    _entity.Email = Email;
                else
                    Email = _entity.Email;
            }
        }

        private bool CanSave()
        {
            return Email != _entity.Email;
        }
    }
}


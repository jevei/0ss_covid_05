using BillingManagement.Business;
using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using Inventaire;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace BillingManagement.UI.ViewModels
{
    class MainViewModel : BaseViewModel
    {
		static BillingManagementContext db = new BillingManagementContext();
		private BaseViewModel _vm;

		public BaseViewModel VM
		{
			get { return _vm; }
			set {
				_vm = value;
				OnPropertyChanged();
			}
		}

		internal static BillingManagementContext GetDb()
		{
			return db;
		}

		private string searchCriteria;

		public string SearchCriteria
		{
			get { return searchCriteria; }
			set { 
				searchCriteria = value;
				OnPropertyChanged();
			}
		}


		CustomerViewModel customerViewModel;
		InvoiceViewModel invoiceViewModel;
		private IEnumerable<Customer> _customers;
		private IEnumerable<Invoice> _invoices;

		public ChangeViewCommand ChangeViewCommand { get; set; }

		public DelegateCommand<object> AddNewItemCommand { get; private set; }

		public DelegateCommand<Invoice> DisplayInvoiceCommand { get; private set; }
		public DelegateCommand<Customer> DisplayCustomerCommand { get; private set; }

		public DelegateCommand<Customer> AddInvoiceToCustomerCommand { get; private set; }
		public RelayCommand<object> SearchCustomerCommand { get; set; }
		public DelegateCommand<object> ExitClickCommand { get; private set; }

		public MainViewModel()
		{
			ChangeViewCommand = new ChangeViewCommand(ChangeView);
			DisplayInvoiceCommand = new DelegateCommand<Invoice>(DisplayInvoice);
			DisplayCustomerCommand = new DelegateCommand<Customer>(DisplayCustomer);
			SearchCustomerCommand = new RelayCommand<object>(SearchCustomer, SearchCustomerCanExecute);
			AddNewItemCommand = new DelegateCommand<object>(AddNewItem, CanAddNewItem);
			AddInvoiceToCustomerCommand = new DelegateCommand<Customer>(AddInvoiceToCustomer);
			ExitClickCommand = new DelegateCommand<object>(ExitClick);


			SeedData();
			_customers = new ObservableCollection<Customer>();
			_invoices = new ObservableCollection<Invoice>();
			_customers = db.Customers.OrderBy(xx => xx.LastName).ToList();
			_invoices = db.Invoices.ToList();
			customerViewModel = new CustomerViewModel(_customers);
			invoiceViewModel = new InvoiceViewModel(_invoices);
			VM = customerViewModel;
			//premier commit
		}

		private void ChangeView(string vm)
		{
			db.SaveChanges();
			switch (vm)
			{
				case "customers":
					VM = customerViewModel;
					break;
				case "invoices":
					VM = invoiceViewModel;
					break;
			}
		}

		private void DisplayInvoice(Invoice invoice)
		{
			db.SaveChanges();
			invoiceViewModel.SelectedInvoice = invoice;
			VM = invoiceViewModel;
		}

		private void DisplayCustomer(Customer customer)
		{
			db.SaveChanges();
			customerViewModel.SelectedCustomer = customer;
			VM = customerViewModel;
		}

		private void AddInvoiceToCustomer(Customer c)
		{
			var invoice = new Invoice(c);
			c.Invoices.Add(invoice);
			DisplayInvoice(invoice);
			db.Invoices.Add(invoice);
			db.SaveChanges();
		}

		private void AddNewItem (object item)
		{
			if (VM == customerViewModel)
			{
				var c = new Customer();
				customerViewModel.Customers.Add(c);
				customerViewModel.SelectedCustomer = c;
				db.Customers.Add(c);
				db.SaveChanges();
			}
		}

		private bool CanAddNewItem(object o)
		{
			bool result = false;

			result = VM == customerViewModel;
			return result;
		}
		private void SearchCustomer(object parameter)
		{
			string input = parameter as string;
			int output;
			string searchMethod;
			if (!Int32.TryParse(input, out output))
			{
				searchMethod = "name";
			}
			else
			{
				searchMethod = "id";
			}

			switch (searchMethod)
			{
				case "name":
					customerViewModel.SelectedCustomer = db.Customers.Where(xx => xx.LastName == input || xx.Name == input).FirstOrDefault();
					break;
				default:
					MessageBox.Show("Unkonwn search method");
					break;
			}
		}
		private bool SearchCustomerCanExecute(object o)
		{
			bool result = false;

			result = VM == customerViewModel;
			return result;
		}
		void SeedData()
		{
			if (db.Customers.Count() == 0)
			{
				List<Customer> Customers = new CustomersDataService().GetAll().ToList();
				List<Invoice> Invoices = new InvoicesDataService(Customers).GetAll().ToList();

				foreach (Customer c in Customers)
				{
					db.Customers.Add(c);
				}
				db.SaveChanges();
			}
			Debug.WriteLine($"Customers : {db.Customers.Count()}");
		}
		private void ExitClick(object item)
		{
			db.SaveChanges();
			App.Current.Shutdown();
		}
	}
}

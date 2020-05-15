using BillingManagement.Business;
using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using Inventaire;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace BillingManagement.UI.ViewModels
{
    class MainViewModel : BaseViewModel
    {
		BillingManagementContext db = new BillingManagementContext();
		private BaseViewModel _vm;

		public BaseViewModel VM
		{
			get { return _vm; }
			set {
				_vm = value;
				OnPropertyChanged();
			}
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
			db.Customers.OrderByDescending(xx => xx.LastName);
			customerViewModel = new CustomerViewModel(db.Customers);
			invoiceViewModel = new InvoiceViewModel(db.Invoices);
			VM = customerViewModel;
			//premier commit
		}

		private void ChangeView(string vm)
		{
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
			invoiceViewModel.SelectedInvoice = invoice;
			VM = invoiceViewModel;
		}

		private void DisplayCustomer(Customer customer)
		{
			customerViewModel.SelectedCustomer = customer;
			VM = customerViewModel;
		}

		private void AddInvoiceToCustomer(Customer c)
		{
			var invoice = new Invoice(c);
			c.Invoices.Add(invoice);
			DisplayInvoice(invoice);
		}

		private void AddNewItem (object item)
		{
			if (VM == customerViewModel)
			{
				var c = new Customer();
				customerViewModel.Customers.Add(c);
				customerViewModel.SelectedCustomer = c;
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
			App.Current.Shutdown();
		}
	}
}

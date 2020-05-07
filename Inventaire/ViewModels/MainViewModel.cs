﻿using BillingManagement.Models;
using BillingManagement.UI.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillingManagement.UI.ViewModels
{
    class MainViewModel : BaseViewModel
    {
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


		public MainViewModel()
		{
			ChangeViewCommand = new ChangeViewCommand(ChangeView);
			DisplayInvoiceCommand = new DelegateCommand<Invoice>(DisplayInvoice);
			DisplayCustomerCommand = new DelegateCommand<Customer>(DisplayCustomer);

			AddNewItemCommand = new DelegateCommand<object>(AddNewItem, CanAddNewItem);
			AddInvoiceToCustomerCommand = new DelegateCommand<Customer>(AddInvoiceToCustomer);

			customerViewModel = new CustomerViewModel();
			invoiceViewModel = new InvoiceViewModel(customerViewModel.Customers);

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

	}
}

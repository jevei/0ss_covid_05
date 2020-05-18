using BillingManagement.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BillingManagement.UI.ViewModels
{
	class InvoiceViewModel : BaseViewModel
	{
		private Invoice selectedInvoice;
		private ObservableCollection<Invoice> invoices;

		public InvoiceViewModel(IEnumerable<Invoice> invoiceData)
		{
			Invoices = new ObservableCollection<Invoice>(invoiceData.ToList());
		}

		public Invoice SelectedInvoice
		{
			get { return selectedInvoice; }
			set { 
				selectedInvoice = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<Invoice> Invoices { 
			get => invoices;
			set { 
				invoices = value;
				OnPropertyChanged();
			}
		}

	}
}

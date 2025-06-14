// Models/Supplier.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace IMS_Group03.Models
{
    public partial class Supplier : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name = string.Empty; // Initialize

        [ObservableProperty]
        private string _contactPerson = string.Empty; // Initialize (or make string? if optional)

        [ObservableProperty]
        private string _email = string.Empty; // Initialize (or make string? if optional)

        [ObservableProperty]
        private string _phone = string.Empty; // Initialize (or make string? if optional)

        [ObservableProperty]
        private string _address = string.Empty; // Initialize (or make string? if optional)

        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow; // Initialize

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow; // Initialize

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

        public Supplier()
        {
            // Fields are now initialized at declaration.
        }
    }
}